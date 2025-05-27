DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Update an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationUpdate @ApplicationID = 1, @Username = 'john@nys.gov'
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationUpdate]
	@ApplicationID						BIGINT
	, @Username							VARCHAR(200)
	, @Title							VARCHAR(20) = NULL
	, @FirstName						VARCHAR(100)
	, @MiddleName						VARCHAR(100) = NULL
	, @LastName							VARCHAR(100)
	, @Suffix							VARCHAR(20) = NULL
	, @DateOfBirth						DATETIME = NULL
	, @Last4SSN							CHAR(4) = NULL
	, @IDTypeCD							VARCHAR(20) = NULL
	, @IDTypeValue						VARCHAR(20) = NULL
	, @IDIssueDate						DATETIME = NULL
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
	, @UpdateProfileInd					BIT = 0
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @HouseholdID BIGINT;
		SET @HouseholdID = ISNULL((SELECT HouseholdID FROM dbo.tblHouseholds WHERE Username = @Username), 0);

		UPDATE dbo.tblHousingApplications
		SET
			Title							= @Title
			, FirstName						= @FirstName
			, MiddleName					= @MiddleName
			, LastName						= @LastName
			, Suffix						= @Suffix
			, DateOfBirth					= @DateOfBirth
			, Last4SSN						= @Last4SSN
			, IDTypeCD						= @IDTypeCD
			, IDTypeValue					= @IDTypeValue
			, IDIssueDate					= @IDIssueDate
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
			, ModifiedBy					= @ModifiedBy
			, ModifiedDate					= GETDATE()
		WHERE ApplicationID = @ApplicationID AND StatusCD IN ('DRAFT');

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'ADD', 'Updated housing application', GETDATE())
				, ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated housing application ' + CONVERT(VARCHAR(20), @ApplicationID), GETDATE());

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
				, ModifiedBy					= @ModifiedBy
				, ModifiedDate					= GETDATE()
			WHERE Username = @Username AND Active = 1;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated site user profile', GETDATE());
		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO