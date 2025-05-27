DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserExchangeKey];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Exchange a key
-- Examples:
--	EXEC dbo.uspSiteUserExchangeKey @ActivationKey = 'KEY' (Exchange By Activation Key)
--	EXEC dbo.uspSiteUserExchangeKey @PasswordResetKey = 'KEY' (Exchange By Password Reset Key)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserExchangeKey]
	@NewKey				VARCHAR(32)
	, @ActivationKey	VARCHAR(32) = NULL
	, @PasswordResetKey	VARCHAR(32) = NULL
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	SET @NewKey				= NULLIF(ISNULL(RTRIM(@NewKey), ''), '');
	SET @ActivationKey		= NULLIF(ISNULL(RTRIM(@ActivationKey), ''), '');
	SET @PasswordResetKey	= NULLIF(ISNULL(RTRIM(@PasswordResetKey), ''), '');

	IF (@NewKey IS NULL AND @ActivationKey IS NULL) OR (@NewKey IS NULL AND @PasswordResetKey IS NULL)
	BEGIN
		SELECT 1;
		RETURN;
	END

	DECLARE @Username VARCHAR(200);
	SET @Username = NULLIF(ISNULL((SELECT Username FROM dbo.tblSiteUsers
									WHERE (@ActivationKey IS NOT NULL AND ActivationKey = @ActivationKey)
										OR (@PasswordResetKey IS NOT NULL AND PasswordResetKey = @PasswordResetKey)), ''), '');

	IF @Username IS NULL
	BEGIN
		SET @ErrorMessage = 'Failed to exchange key for Site User - Associated account not found';
		SELECT -1;
		RETURN;
	END

	BEGIN TRY

		BEGIN TRAN;

		IF @ActivationKey IS NOT NULL
			UPDATE dbo.tblSiteUsers
			SET
				ActivationKey			= @NewKey
				, ModifiedBy			= 'SYSTEM'
				, ModifiedDate			= GETDATE()
			WHERE Username = @Username AND ActivationKey = @ActivationKey;

		ELSE IF @PasswordResetKey IS NOT NULL
			UPDATE dbo.tblSiteUsers
			SET
				PasswordResetKey		= @NewKey
				, ModifiedBy			= 'SYSTEM'
				, ModifiedDate			= GETDATE()
			WHERE Username = @Username AND PasswordResetKey = @PasswordResetKey;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, 'SYSTEM', 'UPDATE', 'Exchanged key for Site User', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to exchange key for Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO