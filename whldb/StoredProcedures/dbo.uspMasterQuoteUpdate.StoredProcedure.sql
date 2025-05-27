DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuoteUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 23-Sep-2024
-- Description:	Update an existing Quote
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterQuoteUpdate @QuoteID = 1, @Text = 'TEXT', @Active = 1, @ModifiedBy = 'USERNAME'
--									, @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuoteUpdate]
	@QuoteID				INT
	, @Text					VARCHAR(1000)
	, @DisplayOnHomePageInd	BIT = 0
	, @Active				BIT
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF @DisplayOnHomePageInd = 1
			UPDATE dbo.tblMasterQuotes SET DisplayOnHomePageInd = 0 WHERE QuoteID <> @QuoteID;

		UPDATE dbo.tblMasterQuotes
		SET
			[Text]					= @Text
			, DisplayOnHomePageInd	= @DisplayOnHomePageInd
			, Active				= @Active
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE QuoteID = @QuoteID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('QUOTE', CONVERT(VARCHAR(20), @QuoteID), @ModifiedBy, 'UPDATE', 'Updated Quote', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Quote - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO