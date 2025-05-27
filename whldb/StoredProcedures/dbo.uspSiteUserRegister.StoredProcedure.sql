DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserRegister];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Register a new site user
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserRegister @Username = 'USERNAME', @PasswordHash = 'HASH'
--						, @ActivationKey = 'KEY', @ActivationKeyExpiry = '12/31/2024 23:59:59'
--						, @EmailAddress = 'USER@TEST.TST'
--						, @PhoneNumber = '1234567890', @PhoneNumberTypeCD = 'CELL'
--						, @LeadTypeCD = 'WEBSITE'
--						, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserRegister]
	@Username					VARCHAR(200)
	, @PasswordHash				VARCHAR(1024)
	, @ActivationKey			VARCHAR(32)
	, @ActivationKeyExpiry		DATETIME
	, @EmailAddress				VARCHAR(200)
	, @AuthRepEmailAddressInd	BIT = 0
	, @PhoneNumber				VARCHAR(10)
	, @PhoneNumberExtn			VARCHAR(10)
	, @PhoneNumberTypeCD		VARCHAR(20)
	, @LeadTypeCD				VARCHAR(20)
	, @LeadTypeOther			VARCHAR(500) = NULL
	, @CreatedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		INSERT INTO dbo.tblSiteUsers (Username, PasswordHash, ActivationKey, ActivationKeyExpiry
										, EmailAddress, AuthRepEmailAddressInd
										, PhoneNumber, PhoneNumberExtn, PhoneNumberTypeCD
										, LeadTypeCD, LeadTypeOther, HouseholdSize
										, CreatedBy, CreatedDate)
			VALUES (@Username, @PasswordHash, @ActivationKey, @ActivationKeyExpiry
						, @EmailAddress, @AuthRepEmailAddressInd
						, @PhoneNumber, @PhoneNumberExtn, @PhoneNumberTypeCD
						, @LeadTypeCD, @LeadTypeOther, 1
						, @Username, GETDATE());

		INSERT INTO dbo.tblHouseholds (Username, CreatedBy, CreatedDate)
			VALUES (@Username, @Username, GETDATE());

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'ADD', 'Added Site User', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Site User - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO