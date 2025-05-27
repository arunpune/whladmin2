DROP PROCEDURE IF EXISTS [dbo].[uspMasterResourceRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 16-Sep-2024
-- Description:	Retrieve a list of Resource configurations, or a single one by ResourceID
-- Examples:
--	EXEC dbo.uspMasterResourceRetrieve (Retrieve All)
--	EXEC dbo.uspMasterResourceRetrieve @ResourceID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterResourceRetrieve]
	@ResourceID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ResourceID, Title, [Text], [Url], DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterResources
	WHERE @ResourceID = 0 OR ResourceID = @ResourceID
	ORDER BY DisplayOrder ASC;

END
GO