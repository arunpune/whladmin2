DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserRetrieveByKey];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Retrieve a single user by key
-- Examples:
--	EXEC dbo.uspSiteUserRetrieveByKey @ActivationKey = 'KEY' (Retrieve By Activation Key)
--	EXEC dbo.uspSiteUserRetrieveByKey @PasswordResetKey = 'KEY' (Retrieve By Password Reset Key)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserRetrieveByKey]
	@ActivationKey		VARCHAR(32) = NULL
	, @PasswordResetKey	VARCHAR(32) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SET @ActivationKey		= NULLIF(ISNULL(RTRIM(@ActivationKey), ''), '');
	SET @PasswordResetKey	= NULLIF(ISNULL(RTRIM(@PasswordResetKey), ''), '');

	SELECT U.Username, U.PasswordHash
		, U.ActivationKey, U.ActivationKeyExpiry, U.UsernameVerifiedInd
		, U.EmailAddress, U.AuthRepEmailAddressInd, U.AltEmailAddress, U.AltEmailAddressKey, U.AltEmailAddressKeyExpiry, U.AltEmailAddressVerifiedInd
		, U.PhoneNumber, U.PhoneNumberExtn, U.PhoneNumberTypeCD, MDPHT.[Description] AS PhoneNumberTypeDescription
		, U.PhoneNumberKey, U.PhoneNumberKeyExpiry, U.PhoneNumberVerifiedInd
		, U.AltPhoneNumber, U.AltPhoneNumberExtn, U.AltPhoneNumberTypeCD, MDPHTA.[Description] AS AltPhoneNumberTypeDescription
		, U.AltPhoneNumberKey, U.AltPhoneNumberKeyExpiry, U.AltPhoneNumberVerifiedInd
		, U.LeadTypeCD, MDLT.[Description] AS LeadTypeDescription, U.LeadTypeOther
		, U.LastLoggedIn
		, U.Title, U.FirstName, U.MiddleName, U.LastName, U.Suffix
		, U.DateOfBirth, U.Last4SSN
		, U.IDTypeCD, MDIT.[Description] AS IDTypeDescription, U.IDTypeValue, U.IDIssueDate
		, U.GenderCD, MDGND.[Description] AS GenderDescription
		, U.RaceCD, MDRAC.[Description] AS RaceDescription
		, U.EthnicityCD, MDETH.[Description] AS EthnicityDescription
		, U.Pronouns
		, U.StudentInd, U.DisabilityInd, U.VeteranInd
		, U.EverLivedInWestchesterInd, U.CountyLivingIn, U.CurrentlyWorkingInWestchesterInd, U.CountyWorkingIn
		, U.OwnRealEstateInd, U.RealEstateValueAmt, U.IncomeValueAmt, U.AssetValueAmt
		, U.LanguagePreferenceCD, MDLNG.[Description] AS LanguagePreferenceDescription, U.LanguagePreferenceOther
		, U.ListingPreferenceCD, MDLST.[Description] AS ListingPreferenceDescription
		, U.SmsNotificationsPreferenceInd
		, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate
		, U.DeactivatedBy, U.DeactivatedDate
		, U.PasswordResetKey, U.PasswordResetKeyExpiry
		, U.HouseholdSize
		, U.Active
	FROM dbo.tblSiteUsers U
	LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = U.IDTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHT ON MDPHT.CodeID = 114 AND MDPHT.Code = U.PhoneNumberTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDPHTA ON MDPHTA.CodeID = 114 AND MDPHTA.Code = U.AltPhoneNumberTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDLT ON MDLT.CodeID = 120 AND MDLT.Code = U.LeadTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDGND ON MDGND.CodeID = 110 AND MDGND.Code = U.GenderCD
	LEFT OUTER JOIN dbo.tblMetadata MDRAC ON MDRAC.CodeID = 111 AND MDRAC.Code = U.RaceCD
	LEFT OUTER JOIN dbo.tblMetadata MDETH ON MDETH.CodeID = 112 AND MDETH.Code = U.EthnicityCD
	LEFT OUTER JOIN dbo.tblMetadata MDLNG ON MDLNG.CodeID = 113 AND MDLNG.Code = U.LanguagePreferenceCD
	LEFT OUTER JOIN dbo.tblMetadata MDLST ON MDLST.CodeID = 107 AND MDLST.Code = U.ListingPreferenceCD
	WHERE (@ActivationKey IS NOT NULL AND U.ActivationKey = @ActivationKey)
		OR (@PasswordResetKey IS NOT NULL AND U.PasswordResetKey = @PasswordResetKey)
	ORDER BY U.FirstName;

END
GO