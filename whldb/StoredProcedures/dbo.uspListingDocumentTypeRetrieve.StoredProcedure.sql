DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentTypeRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of Document Types by ListingID
-- Examples:
--	EXEC dbo.uspListingDocumentTypeRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentTypeRetrieve]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.DocumentTypeID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, LA.Active
	FROM dbo.tblMasterDocumentTypes A
	JOIN dbo.tblListingDocumentTypes LA ON LA.ListingID = @ListingID AND LA.DocumentTypeID = A.DocumentTypeID
	ORDER BY A.[Name] ASC;

END
GO