DROP PROCEDURE IF EXISTS [dbo].[uspMasterMarketingAgentAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Mar-2025
-- Description:	Add a new Marketing Agent
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterMarketingAgentAdd @Name = 'NAME', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterMarketingAgentAdd]
	@Name			VARCHAR(200)
	, @ContactName	VARCHAR(200) = NULL
	, @PhoneNumber	VARCHAR(20) = NULL
	, @EmailAddress	VARCHAR(200) = NULL
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @AgentID INT;

		INSERT INTO dbo.tblMasterMarketingAgents ([Name], ContactName, PhoneNumber, EmailAddress
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@Name, @ContactName, @PhoneNumber, @EmailAddress
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @AgentID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('MKTAGENT', CONVERT(VARCHAR(20), @AgentID), @CreatedBy, 'ADD', 'Added Marketing Agent', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Marketing Agent - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO