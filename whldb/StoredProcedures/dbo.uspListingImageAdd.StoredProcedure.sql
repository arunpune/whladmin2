DROP PROCEDURE IF EXISTS [dbo].[uspListingImageAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Add a new Listing Image
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingImageAdd @ListingID = 1, @Title = 'TITLE', @Contents = '12345', @MimeType = 'image/png', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingImageAdd]
	@ListingID					INT
	, @Title					VARCHAR(200)
	, @ThumbnailContents		VARCHAR(MAX) = NULL
	, @Contents					VARCHAR(MAX)
	, @MimeType					VARCHAR(30)
	, @IsPrimary				BIT = 0
	, @DisplayOnListingsPageInd	BIT = 1
	, @CreatedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ImageID INT;

		INSERT INTO dbo.tblListingImages (ListingID, Title, ThumbnailContents, Contents, MimeType
											, IsPrimary, DisplayOnListingsPageInd
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ListingID, @Title, @ThumbnailContents, @Contents, @MimeType
						, @IsPrimary, @DisplayOnListingsPageInd
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @ImageID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGIMAGE', CONVERT(VARCHAR(20), @ImageID), @CreatedBy, 'ADD', 'Added Listing Image', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @CreatedBy, 'UPDATE', 'Added Listing Image: ' + @Title, GETDATE());

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