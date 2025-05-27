DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationWithdraw];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Withdraw an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationWithdraw @ApplicationID = 1
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationWithdraw]
	@ApplicationID		BIGINT
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			StatusCD			= 'WITHDRAWN'
			, WithdrawnDate		= GETDATE()
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Withdrew Housing Application', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to withdraw Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO