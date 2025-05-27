DROP PROCEDURE IF EXISTS [dbo].[uspReportRetrieveCISByCounty];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Sep-2024
-- Description:	Retrieve report - Central Intake Sign-Ups by County, optionally filtered by State
-- Examples:
--	EXEC dbo.uspReportRetrieveCISByCounty @FromDate = 20240901, @ToDate = 20240930
--	EXEC dbo.uspReportRetrieveCISByCounty @FromDate = 20240901, @ToDate = 20240930, @StateCD = 'NY'
-- =============================================
CREATE PROCEDURE [dbo].[uspReportRetrieveCISByCounty]
	@FromDate		INT
	, @ToDate		INT
	, @StateCD		VARCHAR(2) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SET @StateCD = NULLIF(ISNULL(RTRIM(@StateCD), ''), '');

	SELECT ISNULL(H.PhysicalStateCD, 'XX') AS StateCD, H.PhysicalCounty AS County
		, COUNT(1) AS TotalCount
		, SUM(CASE WHEN U.Active = 1 THEN 1 ELSE 0 END) AS ActiveCount
		, SUM(CASE WHEN U.Active = 0 THEN 1 ELSE 0 END) AS InactiveCount
	FROM dbo.tblHouseholds H
	JOIN dbo.tblSiteUsers U WITH (NOLOCK) ON U.Username = H.Username
	WHERE CONVERT(INT, FORMAT(U.CreatedDate, 'yyyyMMdd')) BETWEEN @FromDate AND @ToDate
		AND (@StateCD IS NULL OR H.PhysicalStateCD = @StateCD)
	GROUP BY H.PhysicalStateCD, H.PhysicalCounty
	ORDER BY H.PhysicalStateCD, H.PhysicalCounty;

END
GO