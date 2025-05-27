DROP PROCEDURE IF EXISTS [dbo].[uspReportRetrieveAFHMByDemographic];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Sep-2024
-- Description:	Retrieve report - Applicant Demographics
-- Examples:
--	EXEC dbo.uspReportRetrieveAFHMByDemographic @FromDate = 20240901, @ToDate = 20240930
--	EXEC dbo.uspReportRetrieveAFHMByDemographic @FromDate = 20240901, @ToDate = 20240930, @ListingID = 202400001
-- =============================================
CREATE PROCEDURE [dbo].[uspReportRetrieveAFHMByDemographic]
	@FromDate		INT
	, @ToDate		INT
	, @ListingID	INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	WITH AppsGenderCTE AS (
		SELECT A.ListingID, A.GenderCD, COUNT(1) AS GenderCount
		FROM dbo.tblHousingApplications A
		WHERE A.StatusCD NOT IN ('DRAFT')
			AND CONVERT(INT, FORMAT(A.SubmittedDate, 'yyyyMMdd')) BETWEEN @FromDate AND @ToDate
		GROUP BY A.ListingID, A.GenderCD
	), AppsRaceCTE AS (
		SELECT A.ListingID, A.RaceCD, COUNT(1) AS RaceCount
		FROM dbo.tblHousingApplications A
		WHERE A.StatusCD NOT IN ('DRAFT')
			AND CONVERT(INT, FORMAT(A.SubmittedDate, 'yyyyMMdd')) BETWEEN @FromDate AND @ToDate
		GROUP BY A.ListingID, A.RaceCD
	), AppsEthnicityCTE AS (
		SELECT A.ListingID, A.EthnicityCD, COUNT(1) AS EthnicityCount
		FROM dbo.tblHousingApplications A
		WHERE A.StatusCD NOT IN ('DRAFT')
			AND CONVERT(INT, FORMAT(A.SubmittedDate, 'yyyyMMdd')) BETWEEN @FromDate AND @ToDate
		GROUP BY A.ListingID, A.EthnicityCD
	)
		SELECT L.ListingID, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
			, L.ResaleInd
			, L.ListingAgeTypeCD, MLAT.[Description] AS ListingAgeTypeDescription
			, L.[Name], L.[Description]
			, L.StreetLine1, L.StreetLine2, L.StreetLine3, L.City, L.StateCD, L.ZipCode
			, L.Municipality, L.MunicipalityUrl, L.SchoolDistrict, L.SchoolDistrictUrl
			, L.StatusCD, MLS.[Description] AS StatusDescription
			, ISNULL([LAG].GenderCD, 'XX') AS GenderCD, [LAG].GenderCount
			, ISNULL(LAR.RaceCD, 'XX') AS RaceCD, LAR.RaceCount
			, ISNULL(LAE.EthnicityCD, 'XX'), LAE.EthnicityCount
		FROM dbo.tblListings L
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN AppsGenderCTE [LAG] ON [LAG].ListingID = L.ListingID
		LEFT OUTER JOIN AppsRaceCTE LAR ON LAR.ListingID = L.ListingID
		LEFT OUTER JOIN AppsEthnicityCTE LAE ON LAE.ListingID = L.ListingID
		WHERE (ISNULL(@ListingID, 0) = 0 OR L.ListingID = @ListingID)
			AND L.StatusCD = 'PUBLISHED'
		ORDER BY L.[Name] ASC;

END
GO