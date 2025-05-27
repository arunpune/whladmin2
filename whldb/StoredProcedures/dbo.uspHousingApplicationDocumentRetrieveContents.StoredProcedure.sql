DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationDocumentRetrieveContents];
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
--	EXEC dbo.uspHousingApplicationDocumentRetrieveContents @Username = 'USERNAME', @ApplicationID = 1, @DocID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationDocumentRetrieveContents]
	@Username			VARCHAR(200)
	, @ApplicationID	BIGINT
	, @DocID			BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT DC.DocContents
	FROM dbo.tblHousingApplicationDocumentContents DC
	JOIN dbo.tblHousingApplicationDocuments D ON D.DocID = DC.DocID
	WHERE D.Username = @Username
		AND D.ApplicationID = @ApplicationID
		AND D.DocID = @DocID;

END
GO