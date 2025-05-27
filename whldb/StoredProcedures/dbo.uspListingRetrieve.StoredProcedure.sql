DROP PROCEDURE IF EXISTS [dbo].[uspListingRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of listings (optionally by status), or a single one by ListingID
-- Examples:
--	EXEC dbo.uspListingRetrieve (Retrieve All)
--	EXEC dbo.uspListingRetrieve @ListingID = 1 (Retrieve One)
--	EXEC dbo.uspListingRetrieve @StatusCD = 'DRAFT' (Retrieve by Status)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingRetrieve]
	@ListingID		INT = 0
	, @StatusCD		VARCHAR(20) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@ListingID, 0) > 0
	BEGIN
		SELECT L.ListingID, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
			, L.ResaleInd
			, L.ListingAgeTypeCD, MLAT.[Description] AS ListingAgeTypeDescription
			, L.[Name], L.[Description]
			, L.StreetLine1, L.StreetLine2, L.StreetLine3, L.City, L.StateCD, L.ZipCode, L.County
			, L.Municipality, L.MunicipalityUrl, L.SchoolDistrict, L.SchoolDistrictUrl
			, L.MapUrl, L.EsriX, L.EsriY, L.WebsiteUrl
			, L.ListingStartDate, L.ListingEndDate
			, L.ApplicationStartDate, L.ApplicationEndDate
			, L.LotteryEligible, L.LotteryDate, L.LotteryID
			, L.WaitlistEligible, L.WaitlistStartDate, L.WaitlistEndDate
			, L.MinHouseholdIncomeAmt, L.MaxHouseholdIncomeAmt
			, L.MinHouseholdSize, L.MaxHouseholdSize
			, L.PetsAllowedInd, L.PetsAllowedText, L.RentIncludesText
			, L.CompletedOrInitialOccupancyYear, L.TermOfAffordability
			, L.MarketingAgentInd
				, L.MarketingAgentID, MMA.Name AS MarketingAgentName
				, L.MarketingAgentApplicationLink
			, L.StatusCD, MLS.[Description] AS StatusDescription
			, L.VersionNo, COALESCE(SL.VersionNo, L.VersionNo) AS PublishedVersionNo
			, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
		FROM dbo.tblListings L
		LEFT OUTER JOIN dbo.tblSiteListings SL ON SL.ListingID = L.ListingID
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN dbo.tblMasterMarketingAgents MMA ON MMA.AgentID = L.MarketingAgentID
		WHERE L.ListingID = @ListingID
		ORDER BY L.[Name] ASC;
		RETURN;
	END

	IF ISNULL(RTRIM(@StatusCD), '') <> ''
	BEGIN
		SELECT L.ListingID, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
			, L.ResaleInd
			, L.ListingAgeTypeCD, MLAT.[Description] AS ListingAgeTypeDescription
			, L.[Name], L.[Description]
			, L.StreetLine1, L.StreetLine2, L.StreetLine3, L.City, L.StateCD, L.ZipCode, L.County
			, L.Municipality, L.MunicipalityUrl, L.SchoolDistrict, L.SchoolDistrictUrl
			, L.MapUrl, L.EsriX, L.EsriY, L.WebsiteUrl
			, L.ListingStartDate, L.ListingEndDate
			, L.ApplicationStartDate, L.ApplicationEndDate
			, L.LotteryEligible, L.LotteryDate, L.LotteryID
			, L.WaitlistEligible, L.WaitlistStartDate, L.WaitlistEndDate
			, L.MinHouseholdIncomeAmt, L.MaxHouseholdIncomeAmt
			, L.MinHouseholdSize, L.MaxHouseholdSize
			, L.PetsAllowedInd, L.PetsAllowedText, L.RentIncludesText
			, L.CompletedOrInitialOccupancyYear, L.TermOfAffordability
			, L.MarketingAgentInd
				, L.MarketingAgentID, MMA.Name AS MarketingAgentName
				, L.MarketingAgentApplicationLink
			, L.StatusCD, MLS.[Description] AS StatusDescription
			, L.VersionNo, COALESCE(SL.VersionNo, L.VersionNo) AS PublishedVersionNo
			, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
		FROM dbo.tblListings L
		LEFT OUTER JOIN dbo.tblSiteListings SL ON SL.ListingID = L.ListingID
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN dbo.tblMasterMarketingAgents MMA ON MMA.AgentID = L.MarketingAgentID
		WHERE L.StatusCD = @StatusCD
		ORDER BY L.[Name] ASC;
		RETURN;
	END

	SELECT L.ListingID, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
		, L.ResaleInd
		, L.ListingAgeTypeCD, MLAT.[Description] AS ListingAgeTypeDescription
		, L.[Name], L.[Description]
		, L.StreetLine1, L.StreetLine2, L.StreetLine3, L.City, L.StateCD, L.ZipCode, L.County
		, L.Municipality, L.MunicipalityUrl, L.SchoolDistrict, L.SchoolDistrictUrl
		, L.MapUrl, L.EsriX, L.EsriY, L.WebsiteUrl
		, L.ListingStartDate, L.ListingEndDate
		, L.ApplicationStartDate, L.ApplicationEndDate
		, L.LotteryEligible, L.LotteryDate, L.LotteryID
		, L.WaitlistEligible, L.WaitlistStartDate, L.WaitlistEndDate
		, L.MinHouseholdIncomeAmt, L.MaxHouseholdIncomeAmt
		, L.MinHouseholdSize, L.MaxHouseholdSize
		, L.PetsAllowedInd, L.PetsAllowedText, L.RentIncludesText
		, L.CompletedOrInitialOccupancyYear, L.TermOfAffordability
		, L.MarketingAgentInd
			, L.MarketingAgentID, MMA.Name AS MarketingAgentName
			, L.MarketingAgentApplicationLink
		, L.StatusCD, MLS.[Description] AS StatusDescription
		, L.VersionNo, COALESCE(SL.VersionNo, L.VersionNo) AS PublishedVersionNo
		, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
	FROM dbo.tblListings L
	LEFT OUTER JOIN dbo.tblSiteListings SL ON SL.ListingID = L.ListingID
	LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
	LEFT OUTER JOIN dbo.tblMasterMarketingAgents MMA ON MMA.AgentID = L.MarketingAgentID
	ORDER BY L.[Name] ASC;

END
GO