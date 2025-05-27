DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationAdd];
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
--	EXEC dbo.uspHousingApplicationAdd @ListingID = 1, @Username = 'john@nys.gov'
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @LeadTypeCD = 'OTHER'
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	EXEC dbo.uspHousingApplicationAdd @ListingID = 1
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @LeadTypeCD = 'OTHER'
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationAdd]
	@ListingID							INT
	, @Username							VARCHAR(200) = NULL
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
	, @SubmissionTypeCD					VARCHAR(20)
	, @CreatedBy						VARCHAR(200)
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
						, @OwnRealEstateInd, @RealEstateValueAmt, @AssetValueAmt, @IncomeValueAmt
						, @LeadTypeCD, @LeadTypeOther, @SubmissionTypeCD
						, 'DRAFT'
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @CreatedBy, 'ADD', 'Added Housing Application', GETDATE());

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