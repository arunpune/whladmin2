DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuestionAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Add a new Question
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterQuestionAdd @CategoryCD = 'PROFILE', @Title = 'TITLE', @AnswerTypeCD = 'TYPECD', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuestionAdd]
	@CategoryCD		VARCHAR(20)
	, @Title		VARCHAR(500)
	, @AnswerTypeCD	VARCHAR(20)
	, @OptionsList	VARCHAR(1000) = NULL
	, @MinLength	INT = 0
	, @MaxLength	INT = 0
	, @HelpText		VARCHAR(1000) = NULL
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @QuestionID INT;

		INSERT INTO dbo.tblMasterQuestions (CategoryCD, Title, AnswerTypeCD, OptionsList, MinLength, [MaxLength], HelpText
												, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@CategoryCD, @Title, @AnswerTypeCD, @OptionsList, @MinLength, @MaxLength, @HelpText
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @QuestionID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('QUESTION', CONVERT(VARCHAR(20), @QuestionID), @CreatedBy, 'ADD', 'Added Question', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Question - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO