DROP PROCEDURE IF EXISTS [dbo].[uspListingUpdateStatus];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Update listing status
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUpdateStatus @ListingID = 1, @StatusCD = 'DRAFT', @Note = 'NOTE'
--									, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUpdateStatus]
	@ListingID				INT
	, @StatusCD				VARCHAR(20)
	, @Note					VARCHAR(MAX) = NULL
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListings
		SET
			StatusCD			= @StatusCD
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE ListingID = @ListingID;

		SET @Note = ISNULL(RTRIM(@Note), 'Updated Listing Status');

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Status - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO