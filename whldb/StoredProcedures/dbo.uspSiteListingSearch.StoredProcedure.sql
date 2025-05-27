DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingSearch];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Jul-2024
-- Description:	Search for listings
-- Examples:
--	EXEC dbo.uspSiteListingSearch (Retrieve All)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingSearch]
	@ListingTypeOptionCD			VARCHAR(20) = 'BOTH'
	, @City							VARCHAR(100) = 'ALL'
	, @SeniorLivingOptionCD			VARCHAR(20) = 'ALL'
	, @AdaptedForDisabilityOptionCD	VARCHAR(20) = 'ALL'
	, @PetsAllowedOptionCD			VARCHAR(20) = 'ALL'
	, @ListingDateFilterOptionCD	VARCHAR(20) = 'ALL'
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ListingIDs TABLE (
		ListingID				INT
		, ListingTypeCD			VARCHAR(20)
	);

	DECLARE @Today INT, @Timestamp BIGINT;
	SELECT @Today = CONVERT(INT, FORMAT(GETDATE(), 'yyyyMMdd'))
		, @Timestamp = CONVERT(BIGINT, FORMAT(GETDATE(), 'yyyyMMddHHmmss'));

	WITH ListingsAdaptedForDisabilitiesCTE AS (
		SELECT DISTINCT A.ListingID
		FROM dbo.tblSiteListingAccessibilities A
	), ListingDatesCTE AS (
		SELECT ListingID, ListingTypeCD, City, ListingAgeTypeCD, PetsAllowedInd
			, CONVERT(BIGINT, FORMAT(ListingStartDate, 'yyyyMMddHHmmss')) AS ListingStartDate
			, CONVERT(BIGINT, FORMAT(ListingEndDate, 'yyyyMMddHHmmss')) AS ListingEndDate
			, CONVERT(BIGINT, FORMAT(ApplicationStartDate, 'yyyyMMddHHmmss')) AS ApplicationStartDate
			, CONVERT(BIGINT, FORMAT(ApplicationEndDate, 'yyyyMMddHHmmss')) AS ApplicationEndDate
			, WaitlistEligible
			, CONVERT(BIGINT, FORMAT(WaitlistStartDate, 'yyyyMMddHHmmss')) AS WaitlistStartDate
			, CONVERT(BIGINT, FORMAT(WaitlistEndDate, 'yyyyMMddHHmmss')) AS WaitlistEndDate
			, CONVERT(BIGINT, FORMAT(LotteryDate, 'yyyyMMddHHmmss')) AS LotteryDate
		FROM dbo.tblSiteListings
		WHERE StatusCD = 'PUBLISHED'
			AND DATEDIFF(DD, ListingStartDate, GETDATE()) >= 0
			AND (ListingEndDate IS NULL OR DATEDIFF(DD, ListingEndDate, GETDATE()) <= 0)
	)
		INSERT INTO @ListingIDs (ListingID, ListingTypeCD)
			SELECT L.ListingID, L.ListingTypeCD
			FROM ListingDatesCTE L
			WHERE (@ListingTypeOptionCD = 'BOTH' OR L.ListingTypeCD = @ListingTypeOptionCD)
				AND (@City = 'ALL' OR L.City = @City)
				AND 1 = CASE ISNULL(RTRIM(@AdaptedForDisabilityOptionCD), 'ALL')
							WHEN 'ALL' THEN 1
							WHEN 'YES' THEN CASE WHEN L.ListingID IN (SELECT ListingID FROM ListingsAdaptedForDisabilitiesCTE) THEN 1 ELSE 0 END
							WHEN 'NO' THEN CASE WHEN L.ListingID NOT IN (SELECT ListingID FROM ListingsAdaptedForDisabilitiesCTE) THEN 1 ELSE 0 END
							ELSE 0
						END
				AND 1 = CASE ISNULL(RTRIM(@SeniorLivingOptionCD), 'ALL')
							WHEN 'ALL' THEN 1
							WHEN 'YES' THEN CASE WHEN L.ListingAgeTypeCD IN ('55+', '62+') THEN 1 ELSE 0 END
							WHEN 'NO' THEN CASE WHEN L.ListingAgeTypeCD NOT IN ('55+', '62+') THEN 1 ELSE 0 END
							ELSE 0
						END
				AND 1 = CASE ISNULL(RTRIM(@PetsAllowedOptionCD), 'ALL')
							WHEN 'ALL' THEN 1
							WHEN 'YES' THEN CASE WHEN L.PetsAllowedInd = 1 THEN 1 ELSE 0 END
							WHEN 'NO' THEN CASE WHEN L.PetsAllowedInd = 0 THEN 1 ELSE 0 END
							ELSE 0
						END
				AND 1 = CASE ISNULL(RTRIM(@ListingDateFilterOptionCD), 'ALL')
							WHEN 'ALL' THEN 1
							WHEN 'CMNGOPPR' THEN CASE WHEN @Timestamp <= L.ApplicationStartDate THEN 1 ELSE 0 END
							WHEN 'ACPTAPPL' THEN CASE WHEN @Timestamp > L.ApplicationStartDate AND (L.ApplicationEndDate IS NULL OR @Timestamp <= L.ApplicationEndDate) THEN 1 ELSE 0 END
							WHEN 'WAITLIST' THEN CASE WHEN L.WaitlistEligible = 1 AND (L.WaitlistStartDate IS NULL OR @Timestamp >= L.WaitlistStartDate) AND (L.WaitlistEndDate IS NULL OR @Timestamp <= L.WaitlistEndDate) THEN 1 ELSE 0 END
							WHEN 'PASTOPPR' THEN CASE WHEN (L.WaitlistEligible = 0 AND @Timestamp > L.ApplicationEndDate) OR (L.WaitlistEligible = 1 AND @Timestamp > L.WaitlistEndDate) THEN 1 ELSE 0 END
							ELSE 0
						END;

	WITH ListingImagesCTE AS (
		SELECT T.ListingID, T.ImageID, LI.Title, LI.ThumbnailContents, LI.Contents, LI.MimeType, LI.IsPrimary
		FROM (
			SELECT I.ListingID, I.ImageID, ROW_NUMBER() OVER (PARTITION BY I.ListingID ORDER BY I.IsPrimary DESC, I.ImageID ASC) AS RowNum
			FROM dbo.tblSiteListingImages I
			JOIN @ListingIDs T ON T.ListingID = I.ListingID
			WHERE I.Active = 1
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
			, L.StatusCD, MLS.[Description] AS StatusDescription
			, L.CreatedDate, L.CreatedBy, L.ModifiedDate, L.ModifiedBy, L.Active
			, L.VersionNo, L.VersionNo AS PublishedVersionNo
			, LI.Title AS ImageTitle
				, LI.ThumbnailContents AS ImageThumbnailContents, LI.Contents AS ImageContents
				, LI.MimeType AS ImageMimeType, LI.IsPrimary AS ImageIsPrimary
		FROM dbo.tblSiteListings L
		JOIN @ListingIDs C ON C.ListingID = L.ListingID AND C.ListingTypeCD = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLT ON MLT.CodeID = 107 AND MLT.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLAT ON MLAT.CodeID = 129 AND MLAT.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLS ON MLS.CodeID = 106 AND MLS.Code = L.StatusCD
		LEFT OUTER JOIN ListingImagesCTE LI ON LI.ListingID = L.ListingID
		ORDER BY L.ApplicationEndDate ASC, L.WaitlistEndDate ASC;

END
GO