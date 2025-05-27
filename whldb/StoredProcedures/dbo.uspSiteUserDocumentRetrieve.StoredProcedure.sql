DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserDocumentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Retrieve a list of documents for a given site user, or a single document for a given site user
-- Examples:
--	EXEC dbo.uspSiteUserDocumentRetrieve @Username = 'USERNAME' (Retrieve All)
--	EXEC dbo.uspSiteUserDocumentRetrieve @Username = 'USERNAME', @DocID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserDocumentRetrieve]
	@Username	VARCHAR(200)
	, @DocID	BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SET @DocID = ISNULL(@DocID, 0);

	SELECT D.DocID, D.Username, D.DocTypeID, MDT.[Name] AS DocTypeDescription
		, D.DocName, D.FileName, D.MimeType
		, D.CreatedBy, D.CreatedDate, D.ModifiedBy, D.ModifiedDate
		, D.Active
	FROM dbo.tblSiteUserDocuments D
	LEFT OUTER JOIN dbo.tblMasterDocumentTypes MDT ON MDT.DocumentTypeID = D.DocTypeID
	WHERE D.Username = @Username
		AND (@DocID = 0 OR D.DocID = @DocID)
		AND D.Active = 1
	ORDER BY D.DocName ASC

END
GO