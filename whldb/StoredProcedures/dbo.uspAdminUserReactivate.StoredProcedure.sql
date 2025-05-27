DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserReactivate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Reactivate an existing Admin User
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspAdminUserReactivate @UserID = 'USER', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserReactivate]
	@UserID				VARCHAR(4)
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblAdminUsers
		SET
			Active 			= 1
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
			, DeactivatedBy	= NULL
			, DeactivatedDate = NULL
		WHERE UserID = @UserID AND Active = 0;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('USER', @UserID, @ModifiedBy, 'UPDATE', 'Reactivated admin user', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to reactivate admin user - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO