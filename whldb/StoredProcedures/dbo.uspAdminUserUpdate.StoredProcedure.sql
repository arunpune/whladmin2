DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- DisplayName:	Update an existing Admin User
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspAdminUserUpdate @UserID = 'USER', @EmailAddress = 'EMAIL', @DisplayName = 'NAME', @OrganizationCD = 'ORGCD', @RoleCD = 'ROLECD', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserUpdate]
	@UserID				VARCHAR(4)
	, @EmailAddress		VARCHAR(200)
	, @DisplayName		VARCHAR(200)
	, @OrganizationCD	VARCHAR(20)
	, @RoleCD			VARCHAR(20)
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
			EmailAddress	= @EmailAddress
			, DisplayName	= @DisplayName
			, OrganizationCD = @OrganizationCD
			, RoleCD		= @RoleCD
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE UserID = @UserID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('USER', @UserID, @ModifiedBy, 'UPDATE', 'Updated admin user', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update admin user - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO