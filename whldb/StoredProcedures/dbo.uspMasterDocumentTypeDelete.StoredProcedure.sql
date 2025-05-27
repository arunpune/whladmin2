DROP PROCEDURE IF EXISTS [dbo].[uspMasterDocumentTypeDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Dec-2024
-- Description:	Delete an existing Document Type
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterDocumentTypeDelete @DocumentTypeID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterDocumentTypeDelete]
	@DocumentTypeID		INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterDocumentTypes SET Active = 0 WHERE DocumentTypeID = @DocumentTypeID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('DOCTYPE', CONVERT(VARCHAR(20), @DocumentTypeID), @ModifiedBy, 'DELETE', 'Deleted Document Type', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @DocumentTypeID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Document Type - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO