DROP PROCEDURE IF EXISTS [dbo].[uspSiteVideoRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Retrieve a list of Video configurations, or a single one by VideoID
-- Examples:
--	EXEC dbo.uspSiteVideoRetrieve (Retrieve All)
--	EXEC dbo.uspSiteVideoRetrieve @VideoID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteVideoRetrieve]
	@VideoID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT VideoID, Title, [Text], [Url], DisplayOrder, DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterVideos
	WHERE (@VideoID = 0 OR VideoID = @VideoID) AND Active = 1
	ORDER BY DisplayOrder ASC;

END
GO