DROP PROCEDURE IF EXISTS [dbo].[uspLotteryRetrievePaged];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Aug-2024
-- Description:	Retrieve a list of lotteries (optionally by ListingID), or a single one by LotteryID
-- Examples:
--	EXEC dbo.uspLotteryRetrievePaged (Retrieve All)
--	EXEC dbo.uspLotteryRetrievePaged @ListingID = 1 (Retrieve All By Listing)
--	EXEC dbo.uspLotteryRetrievePaged @LotteryID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspLotteryRetrievePaged]
	@ListingID		INT = 0
	, @LotteryID	INT = 0
	, @PageNo		INT = 1
	, @PageSize		INT = 100
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@PageNo, 1) < 1 SET @PageNo = 1;
	IF ISNULL(@PageSize, 100) < 100 SET @PageSize = 100;

	IF ISNULL(@LotteryID, 0) > 0
	BEGIN

		SELECT LOT.LotteryID, LOT.ListingID, LOT.ManualInd, LOT.RunDate, LOT.RunBy
			, LOT.StatusCD AS LotteryStatusCD, MLOTS.[Description] AS LotteryStatusDescription
			, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
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
			, L.StatusCD, MLS.[Description] AS StatusDescription
			, L.VersionNo, COALESCE(SL.VersionNo, L.VersionNo) AS PublishedVersionNo
			, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
		FROM dbo.tblLotteries LOT
		JOIN dbo.tblListings L ON L.ListingID = LOT.ListingID
		LEFT OUTER JOIN dbo.tblSiteListings SL ON SL.ListingID = L.ListingID
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN dbo.tblMetadata MLOTS ON MLOTS.CodeID = 127 AND MLOTS.Code = LOT.StatusCD
		WHERE LOT.LotteryID = @LotteryID

		RETURN;
	END

	SELECT LOT.LotteryID, LOT.ListingID, LOT.ManualInd, LOT.RunDate, LOT.RunBy
		, LOT.StatusCD AS LotteryStatusCD, MLOTS.[Description] AS LotteryStatusDescription
		, L.ListingTypeCD, MLT.[Description] AS ListingTypeDescription
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
		, L.StatusCD, MLS.[Description] AS StatusDescription
		, L.VersionNo, COALESCE(SL.VersionNo, L.VersionNo) AS PublishedVersionNo
		, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
	FROM dbo.tblLotteries LOT
	JOIN dbo.tblListings L ON L.ListingID = LOT.ListingID
	LEFT OUTER JOIN dbo.tblSiteListings SL ON SL.ListingID = L.ListingID
	LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
	LEFT OUTER JOIN dbo.tblMetadata MLOTS ON MLOTS.CodeID = 127 AND MLOTS.Code = LOT.StatusCD
	WHERE LOT.ListingID = @ListingID
	ORDER BY LOT.LotteryID ASC
	OFFSET (@PageNo - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY;

	SELECT @PageNo AS PageNo
		, @PageSize AS PageSize
		, CEILING(COUNT(1) / @PageSize) + 1 AS TotalPages
		, COUNT(1) AS TotalRecords
	FROM dbo.tblLotteries LOT
	JOIN dbo.tblListings L ON L.ListingID = LOT.ListingID
	WHERE LOT.ListingID = @ListingID;

END
GO