DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationCommentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 16-Feb-2025
-- Description:	Retrieve a list of housing application comments by Application ID
-- Examples:
--	EXEC dbo.uspSiteHousingApplicationCommentRetrieve @Username = 'USERNAME', @ApplicationID = 1 (Retrieve All)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationCommentRetrieve]
	@Username			VARCHAR(200)
	, @ApplicationID	BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT AC.ApplicationID
		, AC.Comments, AC.InternalOnlyInd
		, AC.CreatedBy, AC.CreatedDate, AC.ModifiedBy, AC.ModifiedDate, A.Active
	FROM dbo.tblHousingApplicationComments AC
	JOIN dbo.tblHousingApplications A ON A.ApplicationID = AC.ApplicationID
	WHERE A.ApplicationID = @ApplicationID AND A.Username = @Username AND AC.InternalOnlyInd = 0
	ORDER BY AC.CreatedDate DESC;

END
GO