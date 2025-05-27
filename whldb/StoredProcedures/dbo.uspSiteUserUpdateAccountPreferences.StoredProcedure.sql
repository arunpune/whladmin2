DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserUpdateAccountPreferences];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update site user preferences
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserUpdateAccountPreferences @Username = 'USERNAME', @LanguagePreferenceCD = 'EN', @ListingPreferenceCD = 'BOTH'
--										, @SmsNotificationsPreferenceInd = 0
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserUpdateAccountPreferences]
	@Username							VARCHAR(200)
	, @LanguagePreferenceCD				VARCHAR(20) = 'EN'
	, @LanguagePreferenceOther			VARCHAR(100) = NULL
	, @ListingPreferenceCD				VARCHAR(20) = 'BOTH'
	, @SmsNotificationsPreferenceInd	BIT	= 0
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
			LanguagePreferenceCD				= @LanguagePreferenceCD
			, LanguagePreferenceOther			= @LanguagePreferenceOther
			, ListingPreferenceCD				= @ListingPreferenceCD
			, SmsNotificationsPreferenceInd		= @SmsNotificationsPreferenceInd
			, ModifiedBy						= @ModifiedBy
			, ModifiedDate						= GETDATE()
		WHERE Username = @Username AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Added Site User Preferences', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Site User Preferences - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO