DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Jan-2025
-- Description:	Retrieve a list of Listing Documents by ListingID, or a single one by DocumentID
-- Examples:
--	EXEC dbo.uspListingDocumentRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspListingDocumentRetrieve @DocumentID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentRetrieve]
	@ListingID		INT = 0
	, @DocumentID	INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@DocumentID, 0) > 0
	BEGIN
		SELECT DocumentID, ListingID, Title, [FileName], Contents, MimeType
			, DisplayOnListingsPageInd
			, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
		FROM dbo.tblListingDocuments
		WHERE DocumentID = @DocumentID;
		RETURN;
	END

	SELECT DocumentID, ListingID, Title, [FileName], Contents, MimeType
		, DisplayOnListingsPageInd
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblListingDocuments
	WHERE ListingID = @ListingID
	ORDER BY [Title] ASC;

END
GO