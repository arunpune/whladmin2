DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserChangePassword];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Change password reset for site user
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserChangePassword @Username = 'USERNAME', @PasswordHash = 'HASH'
--						, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserChangePassword]
	@Username							VARCHAR(200)
	, @PasswordHash						VARCHAR(1024)
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUsers
		SET
			PasswordHash			= @PasswordHash
			, PasswordResetKey		= NULL
			, PasswordResetKeyExpiry = NULL
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE Username = @Username AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated password for Site User', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update password for Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO