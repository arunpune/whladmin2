DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationApplicantRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Retrieve a list of applicants by housing application ID
-- Examples:
--	EXEC dbo.uspHousingApplicationApplicantRetrieve @ApplicationID = 1
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationApplicantRetrieve]
	@ApplicationID	BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT AP.ApplicantID, AP.ApplicationID, AP.MemberID, AP.CoApplicantInd
		, AP.Title, AP.FirstName, AP.MiddleName, AP.LastName, AP.Suffix
		, AP.RelationTypeCD, MDREL.[Description] AS RelationTypeDescription, AP.RelationTypeOther
		, AP.DateOfBirth, AP.Last4SSN
		, AP.IDTypeCD, MDIT.[Description] AS IDTypeDescription, AP.IDTypeValue, AP.IDIssueDate
		, AP.GenderCD, MDGND.[Description] AS GenderDescription
		, AP.RaceCD, MDRAC.[Description] AS RaceDescription
		, AP.EthnicityCD, MDETH.[Description] AS EthnicityDescription
		, AP.Pronouns
		, AP.EverLivedInWestchesterInd, AP.CountyLivingIn, AP.CurrentlyWorkingInWestchesterInd, AP.CountyWorkingIn
		, AP.StudentInd, AP.DisabilityInd, AP.VeteranInd
		, AP.PhoneNumber, AP.PhoneNumberExtn, AP.PhoneNumberTypeCD, MDPHT.[Description] AS PhoneNumberTypeDescription
		, AP.AltPhoneNumber, AP.AltPhoneNumberExtn, AP.AltPhoneNumberTypeCD, MDPHTA.[Description] AS AltPhoneNumberTypeDescription
		, NULL AS Username, AP.EmailAddress, CONVERT(BIT, 0) AS AuthRepEmailAddressInd, AP.AltEmailAddress
		, AP.OwnRealEstateInd, AP.RealEstateValueAmt, AP.AssetValueAmt, AP.IncomeValueAmt
		, AP.CreatedBy, AP.CreatedDate, AP.ModifiedBy, AP.ModifiedDate
		, AP.Active
		, CASE WHEN AP.RelationTypeCD = 'SELF' THEN 1 WHEN AP.CoApplicantInd = 1 THEN 2 ELSE 3 END AS SortOrder
	FROM dbo.tblHousingApplicants AP
	LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = AP.IDTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDREL ON MDREL.CodeID = 109 AND MDREL.Code = AP.RelationTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDGND ON MDGND.CodeID = 110 AND MDGND.Code = AP.GenderCD
	LEFT OUTER JOIN dbo.tblMetadata MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = AP.RaceCD
	LEFT OUTER JOIN dbo.tblMetadata MDETH ON MDETH.CodeID = 112 AND MDETH.Code = AP.EthnicityCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = AP.PhoneNumberTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHTA ON MDPHTA.CodeID = 114 AND MDPHTA.Code = AP.AltPhoneNumberTypeCD
	WHERE AP.ApplicationID = @ApplicationID
	ORDER BY SortOrder, AP.FirstName, AP.LastName;

END
GO