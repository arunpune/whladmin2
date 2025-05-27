DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Jan-2025
-- Description:	Update a Listing Document
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDocumentUpdate @ListingID = 1, @Title = 'TITLE', @Contents = '12345', @MimeType = 'application/pdf', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentUpdate]
	@ListingID					INT
	, @DocumentID				INT
	, @Title					VARCHAR(200)
	, @DisplayOnListingsPageInd	BIT = 1
	, @ModifiedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListingDocuments
		SET
			Title						= @Title
			, DisplayOnListingsPageInd	= @DisplayOnListingsPageInd
			, ModifiedBy				= @ModifiedBy
			, ModifiedDate				= GETDATE()
		WHERE DocumentID = @DocumentID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDOCUMENT', CONVERT(VARCHAR(20), @DocumentID), @ModifiedBy, 'UPDATE', 'Updated Listing Document', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Document: ' + @Title, GETDATE());

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