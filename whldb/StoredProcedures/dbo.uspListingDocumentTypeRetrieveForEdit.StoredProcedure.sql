DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentTypeRetrieveForEdit];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of all Document Types available in the system (selected by ListingID)
-- Examples:
--	EXEC dbo.uspListingDocumentTypeRetrieveForEdit @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentTypeRetrieveForEdit]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.DocumentTypeID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, A.Active
		, CASE WHEN LA.CreatedDate IS NOT NULL THEN 1 ELSE 0 END AS Selected
	FROM dbo.tblMasterDocumentTypes A
	LEFT OUTER JOIN dbo.tblListingDocumentTypes LA ON LA.ListingID = @ListingID AND LA.DocumentTypeID = A.DocumentTypeID
	ORDER BY A.[Name] ASC;

END
GO