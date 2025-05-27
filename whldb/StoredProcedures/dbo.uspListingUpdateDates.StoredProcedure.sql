DROP PROCEDURE IF EXISTS [dbo].[uspListingUpdateDates];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Update listing dates
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUpdateDates @ListingID = 1, @ListingStartDate = '2024-07-01'
--									, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUpdateDates]
	@ListingID					INT
	, @ListingStartDate			DATETIME = NULL
	, @ListingEndDate			DATETIME = NULL
	, @ApplicationStartDate		DATETIME = NULL
	, @ApplicationEndDate		DATETIME = NULL
	, @LotteryEligible			BIT = 0
	, @LotteryDate				DATETIME = NULL
	, @WaitlistEligible			BIT = 0
	, @WaitlistStartDate		DATETIME = NULL
	, @WaitlistEndDate			DATETIME = NULL
	, @ModifiedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListings
		SET
			ListingStartDate		= @ListingStartDate
			, ListingEndDate		= @ListingEndDate
			, ApplicationStartDate	= @ApplicationStartDate
			, ApplicationEndDate	= @ApplicationEndDate
			, LotteryEligible		= @LotteryEligible
			, LotteryDate			= @LotteryDate
			, WaitlistEligible		= @WaitlistEligible
			, WaitlistStartDate		= @WaitlistStartDate
			, WaitlistEndDate		= @WaitlistEndDate
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE ListingID = @ListingID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Dates', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Dates - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO