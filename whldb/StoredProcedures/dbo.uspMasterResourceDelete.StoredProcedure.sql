DROP PROCEDURE IF EXISTS [dbo].[uspMasterResourceDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 16-Sep-2024
-- Description:	Delete an Resource
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterResourceDelete @ResourceID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterResourceDelete]
	@ResourceID		INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterResources
		SET
			Active			= 0
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE ResourceID = @ResourceID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('Resource', CONVERT(VARCHAR(20), @ResourceID), @ModifiedBy, 'DELETE', 'Deleted Resource', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Resource - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO