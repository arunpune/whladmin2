DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuestionRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of Question configurations, or a single one by QuestionID
-- Examples:
--	EXEC dbo.uspMasterQuestionRetrieve (Retrieve All)
--	EXEC dbo.uspMasterQuestionRetrieve @QuestionID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuestionRetrieve]
	@QuestionID	INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT Q.QuestionID, Q.CategoryCD, MC.[Description] AS CategoryDescription
		, Q.Title, Q.AnswerTypeCD, MA.[Description] AS AnswerTypeDescription
		, Q.OptionsList, Q.MinLength, Q.[MaxLength], Q.HelpText
		, Q.CreatedBy, Q.CreatedDate, Q.ModifiedBy, Q.ModifiedDate, Q.Active
	FROM dbo.tblMasterQuestions Q
	LEFT OUTER JOIN dbo.tblMetadata MC ON MC.CodeID = 117 AND MC.Code = Q.CategoryCD
	LEFT OUTER JOIN dbo.tblMetadata MA ON MA.CodeID = 105 AND MA.Code = Q.AnswerTypeCD
	WHERE @QuestionID = 0 OR Q.QuestionID = @QuestionID
	ORDER BY Q.[Title] ASC;

END
GO