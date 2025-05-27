DROP PROCEDURE IF EXISTS [dbo].[uspLotteryGenerator];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Aug-2024
-- Description:	Generate a list of applications in random order for all eligible lotteries, 
--				or by a given listing ID, optionally triggered by manual intervention
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspLotteryGenerator @ErrorMessage = @ErrorMessage OUTPUT -- All eligible listings
--	EXEC dbo.uspLotteryGenerator @ListingID = 1, @RunBy = 'USERNAME', @ManualInd = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Given Listing ID
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspLotteryGenerator]
	@ListingID		INT = 0
	, @RunBy		VARCHAR(200) = 'SYSTEM'
	, @ManualInd	BIT = 0
	, @ReRunInd		BIT = 0
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ListingsTable TABLE (
		ListingID		INT
		, Processed		CHAR(1) DEFAULT('X')
		, ErrorMessage	VARCHAR(MAX)
	);

	DECLARE @ApplicationsTable TABLE (
		ApplicationID	BIGINT
		, LotteryNumber	VARCHAR(20)
	);

	DECLARE @LotteryID INT;

	IF ISNULL(NULLIF(@ListingID, 0), 0) > 0
	BEGIN

		INSERT INTO @ListingsTable (ListingID)
			VALUES (@ListingID);

	END -- IF ISNULL(NULLIF(@ListingID, 0)) > 0
	ELSE
	BEGIN

		IF EXISTS (SELECT 1 FROM dbo.tblMasterConfig WHERE Category = 'LOTTERY' AND SubCategory = 'DEFAULT' AND ConfigKey = 'RUNMODE' AND ConfigValue = 'AUTO' AND Active = 1)
		BEGIN

			INSERT INTO @ListingsTable (ListingID)
				SELECT ListingID
				FROM dbo.tblSiteListings
				WHERE LotteryEligible = 1 AND LotteryID IS NULL
					AND DATEDIFF(SECOND, LotteryDate, GETDATE()) > 0
				ORDER BY LotteryDate ASC;

		END

		-- Do not allow automatic reruns
		SET @ReRunInd = 0;

	END -- IF ISNULL(NULLIF(@ListingID, 0)) > 0 : ELSE

	IF NOT EXISTS (SELECT 1 FROM @ListingsTable)
		RETURN 0;

	DECLARE @TempListingID INT;
	SET @TempListingID = ISNULL((SELECT MIN(ListingID) FROM @ListingsTable WHERE Processed = 'X'), 0);
	WHILE @TempListingID > 0
	BEGIN

		SELECT @LotteryID = NEXT VALUE FOR [dbo].[seqLotteryID];

		BEGIN TRY

			BEGIN TRANSACTION;

			IF @ReRunInd = 1 AND @TempListingID = @ListingID
			BEGIN

				DECLARE @LastLotteryID INT;
				SET @LastLotteryID = ISNULL((SELECT LotteryID FROM dbo.tblListings WHERE ListingID = @TempListingID), 0);

				IF @LastLotteryID > 0
				BEGIN

					-- Reset lottery in applications table
					UPDATE HA
					SET HA.LotteryID = NULL, HA.LotteryNumber = NULL, HA.StatusCD = 'SUBMITTED'
					OUTPUT DELETED.ApplicationID, DELETED.LotteryNumber INTO @ApplicationsTable (ApplicationID, LotteryNumber)
					FROM dbo.tblHousingApplications HA
					WHERE HA.ListingID = @TempListingID AND HA.StatusCD = 'ASSIGNED';

					-- Reset lottery in listings table(s)
					UPDATE dbo.tblListings SET LotteryID = NULL WHERE ListingID = @TempListingID;
					UPDATE dbo.tblSiteListings SET LotteryID = NULL WHERE ListingID = @TempListingID;

					-- Update lotteries table
					UPDATE dbo.tblLotteries SET StatusCD = 'CANCELLED' WHERE LotteryID = @LastLotteryID;

					-- Add audit entries
					INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
						SELECT 'APPLICATION', CONVERT(VARCHAR(20), A.ApplicationID), @RunBy, 'UPDATE'
							, 'Lottery #' + CONVERT(VARCHAR(20), A.LotteryNumber) + ' was cancelled' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END
							, GETDATE()
						FROM @ApplicationsTable A
						UNION
						SELECT 'LISTING', CONVERT(VARCHAR(20), @TempListingID), @RunBy, 'UPDATE'
							, 'Lottery #' + CONVERT(VARCHAR(20), @LastLotteryID) + ' was cancelled' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END
							, GETDATE()
						UNION
						SELECT 'LOTTERY', CONVERT(VARCHAR(20), @LastLotteryID), @RunBy, 'ADD'
							, 'Lottery was cancelled' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END + ' for Listing #' + CONVERT(VARCHAR(20), @TempListingID)
							, GETDATE();

					-- Clean temporary applications table
					DELETE FROM @ApplicationsTable;

				END

			END

			-- Insert unique lottery #/record
			INSERT INTO dbo.tblLotteries (LotteryID, ListingID, RunDate, RunBy, ManualInd, StatusCD)
				VALUES (@LotteryID, @TempListingID, GETDATE(), @RunBy, @ManualInd, 'NEW');

			-- Insert lottery results
			WITH LotteryCTE AS (
				SELECT ApplicationID, ROW_NUMBER() OVER (ORDER BY NEWID()) AS SortOrder
				FROM dbo.tblHousingApplications
				WHERE ListingID = @TempListingID AND StatusCD IN ('SUBMITTED', 'REVIEWED')
					--AND ApplicationID NOT IN (SELECT ApplicationID FROM dbo.tblLotteryResults WHERE ListingID = @TempListingID)
			)
				INSERT INTO dbo.tblLotteryResults (LotteryID, ListingID, ApplicationID, SortOrder, LotteryNumber)
					SELECT @LotteryID, @TempListingID, T.ApplicationID, T.SortOrder, CONVERT(VARCHAR(20), @LotteryID) + '-' + FORMAT(T.SortOrder, '000000')
					FROM LotteryCTE T;

			-- UPDATE dbo.tblLotteryResults
			-- SET LotteryNumber = CONVERT(VARCHAR(20), @LotteryID) + '-' + FORMAT(SortOrder, '000000')
			-- WHERE LotteryID = @LotteryID;

			-- Update listings table
			UPDATE dbo.tblListings SET LotteryID = @LotteryID WHERE ListingID = @TempListingID;
			UPDATE dbo.tblSiteListings SET LotteryID = @LotteryID WHERE ListingID = @TempListingID;

			-- Update applications table
			UPDATE HA
			SET HA.LotteryID		= LR.LotteryID
				, HA.LotteryNumber	= LR.LotteryNumber
				, HA.LotteryDate	= GETDATE()
				, HA.StatusCD		= 'ASSIGNED'
				, HA.OriginalStatusCD = 'ASSIGNED'
			OUTPUT INSERTED.ApplicationID, INSERTED.LotteryNumber INTO @ApplicationsTable (ApplicationID, LotteryNumber)
			FROM dbo.tblHousingApplications HA
			JOIN dbo.tblLotteryResults LR ON LR.ApplicationID = HA.ApplicationID AND LR.ListingID = HA.ListingID
			WHERE LR.LotteryID = @LotteryID;

			-- Add audit entries
			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				SELECT 'APPLICATION', CONVERT(VARCHAR(20), A.ApplicationID), @RunBy, 'UPDATE'
					, 'Lottery #' + CONVERT(VARCHAR(20), A.LotteryNumber) + ' was assigned' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END
					, GETDATE()
				FROM @ApplicationsTable A
				UNION
				SELECT 'LISTING', CONVERT(VARCHAR(20), @TempListingID), @RunBy, 'UPDATE'
					, 'Lottery #' + CONVERT(VARCHAR(20), @LotteryID) + ' was run' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END
					, GETDATE()
				UNION
				SELECT 'LOTTERY', CONVERT(VARCHAR(20), @LotteryID), @RunBy, 'ADD'
					, 'Lottery was run' + CASE WHEN @ManualInd = 1 THEN ' manually' ELSE '' END + ' for Listing #' + CONVERT(VARCHAR(20), @TempListingID)
					, GETDATE();

			-- Update lottery status
			UPDATE dbo.tblLotteries SET StatusCD = 'COMPLETE' WHERE LotteryID = @LotteryID;

			COMMIT TRANSACTION;

			-- Set listing as processed
			UPDATE @ListingsTable SET Processed = 'S' WHERE ListingID = @TempListingID;

		END TRY
		BEGIN CATCH

			IF XACT_STATE() <> 0 ROLLBACK TRAN;
			SET @ErrorMessage = 'Failed to run lottery for ' + CONVERT(VARCHAR(20), @TempListingID) + ' - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
			UPDATE @ListingsTable SET Processed = 'F', ErrorMessage = @ErrorMessage WHERE ListingID = @TempListingID;

		END CATCH

		-- Select next eligible listing to run lottery for
		SET @TempListingID = ISNULL((SELECT MIN(ListingID) FROM @ListingsTable WHERE Processed = 'X'), 0);

	END -- WHILE @ListingID > 0

	IF EXISTS (SELECT 1 FROM @ListingsTable WHERE Processed = 'F')
	BEGIN
		SET @ErrorMessage = 'Failed to run one or more lotteries';
		IF NOT EXISTS (SELECT 1 FROM @ListingsTable WHERE Processed = 'S')
			SELECT -1;
		ELSE
			SELECT CASE WHEN ISNULL(@ListingID, 0) > 0 THEN @LotteryID ELSE 1 END;
	END
	ELSE
	BEGIN
		SET @ErrorMessage = NULL;
		SELECT CASE WHEN ISNULL(@ListingID, 0) > 0 THEN @LotteryID ELSE 1 END;
	END

END
GO