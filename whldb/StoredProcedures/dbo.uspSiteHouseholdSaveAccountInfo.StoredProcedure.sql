DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdSaveAccountInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Sep-2024
-- Description:	Add/update a household account
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdSaveAccountInfo @Username = 'USERNAME', @HouseholdID = 1, @AccountID = 0
--											, @AccountNumber = '1234', @AccountTypeCD = 'CHECKING'
--											, @AccountValueAmt = 1000.00, @InstitutionName = 'NAME OF BANK'
--											, @PrimaryHolderMemberID = 0
--											, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdSaveAccountInfo]
	@Username							VARCHAR(200)
	, @HouseholdID						BIGINT
	, @AccountID						BIGINT
	, @AccountNumber					VARCHAR(4)
	, @AccountTypeCD					VARCHAR(20) = 'OTHER'
	, @AccountTypeOther					VARCHAR(100) = NULL
	, @AccountValueAmt					DECIMAL(17, 2) = NULL
	, @InstitutionName					VARCHAR(200) = NULL
	, @PrimaryHolderMemberID			BIGINT = 0
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF @AccountID > 0
		BEGIN

			UPDATE dbo.tblHouseholdAccounts
			SET
				AccountNumber			= @AccountNumber
				, AccountTypeCD			= @AccountTypeCD
				, AccountTypeOther		= @AccountTypeOther
				, AccountValueAmt		= @AccountValueAmt
				, InstitutionName		= @InstitutionName
				, PrimaryHolderMemberID	= @PrimaryHolderMemberID
				, ModifiedBy			= @ModifiedBy
				, ModifiedDate			= GETDATE()
			WHERE AccountID = @AccountID AND Active = 1;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated Household Account Information', GETDATE())
					, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated Household Account Information', GETDATE())
					, ('HOUSEHOLDACT', CONVERT(VARCHAR(200), @AccountID), @ModifiedBy, 'UPDATE', 'Updated Household Account Information', GETDATE());

		END

		ELSE
		BEGIN

			INSERT INTO dbo.tblHouseholdAccounts (HouseholdID, AccountNumber, AccountTypeCD, AccountTypeOther
													, AccountValueAmt, InstitutionName, PrimaryHolderMemberID
													, CreatedBy, CreatedDate)
				VALUES (@HouseholdID, @AccountNumber, @AccountTypeCD, @AccountTypeOther
							, @AccountValueAmt, @InstitutionName, @PrimaryHolderMemberID
							, @ModifiedBy, GETDATE())
			SELECT @AccountID = SCOPE_IDENTITY();

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Added Household Account', GETDATE())
					, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Added Household Account', GETDATE())
					, ('HOUSEHOLDACT', CONVERT(VARCHAR(200), @AccountID), @ModifiedBy, 'ADD', 'Added Household Account', GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Household Account - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO