DROP PROCEDURE IF EXISTS [dbo].[uspSiteResourceRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Retrieve a list of Resource configurations, or a single one by ResourceID
-- Examples:
--	EXEC dbo.uspSiteResourceRetrieve (Retrieve All)
--	EXEC dbo.uspSiteResourceRetrieve @ResourceID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteResourceRetrieve]
	@ResourceID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ResourceID, Title, [Text], [Url], DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterResources
	WHERE (@ResourceID = 0 OR ResourceID = @ResourceID) AND Active = 1
	ORDER BY DisplayOrder ASC;

END
GO