DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdRetrieveMembers];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Retrieve household members by householdid, or a given one by member ID
-- Examples:
--	EXEC dbo.uspSiteHouseholdRetrieveMembers @HouseholdID = 1 (Retrieve All by Household)
--	EXEC dbo.uspSiteHouseholdRetrieveMembers @HouseholdID = 1, @MemberID = 1 (Retrieve by Household Member)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdRetrieveMembers]
	@HouseholdID	BIGINT
	, @MemberID		BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT 0 AS MemberID, H.HouseholdID, U.Title, U.FirstName, U.MiddleName, U.LastName, U.Suffix
		, 'SELF' AS RelationTypeCD, 'Self' AS RelationTypeDescription, NULL AS RelationTypeOther
		, U.DateOfBirth, U.Last4SSN
		, U.IDTypeCD, MDIT.[Description] AS IDTypeDescription, U.IDTypeValue, U.IDIssueDate
		, U.GenderCD, MDGND.[Description] AS GenderDescription
		, U.RaceCD, MDRAC.[Description] AS RaceDescription
		, U.EthnicityCD, MDETH.[Description] AS EthnicityDescription
		, U.Pronouns
		, U.EverLivedInWestchesterInd, U.CountyLivingIn, U.CurrentlyWorkingInWestchesterInd, U.CountyWorkingIn
		, U.StudentInd, U.DisabilityInd, U.VeteranInd
		, U.PhoneNumber, U.PhoneNumberExtn, U.PhoneNumberTypeCD, MDPHT.[Description] AS PhoneNumberTypeDescription
		, U.AltPhoneNumber, U.AltPhoneNumberExtn, U.AltPhoneNumberTypeCD, MDPHTA.[Description] AS AltPhoneNumberTypeDescription
		, U.Username, U.EmailAddress, U.AuthRepEmailAddressInd, U.AltEmailAddress
		, U.OwnRealEstateInd, U.RealEstateValueAmt, U.AssetValueAmt, U.IncomeValueAmt
		, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
		, U.Active
		, 1 AS SortOrder
	FROM dbo.tblSiteUsers U
	JOIN dbo.tblHouseholds H ON H.Username = U.Username
	LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = U.IDTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDGND ON MDGND.CodeID = 110 AND MDGND.Code = U.GenderCD
	LEFT OUTER JOIN dbo.tblMetadata MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = U.RaceCD
	LEFT OUTER JOIN dbo.tblMetadata MDETH ON MDETH.CodeID = 112 AND MDETH.Code = U.EthnicityCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = U.PhoneNumberTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHTA ON MDPHTA.CodeID = 114 AND MDPHTA.Code = U.AltPhoneNumberTypeCD
	WHERE H.HouseholdID = @HouseholdID
		AND ISNULL(@MemberID, 0) = 0
		AND U.Active = 1

	UNION

	SELECT M.MemberID, M.HouseholdID, M.Title, M.FirstName, M.MiddleName, M.LastName, M.Suffix
		, M.RelationTypeCD, MDREL.[Description] AS RelationTypeDescription, M.RelationTypeOther
		, M.DateOfBirth, M.Last4SSN
		, M.IDTypeCD, MDIT.[Description] AS IDTypeDescription, M.IDTypeValue, M.IDIssueDate
		, M.GenderCD, MDGND.[Description] AS GenderDescription
		, M.RaceCD, MDRAC.[Description] AS RaceDescription
		, M.EthnicityCD, MDETH.[Description] AS EthnicityDescription
		, M.Pronouns
		, M.EverLivedInWestchesterInd, M.CountyLivingIn, M.CurrentlyWorkingInWestchesterInd, M.CountyWorkingIn
		, M.StudentInd, M.DisabilityInd, M.VeteranInd
		, M.PhoneNumber, M.PhoneNumberExtn, M.PhoneNumberTypeCD, MDPHT.[Description] AS PhoneNumberTypeDescription
		, M.AltPhoneNumber, M.AltPhoneNumberExtn, M.AltPhoneNumberTypeCD, MDPHTA.[Description] AS AltPhoneNumberTypeDescription
		, NULL AS Username, M.EmailAddress, CONVERT(BIT, 0) AS AuthRepEmailAddressInd, M.AltEmailAddress
		, M.OwnRealEstateInd, M.RealEstateValueAmt, M.AssetValueAmt, M.IncomeValueAmt
		, M.CreatedBy, M.CreatedDate, M.ModifiedBy, M.ModifiedDate
		, M.Active
		, 2 AS SortOrder
	FROM dbo.tblHouseholdMembers M
	JOIN dbo.tblHouseholds H ON H.HouseholdID = M.HouseholdID
	LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = M.IDTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDREL ON MDREL.CodeID = 109 AND MDREL.Code = M.RelationTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDGND ON MDGND.CodeID = 110 AND MDGND.Code = M.GenderCD
	LEFT OUTER JOIN dbo.tblMetadata MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = M.RaceCD
	LEFT OUTER JOIN dbo.tblMetadata MDETH ON MDETH.CodeID = 112 AND MDETH.Code = M.EthnicityCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = M.PhoneNumberTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHTA ON MDPHTA.CodeID = 114 AND MDPHTA.Code = M.AltPhoneNumberTypeCD
	WHERE M.HouseholdID = @HouseholdID
		AND (ISNULL(@MemberID, 0) = 0 OR M.MemberID = @MemberID)
		AND M.Active = 1

	ORDER BY SortOrder, FirstName;

END
GO