DROP PROCEDURE IF EXISTS [dbo].[uspAuditRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of audit entries by Entity type and identifier
-- Examples:
--	EXEC dbo.uspAuditRetrieve @EntityTypeCD = 'FAQ', @EntityID = '1' (Retrieve By EntityTypeCD, EntityID)
-- =============================================
CREATE PROCEDURE [dbo].[uspAuditRetrieve]
	@EntityTypeCD	VARCHAR(20)
	, @EntityID		VARCHAR(20)
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1000 A.ID
		, A.EntityTypeCD, M.[Description] AS EntityTypeDescription
		, A.EntityID
		, A.Username
		, A.ActionCD, CASE A.ActionCD WHEN 'ADD' THEN 'Add' WHEN 'UPDATE' THEN 'Update' WHEN 'DELETE' THEN 'Delete' ELSE '' END AS ActionDescription
		, A.Note, A.[Timestamp]
	FROM dbo.tblAudit A
	LEFT OUTER JOIN dbo.tblMetadata M ON M.CodeID = 101 AND M.Code = A.EntityTypeCD
	WHERE A.EntityTypeCD = @EntityTypeCD AND A.EntityID = @EntityID
	ORDER BY A.[Timestamp] DESC;

END
GO