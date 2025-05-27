DROP PROCEDURE IF EXISTS [dbo].[uspMasterMarketingAgentUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Mar-2025
-- Description:	Update an existing Marketing Agent
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterMarketingAgentUpdate @Name = 'NAME', @ContactName = 'DESCRIPTION', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterMarketingAgentUpdate]
	@AgentID		INT
	, @Name			VARCHAR(200)
	, @ContactName	VARCHAR(200) = NULL
	, @PhoneNumber	VARCHAR(20) = NULL
	, @EmailAddress	VARCHAR(200) = NULL
	, @Active		BIT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterMarketingAgents
		SET
			[Name]			= @Name
			, ContactName	= @ContactName
			, PhoneNumber	= @PhoneNumber
			, EmailAddress	= @EmailAddress
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE AgentID = @AgentID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('MKTAGENT', CONVERT(VARCHAR(20), @AgentID), @ModifiedBy, 'UPDATE', 'Updated Marketing Agent', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Marketing Agent - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO