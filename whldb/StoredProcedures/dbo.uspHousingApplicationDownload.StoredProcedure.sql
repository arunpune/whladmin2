DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationDownload];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Mar-2025
-- Description:	Download a list of housing applications by listing ID
-- Examples:
--	EXEC dbo.uspHousingApplicationDownload @ListingID = 1 (Retrieve All Applications)
--	EXEC dbo.uspHousingApplicationDownload @ListingID = 1, @SubmissionTypeCD = 'ONLINE' (Retrieve All Online Applications)
--	EXEC dbo.uspHousingApplicationDownload @ListingID = 1, @SubmissionTypeCD = 'PAPER' (Download All Paper Applications)
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationDownload]
	@ListingID			INT
	, @SubmissionTypeCD	VARCHAR(20) = 'ALL'
	, @StatusCD			VARCHAR(20) = 'ALL'
AS
BEGIN
	SET NOCOUNT ON;

		SELECT HA.ApplicationID, HA.Username
			, RIGHT(ISNULL(RTRIM(HA.LotteryNumber), '      '), 6) AS LotteryNumber, HA.LotteryID
			, HA.SubmissionTypeCD, HA.SubmittedDate, HA.StatusCD
			, HA.UnitTypeCDs, HA.AccessibilityCDs
			, HA.LeadTypeCD, HA.LeadTypeOther
			, PA.Title AS ApplicantTitle, PA.FirstName AS ApplicantFirstName, PA.MiddleName AS ApplicantMiddleName, PA.LastName AS ApplicantLastName, PA.Suffix AS ApplicantSuffix
				, PA.DateOfBirth AS ApplicantDateOfBirth, PA.Last4SSN AS ApplicantLast4SSN
				, PA.IDTypeCD AS ApplicantIDTypeCD, PA.IDTypeDescription AS ApplicantIDTypeDescription, PA.IDTypeValue AS ApplicantIDTypeValue, PA.IDIssueDate AS ApplicantIDIssueDate
				, PA.GenderCD AS ApplicantGenderCD, PA.GenderDescription AS ApplicantGenderDescription
				, PA.RaceCD AS ApplicantRaceCD, PA.RaceDescription AS ApplicantRaceDescription
				, PA.EthnicityCD AS ApplicantEthnicityCD, PA.EthnicityDescription AS ApplicantEthnicityDescription
				, PA.Pronouns AS ApplicantPronouns
				, CASE WHEN PA.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantStudentInd
				, CASE WHEN PA.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantDisabilityInd
				, CASE WHEN PA.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantVeteranInd
				, CASE WHEN PA.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantEverLivedInWestchesterInd
				, PA.CountyLivingIn AS ApplicantCountyLivingIn
				, CASE WHEN PA.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantEverLivedInWestchesterInd
				, PA.CountyWorkingIn AS ApplicantCountyWorkingIn
				, PA.PhoneNumber AS ApplicantPhoneNumber, PA.PhoneNumberExtn AS ApplicantPhoneNumberExtn, PA.PhoneNumberTypeCD AS ApplicantPhoneNumberTypeCD
				, PA.AltPhoneNumber AS ApplicantAltPhoneNumber, PA.AltPhoneNumberExtn AS ApplicantAltPhoneNumberExtn, PA.AltPhoneNumberTypeCD AS ApplicantAltPhoneNumberTypeCD
				, PA.EmailAddress AS ApplicantEmailAddress, PA.AltEmailAddress AS ApplicantAltEmailAddress
				, ISNULL(PA.IncomeValueAmt, 0) AS ApplicantIncomeValueAmt, ISNULL(PA.AssetValueAmt, 0) AS ApplicantAssetValueAmt
				, CASE WHEN PA.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS ApplicantOwnRealEstateInd, ISNULL(PA.RealEstateValueAmt, 0) AS ApplicantRealEstateValueAmt
			, HA.PhysicalStreetLine1, HA.PhysicalStreetLine2, HA.PhysicalStreetLine3
				, HA.PhysicalCity, HA.PhysicalStateCD, HA.PhysicalZipCode, HA.PhysicalCounty
			, HA.MailingStreetLine1, HA.MailingStreetLine2, HA.MailingStreetLine3
				, HA.MailingCity, HA.MailingStateCD, HA.MailingZipCode, HA.MailingCounty
			, CA.RelationTypeCD AS CoApplicantRelationTypeCD
				, CA.RelationTypeDescription AS CoApplicantRelationTypeDescription
				, CA.RelationTypeOther AS CoApplicantRelationTypeOther
				, CA.Title AS CoApplicantTitle, CA.FirstName AS CoApplicantFirstName, CA.MiddleName AS CoApplicantMiddleName, CA.LastName AS CoApplicantLastName, CA.Suffix AS CoApplicantSuffix
				, CA.DateOfBirth AS CoApplicantDateOfBirth, CA.Last4SSN AS CoApplicantLast4SSN
				, CA.IDTypeCD AS CoApplicantIDTypeCD, CA.IDTypeDescription AS CoApplicantIDTypeDescription, CA.IDTypeValue AS CoApplicantIDTypeValue, CA.IDIssueDate AS CoApplicantIDIssueDate
				, CA.GenderCD AS CoApplicantGenderCD, CA.GenderDescription AS CoApplicantGenderDescription
				, CA.RaceCD AS CoApplicantRaceCD, CA.RaceDescription AS CoApplicantRaceDescription
				, CA.EthnicityCD AS CoApplicantEthnicityCD, CA.EthnicityDescription AS CoApplicantEthnicityDescription
				, CA.Pronouns AS CoApplicantPronouns
				, CASE WHEN CA.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantStudentInd
				, CASE WHEN CA.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantDisabilityInd
				, CASE WHEN CA.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantVeteranInd
				, CASE WHEN CA.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantEverLivedInWestchesterInd
				, CA.CountyLivingIn AS CoApplicantCountyLivingIn
				, CASE WHEN CA.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantEverLivedInWestchesterInd
				, CA.CountyWorkingIn AS CoApplicantCountyWorkingIn
				, CA.PhoneNumber AS CoApplicantPhoneNumber, CA.PhoneNumberExtn AS CoApplicantPhoneNumberExtn, CA.PhoneNumberTypeCD AS CoApplicantPhoneNumberTypeCD
				, CA.AltPhoneNumber AS CoApplicantAltPhoneNumber, CA.AltPhoneNumberExtn AS CoApplicantAltPhoneNumberExtn, CA.AltPhoneNumberTypeCD AS CoApplicantAltPhoneNumberTypeCD
				, CA.EmailAddress AS CoApplicantEmailAddress, CA.AltEmailAddress AS CoApplicantAltEmailAddress
				, ISNULL(CA.IncomeValueAmt, 0) AS CoApplicantIncomeValueAmt, ISNULL(CA.AssetValueAmt, 0) AS CoApplicantAssetValueAmt
				, CASE WHEN CA.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS CoApplicantOwnRealEstateInd, ISNULL(CA.RealEstateValueAmt, 0) AS CoApplicantRealEstateValueAmt
			, M3.RelationTypeCD AS Member3RelationTypeCD
				, M3.RelationTypeDescription AS Member3RelationTypeDescription
				, M3.RelationTypeOther AS Member3RelationTypeOther
				, M3.Title AS Member3Title, M3.FirstName AS Member3FirstName, M3.MiddleName AS Member3MiddleName, M3.LastName AS Member3LastName, M3.Suffix AS Member3Suffix
				, M3.DateOfBirth AS Member3DateOfBirth, M3.Last4SSN AS Member3Last4SSN
				, M3.IDTypeCD AS Member3IDTypeCD, M3.IDTypeDescription AS Member3IDTypeDescription, M3.IDTypeValue AS Member3IDTypeValue, M3.IDIssueDate AS Member3IDIssueDate
				, M3.GenderCD AS Member3GenderCD, M3.GenderDescription AS Member3GenderDescription
				, M3.RaceCD AS Member3RaceCD, M3.RaceDescription AS Member3RaceDescription
				, M3.EthnicityCD AS Member3EthnicityCD, M3.EthnicityDescription AS Member3EthnicityDescription
				, M3.Pronouns AS Member3Pronouns
				, CASE WHEN M3.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member3StudentInd
				, CASE WHEN M3.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member3DisabilityInd
				, CASE WHEN M3.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member3VeteranInd
				, CASE WHEN M3.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member3EverLivedInWestchesterInd
				, M3.CountyLivingIn AS Member3CountyLivingIn
				, CASE WHEN M3.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member3EverLivedInWestchesterInd
				, M3.CountyWorkingIn AS Member3CountyWorkingIn
				, M3.PhoneNumber AS Member3PhoneNumber, M3.PhoneNumberExtn AS Member3PhoneNumberExtn, M3.PhoneNumberTypeCD AS Member3PhoneNumberTypeCD
				, M3.AltPhoneNumber AS Member3AltPhoneNumber, M3.AltPhoneNumberExtn AS Member3AltPhoneNumberExtn, M3.AltPhoneNumberTypeCD AS Member3AltPhoneNumberTypeCD
				, M3.EmailAddress AS Member3EmailAddress, M3.AltEmailAddress AS Member3AltEmailAddress
				, ISNULL(M3.IncomeValueAmt, 0) AS Member3IncomeValueAmt, ISNULL(M3.AssetValueAmt, 0) AS Member3AssetValueAmt
				, CASE WHEN M3.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member3OwnRealEstateInd, ISNULL(M3.RealEstateValueAmt, 0) AS Member3RealEstateValueAmt
			, M4.RelationTypeCD AS Member4RelationTypeCD
				, M4.RelationTypeDescription AS Member4RelationTypeDescription
				, M4.RelationTypeOther AS Member4RelationTypeOther
				, M4.Title AS Member4Title, M4.FirstName AS Member4FirstName, M4.MiddleName AS Member4MiddleName, M4.LastName AS Member4LastName, M4.Suffix AS Member4Suffix
				, M4.DateOfBirth AS Member4DateOfBirth, M4.Last4SSN AS Member4Last4SSN
				, M4.IDTypeCD AS Member4IDTypeCD, M4.IDTypeDescription AS Member4IDTypeDescription, M4.IDTypeValue AS Member4IDTypeValue, M4.IDIssueDate AS Member4IDIssueDate
				, M4.GenderCD AS Member4GenderCD, M4.GenderDescription AS Member4GenderDescription
				, M4.RaceCD AS Member4RaceCD, M4.RaceDescription AS Member4RaceDescription
				, M4.EthnicityCD AS Member4EthnicityCD, M4.EthnicityDescription AS Member4EthnicityDescription
				, M4.Pronouns AS Member4Pronouns
				, CASE WHEN M4.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member4StudentInd
				, CASE WHEN M4.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member4DisabilityInd
				, CASE WHEN M4.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member4VeteranInd
				, CASE WHEN M4.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member4EverLivedInWestchesterInd
				, M4.CountyLivingIn AS Member4CountyLivingIn
				, CASE WHEN M4.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member4EverLivedInWestchesterInd
				, M4.CountyWorkingIn AS Member4CountyWorkingIn
				, M4.PhoneNumber AS Member4PhoneNumber, M4.PhoneNumberExtn AS Member4PhoneNumberExtn, M4.PhoneNumberTypeCD AS Member4PhoneNumberTypeCD
				, M4.AltPhoneNumber AS Member4AltPhoneNumber, M4.AltPhoneNumberExtn AS Member4AltPhoneNumberExtn, M4.AltPhoneNumberTypeCD AS Member4AltPhoneNumberTypeCD
				, M4.EmailAddress AS Member4EmailAddress, M4.AltEmailAddress AS Member4AltEmailAddress
				, ISNULL(M4.IncomeValueAmt, 0) AS Member4IncomeValueAmt, ISNULL(M4.AssetValueAmt, 0) AS Member4AssetValueAmt
				, CASE WHEN M4.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member4OwnRealEstateInd, ISNULL(M4.RealEstateValueAmt, 0) AS Member4RealEstateValueAmt
			, M5.RelationTypeCD AS Member5RelationTypeCD
				, M5.RelationTypeDescription AS Member5RelationTypeDescription
				, M5.RelationTypeOther AS Member5RelationTypeOther
				, M5.Title AS Member5Title, M5.FirstName AS Member5FirstName, M5.MiddleName AS Member5MiddleName, M5.LastName AS Member5LastName, M5.Suffix AS Member5Suffix
				, M5.DateOfBirth AS Member5DateOfBirth, M5.Last4SSN AS Member5Last4SSN
				, M5.IDTypeCD AS Member5IDTypeCD, M5.IDTypeDescription AS Member5IDTypeDescription, M5.IDTypeValue AS Member5IDTypeValue, M5.IDIssueDate AS Member5IDIssueDate
				, M5.GenderCD AS Member5GenderCD, M5.GenderDescription AS Member5GenderDescription
				, M5.RaceCD AS Member5RaceCD, M5.RaceDescription AS Member5RaceDescription
				, M5.EthnicityCD AS Member5EthnicityCD, M5.EthnicityDescription AS Member5EthnicityDescription
				, M5.Pronouns AS Member5Pronouns
				, CASE WHEN M5.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member5StudentInd
				, CASE WHEN M5.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member5DisabilityInd
				, CASE WHEN M5.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member5VeteranInd
				, CASE WHEN M5.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member5EverLivedInWestchesterInd
				, M5.CountyLivingIn AS Member5CountyLivingIn
				, CASE WHEN M5.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member5EverLivedInWestchesterInd
				, M5.CountyWorkingIn AS Member5CountyWorkingIn
				, M5.PhoneNumber AS Member5PhoneNumber, M5.PhoneNumberExtn AS Member5PhoneNumberExtn, M5.PhoneNumberTypeCD AS Member5PhoneNumberTypeCD
				, M5.AltPhoneNumber AS Member5AltPhoneNumber, M5.AltPhoneNumberExtn AS Member5AltPhoneNumberExtn, M5.AltPhoneNumberTypeCD AS Member5AltPhoneNumberTypeCD
				, M5.EmailAddress AS Member5EmailAddress, M5.AltEmailAddress AS Member5AltEmailAddress
				, ISNULL(M5.IncomeValueAmt, 0) AS Member5IncomeValueAmt, ISNULL(M5.AssetValueAmt, 0) AS Member5AssetValueAmt
				, CASE WHEN M5.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member5OwnRealEstateInd, ISNULL(M5.RealEstateValueAmt, 0) AS Member5RealEstateValueAmt
			, M6.RelationTypeCD AS Member6RelationTypeCD
				, M6.RelationTypeDescription AS Member6RelationTypeDescription
				, M6.RelationTypeOther AS Member6RelationTypeOther
				, M6.Title AS Member6Title, M6.FirstName AS Member6FirstName, M6.MiddleName AS Member6MiddleName, M6.LastName AS Member6LastName, M6.Suffix AS Member6Suffix
				, M6.DateOfBirth AS Member6DateOfBirth, M6.Last4SSN AS Member6Last4SSN
				, M6.IDTypeCD AS Member6IDTypeCD, M6.IDTypeDescription AS Member6IDTypeDescription, M6.IDTypeValue AS Member6IDTypeValue, M6.IDIssueDate AS Member6IDIssueDate
				, M6.GenderCD AS Member6GenderCD, M6.GenderDescription AS Member6GenderDescription
				, M6.RaceCD AS Member6RaceCD, M6.RaceDescription AS Member6RaceDescription
				, M6.EthnicityCD AS Member6EthnicityCD, M6.EthnicityDescription AS Member6EthnicityDescription
				, M6.Pronouns AS Member6Pronouns
				, CASE WHEN M6.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member6StudentInd
				, CASE WHEN M6.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member6DisabilityInd
				, CASE WHEN M6.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member6VeteranInd
				, CASE WHEN M6.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member6EverLivedInWestchesterInd
				, M6.CountyLivingIn AS Member6CountyLivingIn
				, CASE WHEN M6.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member6EverLivedInWestchesterInd
				, M6.CountyWorkingIn AS Member6CountyWorkingIn
				, M6.PhoneNumber AS Member6PhoneNumber, M6.PhoneNumberExtn AS Member6PhoneNumberExtn, M6.PhoneNumberTypeCD AS Member6PhoneNumberTypeCD
				, M6.AltPhoneNumber AS Member6AltPhoneNumber, M6.AltPhoneNumberExtn AS Member6AltPhoneNumberExtn, M6.AltPhoneNumberTypeCD AS Member6AltPhoneNumberTypeCD
				, M6.EmailAddress AS Member6EmailAddress, M6.AltEmailAddress AS Member6AltEmailAddress
				, ISNULL(M6.IncomeValueAmt, 0) AS Member6IncomeValueAmt, ISNULL(M6.AssetValueAmt, 0) AS Member6AssetValueAmt
				, CASE WHEN M6.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member6OwnRealEstateInd, ISNULL(M6.RealEstateValueAmt, 0) AS Member6RealEstateValueAmt
			, M7.RelationTypeCD AS Member7RelationTypeCD
				, M7.RelationTypeDescription AS Member7RelationTypeDescription
				, M7.RelationTypeOther AS Member7RelationTypeOther
				, M7.Title AS Member7Title, M7.FirstName AS Member7FirstName, M7.MiddleName AS Member7MiddleName, M7.LastName AS Member7LastName, M7.Suffix AS Member7Suffix
				, M7.DateOfBirth AS Member7DateOfBirth, M7.Last4SSN AS Member7Last4SSN
				, M7.IDTypeCD AS Member7IDTypeCD, M7.IDTypeDescription AS Member7IDTypeDescription, M7.IDTypeValue AS Member7IDTypeValue, M7.IDIssueDate AS Member7IDIssueDate
				, M7.GenderCD AS Member7GenderCD, M7.GenderDescription AS Member7GenderDescription
				, M7.RaceCD AS Member7RaceCD, M7.RaceDescription AS Member7RaceDescription
				, M7.EthnicityCD AS Member7EthnicityCD, M7.EthnicityDescription AS Member7EthnicityDescription
				, M7.Pronouns AS Member7Pronouns
				, CASE WHEN M7.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member7StudentInd
				, CASE WHEN M7.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member7DisabilityInd
				, CASE WHEN M7.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member7VeteranInd
				, CASE WHEN M7.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member7EverLivedInWestchesterInd
				, M7.CountyLivingIn AS Member7CountyLivingIn
				, CASE WHEN M7.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member7EverLivedInWestchesterInd
				, M7.CountyWorkingIn AS Member7CountyWorkingIn
				, M7.PhoneNumber AS Member7PhoneNumber, M7.PhoneNumberExtn AS Member7PhoneNumberExtn, M7.PhoneNumberTypeCD AS Member7PhoneNumberTypeCD
				, M7.AltPhoneNumber AS Member7AltPhoneNumber, M7.AltPhoneNumberExtn AS Member7AltPhoneNumberExtn, M7.AltPhoneNumberTypeCD AS Member7AltPhoneNumberTypeCD
				, M7.EmailAddress AS Member7EmailAddress, M7.AltEmailAddress AS Member7AltEmailAddress
				, ISNULL(M7.IncomeValueAmt, 0) AS Member7IncomeValueAmt, ISNULL(M7.AssetValueAmt, 0) AS Member7AssetValueAmt
				, CASE WHEN M7.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member7OwnRealEstateInd, ISNULL(M7.RealEstateValueAmt, 0) AS Member7RealEstateValueAmt
			, M8.RelationTypeCD AS Member8RelationTypeCD
				, M8.RelationTypeDescription AS Member8RelationTypeDescription
				, M8.RelationTypeOther AS Member8RelationTypeOther
				, M8.Title AS Member8Title, M8.FirstName AS Member8FirstName, M8.MiddleName AS Member8MiddleName, M8.LastName AS Member8LastName, M8.Suffix AS Member8Suffix
				, M8.DateOfBirth AS Member8DateOfBirth, M8.Last4SSN AS Member8Last4SSN
				, M8.IDTypeCD AS Member8IDTypeCD, M8.IDTypeDescription AS Member8IDTypeDescription, M8.IDTypeValue AS Member8IDTypeValue, M8.IDIssueDate AS Member8IDIssueDate
				, M8.GenderCD AS Member8GenderCD, M8.GenderDescription AS Member8GenderDescription
				, M8.RaceCD AS Member8RaceCD, M8.RaceDescription AS Member8RaceDescription
				, M8.EthnicityCD AS Member8EthnicityCD, M8.EthnicityDescription AS Member8EthnicityDescription
				, M8.Pronouns AS Member8Pronouns
				, CASE WHEN M8.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member8StudentInd
				, CASE WHEN M8.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member8DisabilityInd
				, CASE WHEN M8.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member8VeteranInd
				, CASE WHEN M8.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member8EverLivedInWestchesterInd
				, M8.CountyLivingIn AS Member8CountyLivingIn
				, CASE WHEN M8.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member8EverLivedInWestchesterInd
				, M8.CountyWorkingIn AS Member8CountyWorkingIn
				, M8.PhoneNumber AS Member8PhoneNumber, M8.PhoneNumberExtn AS Member8PhoneNumberExtn, M8.PhoneNumberTypeCD AS Member8PhoneNumberTypeCD
				, M8.AltPhoneNumber AS Member8AltPhoneNumber, M8.AltPhoneNumberExtn AS Member8AltPhoneNumberExtn, M8.AltPhoneNumberTypeCD AS Member8AltPhoneNumberTypeCD
				, M8.EmailAddress AS Member8EmailAddress, M8.AltEmailAddress AS Member8AltEmailAddress
				, ISNULL(M8.IncomeValueAmt, 0) AS Member8IncomeValueAmt, ISNULL(M8.AssetValueAmt, 0) AS Member8AssetValueAmt
				, CASE WHEN M8.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member8OwnRealEstateInd, ISNULL(M8.RealEstateValueAmt, 0) AS Member8RealEstateValueAmt
			, M9.RelationTypeCD AS Member9RelationTypeCD
				, M9.RelationTypeDescription AS Member9RelationTypeDescription
				, M9.RelationTypeOther AS Member9RelationTypeOther
				, M9.Title AS Member9Title, M9.FirstName AS Member9FirstName, M9.MiddleName AS Member9MiddleName, M9.LastName AS Member9LastName, M9.Suffix AS Member9Suffix
				, M9.DateOfBirth AS Member9DateOfBirth, M9.Last4SSN AS Member9Last4SSN
				, M9.IDTypeCD AS Member9IDTypeCD, M9.IDTypeDescription AS Member9IDTypeDescription, M9.IDTypeValue AS Member9IDTypeValue, M9.IDIssueDate AS Member9IDIssueDate
				, M9.GenderCD AS Member9GenderCD, M9.GenderDescription AS Member9GenderDescription
				, M9.RaceCD AS Member9RaceCD, M9.RaceDescription AS Member9RaceDescription
				, M9.EthnicityCD AS Member9EthnicityCD, M9.EthnicityDescription AS Member9EthnicityDescription
				, M9.Pronouns AS Member9Pronouns
				, CASE WHEN M9.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member9StudentInd
				, CASE WHEN M9.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member9DisabilityInd
				, CASE WHEN M9.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member9VeteranInd
				, CASE WHEN M9.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member9EverLivedInWestchesterInd
				, M9.CountyLivingIn AS Member9CountyLivingIn
				, CASE WHEN M9.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member9EverLivedInWestchesterInd
				, M9.CountyWorkingIn AS Member9CountyWorkingIn
				, M9.PhoneNumber AS Member9PhoneNumber, M9.PhoneNumberExtn AS Member9PhoneNumberExtn, M9.PhoneNumberTypeCD AS Member9PhoneNumberTypeCD
				, M9.AltPhoneNumber AS Member9AltPhoneNumber, M9.AltPhoneNumberExtn AS Member9AltPhoneNumberExtn, M9.AltPhoneNumberTypeCD AS Member9AltPhoneNumberTypeCD
				, M9.EmailAddress AS Member9EmailAddress, M9.AltEmailAddress AS Member9AltEmailAddress
				, ISNULL(M9.IncomeValueAmt, 0) AS Member9IncomeValueAmt, ISNULL(M9.AssetValueAmt, 0) AS Member9AssetValueAmt
				, CASE WHEN M9.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member9OwnRealEstateInd, ISNULL(M9.RealEstateValueAmt, 0) AS Member9RealEstateValueAmt
			, M10.RelationTypeCD AS Member10RelationTypeCD
				, M10.RelationTypeDescription AS Member10RelationTypeDescription
				, M10.RelationTypeOther AS Member10RelationTypeOther
				, M10.Title AS Member10Title, M10.FirstName AS Member10FirstName, M10.MiddleName AS Member10MiddleName, M10.LastName AS Member10LastName, M10.Suffix AS Member10Suffix
				, M10.DateOfBirth AS Member10DateOfBirth, M10.Last4SSN AS Member10Last4SSN
				, M10.IDTypeCD AS Member10IDTypeCD, M10.IDTypeDescription AS Member10IDTypeDescription, M10.IDTypeValue AS Member10IDTypeValue, M10.IDIssueDate AS Member10IDIssueDate
				, M10.GenderCD AS Member10GenderCD, M10.GenderDescription AS Member10GenderDescription
				, M10.RaceCD AS Member10RaceCD, M10.RaceDescription AS Member10RaceDescription
				, M10.EthnicityCD AS Member10EthnicityCD, M10.EthnicityDescription AS Member10EthnicityDescription
				, M10.Pronouns AS Member10Pronouns
				, CASE WHEN M10.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member10StudentInd
				, CASE WHEN M10.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member10DisabilityInd
				, CASE WHEN M10.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member10VeteranInd
				, CASE WHEN M10.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member10EverLivedInWestchesterInd
				, M10.CountyLivingIn AS Member10CountyLivingIn
				, CASE WHEN M10.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member10EverLivedInWestchesterInd
				, M10.CountyWorkingIn AS Member10CountyWorkingIn
				, M10.PhoneNumber AS Member10PhoneNumber, M10.PhoneNumberExtn AS Member10PhoneNumberExtn, M10.PhoneNumberTypeCD AS Member10PhoneNumberTypeCD
				, M10.AltPhoneNumber AS Member10AltPhoneNumber, M10.AltPhoneNumberExtn AS Member10AltPhoneNumberExtn, M10.AltPhoneNumberTypeCD AS Member10AltPhoneNumberTypeCD
				, M10.EmailAddress AS Member10EmailAddress, M10.AltEmailAddress AS Member10AltEmailAddress
				, ISNULL(M10.IncomeValueAmt, 0) AS Member10IncomeValueAmt, ISNULL(M10.AssetValueAmt, 0) AS Member10AssetValueAmt
				, CASE WHEN M10.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member10OwnRealEstateInd, ISNULL(M10.RealEstateValueAmt, 0) AS Member10RealEstateValueAmt
			, M11.RelationTypeCD AS Member11RelationTypeCD
				, M11.RelationTypeDescription AS Member11RelationTypeDescription
				, M11.RelationTypeOther AS Member11RelationTypeOther
				, M11.Title AS Member11Title, M11.FirstName AS Member11FirstName, M11.MiddleName AS Member11MiddleName, M11.LastName AS Member11LastName, M11.Suffix AS Member11Suffix
				, M11.DateOfBirth AS Member11DateOfBirth, M11.Last4SSN AS Member11Last4SSN
				, M11.IDTypeCD AS Member11IDTypeCD, M11.IDTypeDescription AS Member11IDTypeDescription, M11.IDTypeValue AS Member11IDTypeValue, M11.IDIssueDate AS Member11IDIssueDate
				, M11.GenderCD AS Member11GenderCD, M11.GenderDescription AS Member11GenderDescription
				, M11.RaceCD AS Member11RaceCD, M11.RaceDescription AS Member11RaceDescription
				, M11.EthnicityCD AS Member11EthnicityCD, M11.EthnicityDescription AS Member11EthnicityDescription
				, M11.Pronouns AS Member11Pronouns
				, CASE WHEN M11.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member11StudentInd
				, CASE WHEN M11.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member11DisabilityInd
				, CASE WHEN M11.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member11VeteranInd
				, CASE WHEN M11.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member11EverLivedInWestchesterInd
				, M11.CountyLivingIn AS Member11CountyLivingIn
				, CASE WHEN M11.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member11EverLivedInWestchesterInd
				, M11.CountyWorkingIn AS Member11CountyWorkingIn
				, M11.PhoneNumber AS Member11PhoneNumber, M11.PhoneNumberExtn AS Member11PhoneNumberExtn, M11.PhoneNumberTypeCD AS Member11PhoneNumberTypeCD
				, M11.AltPhoneNumber AS Member11AltPhoneNumber, M11.AltPhoneNumberExtn AS Member11AltPhoneNumberExtn, M11.AltPhoneNumberTypeCD AS Member11AltPhoneNumberTypeCD
				, M11.EmailAddress AS Member11EmailAddress, M11.AltEmailAddress AS Member11AltEmailAddress
				, ISNULL(M11.IncomeValueAmt, 0) AS Member11IncomeValueAmt, ISNULL(M11.AssetValueAmt, 0) AS Member11AssetValueAmt
				, CASE WHEN M11.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member11OwnRealEstateInd, ISNULL(M11.RealEstateValueAmt, 0) AS Member11RealEstateValueAmt
			, M12.RelationTypeCD AS Member12RelationTypeCD
				, M12.RelationTypeDescription AS Member12RelationTypeDescription
				, M12.RelationTypeOther AS Member12RelationTypeOther
				, M12.Title AS Member12Title, M12.FirstName AS Member12FirstName, M12.MiddleName AS Member12MiddleName, M12.LastName AS Member12LastName, M12.Suffix AS Member12Suffix
				, M12.DateOfBirth AS Member12DateOfBirth, M12.Last4SSN AS Member12Last4SSN
				, M12.IDTypeCD AS Member12IDTypeCD, M12.IDTypeDescription AS Member12IDTypeDescription, M12.IDTypeValue AS Member12IDTypeValue, M12.IDIssueDate AS Member12IDIssueDate
				, M12.GenderCD AS Member12GenderCD, M12.GenderDescription AS Member12GenderDescription
				, M12.RaceCD AS Member12RaceCD, M12.RaceDescription AS Member12RaceDescription
				, M12.EthnicityCD AS Member12EthnicityCD, M12.EthnicityDescription AS Member12EthnicityDescription
				, M12.Pronouns AS Member12Pronouns
				, CASE WHEN M12.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS Member12StudentInd
				, CASE WHEN M12.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS Member12DisabilityInd
				, CASE WHEN M12.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS Member12VeteranInd
				, CASE WHEN M12.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member12EverLivedInWestchesterInd
				, M12.CountyLivingIn AS Member12CountyLivingIn
				, CASE WHEN M12.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS Member12EverLivedInWestchesterInd
				, M12.CountyWorkingIn AS Member12CountyWorkingIn
				, M12.PhoneNumber AS Member12PhoneNumber, M12.PhoneNumberExtn AS Member12PhoneNumberExtn, M12.PhoneNumberTypeCD AS Member12PhoneNumberTypeCD
				, M12.AltPhoneNumber AS Member12AltPhoneNumber, M12.AltPhoneNumberExtn AS Member12AltPhoneNumberExtn, M12.AltPhoneNumberTypeCD AS Member12AltPhoneNumberTypeCD
				, M12.EmailAddress AS Member12EmailAddress, M12.AltEmailAddress AS Member12AltEmailAddress
				, ISNULL(M12.IncomeValueAmt, 0) AS Member12IncomeValueAmt, ISNULL(M12.AssetValueAmt, 0) AS Member12AssetValueAmt
				, CASE WHEN M12.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS Member12OwnRealEstateInd, ISNULL(M12.RealEstateValueAmt, 0) AS Member12RealEstateValueAmt
		FROM dbo.tblHousingApplications HA
		JOIN dbo.uvwHousingApplicants PA ON PA.ApplicationID = HA.ApplicationID AND PA.RelationTypeCD = 'SELF'
		LEFT OUTER JOIN dbo.uvwHousingApplicants CA ON CA.ApplicationID = HA.ApplicationID AND CA.CoApplicantInd = 1
		LEFT OUTER JOIN dbo.uvwHousingApplicants M3 ON M3.ApplicationID = HA.ApplicationID AND M3.ApplicantSortOrder = 3
		LEFT OUTER JOIN dbo.uvwHousingApplicants M4 ON M4.ApplicationID = HA.ApplicationID AND M4.ApplicantSortOrder = 4
		LEFT OUTER JOIN dbo.uvwHousingApplicants M5 ON M5.ApplicationID = HA.ApplicationID AND M5.ApplicantSortOrder = 5
		LEFT OUTER JOIN dbo.uvwHousingApplicants M6 ON M6.ApplicationID = HA.ApplicationID AND M6.ApplicantSortOrder = 6
		LEFT OUTER JOIN dbo.uvwHousingApplicants M7 ON M7.ApplicationID = HA.ApplicationID AND M7.ApplicantSortOrder = 7
		LEFT OUTER JOIN dbo.uvwHousingApplicants M8 ON M8.ApplicationID = HA.ApplicationID AND M8.ApplicantSortOrder = 8
		LEFT OUTER JOIN dbo.uvwHousingApplicants M9 ON M9.ApplicationID = HA.ApplicationID AND M9.ApplicantSortOrder = 9
		LEFT OUTER JOIN dbo.uvwHousingApplicants M10 ON M10.ApplicationID = HA.ApplicationID AND M10.ApplicantSortOrder = 10
		LEFT OUTER JOIN dbo.uvwHousingApplicants M11 ON M11.ApplicationID = HA.ApplicationID AND M11.ApplicantSortOrder = 11
		LEFT OUTER JOIN dbo.uvwHousingApplicants M12 ON M12.ApplicationID = HA.ApplicationID AND M12.ApplicantSortOrder = 12
		WHERE HA.ListingID = @ListingID
			AND (@SubmissionTypeCD = 'ALL' OR HA.SubmissionTypeCD = @SubmissionTypeCD)
			AND (@StatusCD = 'ALL' OR HA.StatusCD = @StatusCD)
			AND HA.StatusCD <> 'DRAFT'
		ORDER BY HA.ApplicationID ASC;

END
GO