DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserAuthenticate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Authenticate an admin user by email address
-- Examples:
--	EXEC dbo.uspAdminUserAuthenticate @UserID = 'USER'
--	EXEC dbo.uspAdminUserAuthenticate @EmailAddress = 'USER@TEST.TST'
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserAuthenticate]
	@UserID			VARCHAR(4) = NULL
	, @EmailAddress	VARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(RTRIM(@UserID), '') <> ''
	BEGIN

		UPDATE dbo.tblAdminUsers SET LastLoggedIn = GETDATE() WHERE UserID = @UserID AND Active = 1;

		SELECT U.UserID, U.EmailAddress, U.DisplayName, U.OrganizationCD
			, U.RoleCD, R.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata R ON R.CodeID = 102 AND R.Code = U.RoleCD
		WHERE U.UserID = @UserID AND U.Active = 1;

	END
	ELSE IF ISNULL(RTRIM(@EmailAddress), '') <> ''
	BEGIN

		UPDATE dbo.tblAdminUsers SET LastLoggedIn = GETDATE() WHERE EmailAddress = @EmailAddress AND Active = 1;

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