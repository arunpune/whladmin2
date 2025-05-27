DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserNotificationUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 09-Oct-2024
-- Description:	Update a site user notification
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserNotificationUpdate @NotificationID = 1, @ReadInd = 0
--											, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	EXEC dbo.uspSiteUserNotificationUpdate @NotificationID = 1, @ReadInd = 1
--											, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserNotificationUpdate]
	@NotificationID		BIGINT
	, @ReadInd			BIT
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
			ReadInd				= @ReadInd
			, ReadTimestamp		= CASE WHEN @ReadInd = 1 THEN GETDATE() ELSE ReadTimestamp END
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE NotificationID = CASE WHEN @NotificationID = -999 THEN NotificationID ELSE @NotificationID END;

		DECLARE @Note VARCHAR(500);
		SET @Note = CASE
						WHEN @NotificationID = -999 THEN
							CASE
								WHEN @ReadInd = 1 THEN 'Read all notifications.'
								ELSE 'Marked unread all notifications.'
							END
						ELSE
							CASE
								WHEN @ReadInd = 1 THEN 'Read notification #' + CONVERT(VARCHAR(20), @NotificationID) + '.'
								ELSE 'Marked unread notification #' + CONVERT(VARCHAR(20), @NotificationID) + '.'
							END
					END;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @ModifiedBy, @ModifiedBy, 'UPDATE', @Note, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update notification for Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO