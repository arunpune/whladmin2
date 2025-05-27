DROP PROCEDURE IF EXISTS [dbo].[uspMasterRoleRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of master roles, or a single one by RoleCD
-- Examples:
--	EXEC dbo.uspMasterRoleRetrieve (Retrieve All)
--	EXEC dbo.uspMasterRoleRetrieve @RoleCD = 'ROLE' (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterRoleRetrieve]
	@RoleCD		VARCHAR(20) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SELECT RoleCD, RoleDescription
	FROM dbo.tblMasterRoles
	WHERE @RoleCD IS NULL OR RoleCD = @RoleCD
	ORDER BY RoleDescription ASC;

END
GO