DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationRetrieveDuplicatesByEmailAddress];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of potential duplicate applications by Listing ID and Email Address
-- Examples:
--	EXEC dbo.uspHousingApplicationRetrieveDuplicatesByEmailAddress @ListingID = 1, @EmailAddress = 'abc@def.com'
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationRetrieveDuplicatesByEmailAddress]
	@ListingID			INT
	, @EmailAddress		VARCHAR(200)
AS
BEGIN
	SET NOCOUNT ON;

	WITH DuplicateApplicationsCTE AS (
		SELECT A.ApplicationID, 'PRIMARY' AS EmailAddressTypeCD, ISNULL(RTRIM(A.EmailAddress), '') AS EmailAddress
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.EmailAddress), '') <> ''
		UNION
		SELECT A.ApplicationID, 'ALTERNATE' AS EmailAddressTypeCD, ISNULL(RTRIM(A.AltEmailAddress), '') AS EmailAddress
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.AltEmailAddress), '') <> ''
	)
		SELECT A.ApplicationID
				, A.Title, A.FirstName, A.MiddleName, A.LastName, A.Suffix
				, A.DateOfBirth, A.Last4SSN
				, A.IDTypeCD, MDIT.[Description] AS IDTypeDescription, A.IDTypeValue, A.IDIssueDate
				, A.GenderCD, A.RaceCD, A.EthnicityCD, A.Pronouns
				, A.StudentInd, A.DisabilityInd, A.VeteranInd
				, A.PhoneNumberTypeCD, A.PhoneNumber, A.PhoneNumberExtn
				, A.AltPhoneNumberTypeCD, A.AltPhoneNumber, A.AltPhoneNumberExtn
				, A.EmailAddress, A.AltEmailAddress
				, A.EverLivedInWestchesterInd, A.CountyLivingIn
				, A.CurrentlyWorkingInWestchesterInd, A.CountyWorkingIn
				, A.OwnRealEstateInd, A.RealEstateValueAmt, A.AssetValueAmt, A.IncomeValueAmt
				, A.LeadTypeCD, A.LeadTypeOther
				, A.SubmissionTypeCD, MDST.[Description] AS SubmissionTypeDescription
				, A.LotteryID, A.LotteryDate, A.LotteryNumber
			, A.ListingID
				, L.ListingTypeCD, MLLTP.[Description] AS ListingTypeDescription
				, L.ResaleInd
				, L.ListingAgeTypeCD, MLLATP.[Description] AS ListingAgeTypeDescription
				, L.[Name], L.[Description]
				, L.StreetLine1, L.StreetLine2, L.StreetLine3, L.City, L.StateCD, L.ZipCode
				, L.Municipality, L.MunicipalityUrl, L.SchoolDistrict, L.SchoolDistrictUrl
				, L.MapUrl, L.EsriX, L.EsriY, L.WebsiteUrl
				, L.ListingStartDate, L.ListingEndDate
				, L.ApplicationStartDate, L.ApplicationEndDate, L.LotteryDate
				, L.WaitlistEligible, L.WaitlistStartDate, L.WaitlistEndDate
				, L.MinHouseholdIncomeAmt, L.MaxHouseholdIncomeAmt
				, L.MinHouseholdSize, L.MaxHouseholdSize
				, L.PetsAllowedInd, L.PetsAllowedText, L.RentIncludesText
				, L.CompletedOrInitialOccupancyYear, L.TermOfAffordability
				, L.StatusCD AS ListingStatusCD, MDLST.[Description] AS ListingStatusDescription
			, A.AddressInd, A.MailingStreetLine1, A.MailingStreetLine2, A.MailingStreetLine3
				, A.MailingCity, A.MailingStateCD, A.MailingZipCode
				, A.DifferentMailingAddressInd, A.PhysicalStreetLine1, A.PhysicalStreetLine2, A.PhysicalStreetLine3
				, A.PhysicalCity, A.PhysicalStateCD, A.PhysicalZipCode
				, A.VoucherInd, A.VoucherCDs, A.VoucherOther, A.VoucherAdminName
				, A.LiveInAideInd
				, A.UnitTypeCDs
			, A.CoApplicantMemberID, A.MemberIDs, A.AccountIDs, A.AccessibilityCDs
			, A.[Username]
			, A.HouseholdID
			, A.StatusCD, MDAPL.[Description] AS StatusDescription
			, A.CreatedBy, A.CreatedDate, A.ModifiedBy, A.ModifiedDate, A.Active
			, A.SubmittedDate, A.WithdrawnDate, A.ReceivedDate
			, A.DuplicateCheckCD, A.DuplicateCheckResponseDueDate
			, A.DisqualifiedInd, A.DisqualificationCD, MDDR.[Description] AS DisqualificationDescription
				, A.DisqualificationOther, A.DisqualificationReason
		FROM dbo.tblHousingApplications A
		JOIN dbo.tblSiteListings L ON L.ListingID = A.ListingID
		LEFT OUTER JOIN dbo.tblMetadata MDLST ON MDLST.CodeID = 106 AND MDLST.Code = L.StatusCD
		LEFT OUTER JOIN dbo.tblMetadata MLLTP ON MLLTP.CodeID = 107 AND MLLTP.Code = L.ListingTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MLLATP ON MLLATP.CodeID = 129 AND MLLATP.Code = L.ListingAgeTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDAPL ON MDAPL.CodeID = 127 AND MDAPL.Code = A.StatusCD
		LEFT OUTER JOIN dbo.tblMetadata MDGND ON MDGND.CodeID = 110 AND MDGND.Code = A.GenderCD
		LEFT OUTER JOIN dbo.tblMetadata MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = A.RaceCD
		LEFT OUTER JOIN dbo.tblMetadata MDETH ON MDETH.CodeID = 112 AND MDETH.Code = A.EthnicityCD
		LEFT OUTER JOIN dbo.tblMetadata MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = A.PhoneNumberTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDAPHT ON MDAPHT.CodeID = 114 AND MDAPHT.Code = A.AltPhoneNumberTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDLT ON MDLT.CodeID = 120 AND MDLT.Code = A.LeadTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDST ON MDST.CodeID = 128 AND MDST.Code = A.SubmissionTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = A.IDTypeCD
		LEFT OUTER JOIN dbo.tblMetadata MDDR ON MDDR.CodeID = 127 AND MDDR.Code = A.DisqualificationCD
		WHERE A.ListingID = @ListingID
			AND A.ApplicationID IN (SELECT DISTINCT ApplicationID FROM DuplicateApplicationsCTE WHERE EmailAddress = @EmailAddress)
		ORDER BY [ApplicationID] ASC;

END
GO