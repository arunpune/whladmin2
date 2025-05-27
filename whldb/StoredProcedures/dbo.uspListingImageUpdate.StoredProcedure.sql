DROP PROCEDURE IF EXISTS [dbo].[uspListingImageUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Nov-2024
-- Description:	Update a Listing Image
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingImageUpdate @ListingID = 1, @Title = 'TITLE', @Contents = '12345', @MimeType = 'image/png', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingImageUpdate]
	@ListingID					INT
	, @ImageID					INT
	, @Title					VARCHAR(200)
	, @IsPrimary				BIT = 0
	, @DisplayOnListingsPageInd	BIT = 1
	, @ModifiedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListingImages
		SET
			Title						= @Title
			, IsPrimary					= @IsPrimary
			, DisplayOnListingsPageInd	= CASE WHEN @IsPrimary = 1 THEN 1 ELSE @DisplayOnListingsPageInd END
			, ModifiedBy				= @ModifiedBy
			, ModifiedDate				= GETDATE()
		WHERE ImageID = @ImageID;

		IF @IsPrimary = 1
			UPDATE tblListingImages SET IsPrimary = 0 WHERE ListingID = @ListingID AND ImageID <> @ImageID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGIMAGE', CONVERT(VARCHAR(20), @ImageID), @ModifiedBy, 'UPDATE', 'Updated Listing Image', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Image: ' + @Title, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Listing Image - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO