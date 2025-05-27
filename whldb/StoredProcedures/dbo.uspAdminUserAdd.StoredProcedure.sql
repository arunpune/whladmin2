DROP PROCEDURE IF EXISTS [dbo].[uspAdminUserAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- DisplayName:	Add a new Admin User
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspAdminUserAdd @UserID = 'UID', @EmailAddress = 'EMAIL', @DisplayName = 'NAME', @OrganizationCD = 'ORGCD', @RoleCD = 'ROLECD', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspAdminUserAdd]
	@UserID				VARCHAR(4)
	, @EmailAddress		VARCHAR(200)
	, @DisplayName		VARCHAR(200)
	, @OrganizationCD	VARCHAR(20)
	, @RoleCD			VARCHAR(20)
	, @CreatedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		INSERT INTO dbo.tblAdminUsers (UserID, [EmailAddress], DisplayName, OrganizationCD, RoleCD
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@UserID, @EmailAddress, @DisplayName, @OrganizationCD, @RoleCD
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('USER', @UserID, @CreatedBy, 'ADD', 'Added admin user', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add admin user - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO