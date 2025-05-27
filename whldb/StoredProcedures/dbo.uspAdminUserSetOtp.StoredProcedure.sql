DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserSetOtp];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 08-Nov-2024
-- DisplayName:	Set OTP for Admin User
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspAdminUserSetOtp @UserID = 'UID', @EmailAddress = 'EMAIL', @DisplayName = 'NAME', @OrganizationCD = 'ORGCD', @RoleCD = 'ROLECD', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserSetOtp]
	@EmailAddress		VARCHAR(200)
	, @OTP				VARCHAR(16)
	, @OTPExpiry		DATETIME
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	BEGIN TRY

		UPDATE dbo.tblAdminUsers
		SET OTP = @OTP, OTPExpiry = @OTPExpiry
		WHERE EmailAddress = @EmailAddress;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		SET @ErrorMessage = 'Failed to set OTP for admin user - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO