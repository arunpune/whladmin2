DROP PROCEDURE IF EXISTS [dbo].[uspReportRetrieveCISByState];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Sep-2024
-- Description:	Retrieve report - Central Intake Sign-Ups by State
-- Examples:
--	EXEC dbo.uspReportRetrieveCISByState @FromDate = 20240901, @ToDate = 20240930
-- =============================================
CREATE PROCEDURE [dbo].[uspReportRetrieveCISByState]
	@FromDate		INT
	, @ToDate		INT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT ISNULL(H.PhysicalStateCD, 'XX') AS StateCD
		, COUNT(1) AS TotalCount
		, SUM(CASE WHEN U.Active = 1 THEN 1 ELSE 0 END) AS ActiveCount
		, SUM(CASE WHEN U.Active = 0 THEN 1 ELSE 0 END) AS InactiveCount
	FROM dbo.tblHouseholds H WITH (NOLOCK)
	JOIN dbo.tblSiteUsers U WITH (NOLOCK) ON U.Username = H.Username
	WHERE CONVERT(INT, FORMAT(U.CreatedDate, 'yyyyMMdd')) BETWEEN @FromDate AND @ToDate
	GROUP BY H.PhysicalStateCD
	ORDER BY H.PhysicalStateCD;

END
GO