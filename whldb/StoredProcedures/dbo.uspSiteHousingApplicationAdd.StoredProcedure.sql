DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Add a new Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationAdd @ListingID = 1, @Username = 'john@nys.gov'
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @LeadTypeCD = 'OTHER'
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationAdd]
	@ListingID							INT
	, @Username							VARCHAR(200)
	, @Title							VARCHAR(20) = NULL
	, @FirstName						VARCHAR(100)
	, @MiddleName						VARCHAR(100) = NULL
	, @LastName							VARCHAR(100)
	, @Suffix							VARCHAR(20) = NULL
	, @GenderCD							VARCHAR(20)
	, @RaceCD							VARCHAR(20)
	, @EthnicityCD						VARCHAR(20)
	, @StudentInd						BIT	= 0
	, @DisabilityInd					BIT = 0
	, @VeteranInd						BIT = 0
	, @Pronouns							VARCHAR(20) = NULL
	, @EverLivedInWestchesterInd		BIT = 0
	, @CountyLivingIn					VARCHAR(100) = NULL
	, @CurrentlyWorkingInWestchesterInd BIT = 0
	, @CountyWorkingIn					VARCHAR(100) = NULL
	, @HouseholdSize					INT = 1
	, @PhoneNumberTypeCD				VARCHAR(20) = NULL
	, @PhoneNumber						VARCHAR(10) = NULL
	, @PhoneNumberExtn					VARCHAR(10) = NULL
	, @AltPhoneNumberTypeCD				VARCHAR(20) = NULL
	, @AltPhoneNumber					VARCHAR(10) = NULL
	, @AltPhoneNumberExtn				VARCHAR(10) = NULL
	, @EmailAddress						VARCHAR(200) = NULL
	, @AltEmailAddress					VARCHAR(200) = NULL
	, @OwnRealEstateInd					BIT = 0
	, @RealEstateValueAmt				DECIMAL(17, 2) = NULL
	, @AssetValueAmt					DECIMAL(17, 2) = NULL
	, @IncomeValueAmt					DECIMAL(17, 2) = NULL
	, @LeadTypeCD						VARCHAR(20)
	, @LeadTypeOther					VARCHAR(500) = NULL
	, @CreatedBy						VARCHAR(200)
	, @UpdateProfileInd					BIT = 0
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ApplicationID BIGINT;
		SELECT @ApplicationID = NEXT VALUE FOR [dbo].[seqApplicationID];

		DECLARE @HouseholdID BIGINT;
		SET @HouseholdID = ISNULL((SELECT HouseholdID FROM dbo.tblHouseholds WHERE Username = @Username), 0);

		INSERT INTO dbo.tblHousingApplications (ApplicationID, ListingID, Username, HouseholdID
												, Title, FirstName, MiddleName, LastName, Suffix
												, GenderCD, RaceCD, EthnicityCD, Pronouns
												, StudentInd, DisabilityInd, VeteranInd
												, PhoneNumberTypeCD, PhoneNumber, PhoneNumberExtn
												, AltPhoneNumberTypeCD, AltPhoneNumber, AltPhoneNumberExtn
												, EmailAddress, AltEmailAddress
												, EverLivedInWestchesterInd, CountyLivingIn
												, CurrentlyWorkingInWestchesterInd, CountyWorkingIn
												, HouseholdSize
												, OwnRealEstateInd, RealEstateValueAmt, AssetValueAmt, IncomeValueAmt
												, LeadTypeCD, LeadTypeOther, SubmissionTypeCD
												, StatusCD
												, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ApplicationID, @ListingID, @Username, @HouseholdID
						, @Title, @FirstName, @MiddleName, @LastName, @Suffix
						, @GenderCD, @RaceCD, @EthnicityCD, @Pronouns
						, @StudentInd, @DisabilityInd, @VeteranInd
						, @PhoneNumberTypeCD, @PhoneNumber, @PhoneNumberExtn
						, @AltPhoneNumberTypeCD, @AltPhoneNumber, @AltPhoneNumberExtn
						, @EmailAddress, @AltEmailAddress
						, @EverLivedInWestchesterInd, @CountyLivingIn
						, @CurrentlyWorkingInWestchesterInd, @CountyWorkingIn
						, @HouseholdSize
						, @OwnRealEstateInd, @RealEstateValueAmt, @AssetValueAmt, @IncomeValueAmt
						, @LeadTypeCD, @LeadTypeOther, 'ONLINE'
						, 'DRAFT'
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);

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
		WHERE HA.ApplicationID = @ApplicationID ;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @CreatedBy, 'ADD', 'Added housing application', GETDATE())
				, ('SITEUSER', @Username, @Username, 'UPDATE', 'Added housing application ' + CONVERT(VARCHAR(20), @ApplicationID) + ' for listing ' + CONVERT(VARCHAR(20), @ListingID), GETDATE());

		IF ISNULL(@UpdateProfileInd, 0) = 1
		BEGIN
			UPDATE dbo.tblSiteUsers
			SET
				Title							= @Title
				, FirstName						= @FirstName
				, MiddleName					= @MiddleName
				, LastName						= @LastName
				, Suffix						= @Suffix
				, GenderCD						= @GenderCD
				, RaceCD						= @RaceCD
				, EthnicityCD					= @EthnicityCD
				, StudentInd					= @StudentInd
				, DisabilityInd					= @DisabilityInd
				, VeteranInd					= @VeteranInd
				, Pronouns						= @Pronouns
				, EverLivedInWestchesterInd		= @EverLivedInWestchesterInd
				, CountyLivingIn				= @CountyLivingIn
				, CurrentlyWorkingInWestchesterInd = @CurrentlyWorkingInWestchesterInd
				, CountyWorkingIn				= @CountyWorkingIn
				, HouseholdSize					= @HouseholdSize
				, PhoneNumber					= @PhoneNumber
				, PhoneNumberExtn				= @PhoneNumberExtn
				, PhoneNumberTypeCD				= @PhoneNumberTypeCD
				, AltPhoneNumber				= @AltPhoneNumber
				, AltPhoneNumberExtn			= @AltPhoneNumberExtn
				, AltPhoneNumberTypeCD			= @AltPhoneNumberTypeCD
				, AltEmailAddress				= @AltEmailAddress
				, OwnRealEstateInd				= @OwnRealEstateInd
				, RealEstateValueAmt			= @RealEstateValueAmt
				, AssetValueAmt					= @AssetValueAmt
				, IncomeValueAmt				= @IncomeValueAmt
				, ModifiedBy					= @CreatedBy
				, ModifiedDate					= GETDATE()
			WHERE Username = @Username AND Active = 1;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated site user profile', GETDATE());
		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ApplicationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO