DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationSubmitEx];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Cot-2024
-- Description:	Submit an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationSubmitEx @ApplicationID = 1, @Username = 'USERNAME', @StatusCD = 'SUBMITTED'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationSubmitEx]
	@ApplicationID				BIGINT
	, @ListingID				INT
	, @Username					VARCHAR(200)
	, @UnitTypeCDs				VARCHAR(200) = NULL
	, @MemberIDs				VARCHAR(1000) = NULL
	, @CoApplicantMemberID		BIGINT = NULL
	, @AccountIDs				VARCHAR(1000) = NULL
	, @AccessibilityCDs			VARCHAR(1000) = NULL
	, @LeadTypeCD				VARCHAR(20)
	, @LeadTypeOther			VARCHAR(500) = NULL
	, @ModifiedBy				VARCHAR(200)
	, @StatusCD					VARCHAR(20)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @AppNote VARCHAR(1000), @UserNote VARCHAR(1000);
	SET @AppNote = 'Submitted housing application'
					+ CASE WHEN @StatusCD = 'WAITLISTED' THEN ' for waitlist' ELSE '' END;
	SET @UserNote = @AppNote + ' ' + CONVERT(VARCHAR(20), @ApplicationID) + ' for listing ' + CONVERT(VARCHAR(20), @ListingID);

	DECLARE @MemberIDsTable TABLE (
		MemberID BIGINT
	);
	IF LEN(ISNULL(RTRIM(@MemberIDs), '')) > 0
		INSERT INTO @MemberIDsTable (MemberID)
			SELECT [value] FROM STRING_SPLIT(@MemberIDs, ',')
			WHERE ISNULL(RTRIM([value]), '') <> '';

	BEGIN TRY

		BEGIN TRAN;

		IF ISNULL(@ApplicationID, 0) > 0
		BEGIN

			UPDATE dbo.tblHousingApplications
			SET
				UnitTypeCDs				= @UnitTypeCDs
				, CoApplicantMemberID	= @CoApplicantMemberID
				, MemberIDs				= @MemberIDs
				, AccountIDs			= @AccountIDs
				, AccessibilityCDs		= @AccessibilityCDs
				, LeadTypeCD			= @LeadTypeCD
				, LeadTypeOther			= @LeadTypeOther
				, StatusCD				= @StatusCD
				, OriginalStatusCD		= @StatusCD
				, SubmittedDate			= GETDATE()
				, ModifiedBy			= @ModifiedBy
				, ModifiedDate			= GETDATE()
			WHERE ApplicationID = @ApplicationID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', @AppNote, GETDATE())
					, ('SITEUSER', @Username, @Username, 'UPDATE', @Usernote, GETDATE());

		END -- IF ISNULL(@ApplicationID, 0) > 0
		ELSE
		BEGIN

			SELECT @ApplicationID = NEXT VALUE FOR [dbo].[seqApplicationID];

			DECLARE @HouseholdID BIGINT;
			SET @HouseholdID = ISNULL((SELECT HouseholdID FROM dbo.tblHouseholds WHERE Username = @Username), 0);

			INSERT INTO dbo.tblHousingApplications (ApplicationID, ListingID, Username, HouseholdID
													, UnitTypeCDs, CoApplicantMemberID, MemberIDs, AccountIDs
													, Title, FirstName, MiddleName, LastName, Suffix
													, DateOfBirth, Last4SSN, IDTypeCD, IDTypeValue, IDIssueDate
													, GenderCD, RaceCD, EthnicityCD, Pronouns
													, StudentInd, DisabilityInd, VeteranInd
													, PhoneNumberTypeCD, PhoneNumber, PhoneNumberExtn
													, AltPhoneNumberTypeCD, AltPhoneNumber, AltPhoneNumberExtn
													, EmailAddress, AltEmailAddress
													, EverLivedInWestchesterInd, CountyLivingIn
													, CurrentlyWorkingInWestchesterInd, CountyWorkingIn
													, HouseholdSize
													, OwnRealEstateInd, RealEstateValueAmt, AssetValueAmt, IncomeValueAmt
													, AccessibilityCDs
													, LeadTypeCD, LeadTypeOther, SubmissionTypeCD
													, StatusCD, OriginalStatusCD, SubmittedDate
													, CreatedBy, CreatedDate, Active)
				SELECT @ApplicationID, @ListingID, @Username, @HouseholdID
					, @UnitTypeCDs, @CoApplicantMemberID, @MemberIDs, @AccountIDs
					, U.Title, U.FirstName, U.MiddleName, U.LastName, U.Suffix
					, U.DateOfBirth, U.Last4SSN, U.IDTypeCD, U.IDTypeValue, U.IDIssueDate
					, U.GenderCD, U.RaceCD, U.EthnicityCD, U.Pronouns
					, U.StudentInd, U.DisabilityInd, U.VeteranInd
					, U.PhoneNumberTypeCD, U.PhoneNumber, U.PhoneNumberExtn
					, U.AltPhoneNumberTypeCD, U.AltPhoneNumber, U.AltPhoneNumberExtn
					, U.EmailAddress, U.AltEmailAddress
					, U.EverLivedInWestchesterInd, U.CountyLivingIn
					, U.CurrentlyWorkingInWestchesterInd, U.CountyWorkingIn
					, U.HouseholdSize
					, U.OwnRealEstateInd, U.RealEstateValueAmt, U.AssetValueAmt, U.IncomeValueAmt
					, @AccessibilityCDs
					, @LeadTypeCD, @LeadTypeOther, 'ONLINE'
					, @StatusCD, @StatusCD, GETDATE()
					, @ModifiedBy, GETDATE(), 1
				FROM dbo.tblSiteUsers U
				WHERE U.Username = @Username;

			UPDATE HA
			SET
				HA.AddressInd				= H.AddressInd
				, HA.PhysicalStreetLine1	= H.PhysicalStreetLine1
				, HA.PhysicalStreetLine2	= H.PhysicalStreetLine2
				, HA.PhysicalStreetLine3	= H.PhysicalStreetLine3
				, HA.PhysicalCity			= H.PhysicalCity
				, HA.PhysicalStateCD		= H.PhysicalStateCD
				, HA.PhysicalZipCode		= H.PhysicalZipCode
				, HA.PhysicalCounty			= H.PhysicalCounty
				, HA.DifferentMailingAddressInd = H.DifferentMailingAddressInd
				, HA.MailingStreetLine1		= H.MailingStreetLine1
				, HA.MailingStreetLine2		= H.MailingStreetLine2
				, HA.MailingStreetLine3		= H.MailingStreetLine3
				, HA.MailingCity			= H.MailingCity
				, HA.MailingStateCD			= H.MailingStateCD
				, HA.MailingZipCode			= H.MailingZipCode
				, HA.MailingCounty			= H.MailingCounty
				, HA.VoucherInd				= H.VoucherInd
				, HA.VoucherCDs				= H.VoucherCDs
				, HA.VoucherOther			= H.VoucherOther
				, HA.VoucherAdminName		= H.VoucherAdminName
				, HA.LiveInAideInd			= H.LiveInAideInd
			FROM dbo.tblHousingApplications HA
			JOIN dbo.tblHouseholds H ON H.Username = @Username
			WHERE HA.ApplicationID = @ApplicationID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'ADD', @AppNote, GETDATE())
					, ('SITEUSER', @Username, @Username, 'UPDATE', @UserNote, GETDATE());

		END

		DECLARE @ApplicantsMap TABLE (
			MemberID		BIGINT
			, ApplicantID	BIGINT
			, ApplicantSortOrder INT
		);

		-- Add Primary Applicant
		DELETE FROM dbo.tblHousingApplicants WHERE ApplicationID = @ApplicationID;
		INSERT INTO dbo.tblHousingApplicants (ApplicationID, MemberID, CoApplicantInd
												, RelationTypeCD, RelationTypeOther
												, Title, FirstName, MiddleName, LastName, Suffix
												, DateOfBirth, Last4SSN, IDTypeCD, IDTypeValue, IDIssueDate
												, GenderCD, RaceCD, EthnicityCD, Pronouns
												, StudentInd, DisabilityInd, VeteranInd
												, PhoneNumberTypeCD, PhoneNumber, PhoneNumberExtn
												, AltPhoneNumberTypeCD, AltPhoneNumber, AltPhoneNumberExtn
												, EmailAddress, AltEmailAddress
												, EverLivedInWestchesterInd, CountyLivingIn
												, CurrentlyWorkingInWestchesterInd, CountyWorkingIn
												, OwnRealEstateInd, RealEstateValueAmt, AssetValueAmt, IncomeValueAmt
												, ApplicantSortOrder
												, CreatedBy, CreatedDate, Active)
			OUTPUT INSERTED.MemberID, INSERTED.ApplicantID, INSERTED.ApplicantSortOrder
				INTO @ApplicantsMap (MemberID, ApplicantID, ApplicantSortOrder)
			SELECT @ApplicationID, 0, 0
				, 'SELF', NULL
				, U.Title, U.FirstName, U.MiddleName, U.LastName, U.Suffix
				, U.DateOfBirth, U.Last4SSN, U.IDTypeCD, U.IDTypeValue, U.IDIssueDate
				, U.GenderCD, U.RaceCD, U.EthnicityCD, U.Pronouns
				, U.StudentInd, U.DisabilityInd, U.VeteranInd
				, U.PhoneNumberTypeCD, U.PhoneNumber, U.PhoneNumberExtn
				, U.AltPhoneNumberTypeCD, U.AltPhoneNumber, U.AltPhoneNumberExtn
				, U.EmailAddress, U.AltEmailAddress
				, U.EverLivedInWestchesterInd, U.CountyLivingIn
				, U.CurrentlyWorkingInWestchesterInd, U.CountyWorkingIn
				, U.OwnRealEstateInd, U.RealEstateValueAmt, U.AssetValueAmt, U.IncomeValueAmt
				, 1
				, @ModifiedBy, GETDATE(), 1
			FROM dbo.tblSiteUsers U
			WHERE U.Username = @Username;

		-- Add Co-Applicants and Household Members
		IF EXISTS (SELECT 1 FROM @MemberIDsTable)
			INSERT INTO dbo.tblHousingApplicants (ApplicationID, MemberID, CoApplicantInd
													, RelationTypeCD, RelationTypeOther
													, Title, FirstName, MiddleName, LastName, Suffix
													, DateOfBirth, Last4SSN, IDTypeCD, IDTypeValue, IDIssueDate
													, GenderCD, RaceCD, EthnicityCD, Pronouns
													, StudentInd, DisabilityInd, VeteranInd
													, PhoneNumberTypeCD, PhoneNumber, PhoneNumberExtn
													, AltPhoneNumberTypeCD, AltPhoneNumber, AltPhoneNumberExtn
													, EmailAddress, AltEmailAddress
													, EverLivedInWestchesterInd, CountyLivingIn
													, CurrentlyWorkingInWestchesterInd, CountyWorkingIn
													, OwnRealEstateInd, RealEstateValueAmt, AssetValueAmt, IncomeValueAmt
													, ApplicantSortOrder
													, CreatedBy, CreatedDate, Active)
				OUTPUT INSERTED.MemberID, INSERTED.ApplicantID, INSERTED.ApplicantSortOrder
					INTO @ApplicantsMap (MemberID, ApplicantID, ApplicantSortOrder)
				SELECT @ApplicationID, U.MemberID, CASE WHEN U.MemberID = @CoApplicantMemberID THEN 1 ELSE 0 END
					, U.RelationTypeCD, U.RelationTypeOther
					, U.Title, U.FirstName, U.MiddleName, U.LastName, U.Suffix
					, U.DateOfBirth, U.Last4SSN, U.IDTypeCD, U.IDTypeValue, U.IDIssueDate
					, U.GenderCD, U.RaceCD, U.EthnicityCD, U.Pronouns
					, U.StudentInd, U.DisabilityInd, U.VeteranInd
					, U.PhoneNumberTypeCD, U.PhoneNumber, U.PhoneNumberExtn
					, U.AltPhoneNumberTypeCD, U.AltPhoneNumber, U.AltPhoneNumberExtn
					, U.EmailAddress, U.AltEmailAddress
					, U.EverLivedInWestchesterInd, U.CountyLivingIn
					, U.CurrentlyWorkingInWestchesterInd, U.CountyWorkingIn
					, U.OwnRealEstateInd, U.RealEstateValueAmt, U.AssetValueAmt, U.IncomeValueAmt
					, CASE WHEN U.MemberID = @CoApplicantMemberID THEN 2 ELSE 999 END
					, @ModifiedBy, GETDATE(), 1
				FROM dbo.tblHouseholdMembers U
				JOIN @MemberIDsTable T ON T.MemberID = U.MemberID
				WHERE U.HouseholdID = @HouseholdID;

		-- Update ApplicantSortOrder
		WITH C AS (
			SELECT ApplicantID, 2 + ROW_NUMBER() OVER (ORDER BY ApplicantID) AS ApplicantSortOrder
			FROM @ApplicantsMap
			WHERE ApplicantSortOrder = 999
		)
			UPDATE HA
			SET HA.ApplicantSortOrder = C.ApplicantSortOrder
			FROM tblHousingApplicants HA
			JOIN C ON C.ApplicantID = HA.ApplicantID;

		-- Add Assets
		DELETE FROM dbo.tblHousingApplicantAccounts WHERE ApplicationID = @ApplicationID;
		INSERT INTO dbo.tblHousingApplicantAccounts (ApplicationID, ApplicantID
												, AccountNumber, AccountTypeCD, AccountTypeOther
												, AccountValueAmt, InstitutionName
												, CreatedBy, CreatedDate, Active)
			SELECT @ApplicationID, APPL.ApplicantID
				, ACCT.AccountNumber, ACCT.AccountTypeCD, ACCT.AccountTypeOther
				, ACCT.AccountValueAmt, ACCT.InstitutionName
				, @ModifiedBy, GETDATE(), 1
			FROM dbo.tblHouseholdAccounts ACCT
			JOIN @ApplicantsMap APPL ON APPL.MemberID = ACCT.PrimaryHolderMemberID
			WHERE ACCT.HouseholdID = @HouseholdID;

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ApplicationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to submit Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO