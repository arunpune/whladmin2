DROP PROCEDURE IF EXISTS [dbo].[uspListingFundingSourceSave];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Save Listing Funding Sources
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingFundingSourceSave @ListingID = 1, @FundingSourceIDs = '1,2,3', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingFundingSourceSave]
	@ListingID				INT
	, @FundingSourceIDs		VARCHAR(MAX)
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF ISNULL(RTRIM(@FundingSourceIDs), '') = ''
		BEGIN

			DELETE FROM dbo.tblListingFundingSources WHERE ListingID = @ListingID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing FundingSources.', GETDATE());

		END
		ELSE
		BEGIN

			DECLARE @FundingSources TABLE (
				ListingID		INT
				, FundingSourceID		INT
				, ChangeType	CHAR(1) DEFAULT('-')
			);

			INSERT INTO @FundingSources (ListingID, FundingSourceID)
				SELECT @ListingID, [Value] FROM STRING_SPLIT(@FundingSourceIDs, ',');

			-- Add new entries
			INSERT INTO dbo.tblListingFundingSources (ListingID, FundingSourceID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
				SELECT A.ListingID, A.FundingSourceID, @ModifiedBy, GETDATE(), @ModifiedBy, GETDATE(), 1
				FROM @FundingSources A
				WHERE A.FundingSourceID NOT IN (
					SELECT FundingSourceID FROM dbo.tblListingFundingSources WHERE ListingID = @ListingID
				);

			-- Delete old entries
			DELETE FROM dbo.tblListingFundingSources
			WHERE ListingID = @ListingID AND FundingSourceID NOT IN (
				SELECT FundingSourceID FROM @FundingSources
			);

			DECLARE @Note VARCHAR(MAX);
			SET @Note = ISNULL((SELECT STRING_AGG(A.[Name], ', ') AS FundingSources
								FROM dbo.tblMasterFundingSources A
								JOIN dbo.tblListingFundingSources LA ON LA.ListingID = @ListingID AND LA.FundingSourceID = A.FundingSourceID), '');
			IF LEN(@Note) = 0 SET @Note = 'Updated Listing Funding Sources.'
			ELSE SET @Note = 'Updated Listing Funding Sources - ' + @Note + '.';

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Listing Funding Sources - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO