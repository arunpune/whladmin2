DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of admin users, or a single one by email address
-- Examples:
--	EXEC dbo.uspAdminUserRetrieve (Retrieve All)
--	EXEC dbo.uspAdminUserRetrieve @UserID = 'USER' (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserRetrieve]
	@UserID			VARCHAR(200) = NULL
	, @EmailAddress	VARCHAR(200) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(RTRIM(@UserID), '') <> ''
		SELECT U.UserID, U.EmailAddress, U.DisplayName
			, U.OrganizationCD, MO.[Description] AS OrganizationDescription
			, U.RoleCD, MR.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata MO ON MO.CodeID = 118 AND MO.Code = U.OrganizationCD
		LEFT OUTER JOIN dbo.tblMetadata MR ON MR.CodeID = 102 AND MR.Code = U.RoleCD
		WHERE U.UserID = @UserID;

	ELSE IF ISNULL(RTRIM(@EmailAddress), '') <> ''
		SELECT U.UserID, U.EmailAddress, U.DisplayName
			, U.OrganizationCD, MO.[Description] AS OrganizationDescription
			, U.RoleCD, MR.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata MO ON MO.CodeID = 118 AND MO.Code = U.OrganizationCD
		LEFT OUTER JOIN dbo.tblMetadata MR ON MR.CodeID = 102 AND MR.Code = U.RoleCD
		WHERE U.EmailAddress = @EmailAddress;

	ELSE
		SELECT U.UserID, U.EmailAddress, U.DisplayName
			, U.OrganizationCD, MO.[Description] AS OrganizationDescription
			, U.RoleCD, MR.[Description] AS RoleDescription
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
			, U.DeactivatedBy, U.DeactivatedDate, U.LastLoggedIn
			, U.Active
		FROM dbo.tblAdminUsers U
		LEFT OUTER JOIN dbo.tblMetadata MO ON MO.CodeID = 118 AND MO.Code = U.OrganizationCD
		LEFT OUTER JOIN dbo.tblMetadata MR ON MR.CodeID = 102 AND MR.Code = U.RoleCD
		ORDER BY U.DisplayName;

END
GO