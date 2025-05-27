DROP PROCEDURE IF EXISTS [dbo].[uspMasterFundingSourceUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Dec-2024
-- Description:	Update an existing Funding Source
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterFundingSourceUpdate @Name = 'NAME', @Description = 'DESCRIPTION', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFundingSourceUpdate]
	@FundingSourceID		INT
	, @Name			VARCHAR(200)
	, @Description	VARCHAR(4000)
	, @Active		BIT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterFundingSources
		SET
			[Name]			= @Name
			, [Description] = @Description
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE FundingSourceID = @FundingSourceID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('FUNDSRC', CONVERT(VARCHAR(20), @FundingSourceID), @ModifiedBy, 'UPDATE', 'Updated Funding Source', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Funding Source - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO