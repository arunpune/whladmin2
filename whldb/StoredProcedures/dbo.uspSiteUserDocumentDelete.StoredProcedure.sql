DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserDocumentDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Delete a site user document
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserDocumentDelete @Username = 'USERNAME', @DocID = 1
--						, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserDocumentDelete]
	@Username			VARCHAR(200)
	, @DocID			BIGINT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUserDocuments SET Active = 0 WHERE Username = @Username AND DocID = @DocID;

		DECLARE @FileName VARCHAR(250);
		SELECT @FileName = FileName FROM dbo.tblSiteUserDocuments WHERE Username = @Username AND DocID = @DocID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Deleted Document ' + @FileName, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Site User Document - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO