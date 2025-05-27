DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdDeleteAccountInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Sep-2024
-- Description:	Delete a household account
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdDeleteAccountInfo @Username = 'USERNAME', @HouseholdID = 1, @AccountID = 1
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdDeleteAccountInfo]
	@Username			VARCHAR(200)
	, @HouseholdID		BIGINT
	, @AccountID		BIGINT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHouseholdAccounts
		SET
			Active			= 0
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE AccountID = @AccountID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Deleted Household Account Information', GETDATE())
				, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Deleted Household Account Information', GETDATE())
				, ('HOUSEHOLDMBR', CONVERT(VARCHAR(200), @AccountID), @ModifiedBy, 'DELETE', 'Deleted Household Account Information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Household Account - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO