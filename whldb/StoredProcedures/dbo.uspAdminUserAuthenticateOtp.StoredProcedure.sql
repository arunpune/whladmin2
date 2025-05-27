DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserAuthenticateOtp];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 08-Nov-2024
-- Description:	Authenticate an admin user by email address and OTP
-- Examples:
--	EXEC dbo.uspAdminUserAuthenticateOtp @EmailAddress = 'USER@TEST.TST', @OTP = 'OTP'
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserAuthenticateOtp]
	@EmailAddress	VARCHAR(100)
	, @OTP			VARCHAR(16)
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (SELECT 1 FROM dbo.tblAdminUsers WHERE EmailAddress = @EmailAddress AND Active = 1 AND OTP = @OTP AND DATEDIFF(SECOND, GETDATE(), OTPExpiry) > 0 )
	BEGIN

		UPDATE dbo.tblAdminUsers
		SET OTP				= NULL
			, OTPExpiry		= NULL
			, LastLoggedIn	= GETDATE()
		WHERE EmailAddress = @EmailAddress AND Active = 1;

		SELECT U.UserID, U.EmailAddress, U.DisplayName, U.OrganizationCD
			, U.RoleCD, R.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata R ON R.CodeID = 102 AND R.Code = U.RoleCD
		WHERE U.EmailAddress = @EmailAddress AND U.Active = 1;

	END
	ELSE
		SELECT U.UserID, U.EmailAddress, U.DisplayName, U.OrganizationCD
			, U.RoleCD, R.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata R ON R.CodeID = 102 AND R.Code = U.RoleCD
		WHERE U.EmailAddress = 'XXX' AND U.Active = 1;

END
GO