DROP VIEW IF EXISTS [dbo].[uvwHousingApplicants];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

CREATE VIEW [dbo].[uvwHousingApplicants]
AS
    SELECT AP.ApplicantID, AP.ApplicationID, AP.MemberID, AP.CoApplicantInd
        , AP.Title, AP.FirstName, AP.MiddleName, AP.LastName, AP.Suffix
        , AP.RelationTypeCD, MDREL.[Description] AS RelationTypeDescription
            , AP.RelationTypeOther
        , AP.DateOfBirth, AP.Last4SSN
        , AP.IDTypeCD, MDIT.[Description] AS IDTypeDescription, AP.IDTypeValue, AP.IDIssueDate
        , AP.GenderCD, MDGND.[Description] AS GenderDescription
        , AP.RaceCD, MDRAC.[Description] AS RaceDescription
        , AP.EthnicityCD, MDETH.[Description] AS EthnicityDescription
        , AP.Pronouns
        , AP.EverLivedInWestchesterInd
            , CASE WHEN AP.EverLivedInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS EverLivedInWestchesterDescription
        , AP.CountyLivingIn
        , AP.CurrentlyWorkingInWestchesterInd
            , CASE WHEN AP.CurrentlyWorkingInWestchesterInd = 1 THEN 'YES' ELSE 'NO' END AS CurrentlyWorkingInWestchesterDescription
        , AP.CountyWorkingIn
        , AP.StudentInd
            , CASE WHEN AP.StudentInd = 1 THEN 'YES' ELSE 'NO' END AS StudentDescription
        , AP.DisabilityInd
            , CASE WHEN AP.DisabilityInd = 1 THEN 'YES' ELSE 'NO' END AS DisabilityDescription
        , AP.VeteranInd
            , CASE WHEN AP.VeteranInd = 1 THEN 'YES' ELSE 'NO' END AS VeteranDescription
        , AP.PhoneNumber, AP.PhoneNumberExtn, AP.PhoneNumberTypeCD, MDPHT.[Description] AS PhoneNumberTypeDescription
        , AP.AltPhoneNumber, AP.AltPhoneNumberExtn, AP.AltPhoneNumberTypeCD, MDPHTA.[Description] AS AltPhoneNumberTypeDescription
        , NULL AS Username
        , AP.EmailAddress, CONVERT(BIT, 0) AS AuthRepEmailAddressInd, AP.AltEmailAddress
        , AP.OwnRealEstateInd, CASE WHEN AP.OwnRealEstateInd = 1 THEN 'YES' ELSE 'NO' END AS OwnRealEstateDescription, AP.RealEstateValueAmt
        , AP.AssetValueAmt, AP.IncomeValueAmt
        , AP.CreatedBy, AP.CreatedDate, AP.ModifiedBy, AP.ModifiedDate, AP.Active
        , AP.ApplicantSortOrder
        , HA.ListingID, HA.StatusCD
    FROM dbo.tblHousingApplicants AS AP
    JOIN dbo.tblHousingApplications AS HA ON AP.ApplicationID = HA.ApplicationID
    LEFT OUTER JOIN dbo.tblMetadata AS MDIT ON MDIT.CodeID = 125 AND MDIT.Code = AP.IDTypeCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDREL ON MDREL.CodeID = 109 AND MDREL.Code = AP.RelationTypeCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDGND ON MDGND.CodeID = 110 AND MDGND.Code = AP.GenderCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = AP.RaceCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDETH ON MDETH.CodeID = 112 AND MDETH.Code = AP.EthnicityCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = AP.PhoneNumberTypeCD
    LEFT OUTER JOIN dbo.tblMetadata AS MDPHTA ON MDPHTA.CodeID = 114 AND MDPHTA.Code = AP.AltPhoneNumberTypeCD
GO