DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationDocumentDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Delete a document associated with an application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationDocumentDelete @Username = 'USERNAME', @DocID = 1
--						, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationDocumentDelete]
	@Username			VARCHAR(200)
	, @ApplicationID	BIGINT
	, @DocID			BIGINT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplicationDocuments SET Active = 0 WHERE Username = @Username AND ApplicationID = @ApplicationID AND DocID = @DocID;

		DECLARE @FileName VARCHAR(250);
		SELECT @FileName = FileName FROM dbo.tblHousingApplicationDocuments WHERE Username = @Username AND ApplicationID = @ApplicationID AND DocID = @DocID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @Username, 'UPDATE', 'Deleted Document ' + @FileName, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Housing Application Document - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO