DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserActivatePhoneNumber];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Activate phone number for a site user
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserActivatePhoneNumber @Username = 'USERNAME', @PhoneNumberKey = 'KEY'
--						, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserActivatePhoneNumber]
	@Username			VARCHAR(200)
	, @PhoneNumberKey	VARCHAR(32)
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUsers
		SET
			PhoneNumberKey				= NULL
			, PhoneNumberKeyExpiry		= NULL
			, PhoneNumberVerifiedInd	= 1
			, ModifiedBy				= @ModifiedBy
			, ModifiedDate				= GETDATE()
		WHERE Username = @Username AND PhoneNumberKey = @PhoneNumberKey AND Active = 1 AND PhoneNumberVerifiedInd = 0;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Verified/activated Site User Phone Number', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to activate Site User Phone Number - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO