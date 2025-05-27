DROP PROCEDURE IF EXISTS [dbo].[uspMasterNotificationDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Add a new Notification
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterNotificationDelete @NotificationID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterNotificationDelete]
	@NotificationID		INT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterNotifications SET Active = 0 WHERE NotificationID = @NotificationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('NOTIFICATION', CONVERT(VARCHAR(20), @NotificationID), @ModifiedBy, 'DELETE', 'Deleted Notification', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @NotificationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Notification - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO