DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuoteAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 23-Sep-2024
-- Description:	Add a new Quote
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterQuoteAdd @Text = 'TEXT', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuoteAdd]
	@Text					VARCHAR(1000)
	, @DisplayOnHomePageInd	BIT = 0
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF @DisplayOnHomePageInd = 1
			UPDATE dbo.tblMasterQuotes SET DisplayOnHomePageInd = 0;

		DECLARE @QuoteID INT;

		INSERT INTO dbo.tblMasterQuotes ([Text], DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@Text, @DisplayOnHomePageInd, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @QuoteID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('QUOTE', CONVERT(VARCHAR(20), @QuoteID), @CreatedBy, 'ADD', 'Added Quote', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Quote - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO