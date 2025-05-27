DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationUpdateAccounts];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Update accounts associated with the application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationUpdateAccounts @ApplicationID = 1, @Username = 'USERNAME', @Last4SSN = '1234', @DateOfBirth = '1/1/2000'
--										, @IDTypeCD = 'STATEDL', @IDTypeValue = 'NY 1234', @IDIssueDate = '1/1/2000'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationUpdateAccounts]
	@ApplicationID			BIGINT
	, @Username				VARCHAR(200)
	, @AccountIDs			VARCHAR(1000) = NULL
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			AccountIDs			= @AccountIDs
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @Username, 'ADD', 'Updated accounts information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Accounts Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO