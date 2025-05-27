DROP PROCEDURE IF EXISTS [dbo].[uspMasterDocumentTypeRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Dec-2024
-- Description:	Retrieve a list of master document types, or a single one by DocumentTypeID
-- Examples:
--	EXEC dbo.uspMasterDocumentTypeRetrieve (Retrieve All)
--	EXEC dbo.uspMasterDocumentTypeRetrieve @DocumentTypeID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterDocumentTypeRetrieve]
	@DocumentTypeID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	WITH CountsCTE AS (
		SELECT DocumentTypeID, COUNT(1) AS UsageCount
		FROM dbo.tblListingDocumentTypes
		GROUP BY DocumentTypeID
	)
		SELECT A.DocumentTypeID, A.[Name], A.[Description]
			, ISNULL(C.UsageCount, 0) AS UsageCount
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterDocumentTypes A
		LEFT OUTER JOIN CountsCTE C ON C.DocumentTypeID = A.DocumentTypeID
		WHERE @DocumentTypeID = 0 OR A.DocumentTypeID = @DocumentTypeID
		ORDER BY A.[Name] ASC;

END
GO