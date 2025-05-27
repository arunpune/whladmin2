DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdUpdateLiveInAideInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update live-in aide information for an existing household
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdUpdateLiveInAideInfo @HouseholdID = 1, @Username = 'USERNAME', @LiveInAideInd = 1
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdUpdateLiveInAideInfo]
	@HouseholdID					BIGINT
	, @Username						VARCHAR(200)
	, @LiveInAideInd				BIT
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHouseholds
		SET
			LiveInAideInd		= @LiveInAideInd
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE HouseholdID = @HouseholdID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated Household Live-in Aide Information', GETDATE())
				, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated Household Live-in Aide Information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Household Live-in Aide Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO