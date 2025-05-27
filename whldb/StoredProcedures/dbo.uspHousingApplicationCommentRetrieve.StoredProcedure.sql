DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationCommentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Dec-2024
-- Description:	Retrieve a list of housing application comments by Application ID
-- Examples:
--	EXEC dbo.uspHousingApplicationCommentRetrieve @ApplicationID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationCommentRetrieve]
	@ApplicationID	BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT AC.ApplicationID
		, AC.Comments, AC.InternalOnlyInd
		, AC.CreatedBy, AC.CreatedDate, AC.ModifiedBy, AC.ModifiedDate, A.Active
	FROM dbo.tblHousingApplicationComments AC
	JOIN dbo.tblHousingApplications A ON A.ApplicationID = AC.ApplicationID
	WHERE A.ApplicationID = @ApplicationID
	ORDER BY AC.CreatedDate DESC;

END
GO