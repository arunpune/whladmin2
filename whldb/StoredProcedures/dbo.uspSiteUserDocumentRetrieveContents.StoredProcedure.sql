DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserDocumentRetrieveContents];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Retrieve document contents
-- Examples:
--	EXEC dbo.uspSiteUserDocumentRetrieveContents @Username = 'USERNAME', @DocID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserDocumentRetrieveContents]
	@Username	VARCHAR(200)
	, @DocID	BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DC.DocContents
	FROM dbo.tblSiteUserDocumentContents DC
	JOIN dbo.tblSiteUserDocuments D ON D.DocID = DC.DocID
	WHERE D.Username = @Username
		AND D.DocID = @DocID;

END
GO