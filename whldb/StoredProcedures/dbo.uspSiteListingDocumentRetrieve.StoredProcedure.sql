DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingDocumentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Documents by ListingID, or a single one by DocumentID
-- Examples:
--	EXEC dbo.uspSiteListingDocumentRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspSiteListingDocumentRetrieve @DocumentID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingDocumentRetrieve]
	@ListingID			INT = 0
	, @DocumentID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@DocumentID, 0) > 0
	BEGIN
		SELECT DocumentID, ListingID, Title, [FileName], Contents, MimeType
			, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
		FROM dbo.tblSiteListingDocuments
		WHERE DocumentID = @DocumentID;
		RETURN;
	END

	SELECT DocumentID, ListingID, Title, [FileName], Contents, MimeType
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblSiteListingDocuments
	WHERE ListingID = @ListingID
	ORDER BY [Title] ASC;

END
GO