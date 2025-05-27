DROP PROCEDURE IF EXISTS [dbo].[uspNoteRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of notes by Entity type and identifier
-- Examples:
--	EXEC dbo.uspNoteRetrieve @EntityTypeCD = 'FAQ', @EntityID = '1' (Retrieve By EntityTypeCD, EntityID)
-- =============================================
CREATE PROCEDURE [dbo].[uspNoteRetrieve]
	@EntityTypeCD	VARCHAR(20)
	, @EntityID		VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1000 A.ID
		, A.EntityTypeCD, M.[Description] AS EntityTypeDescription, A.EntityID
		, A.Username, A.Note, A.[Timestamp]
	FROM dbo.tblNotes A
	LEFT OUTER JOIN dbo.tblMetadata M ON M.CodeID = 101 AND M.Code = A.EntityTypeCD
	WHERE A.EntityTypeCD = @EntityTypeCD AND A.EntityID = @EntityID
	ORDER BY A.[Timestamp] DESC;

END
GO