DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserUpdateAccountProfile];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update site user profile
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserUpdateAccountProfile @Username = 'USERNAME', @FirstName = 'FirstName', @LastName = 'LastName
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserUpdateAccountProfile]
	@Username							VARCHAR(200)
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
	, @HouseholdSize					INT = 1
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUsers
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
			, ModifiedBy					= @ModifiedBy
			, ModifiedDate					= GETDATE()
		WHERE Username = @Username AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated Site User Profile', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Site User Profile - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO