DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdSaveMemberProfile];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Add/update a household member
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdSaveMemberProfile @Username = 'USERNAME', @HouseholdID = 1, @MemberID = 0
--										, @FirstName = 'FirstName', @LastName = 'LastName
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdSaveMemberProfile]
	@Username							VARCHAR(200)
	, @HouseholdID						BIGINT
	, @MemberID							BIGINT
	, @RelationTypeCD					VARCHAR(20) = 'OTHER'
	, @RelationTypeOther				VARCHAR(100) = NULL
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
	, @GenderCD							VARCHAR(20) = ''
	, @RaceCD							VARCHAR(20) = ''
	, @EthnicityCD						VARCHAR(20) = ''
	, @StudentInd						BIT	= 0
	, @DisabilityInd					BIT = 0
	, @VeteranInd						BIT = 0
	, @Pronouns							VARCHAR(20) = NULL
	, @EverLivedInWestchesterInd		BIT = 0
	, @CountyLivingIn					VARCHAR(100) = NULL
	, @CurrentlyWorkingInWestchesterInd BIT = 0
	, @CountyWorkingIn					VARCHAR(100) = NULL
	, @PhoneNumber						VARCHAR(10) = NULL
	, @PhoneNumberExtn					VARCHAR(10) = NULL
	, @PhoneNumberTypeCD				VARCHAR(20) = NULL
	, @AltPhoneNumber					VARCHAR(10) = NULL
	, @AltPhoneNumberExtn				VARCHAR(10) = NULL
	, @AltPhoneNumberTypeCD				VARCHAR(20) = NULL
	, @EmailAddress						VARCHAR(200) = NULL
	, @AltEmailAddress					VARCHAR(200) = NULL
	, @OwnRealEstateInd					BIT = 0
	, @RealEstateValueAmt				DECIMAL(17, 2) = NULL
	, @AssetValueAmt					DECIMAL(17, 2) = NULL
	, @IncomeValueAmt					DECIMAL(17, 2) = NULL
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF @MemberID > 0
		BEGIN

			UPDATE dbo.tblHouseholdMembers
			SET
				RelationTypeCD						= @RelationTypeCD
				, RelationTypeOther					= @RelationTypeOther
				, Title								= @Title
				, FirstName							= @FirstName
				, MiddleName						= @MiddleName
				, LastName							= @LastName
				, Suffix							= @Suffix
				, DateOfBirth						= @DateOfBirth
				, Last4SSN							= @Last4SSN
				, IDTypeCD							= @IDTypeCD
				, IDTypeValue						= @IDTypeValue
				, IDIssueDate						= @IDIssueDate
				, GenderCD							= @GenderCD
				, RaceCD							= @RaceCD
				, EthnicityCD						= @EthnicityCD
				, StudentInd						= @StudentInd
				, DisabilityInd						= @DisabilityInd
				, VeteranInd						= @VeteranInd
				, Pronouns							= @Pronouns
				, EverLivedInWestchesterInd 		= @EverLivedInWestchesterInd
				, CountyLivingIn					= @CountyLivingIn
				, CurrentlyWorkingInWestchesterInd	= @CurrentlyWorkingInWestchesterInd
				, CountyWorkingIn					= @CountyWorkingIn
				, PhoneNumber						= @PhoneNumber
				, PhoneNumberExtn					= @PhoneNumberExtn
				, PhoneNumberTypeCD					= @PhoneNumberTypeCD
				, AltPhoneNumber					= @AltPhoneNumber
				, AltPhoneNumberExtn				= @AltPhoneNumberExtn
				, AltPhoneNumberTypeCD				= @AltPhoneNumberTypeCD
				, EmailAddress						= @EmailAddress
				, AltEmailAddress					= @AltEmailAddress
				, OwnRealEstateInd					= @OwnRealEstateInd
				, RealEstateValueAmt				= @RealEstateValueAmt
				, AssetValueAmt						= @AssetValueAmt
				, IncomeValueAmt					= @IncomeValueAmt
				, ModifiedBy						= @ModifiedBy
				, ModifiedDate						= GETDATE()
			WHERE MemberID = @MemberID AND Active = 1;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated Household Member Profile', GETDATE())
					, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated Household Member Profile', GETDATE())
					, ('HOUSEHOLDMBR', CONVERT(VARCHAR(200), @MemberID), @ModifiedBy, 'UPDATE', 'Updated Household Member Profile', GETDATE());

		END

		ELSE
		BEGIN

			INSERT INTO dbo.tblHouseholdMembers (HouseholdID, Title, FirstName, MiddleName, LastName, Suffix
													, RelationTypeCD, RelationTypeOther
													, DateOfBirth, Last4SSN
													, IDTypeCD, IDTypeValue, IDIssueDate
													, GenderCD, RaceCD, EthnicityCD
													, StudentInd, DisabilityInd, VeteranInd, Pronouns
													, CountyLivingIn, CountyWorkingIn
													, PhoneNumber, PhoneNumberExtn, PhoneNumberTypeCD
													, AltPhoneNumber, AltPhoneNumberExtn, AltPhoneNumberTypeCD
													, EmailAddress, AltEmailAddress
													, OwnRealEstateInd, RealEstateValueAmt, AssetValueAmt, IncomeValueAmt
													, CreatedBy, CreatedDate)
				VALUES (@HouseholdID, @Title, @FirstName, @MiddleName, @LastName, @Suffix
							, @RelationTypeCD, @RelationTypeOther
							, @DateOfBirth, @Last4SSN
							, @IDTypeCD, @IDTypeValue, @IDIssueDate
							, @GenderCD, @RaceCD, @EthnicityCD
							, @StudentInd, @DisabilityInd, @VeteranInd, @Pronouns
							, @CountyLivingIn, @CountyWorkingIn
							, @PhoneNumber, @PhoneNumberExtn, @PhoneNumberTypeCD
							, @AltPhoneNumber, @AltPhoneNumberExtn, @AltPhoneNumberTypeCD
							, @EmailAddress, @AltEmailAddress
							, @OwnRealEstateInd, @RealEstateValueAmt, @AssetValueAmt, @IncomeValueAmt
							, @ModifiedBy, GETDATE())
			SELECT @MemberID = SCOPE_IDENTITY();

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Added Household Member', GETDATE())
					, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Added Household Member Profile', GETDATE())
					, ('HOUSEHOLDMBR', CONVERT(VARCHAR(200), @MemberID), @ModifiedBy, 'ADD', 'Added Household Member Profile', GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @MemberID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Household Member - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO