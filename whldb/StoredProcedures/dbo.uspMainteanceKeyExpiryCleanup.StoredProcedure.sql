DROP PROCEDURE IF EXISTS [dbo].[uspMaintenanceKeyExpiryCleanup];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 27-Jul-2024
-- Description:	Clean up any expired keys
-- Examples:
--	EXEC dbo.uspMaintenanceKeyExpiryCleanup
-- =============================================
CREATE PROCEDURE [dbo].[uspMaintenanceKeyExpiryCleanup]
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE dbo.tblSiteUsers
	SET ActivationKey = NULL, ActivationKeyExpiry = NULL
	WHERE DATEDIFF(SECOND, ActivationKeyExpiry, GETDATE()) > 0;

	UPDATE dbo.tblSiteUsers
	SET PasswordResetKey = NULL, PasswordResetKeyExpiry = NULL
	WHERE DATEDIFF(SECOND, PasswordResetKeyExpiry, GETDATE()) > 0;

	UPDATE dbo.tblSiteUsers
	SET AltEmailAddressKey = NULL, AltEmailAddressKeyExpiry = NULL
	WHERE DATEDIFF(SECOND, AltEmailAddressKeyExpiry, GETDATE()) > 0;

	UPDATE dbo.tblSiteUsers
	SET PhoneNumberKey = NULL, PhoneNumberKeyExpiry = NULL
	WHERE DATEDIFF(SECOND, PhoneNumberKeyExpiry, GETDATE()) > 0;

	UPDATE dbo.tblSiteUsers
	SET AltPhoneNumberKey = NULL, AltPhoneNumberKeyExpiry = NULL
	WHERE DATEDIFF(SECOND, AltPhoneNumberKeyExpiry, GETDATE()) > 0;

END
GO