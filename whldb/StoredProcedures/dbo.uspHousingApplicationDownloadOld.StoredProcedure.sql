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

	WITH ApplicantsCTE AS (
		SELECT 2 + ROW_NUMBER() OVER (ORDER BY APPL.ApplicantID) AS ApplicantSortID
			, APPL.*
		FROM dbo.tblHousingApplicants APPL
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = APPL.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND (@SubmissionTypeCD = 'ALL' OR HA.SubmissionTypeCD = @SubmissionTypeCD)
			AND (@StatusCD = 'ALL' OR HA.StatusCD = @StatusCD)
			AND HA.StatusCD <> 'DRAFT'
			AND (APPL.RelationTypeCD <> 'SELF' AND APPL.CoApplicantInd <> 1)
		--ORDER BY HA.ApplicationID, ApplicantSortID
	)
		SELECT HA.ApplicationID, HA.Username
			, RIGHT(ISNULL(RTRIM(HA.LotteryNumber), '      '), 6) AS LotteryNumber, HA.LotteryID
			, HA.SubmissionTypeCD, HA.SubmittedDate, HA.StatusCD
			, HA.UnitTypeCDs, HA.AccessibilityCDs
			, HA.LeadTypeCD, HA.LeadTypeOther
			, PA.Title AS ApplicantTitle, PA.FirstName AS ApplicantFirstName, PA.MiddleName AS ApplicantMiddleName, PA.LastName AS ApplicantLastName, PA.Suffix AS ApplicantSuffix
				, PA.DateOfBirth AS ApplicantDateOfBirth, PA.Last4SSN AS ApplicantLast4SSN
				, PA.IDTypeCD AS ApplicantIDTypeCD, PA.IDTypeValue AS ApplicantIDTypeValue, PA.IDIssueDate AS ApplicantIDIssueDate
				, PA.GenderCD AS ApplicantGenderCD, PA.RaceCD AS ApplicantRaceCD, PA.EthnicityCD AS ApplicantEthnicityCD, PA.Pronouns AS ApplicantPronouns
				, PA.StudentInd AS ApplicantStudentInd, PA.DisabilityInd AS ApplicantDisabilityInd, PA.VeteranInd AS ApplicantVeteranInd
				, PA.EverLivedInWestchesterInd AS ApplicantEverLivedInWestchesterInd, PA.CountyLivingIn AS ApplicantCountyLivingIn
				, PA.EverLivedInWestchesterInd AS ApplicantEverLivedInWestchesterInd, PA.CountyWorkingIn AS ApplicantCountyWorkingIn
				, PA.PhoneNumber AS ApplicantPhoneNumber, PA.PhoneNumberExtn AS ApplicantPhoneNumberExtn, PA.PhoneNumberTypeCD AS ApplicantPhoneNumberTypeCD
				, PA.AltPhoneNumber AS ApplicantAltPhoneNumber, PA.AltPhoneNumberExtn AS ApplicantAltPhoneNumberExtn, PA.AltPhoneNumberTypeCD AS ApplicantAltPhoneNumberTypeCD
				, PA.EmailAddress AS ApplicantEmailAddress, PA.AltEmailAddress AS ApplicantAltEmailAddress
			, HA.PhysicalStreetLine1, HA.PhysicalStreetLine2, HA.PhysicalStreetLine3
				, HA.PhysicalCity, HA.PhysicalStateCD, HA.PhysicalZipCode, HA.PhysicalCounty
			, HA.MailingStreetLine1, HA.MailingStreetLine2, HA.MailingStreetLine3
				, HA.MailingCity, HA.MailingStateCD, HA.MailingZipCode, HA.MailingCounty
			, CA.RelationTypeCD AS CoApplicantRelationTypeCD
				, CA.Title AS CoApplicantTitle, CA.FirstName AS CoApplicantFirstName, CA.MiddleName AS CoApplicantMiddleName, CA.LastName AS CoApplicantLastName, CA.Suffix AS CoApplicantSuffix
				, CA.DateOfBirth AS CoApplicantDateOfBirth, CA.Last4SSN AS CoApplicantLast4SSN
				, CA.IDTypeCD AS CoApplicantIDTypeCD, CA.IDTypeValue AS CoApplicantIDTypeValue, CA.IDIssueDate AS CoApplicantIDIssueDate
				, CA.GenderCD AS CoApplicantGenderCD, CA.RaceCD AS CoApplicantRaceCD, CA.EthnicityCD AS CoApplicantEthnicityCD, CA.Pronouns AS CoApplicantPronouns
				, CA.StudentInd AS CoApplicantStudentInd, CA.DisabilityInd AS CoApplicantDisabilityInd, CA.VeteranInd AS CoApplicantVeteranInd
				, CA.EverLivedInWestchesterInd AS CoApplicantEverLivedInWestchesterInd, CA.CountyLivingIn AS CoApplicantCountyLivingIn
				, CA.EverLivedInWestchesterInd AS CoApplicantEverLivedInWestchesterInd, CA.CountyWorkingIn AS CoApplicantCountyWorkingIn
				, CA.PhoneNumber AS CoApplicantPhoneNumber, CA.PhoneNumberExtn AS CoApplicantPhoneNumberExtn, CA.PhoneNumberTypeCD AS CoApplicantPhoneNumberTypeCD
				, CA.AltPhoneNumber AS CoApplicantAltPhoneNumber, CA.AltPhoneNumberExtn AS CoApplicantAltPhoneNumberExtn, CA.AltPhoneNumberTypeCD AS CoApplicantAltPhoneNumberTypeCD
				, CA.EmailAddress AS CoApplicantEmailAddress, CA.AltEmailAddress AS CoApplicantAltEmailAddress
			, M1.RelationTypeCD AS Member1RelationTypeCD
				, M1.Title AS Member1Title, M1.FirstName AS Member1FirstName, M1.MiddleName AS Member1MiddleName, M1.LastName AS Member1LastName, M1.Suffix AS Member1Suffix
				, M1.DateOfBirth AS Member1DateOfBirth, M1.Last4SSN AS Member1Last4SSN
				, M1.IDTypeCD AS Member1IDTypeCD, M1.IDTypeValue AS Member1IDTypeValue, M1.IDIssueDate AS Member1IDIssueDate
				, M1.GenderCD AS Member1GenderCD, M1.RaceCD AS Member1RaceCD, M1.EthnicityCD AS Member1EthnicityCD, M1.Pronouns AS Member1Pronouns
				, M1.StudentInd AS Member1StudentInd, M1.DisabilityInd AS Member1DisabilityInd, M1.VeteranInd AS Member1VeteranInd
				, M1.EverLivedInWestchesterInd AS Member1EverLivedInWestchesterInd, M1.CountyLivingIn AS Member1CountyLivingIn
				, M1.EverLivedInWestchesterInd AS Member1EverLivedInWestchesterInd, M1.CountyWorkingIn AS Member1CountyWorkingIn
				, M1.PhoneNumber AS Member1PhoneNumber, M1.PhoneNumberExtn AS Member1PhoneNumberExtn, M1.PhoneNumberTypeCD AS Member1PhoneNumberTypeCD
				, M1.AltPhoneNumber AS Member1AltPhoneNumber, M1.AltPhoneNumberExtn AS Member1AltPhoneNumberExtn, M1.AltPhoneNumberTypeCD AS Member1AltPhoneNumberTypeCD
				, M1.EmailAddress AS Member1EmailAddress, M1.AltEmailAddress AS Member1AltEmailAddress
			, M2.RelationTypeCD AS Member2RelationTypeCD
				, M2.Title AS Member2Title, M2.FirstName AS Member2FirstName, M2.MiddleName AS Member2MiddleName, M2.LastName AS Member2LastName, M2.Suffix AS Member2Suffix
				, M2.DateOfBirth AS Member2DateOfBirth, M2.Last4SSN AS Member2Last4SSN
				, M2.IDTypeCD AS Member2IDTypeCD, M2.IDTypeValue AS Member2IDTypeValue, M2.IDIssueDate AS Member2IDIssueDate
				, M2.GenderCD AS Member2GenderCD, M2.RaceCD AS Member2RaceCD, M2.EthnicityCD AS Member2EthnicityCD, M2.Pronouns AS Member2Pronouns
				, M2.StudentInd AS Member2StudentInd, M2.DisabilityInd AS Member2DisabilityInd, M2.VeteranInd AS Member2VeteranInd
				, M2.EverLivedInWestchesterInd AS Member2EverLivedInWestchesterInd, M2.CountyLivingIn AS Member2CountyLivingIn
				, M2.EverLivedInWestchesterInd AS Member2EverLivedInWestchesterInd, M2.CountyWorkingIn AS Member2CountyWorkingIn
				, M2.PhoneNumber AS Member2PhoneNumber, M2.PhoneNumberExtn AS Member2PhoneNumberExtn, M2.PhoneNumberTypeCD AS Member2PhoneNumberTypeCD
				, M2.AltPhoneNumber AS Member2AltPhoneNumber, M2.AltPhoneNumberExtn AS Member2AltPhoneNumberExtn, M2.AltPhoneNumberTypeCD AS Member2AltPhoneNumberTypeCD
				, M2.EmailAddress AS Member2EmailAddress, M2.AltEmailAddress AS Member2AltEmailAddress
			, M3.RelationTypeCD AS Member3RelationTypeCD
				, M3.Title AS Member3Title, M3.FirstName AS Member3FirstName, M3.MiddleName AS Member3MiddleName, M3.LastName AS Member3LastName, M3.Suffix AS Member3Suffix
				, M3.DateOfBirth AS Member3DateOfBirth, M3.Last4SSN AS Member3Last4SSN
				, M3.IDTypeCD AS Member3IDTypeCD, M3.IDTypeValue AS Member3IDTypeValue, M3.IDIssueDate AS Member3IDIssueDate
				, M3.GenderCD AS Member3GenderCD, M3.RaceCD AS Member3RaceCD, M3.EthnicityCD AS Member3EthnicityCD, M3.Pronouns AS Member3Pronouns
				, M3.StudentInd AS Member3StudentInd, M3.DisabilityInd AS Member3DisabilityInd, M3.VeteranInd AS Member3VeteranInd
				, M3.EverLivedInWestchesterInd AS Member3EverLivedInWestchesterInd, M3.CountyLivingIn AS Member3CountyLivingIn
				, M3.EverLivedInWestchesterInd AS Member3EverLivedInWestchesterInd, M3.CountyWorkingIn AS Member3CountyWorkingIn
				, M3.PhoneNumber AS Member3PhoneNumber, M3.PhoneNumberExtn AS Member3PhoneNumberExtn, M3.PhoneNumberTypeCD AS Member3PhoneNumberTypeCD
				, M3.AltPhoneNumber AS Member3AltPhoneNumber, M3.AltPhoneNumberExtn AS Member3AltPhoneNumberExtn, M3.AltPhoneNumberTypeCD AS Member3AltPhoneNumberTypeCD
				, M3.EmailAddress AS Member3EmailAddress, M3.AltEmailAddress AS Member3AltEmailAddress
			, M4.RelationTypeCD AS Member4RelationTypeCD
				, M4.Title AS Member4Title, M4.FirstName AS Member4FirstName, M4.MiddleName AS Member4MiddleName, M4.LastName AS Member4LastName, M4.Suffix AS Member4Suffix
				, M4.DateOfBirth AS Member4DateOfBirth, M4.Last4SSN AS Member4Last4SSN
				, M4.IDTypeCD AS Member4IDTypeCD, M4.IDTypeValue AS Member4IDTypeValue, M4.IDIssueDate AS Member4IDIssueDate
				, M4.GenderCD AS Member4GenderCD, M4.RaceCD AS Member4RaceCD, M4.EthnicityCD AS Member4EthnicityCD, M4.Pronouns AS Member4Pronouns
				, M4.StudentInd AS Member4StudentInd, M4.DisabilityInd AS Member4DisabilityInd, M4.VeteranInd AS Member4VeteranInd
				, M4.EverLivedInWestchesterInd AS Member4EverLivedInWestchesterInd, M4.CountyLivingIn AS Member4CountyLivingIn
				, M4.EverLivedInWestchesterInd AS Member4EverLivedInWestchesterInd, M4.CountyWorkingIn AS Member4CountyWorkingIn
				, M4.PhoneNumber AS Member4PhoneNumber, M4.PhoneNumberExtn AS Member4PhoneNumberExtn, M4.PhoneNumberTypeCD AS Member4PhoneNumberTypeCD
				, M4.AltPhoneNumber AS Member4AltPhoneNumber, M4.AltPhoneNumberExtn AS Member4AltPhoneNumberExtn, M4.AltPhoneNumberTypeCD AS Member4AltPhoneNumberTypeCD
				, M4.EmailAddress AS Member4EmailAddress, M4.AltEmailAddress AS Member4AltEmailAddress
			, M5.RelationTypeCD AS Member5RelationTypeCD
				, M5.Title AS Member5Title, M5.FirstName AS Member5FirstName, M5.MiddleName AS Member5MiddleName, M5.LastName AS Member5LastName, M5.Suffix AS Member5Suffix
				, M5.DateOfBirth AS Member5DateOfBirth, M5.Last4SSN AS Member5Last4SSN
				, M5.IDTypeCD AS Member5IDTypeCD, M5.IDTypeValue AS Member5IDTypeValue, M5.IDIssueDate AS Member5IDIssueDate
				, M5.GenderCD AS Member5GenderCD, M5.RaceCD AS Member5RaceCD, M5.EthnicityCD AS Member5EthnicityCD, M5.Pronouns AS Member5Pronouns
				, M5.StudentInd AS Member5StudentInd, M5.DisabilityInd AS Member5DisabilityInd, M5.VeteranInd AS Member5VeteranInd
				, M5.EverLivedInWestchesterInd AS Member5EverLivedInWestchesterInd, M5.CountyLivingIn AS Member5CountyLivingIn
				, M5.EverLivedInWestchesterInd AS Member5EverLivedInWestchesterInd, M5.CountyWorkingIn AS Member5CountyWorkingIn
				, M5.PhoneNumber AS Member5PhoneNumber, M5.PhoneNumberExtn AS Member5PhoneNumberExtn, M5.PhoneNumberTypeCD AS Member5PhoneNumberTypeCD
				, M5.AltPhoneNumber AS Member5AltPhoneNumber, M5.AltPhoneNumberExtn AS Member5AltPhoneNumberExtn, M5.AltPhoneNumberTypeCD AS Member5AltPhoneNumberTypeCD
				, M5.EmailAddress AS Member5EmailAddress, M5.AltEmailAddress AS Member5AltEmailAddress
			, M6.RelationTypeCD AS Member6RelationTypeCD
				, M6.Title AS Member6Title, M6.FirstName AS Member6FirstName, M6.MiddleName AS Member6MiddleName, M6.LastName AS Member6LastName, M6.Suffix AS Member6Suffix
				, M6.DateOfBirth AS Member6DateOfBirth, M6.Last4SSN AS Member6Last4SSN
				, M6.IDTypeCD AS Member6IDTypeCD, M6.IDTypeValue AS Member6IDTypeValue, M6.IDIssueDate AS Member6IDIssueDate
				, M6.GenderCD AS Member6GenderCD, M6.RaceCD AS Member6RaceCD, M6.EthnicityCD AS Member6EthnicityCD, M6.Pronouns AS Member6Pronouns
				, M6.StudentInd AS Member6StudentInd, M6.DisabilityInd AS Member6DisabilityInd, M6.VeteranInd AS Member6VeteranInd
				, M6.EverLivedInWestchesterInd AS Member6EverLivedInWestchesterInd, M6.CountyLivingIn AS Member6CountyLivingIn
				, M6.EverLivedInWestchesterInd AS Member6EverLivedInWestchesterInd, M6.CountyWorkingIn AS Member6CountyWorkingIn
				, M6.PhoneNumber AS Member6PhoneNumber, M6.PhoneNumberExtn AS Member6PhoneNumberExtn, M6.PhoneNumberTypeCD AS Member6PhoneNumberTypeCD
				, M6.AltPhoneNumber AS Member6AltPhoneNumber, M6.AltPhoneNumberExtn AS Member6AltPhoneNumberExtn, M6.AltPhoneNumberTypeCD AS Member6AltPhoneNumberTypeCD
				, M6.EmailAddress AS Member6EmailAddress, M6.AltEmailAddress AS Member6AltEmailAddress
			, M7.RelationTypeCD AS Member7RelationTypeCD
				, M7.Title AS Member7Title, M7.FirstName AS Member7FirstName, M7.MiddleName AS Member7MiddleName, M7.LastName AS Member7LastName, M7.Suffix AS Member7Suffix
				, M7.DateOfBirth AS Member7DateOfBirth, M7.Last4SSN AS Member7Last4SSN
				, M7.IDTypeCD AS Member7IDTypeCD, M7.IDTypeValue AS Member7IDTypeValue, M7.IDIssueDate AS Member7IDIssueDate
				, M7.GenderCD AS Member7GenderCD, M7.RaceCD AS Member7RaceCD, M7.EthnicityCD AS Member7EthnicityCD, M7.Pronouns AS Member7Pronouns
				, M7.StudentInd AS Member7StudentInd, M7.DisabilityInd AS Member7DisabilityInd, M7.VeteranInd AS Member7VeteranInd
				, M7.EverLivedInWestchesterInd AS Member7EverLivedInWestchesterInd, M7.CountyLivingIn AS Member7CountyLivingIn
				, M7.EverLivedInWestchesterInd AS Member7EverLivedInWestchesterInd, M7.CountyWorkingIn AS Member7CountyWorkingIn
				, M7.PhoneNumber AS Member7PhoneNumber, M7.PhoneNumberExtn AS Member7PhoneNumberExtn, M7.PhoneNumberTypeCD AS Member7PhoneNumberTypeCD
				, M7.AltPhoneNumber AS Member7AltPhoneNumber, M7.AltPhoneNumberExtn AS Member7AltPhoneNumberExtn, M7.AltPhoneNumberTypeCD AS Member7AltPhoneNumberTypeCD
				, M7.EmailAddress AS Member7EmailAddress, M7.AltEmailAddress AS Member7AltEmailAddress
			, M8.RelationTypeCD AS Member8RelationTypeCD
				, M8.Title AS Member8Title, M8.FirstName AS Member8FirstName, M8.MiddleName AS Member8MiddleName, M8.LastName AS Member8LastName, M8.Suffix AS Member8Suffix
				, M8.DateOfBirth AS Member8DateOfBirth, M8.Last4SSN AS Member8Last4SSN
				, M8.IDTypeCD AS Member8IDTypeCD, M8.IDTypeValue AS Member8IDTypeValue, M8.IDIssueDate AS Member8IDIssueDate
				, M8.GenderCD AS Member8GenderCD, M8.RaceCD AS Member8RaceCD, M8.EthnicityCD AS Member8EthnicityCD, M8.Pronouns AS Member8Pronouns
				, M8.StudentInd AS Member8StudentInd, M8.DisabilityInd AS Member8DisabilityInd, M8.VeteranInd AS Member8VeteranInd
				, M8.EverLivedInWestchesterInd AS Member8EverLivedInWestchesterInd, M8.CountyLivingIn AS Member8CountyLivingIn
				, M8.EverLivedInWestchesterInd AS Member8EverLivedInWestchesterInd, M8.CountyWorkingIn AS Member8CountyWorkingIn
				, M8.PhoneNumber AS Member8PhoneNumber, M8.PhoneNumberExtn AS Member8PhoneNumberExtn, M8.PhoneNumberTypeCD AS Member8PhoneNumberTypeCD
				, M8.AltPhoneNumber AS Member8AltPhoneNumber, M8.AltPhoneNumberExtn AS Member8AltPhoneNumberExtn, M8.AltPhoneNumberTypeCD AS Member8AltPhoneNumberTypeCD
				, M8.EmailAddress AS Member8EmailAddress, M8.AltEmailAddress AS Member8AltEmailAddress
			, M9.RelationTypeCD AS Member9RelationTypeCD
				, M9.Title AS Member9Title, M9.FirstName AS Member9FirstName, M9.MiddleName AS Member9MiddleName, M9.LastName AS Member9LastName, M9.Suffix AS Member9Suffix
				, M9.DateOfBirth AS Member9DateOfBirth, M9.Last4SSN AS Member9Last4SSN
				, M9.IDTypeCD AS Member9IDTypeCD, M9.IDTypeValue AS Member9IDTypeValue, M9.IDIssueDate AS Member9IDIssueDate
				, M9.GenderCD AS Member9GenderCD, M9.RaceCD AS Member9RaceCD, M9.EthnicityCD AS Member9EthnicityCD, M9.Pronouns AS Member9Pronouns
				, M9.StudentInd AS Member9StudentInd, M9.DisabilityInd AS Member9DisabilityInd, M9.VeteranInd AS Member9VeteranInd
				, M9.EverLivedInWestchesterInd AS Member9EverLivedInWestchesterInd, M9.CountyLivingIn AS Member9CountyLivingIn
				, M9.EverLivedInWestchesterInd AS Member9EverLivedInWestchesterInd, M9.CountyWorkingIn AS Member9CountyWorkingIn
				, M9.PhoneNumber AS Member9PhoneNumber, M9.PhoneNumberExtn AS Member9PhoneNumberExtn, M9.PhoneNumberTypeCD AS Member9PhoneNumberTypeCD
				, M9.AltPhoneNumber AS Member9AltPhoneNumber, M9.AltPhoneNumberExtn AS Member9AltPhoneNumberExtn, M9.AltPhoneNumberTypeCD AS Member9AltPhoneNumberTypeCD
				, M9.EmailAddress AS Member9EmailAddress, M9.AltEmailAddress AS Member9AltEmailAddress
			, M10.RelationTypeCD AS Member10RelationTypeCD
				, M10.Title AS Member10Title, M10.FirstName AS Member10FirstName, M10.MiddleName AS Member10MiddleName, M10.LastName AS Member10LastName, M10.Suffix AS Member10Suffix
				, M10.DateOfBirth AS Member10DateOfBirth, M10.Last4SSN AS Member10Last4SSN
				, M10.IDTypeCD AS Member10IDTypeCD, M10.IDTypeValue AS Member10IDTypeValue, M10.IDIssueDate AS Member10IDIssueDate
				, M10.GenderCD AS Member10GenderCD, M10.RaceCD AS Member10RaceCD, M10.EthnicityCD AS Member10EthnicityCD, M10.Pronouns AS Member10Pronouns
				, M10.StudentInd AS Member10StudentInd, M10.DisabilityInd AS Member10DisabilityInd, M10.VeteranInd AS Member10VeteranInd
				, M10.EverLivedInWestchesterInd AS Member10EverLivedInWestchesterInd, M10.CountyLivingIn AS Member10CountyLivingIn
				, M10.EverLivedInWestchesterInd AS Member10EverLivedInWestchesterInd, M10.CountyWorkingIn AS Member10CountyWorkingIn
				, M10.PhoneNumber AS Member10PhoneNumber, M10.PhoneNumberExtn AS Member10PhoneNumberExtn, M10.PhoneNumberTypeCD AS Member10PhoneNumberTypeCD
				, M10.AltPhoneNumber AS Member10AltPhoneNumber, M10.AltPhoneNumberExtn AS Member10AltPhoneNumberExtn, M10.AltPhoneNumberTypeCD AS Member10AltPhoneNumberTypeCD
				, M10.EmailAddress AS Member10EmailAddress, M10.AltEmailAddress AS Member10AltEmailAddress
		FROM dbo.tblHousingApplications HA
		JOIN dbo.tblHousingApplicants PA ON PA.ApplicationID = HA.ApplicationID AND PA.RelationTypeCD = 'SELF'
		LEFT OUTER JOIN dbo.tblHousingApplicants CA ON CA.ApplicationID = HA.ApplicationID AND CA.RelationTypeCD <> 'SELF' AND CA.CoApplicantInd = 1
		LEFT OUTER JOIN ApplicantsCTE M1 ON M1.ApplicationID = HA.ApplicationID AND M1.ApplicantSortID = 3
		LEFT OUTER JOIN ApplicantsCTE M2 ON M2.ApplicationID = HA.ApplicationID AND M2.ApplicantSortID = 4
		LEFT OUTER JOIN ApplicantsCTE M3 ON M3.ApplicationID = HA.ApplicationID AND M3.ApplicantSortID = 5
		LEFT OUTER JOIN ApplicantsCTE M4 ON M4.ApplicationID = HA.ApplicationID AND M4.ApplicantSortID = 6
		LEFT OUTER JOIN ApplicantsCTE M5 ON M5.ApplicationID = HA.ApplicationID AND M5.ApplicantSortID = 7
		LEFT OUTER JOIN ApplicantsCTE M6 ON M6.ApplicationID = HA.ApplicationID AND M6.ApplicantSortID = 8
		LEFT OUTER JOIN ApplicantsCTE M7 ON M7.ApplicationID = HA.ApplicationID AND M7.ApplicantSortID = 9
		LEFT OUTER JOIN ApplicantsCTE M8 ON M8.ApplicationID = HA.ApplicationID AND M8.ApplicantSortID = 10
		LEFT OUTER JOIN ApplicantsCTE M9 ON M9.ApplicationID = HA.ApplicationID AND M9.ApplicantSortID = 11
		LEFT OUTER JOIN ApplicantsCTE M10 ON M10.ApplicationID = HA.ApplicationID AND M10.ApplicantSortID = 12
		WHERE HA.ListingID = @ListingID
			AND (@SubmissionTypeCD = 'ALL' OR HA.SubmissionTypeCD = @SubmissionTypeCD)
			AND (@StatusCD = 'ALL' OR HA.StatusCD = @StatusCD)
			AND HA.StatusCD <> 'DRAFT'
		ORDER BY HA.ApplicationID ASC;

END
GO