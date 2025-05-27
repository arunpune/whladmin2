DROP PROCEDURE IF EXISTS [dbo].[uspMasterNotificationUpdate];
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
--	EXEC dbo.uspMasterNotificationUpdate @NotificationID = 1, @CategoryCD = 'CATEGORY', @Title = 'TITLE', @Text = 'TEXT'
--											, @FrequencyCD = 'FREQ', @FrequencyInterval = 0, @NotificationList = 'LIST'
--											, @Active = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterNotificationUpdate]
	@NotificationID			INT
	, @CategoryCD			VARCHAR(20)
	, @Title				VARCHAR(200)
	, @Text					VARCHAR(1000)
	, @FrequencyCD			VARCHAR(20)
	, @FrequencyInterval	INT
	, @NotificationList		VARCHAR(1000) = NULL
	, @InternalNotificationList VARCHAR(1000) = NULL
	, @Active				BIT
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterNotifications
		SET
			[Text]				= @Text
			, FrequencyCD		= @FrequencyCD
			, FrequencyInterval	= @FrequencyInterval
			, NotificationList	= @NotificationList
			, InternalNotificationList = @InternalNotificationList
			, Active			= @Active
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE NotificationID = @NotificationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('NOTIFICATION', CONVERT(VARCHAR(20), @NotificationID), @ModifiedBy, 'UPDATE', 'Updated Notification', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Notification - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO