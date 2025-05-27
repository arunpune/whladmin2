DROP PROCEDURE IF EXISTS [dbo].[uspListingImageDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Delete an existing Listing Image
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingImageDelete @ImageID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingImageDelete]
	@ImageID		INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT, @Title VARCHAR(200);
		SET @ListingID	= ISNULL((SELECT ListingID FROM dbo.tblListingImages WHERE ImageID = @ImageID), 0);
		SET @Title		= ISNULL((SELECT Title FROM dbo.tblListingImages WHERE ImageID = @ImageID), 'N/A');

		DELETE FROM dbo.tblListingImages
		WHERE ImageID = @ImageID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGIMAGE', CONVERT(VARCHAR(20), @ImageID), @ModifiedBy, 'DELETE', 'Deleted Listing Image', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Image: ' + @Title, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Listing Image - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO