DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationDocumentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Retrieve a list of documents for a given application, or a single document for a given site user
-- Examples:
--	EXEC dbo.uspHousingApplicationDocumentRetrieve @Username = 'USERNAME', @ApplicationID = 1 (Retrieve All)
--	EXEC dbo.uspHousingApplicationDocumentRetrieve @Username = 'USERNAME', @DocID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationDocumentRetrieve]
	@Username			VARCHAR(200)
	, @ApplicationID	BIGINT = 0
	, @DocID			BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SET @ApplicationID	= ISNULL(@ApplicationID, 0);
	SET @DocID			= ISNULL(@DocID, 0);

	SELECT D.DocID, D.ApplicationID, D.Username, D.DocTypeID, MDT.[Name] AS DocTypeDescription
		, D.DocName, D.FileName, D.MimeType
		, D.CreatedBy, D.CreatedDate, D.ModifiedBy, D.ModifiedDate
		, D.Active
	FROM dbo.tblHousingApplicationDocuments D
	LEFT OUTER JOIN dbo.tblMasterDocumentTypes MDT ON MDT.DocumentTypeID = D.DocTypeID
	WHERE D.Username = @Username
		AND (@DocID = 0 OR D.DocID = @DocID)
		AND (@ApplicationID = 0 OR D.ApplicationID = @ApplicationID)
		AND D.Active = 1
	ORDER BY D.DocName ASC

END
GO