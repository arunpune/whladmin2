DROP PROCEDURE IF EXISTS [dbo].[uspListingPublish];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Publish a listing
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingPublish @ListingID = 1, @StatusCD = 'DRAFT', @Note = 'NOTE'
--									, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingPublish]
	@ListingID				INT
	, @StatusCD				VARCHAR(20)
	, @Note					VARCHAR(MAX) = NULL
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListings
		SET
			StatusCD			= @StatusCD
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE ListingID = @ListingID;

		SET @Note = ISNULL(RTRIM(@Note), 'Published listing');

		DELETE FROM dbo.tblSiteListingUnitHouseholds WHERE UnitID IN (SELECT UnitID FROM dbo.tblSiteListingUnits WHERE ListingID = @ListingID);
		DELETE FROM dbo.tblSiteListingUnits WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingAccessibilities WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingAmenities WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingDeclarations WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingDisclosures WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingDocumentTypes WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingFundingSources WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingImages WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListingDocuments WHERE ListingID = @ListingID;
		DELETE FROM dbo.tblSiteListings WHERE ListingID = @ListingID;

		-- Site listing
		INSERT INTO dbo.tblSiteListings (ListingID, ListingTypeCD, ResaleInd, ListingAgeTypeCD, [Name], [Description]
										, WebsiteUrl
										, StreetLine1, StreetLine2, StreetLine3, City, StateCD, ZipCode, County
										, Municipality, MunicipalityUrl, SchoolDistrict, SchoolDistrictUrl
										, MapUrl, EsriX, EsriY
										, ListingStartDate, ListingEndDate
										, ApplicationStartDate, ApplicationEndDate
										, LotteryEligible, LotteryDate
										, WaitlistEligible, WaitlistStartDate, WaitlistEndDate
										, MinHouseholdIncomeAmt, MaxHouseholdIncomeAmt, MinHouseholdSize, MaxHouseholdSize
										, PetsAllowedInd, PetsAllowedText, RentIncludesText
										, CompletedOrInitialOccupancyYear, TermOfAffordability
										, MarketingAgentInd, MarketingAgentID, MarketingAgentApplicationLink
										, LotteryID
										, StatusCD, VersionNo
										, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ListingID, ListingTypeCD, ResaleInd, ListingAgeTypeCD, [Name], [Description]
				, WebsiteUrl
				, StreetLine1, StreetLine2, StreetLine3, City, StateCD, ZipCode, County
				, Municipality, MunicipalityUrl, SchoolDistrict, SchoolDistrictUrl
				, MapUrl, EsriX, EsriY
				, ListingStartDate, ListingEndDate
				, ApplicationStartDate, ApplicationEndDate
				, LotteryEligible, LotteryDate
				, WaitlistEligible, WaitlistStartDate, WaitlistEndDate
				, MinHouseholdIncomeAmt, MaxHouseholdIncomeAmt, MinHouseholdSize, MaxHouseholdSize
				, PetsAllowedInd, PetsAllowedText, RentIncludesText
				, CompletedOrInitialOccupancyYear, TermOfAffordability
				, MarketingAgentInd, MarketingAgentID, MarketingAgentApplicationLink
				, LotteryID
				, StatusCD, VersionNo
				, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListings
			WHERE ListingID = @ListingID;

		-- Accessibilities
		INSERT INTO dbo.tblSiteListingAccessibilities (ListingID, AccessibilityCD, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ListingID, AccessibilityCD, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingAccessibilities
			WHERE ListingID = @ListingID;

		-- Amenities
		INSERT INTO dbo.tblSiteListingAmenities (ListingID, AmenityID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ListingID, AmenityID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingAmenities
			WHERE ListingID = @ListingID;

		-- Declarations
		INSERT INTO dbo.tblSiteListingDeclarations (DeclarationID, ListingID, [Text], SortOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT DeclarationID, ListingID, [Text], SortOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingDeclarations
			WHERE ListingID = @ListingID;

		-- Disclosures
		INSERT INTO dbo.tblSiteListingDisclosures (DisclosureID, ListingID, [Text], SortOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT DisclosureID, ListingID, [Text], SortOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingDisclosures
			WHERE ListingID = @ListingID;

		-- Document Types
		INSERT INTO dbo.tblSiteListingDocumentTypes (ListingID, DocumentTypeID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ListingID, DocumentTypeID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingDocumentTypes
			WHERE ListingID = @ListingID;

		-- Funding Sources
		INSERT INTO dbo.tblSiteListingFundingSources (ListingID, FundingSourceID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ListingID, FundingSourceID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingFundingSources
			WHERE ListingID = @ListingID;

		-- Images
		INSERT INTO dbo.tblSiteListingImages (ImageID, ListingID, Title, ThumbnailContents, Contents, MimeType
											, IsPrimary, DisplayOnListingsPageInd
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT ImageID, ListingID, Title, ThumbnailContents, Contents, MimeType
				, IsPrimary, DisplayOnListingsPageInd
				, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingImages
			WHERE ListingID = @ListingID AND Active = 1 AND DisplayOnListingsPageInd = 1;

		-- Documents
		INSERT INTO dbo.tblSiteListingDocuments (DocumentID, ListingID, Title, [FileName], Contents, MimeType
													, DisplayOnListingsPageInd
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT DocumentID, ListingID, Title, [FileName], Contents, MimeType
				, DisplayOnListingsPageInd
				, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingDocuments
			WHERE ListingID = @ListingID AND Active = 1 AND DisplayOnListingsPageInd = 1;

		-- Units
		INSERT INTO dbo.tblSiteListingUnits (UnitID, ListingID, UnitTypeCD, BedroomCnt, BathroomCnt, BathroomCntPart, SquareFootage, AreaMedianIncomePct
											, MonthlyRentAmt, AssetLimitAmt, EstimatedPriceAmt, SubsidyAmt
											, MonthlyTaxesAmt, MonthlyMaintenanceAmt, MonthlyInsuranceAmt
											, UnitsAvailableCnt
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT UnitID, ListingID, UnitTypeCD, BedroomCnt, BathroomCnt, BathroomCntPart, SquareFootage, AreaMedianIncomePct
				, MonthlyRentAmt, AssetLimitAmt, EstimatedPriceAmt, SubsidyAmt
				, MonthlyTaxesAmt, MonthlyMaintenanceAmt, MonthlyInsuranceAmt
				, UnitsAvailableCnt
				, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
			FROM dbo.tblListingUnits
			WHERE ListingID = @ListingID;

		-- Unit Households
		INSERT INTO dbo.tblSiteListingUnitHouseholds (UnitHouseholdID, UnitID, HouseholdSize, MinHouseholdIncomeAmt, MaxHouseholdIncomeAmt
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT UH.UnitHouseholdID, UH.UnitID, UH.HouseholdSize, UH.MinHouseholdIncomeAmt, UH.MaxHouseholdIncomeAmt
				, UH.CreatedBy, UH.CreatedDate, UH.ModifiedBy, UH.ModifiedDate, UH.Active
			FROM dbo.tblListingUnitHouseholds UH
			JOIN dbo.tblListingUnits U ON U.UnitID = UH.UnitID
			WHERE U.ListingID = @ListingID;

		-- Audit
		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Status - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO