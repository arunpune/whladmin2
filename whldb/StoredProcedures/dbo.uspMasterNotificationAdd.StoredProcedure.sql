DROP PROCEDURE IF EXISTS [dbo].[uspMasterNotificationAdd];
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
--	EXEC dbo.uspMasterNotificationAdd @CategoryCD = 'CATEGORY', @Title = 'TITLE', @Text = 'TEXT'
--										, @FrequencyCD = 'FREQ', @FrequencyInterval = 0, @NotificationList = 'LIST'
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterNotificationAdd]
	@CategoryCD				VARCHAR(20)
	, @Title				VARCHAR(200)
	, @Text					VARCHAR(1000)
	, @FrequencyCD			VARCHAR(20)
	, @FrequencyInterval	INT
	, @NotificationList		VARCHAR(1000) = NULL
	, @InternalNotificationList VARCHAR(1000) = NULL
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @NotificationID INT;

		INSERT INTO dbo.tblMasterNotifications (CategoryCD, Title, [Text], FrequencyCD, FrequencyInterval
													, NotificationList, InternalNotificationList
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@CategoryCD, @Title, @Text, @FrequencyCD, @FrequencyInterval
						, @NotificationList, @InternalNotificationList
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @NotificationID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('NOTIFICATION', CONVERT(VARCHAR(20), @NotificationID), @CreatedBy, 'ADD', 'Added Notification', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Notification - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO