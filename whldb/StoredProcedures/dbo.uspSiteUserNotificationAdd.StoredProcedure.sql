DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserNotificationAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 09-Oct-2024
-- Description:	Add a site user notification
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserNotificationAdd @Username = 'USERNAME, @Subject = 'SUBJECT', @Body = 'BODY', @EmailSentInd = 1
--											, @CreatedBy = 'SYSTEM', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserNotificationAdd]
	@Username			VARCHAR(200)
	, @Subject			VARCHAR(1000)
	, @Body				VARCHAR(MAX) = NULL
	, @EmailSentInd		BIT = 0
	, @CreatedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @NotificationID BIGINT;

		DECLARE @EmailTimestamp DATETIME;
		SET @EmailTimestamp = CASE WHEN @EmailSentInd = 1 THEN GETDATE() ELSE NULL END;

		INSERT INTO dbo.tblSiteUserNotifications (Username, [Subject], Body, EmailSentInd, EmailTimestamp
													, CreatedBy, CreatedDate)
			VALUES (@Username, @Subject, @Body, @EmailSentInd, @EmailTimestamp
					, @CreatedBy, GETDATE());

		SELECT @NotificationID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @CreatedBy, 'UPDATE', 'Added notification #' + CONVERT(VARCHAR(20), @NotificationID) + '.', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add notification for Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO