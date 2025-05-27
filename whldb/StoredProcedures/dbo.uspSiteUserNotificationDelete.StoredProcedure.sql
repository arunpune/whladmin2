DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserNotificationDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 09-Oct-2024
-- Description:	Delete a site user notification
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserNotificationDelete @NotificationID = 1
--											, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserNotificationDelete]
	@NotificationID		BIGINT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUserNotifications
		SET
			Active				= 0
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE NotificationID = CASE WHEN @NotificationID = -999 THEN NotificationID ELSE @NotificationID END;

		DECLARE @Note VARCHAR(200);
		SET @Note = CASE
						WHEN @NotificationID = -999 THEN 'Deleted all notifications.'
						ELSE 'Deleted notification #' + CONVERT(VARCHAR(20), @NotificationID) + '.'
					END;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @ModifiedBy, @ModifiedBy, 'UPDATE', @Note, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete notification for Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO