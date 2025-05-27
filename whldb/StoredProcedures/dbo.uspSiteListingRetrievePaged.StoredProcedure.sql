DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingRetrievePaged];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Jul-2024
-- Description:	Retrieve a list of listings, or a single one by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingRetrievePaged (Retrieve All)
--	EXEC dbo.uspSiteListingRetrievePaged @ListingID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingRetrievePaged]
	@ListingID		BIGINT = 0
	, @PageNo		INT = 1
	, @PageSize		INT = 100
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@PageNo, 1) < 1 SET @PageNo = 1;
	IF ISNULL(@PageSize, 100) < 100 SET @PageNo = 100;

	WITH ListingImagesCTE AS (
		SELECT T.ListingID, T.ImageID, LI.Title, LI.ThumbnailContents, LI.Contents, LI.MimeType, LI.IsPrimary
		FROM (
			SELECT ListingID, ImageID, ROW_NUMBER() OVER (PARTITION BY ListingID ORDER BY IsPrimary DESC, ImageID ASC) AS RowNum
			FROM dbo.tblSiteListingImages
			WHERE Active = 1
		) T
		JOIN dbo.tblSiteListingImages LI ON LI.ListingID = T.ListingID AND LI.ImageID = T.ImageID
		WHERE T.RowNum = 1
	)
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
			, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
			, L.VersionNo, L.VersionNo AS PublishedVersionNo
			, LI.Title AS ImageTitle
				, LI.ThumbnailContents AS ImageThumbnailContents, LI.Contents AS ImageContents
				, LI.MimeType AS ImageMimeType, LI.IsPrimary AS ImageIsPrimary
		FROM dbo.tblSiteListings L
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN dbo.tblMasterMarketingAgents MMA ON MMA.AgentID = L.MarketingAgentID
		LEFT OUTER JOIN ListingImagesCTE LI ON LI.ListingID = L.ListingID
		WHERE (ISNULL(@ListingID, 0) = 0 OR L.ListingID = @ListingID)
			AND L.StatusCD = 'PUBLISHED'
			AND DATEDIFF(DD, L.ListingStartDate, GETDATE()) >= 0
		ORDER BY L.[Name] ASC
		OFFSET (@PageNo - 1) * @PageSize ROWS
		FETCH NEXT @PageSize ROWS ONLY;

END
GO