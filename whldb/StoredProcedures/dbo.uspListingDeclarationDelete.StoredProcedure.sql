DROP PROCEDURE IF EXISTS [dbo].[uspListingDeclarationDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Delete an existing Listing Declaration
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDeclarationDelete @DeclarationID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDeclarationDelete]
	@DeclarationID			INT
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT, @Text VARCHAR(1000);
		SELECT @ListingID = ListingID, @Text = [Text] FROM dbo.tblListingDeclarations WHERE DeclarationID = @DeclarationID;

		DELETE FROM dbo.tblListingDeclarations WHERE DeclarationID = @DeclarationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDECL', CONVERT(VARCHAR(20), @DeclarationID), @ModifiedBy, 'DELETE', 'Deleted Listing Declaration', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'DELETE', 'Deleted Listing Declaration: ' + @Text, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Listing Declaration - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO