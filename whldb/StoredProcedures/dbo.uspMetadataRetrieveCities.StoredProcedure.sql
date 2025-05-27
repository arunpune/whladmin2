DROP PROCEDURE IF EXISTS [dbo].[uspMetadataRetrieveCities];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Jan-2025
-- Description:	Retrieve a list of distinct cities
-- Examples:
--	EXEC dbo.uspMetadataRetrieveCities (Retrieve All)
-- =============================================
CREATE PROCEDURE [dbo].[uspMetadataRetrieveCities]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT L.City
	FROM dbo.tblSiteListings L
	ORDER BY L.City ASC;

END
GO