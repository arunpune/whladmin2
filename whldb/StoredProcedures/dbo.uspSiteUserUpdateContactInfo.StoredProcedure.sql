DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserUpdateContactInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update site user contact information
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserUpdateContactInfo @Username = 'USERNAME', @PhoneNumber = '1111111111', @LastName = 'LastName
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserUpdateContactInfo]
	@Username						VARCHAR(200)
	, @EmailAddress					VARCHAR(200)
	, @AuthRepEmailAddressInd		BIT = 0
	, @AltEmailAddress				VARCHAR(200) = NULL
	, @AltEmailAddressKey			VARCHAR(32) = NULL
	, @AltEmailAddressKeyExpiry		DATETIME = NULL
	, @AltEmailAddressVerifiedInd	BIT = 0
	, @PhoneNumberTypeCD			VARCHAR(20)
	, @PhoneNumber					VARCHAR(10)
	, @PhoneNumberExtn				VARCHAR(10) = NULL
	, @PhoneNumberKey				VARCHAR(32) = NULL
	, @PhoneNumberKeyExpiry			DATETIME = NULL
	, @PhoneNumberVerifiedInd		BIT = 0
	, @AltPhoneNumberTypeCD			VARCHAR(20) = ''
	, @AltPhoneNumber				VARCHAR(10) = NULL
	, @AltPhoneNumberExtn			VARCHAR(10) = NULL
	, @AltPhoneNumberKey			VARCHAR(32) = NULL
	, @AltPhoneNumberKeyExpiry		DATETIME = NULL
	, @AltPhoneNumberVerifiedInd	BIT = 0
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUsers
		SET
			PhoneNumber						= @PhoneNumber
			, PhoneNumberExtn				= @PhoneNumberExtn
			, PhoneNumberTypeCD				= @PhoneNumberTypeCD
			, PhoneNumberKey				= @PhoneNumberKey
			, PhoneNumberKeyExpiry			= @PhoneNumberKeyExpiry
			, PhoneNumberVerifiedInd		= @PhoneNumberVerifiedInd
			, AltPhoneNumber				= @AltPhoneNumber
			, AltPhoneNumberExtn			= @AltPhoneNumberExtn
			, AltPhoneNumberTypeCD			= @AltPhoneNumberTypeCD
			, AltPhoneNumberKey				= @AltPhoneNumberKey
			, AltPhoneNumberKeyExpiry		= @AltPhoneNumberKeyExpiry
			, AltPhoneNumberVerifiedInd		= @AltPhoneNumberVerifiedInd
			, EmailAddress					= @EmailAddress
			, AuthRepEmailAddressInd		= @AuthRepEmailAddressInd
			, AltEmailAddress				= @AltEmailAddress
			, AltEmailAddressKey			= @AltEmailAddressKey
			, AltEmailAddressKeyExpiry		= @AltEmailAddressKeyExpiry
			, AltEmailAddressVerifiedInd	= @AltEmailAddressVerifiedInd
			, ModifiedBy					= @ModifiedBy
			, ModifiedDate					= GETDATE()
		WHERE Username = @Username AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Added Site User Contact Information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Site User Contact Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO