DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Jan-2025
-- Description:	Delete an existing Listing Document
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDocumentDelete @DocumentID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentDelete]
	@DocumentID		INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT, @Title VARCHAR(200);
		SET @ListingID	= ISNULL((SELECT ListingID FROM dbo.tblListingDocuments WHERE DocumentID = @DocumentID), 0);
		SET @Title		= ISNULL((SELECT Title FROM dbo.tblListingDocuments WHERE DocumentID = @DocumentID), 'N/A');

		DELETE FROM dbo.tblListingDocuments
		WHERE DocumentID = @DocumentID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDOCUMENT', CONVERT(VARCHAR(20), @DocumentID), @ModifiedBy, 'DELETE', 'Deleted Listing Document', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Document: ' + @Title, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Listing Document - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO