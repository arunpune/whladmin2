DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationUpdate];
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
--	EXEC dbo.uspHousingApplicationUpdate @ApplicationID = 1
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @LeadTypeCD = 'OTHER'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationUpdate]
	@ApplicationID						BIGINT
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
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			Title								= @Title
			, FirstName							= @FirstName
			, MiddleName						= @MiddleName
			, LastName							= @LastName
			, Suffix							= @Suffix
			, GenderCD							= @GenderCD
			, RaceCD							= @RaceCD
			, EthnicityCD						= @EthnicityCD
			, StudentInd						= @StudentInd
			, DisabilityInd						= @DisabilityInd
			, VeteranInd						= @VeteranInd
			, Pronouns							= @Pronouns
			, EverLivedInWestchesterInd			= @EverLivedInWestchesterInd
			, CountyLivingIn					= @CountyLivingIn
			, CurrentlyWorkingInWestchesterInd = @CurrentlyWorkingInWestchesterInd
			, CountyWorkingIn					= @CountyWorkingIn
			, OwnRealEstateInd					= @OwnRealEstateInd
			, RealEstateValueAmt				= @RealEstateValueAmt
			, AssetValueAmt						= @AssetValueAmt
			, IncomeValueAmt					= @IncomeValueAmt
			, ModifiedBy						= @ModifiedBy
			, ModifiedDate						= GETDATE()
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Updated Housing Application', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ApplicationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO