DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuestionUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Update an existing Question
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterQuestionUpdate @QuestionID = 1, @CategoryCD = 'PROFILE', @Title = 'TITLE'
--										, @AnswerTypeCD = 'TYPECD', @Active = 1, @ModifiedBy = 'USERNAME'
--										, @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuestionUpdate]
	@QuestionID		INT
	, @CategoryCD	VARCHAR(20)
	, @Title		VARCHAR(500)
	, @AnswerTypeCD	VARCHAR(20)
	, @OptionsList	VARCHAR(1000) = NULL
	, @MinLength	INT = 0
	, @MaxLength	INT = 0
	, @HelpText		VARCHAR(1000) = NULL
	, @Active		BIT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterQuestions
		SET
			CategoryCD		= @CategoryCD
			, Title			= @Title
			, AnswerTypeCD	= @AnswerTypeCD
			, OptionsList	= @OptionsList
			, MinLength		= @MinLength
			, [MaxLength]	= @MaxLength
			, HelpText		= @HelpText
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE QuestionID = @QuestionID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('QUESTION', CONVERT(VARCHAR(20), @QuestionID), @ModifiedBy, 'UPDATE', 'Updated Question', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Question - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO