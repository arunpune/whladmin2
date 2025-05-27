DROP PROCEDURE IF EXISTS [dbo].[uspMasterVideoRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of Video configurations, or a single one by VideoID
-- Examples:
--	EXEC dbo.uspMasterVideoRetrieve (Retrieve All)
--	EXEC dbo.uspMasterVideoRetrieve @VideoID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterVideoRetrieve]
	@VideoID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VideoID, Title, [Text], [Url], DisplayOrder, DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterVideos
	WHERE @VideoID = 0 OR VideoID = @VideoID
	ORDER BY DisplayOrder ASC;

END
GO