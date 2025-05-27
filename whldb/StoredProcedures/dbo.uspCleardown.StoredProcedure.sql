DROP PROCEDURE IF EXISTS [dbo].[uspCleardown];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Jun-2024
-- Description:	Perform a cleardown
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspCleardown @ListingData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Only listing data
--	EXEC dbo.uspCleardown @MasterData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Only master data
--	EXEC dbo.uspCleardown @UserData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Only user data
--	EXEC dbo.uspCleardown @ListingData = 1, @MasterData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Only listing and master data
--	EXEC dbo.uspCleardown @SiteApplicationData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- Only site application data
--	EXEC dbo.uspCleardown @SiteListingData = 1, @SiteUserData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- All site listing and site user data
--	EXEC dbo.uspCleardown @ListingData = 1, @MasterData = 1, @UserData = 1, @SiteListingData = 1, @SiteUserData = 1, @ErrorMessage = @ErrorMessage OUTPUT -- All data
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspCleardown]
	@ListingData			BIT = 0
	, @MasterData			BIT = 0
	, @UserData				BIT = 0
	, @SiteListingData		BIT = 0
	, @SiteApplicationData	BIT = 0
	, @SiteUserData			BIT = 0
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @AuditNote VARCHAR(MAX);
	SET @AuditNote = CASE WHEN @ListingData = 1 THEN 'Listing Data' ELSE '' END;
	SET @AuditNote = @AuditNote + CASE WHEN LEN(@AuditNote) > 0 THEN ', ' ELSE '' END + CASE WHEN @MasterData = 1 THEN 'Master Data' ELSE '' END;
	SET @AuditNote = @AuditNote + CASE WHEN LEN(@AuditNote) > 0 THEN ', ' ELSE '' END + CASE WHEN @UserData = 1 THEN 'User Data' ELSE '' END;
	IF @ListingData = 0 AND @MasterData = 0 AND @UserData = 0 RETURN;

	BEGIN TRY

		BEGIN TRAN

		IF @ListingData = 1 AND @MasterData = 1 AND @UserData = 1 AND @SiteListingData = 1 AND @SiteUserData = 1
			TRUNCATE TABLE dbo.tblAudit;

		IF @SiteUserData = 1 OR @SiteApplicationData = 1
		BEGIN
			DELETE FROM dbo.tblAudit WHERE EntityTypeCD IN ('APPLICATION', 'LOTTERY');
			TRUNCATE TABLE dbo.tblLotteryResults;
			TRUNCATE TABLE dbo.tblLotteries;
			TRUNCATE TABLE dbo.tblHousingApplicants;
			TRUNCATE TABLE dbo.tblHousingApplicationDocumentContents;
			TRUNCATE TABLE dbo.tblHousingApplicationDocuments;
			TRUNCATE TABLE dbo.tblHousingApplicationComments;
			TRUNCATE TABLE dbo.tblHousingApplications;
		END

		IF @SiteUserData = 1
		BEGIN
			DELETE FROM dbo.tblAudit WHERE EntityTypeCD IN ('SITEUSER', 'HOUSEHOLD', 'HOUSEHOLDACT', 'HOUSEHOLDMBR');
			TRUNCATE TABLE dbo.tblHouseholdAccounts;
			TRUNCATE TABLE dbo.tblHouseholdMembers;
			TRUNCATE TABLE dbo.tblHouseholds;
			TRUNCATE TABLE dbo.tblSiteUserNotifications;
			TRUNCATE TABLE dbo.tblSiteUserDocumentContents;
			TRUNCATE TABLE dbo.tblSiteUserDocuments;
			TRUNCATE TABLE dbo.tblSiteUsers;
		END

		IF @SiteListingData = 1 OR @ListingData = 1
		BEGIN
			TRUNCATE TABLE dbo.tblSiteListingUnitHouseholds;
			TRUNCATE TABLE dbo.tblSiteListingUnits;
			TRUNCATE TABLE dbo.tblSiteListingImages;
			TRUNCATE TABLE dbo.tblSiteListingFundingSources;
			TRUNCATE TABLE dbo.tblSiteListingDocumentTypes;
			TRUNCATE TABLE dbo.tblSiteListingDocuments;
			TRUNCATE TABLE dbo.tblSiteListingDisclosures;
			TRUNCATE TABLE dbo.tblSiteListingDeclarations;
			TRUNCATE TABLE dbo.tblSiteListingAmenities;
			TRUNCATE TABLE dbo.tblSiteListingAccessibilities;
			TRUNCATE TABLE dbo.tblSiteListings;
		END

		IF @ListingData = 1
		BEGIN
			DELETE FROM dbo.tblAudit WHERE EntityTypeCD IN ('UNIT', 'LISTING', 'IMAGE', 'LISTINGIMAGE', 'LISTINGDOCUMENT', 'LISTINGDECL', 'LISTINGDSCL');
			TRUNCATE TABLE dbo.tblListingUnitHouseholds;
			TRUNCATE TABLE dbo.tblListingUnits;
			TRUNCATE TABLE dbo.tblListingImages;
			TRUNCATE TABLE dbo.tblListingFundingSources;
			TRUNCATE TABLE dbo.tblListingDocumentTypes;
			TRUNCATE TABLE dbo.tblListingDocuments;
			TRUNCATE TABLE dbo.tblListingDisclosures;
			TRUNCATE TABLE dbo.tblListingDeclarations;
			TRUNCATE TABLE dbo.tblListingAmenities;
			TRUNCATE TABLE dbo.tblListingAccessibilities;
			TRUNCATE TABLE dbo.tblListings;
		END

		IF @MasterData = 1
		BEGIN
			DELETE FROM dbo.tblAudit WHERE EntityTypeCD IN ('AMENITY', 'AMI', 'AMORTIZATION', 'FAQ', 'MKTAGENT', 'NOTIFICATION', 'QUESTION', 'QUOTE', 'RESOURCE', 'VIDEO');
			TRUNCATE TABLE dbo.tblMasterAmenities;
			TRUNCATE TABLE dbo.tblMasterAMIs;
			TRUNCATE TABLE dbo.tblMasterAmortizations;
			TRUNCATE TABLE dbo.tblMasterFAQs;
			TRUNCATE TABLE dbo.tblMasterMarketingAgents;
			TRUNCATE TABLE dbo.tblMasterNotifications;
			TRUNCATE TABLE dbo.tblMasterQuestions;
			TRUNCATE TABLE dbo.tblMasterQuotes;
			TRUNCATE TABLE dbo.tblMasterResources;
			TRUNCATE TABLE dbo.tblMasterVideos;
		END

		IF @UserData = 1
		BEGIN
			DELETE FROM dbo.tblAudit WHERE EntityTypeCD IN ('ADMINUSER');
			TRUNCATE TABLE dbo.tblAdminUsers;
		END

		SET @AuditNote = 'Performed cleardown for - ' + @AuditNote;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('CLEARDOWN', '999999999', 'SYSTEM', 'DELETE', @AuditNote, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to perform cleardown - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO