DROP PROCEDURE IF EXISTS [dbo].[uspReportRetrieveMarketEvaluationByListing];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Retrieve report - Market Evaluations By Listing
-- Examples:
--	EXEC dbo.uspReportRetrieveMarketEvaluationByListing @ListingID = 202401002, @FromDate = 20240901, @ToDate = 20241030
-- =============================================
CREATE PROCEDURE [dbo].[uspReportRetrieveMarketEvaluationByListing]
	@ListingID		INT
	, @FromDate		INT
	, @ToDate		INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @AppsTable TABLE (
		ApplicationID	BIGINT
		, GenderCD		VARCHAR(20)
		, RaceCD		VARCHAR(20)
		, EthnicityCD	VARCHAR(20)
		, HouseholdSize	INT
		, VoucherInd	BIT
		, MobilityInd	BIT
		, HearingInd	BIT
		, VisionInd		BIT
		, SubmittedDate INT
	);

	INSERT INTO @AppsTable (ApplicationID, GenderCD, RaceCD, EthnicityCD, VoucherInd, MobilityInd, HearingInd, VisionInd, SubmittedDate)
		SELECT A.ApplicationID, A.GenderCD, A.RaceCD, A.EthnicityCD
			, A.VoucherInd
			, CASE WHEN ISNULL(A.AccessibilityCDs, '') LIKE '%WHEELCHAIR%' THEN 1 ELSE 0 END AS MobilityInd
			, CASE WHEN ISNULL(A.AccessibilityCDs, '') LIKE '%HEARING%' THEN 1 ELSE 0 END AS HearingInd
			, CASE WHEN ISNULL(A.AccessibilityCDs, '') LIKE '%VISION%' THEN 1 ELSE 0 END AS VisionInd
			, FORMAT(A.SubmittedDate, 'yyyyMMdd') AS SubmittedDate
		FROM dbo.tblHousingApplications A
		WHERE ListingID = @ListingID AND FORMAT(A.SubmittedDate, 'yyyyMMdd') BETWEEN @FromDate AND @ToDate;

	DECLARE @TotalAppsCount INT;
	SET @TotalAppsCount = ISNULL((SELECT COUNT(1) FROM @AppsTable), 0);

	IF @TotalAppsCount = 0
	BEGIN
		SELECT 'GENERAL' AS CategoryCD, 'TOTAL' AS CategoryKey, 'Total Applications' AS CategoryDescription
			, @TotalAppsCount AS CategoryCount, 0 AS CategoryPercentage
			, 1 AS SortOrder;
		RETURN;
	END

	SELECT 'GENERAL' AS CategoryCD, 'TOTAL' AS CategoryKey, 'Total Applications' AS CategoryDescription
		, @TotalAppsCount AS CategoryCount, 0 AS CategoryPercentage
		, 1 AS SortOrder
	FROM @AppsTable

	UNION

	SELECT 'GENDER' AS CategoryCD, A.GenderCD, COALESCE(M.[Description], A.GenderCD) AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 2 AS SortOrder
	FROM @AppsTable A
	JOIN dbo.tblMetadata M ON M.CodeID = 110 AND M.Code = A.GenderCD
	GROUP BY A.GenderCD, COALESCE(M.[Description], A.GenderCD)

	UNION

	SELECT 'RACE' AS CategoryCD, A.RaceCD, COALESCE(M.[Description], A.RaceCD) AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 3 AS SortOrder
	FROM @AppsTable A
	JOIN dbo.tblMetadata M ON M.CodeID = 111 AND M.Code = A.RaceCD
	GROUP BY A.RaceCD, COALESCE(M.[Description], A.RaceCD)

	UNION

	SELECT 'ETHNICITY' AS CategoryCD, A.EthnicityCD, COALESCE(M.[Description], A.EthnicityCD) AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 4 AS SortOrder
	FROM @AppsTable A
	JOIN dbo.tblMetadata M ON M.CodeID = 112 AND M.Code = A.EthnicityCD
	GROUP BY A.EthnicityCD, COALESCE(M.[Description], A.EthnicityCD)

	UNION

	SELECT 'VOUCHER' AS CategoryCD, 'VOUCHER', 'Vouchers' AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 5 AS SortOrder
	FROM @AppsTable A
	JOIN dbo.tblMetadata M ON M.CodeID = 112 AND M.Code = A.EthnicityCD
	WHERE A.VoucherInd = 1
	GROUP BY A.EthnicityCD

	UNION

	SELECT 'ACCESSIBILITY' AS CategoryCD, 'MOBILITY', 'Mobility' AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 6 AS SortOrder
	FROM @AppsTable A
	WHERE A.MobilityInd = 1

	UNION

	SELECT 'ACCESSIBILITY' AS CategoryCD, 'HEARING', 'Hearing' AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 7 AS SortOrder
	FROM @AppsTable A
	WHERE A.HearingInd = 1

	UNION

	SELECT 'ACCESSIBILITY' AS CategoryCD, 'VISION', 'Vision' AS CategoryDescription
		, COUNT(1) AS CategoryCount, (COUNT(1) * 100)/@TotalAppsCount AS CategoryPercentage
		, 8 AS SortOrder
	FROM @AppsTable A
	WHERE A.VisionInd = 1

	ORDER BY SortOrder;

END
GO