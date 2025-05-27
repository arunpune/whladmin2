DROP PROCEDURE IF EXISTS [dbo].[uspMasterFundingSourceAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Add a new Funding Source
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterFundingSourceAdd @Name = 'NAME', @Description = 'DESCRIPTION', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFundingSourceAdd]
	@Name			VARCHAR(200)
	, @Description	VARCHAR(4000)
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @FundingSourceID INT;

		INSERT INTO dbo.tblMasterFundingSources ([Name], [Description], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@Name, @Description, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @FundingSourceID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('FUNDSRC', CONVERT(VARCHAR(20), @FundingSourceID), @CreatedBy, 'ADD', 'Added Funding Source', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Funding Source - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO