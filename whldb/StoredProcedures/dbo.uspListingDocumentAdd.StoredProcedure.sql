DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Jan-2025
-- Description:	Add a new Listing Document
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDocumentAdd @ListingID = 1, @Title = 'TITLE', @FileName = 'TITLE.pdf'
--									, @Contents = '12345', @MimeType = 'application/pdf'
--									, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentAdd]
	@ListingID					INT
	, @Title					VARCHAR(200)
	, @FileName					VARCHAR(250)
	, @Contents					VARCHAR(MAX)
	, @MimeType					VARCHAR(30)
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

		INSERT INTO dbo.tblListingDocuments (ListingID, Title, [FileName], Contents, MimeType
											, DisplayOnListingsPageInd
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ListingID, @Title, @FileName, @Contents, @MimeType
						, @DisplayOnListingsPageInd
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @ImageID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDOCUMENT', CONVERT(VARCHAR(20), @ImageID), @CreatedBy, 'ADD', 'Added Listing Document', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @CreatedBy, 'UPDATE', 'Added Listing Document: ' + @Title, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Listing Document - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO