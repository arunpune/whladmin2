DROP PROCEDURE IF EXISTS [dbo].[uspListingImageRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Images by ListingID, or a single one by ImageID
-- Examples:
--	EXEC dbo.uspListingImageRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspListingImageRetrieve @ImageID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingImageRetrieve]
	@ListingID		INT = 0
	, @ImageID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@ImageID, 0) > 0
	BEGIN
		SELECT ImageID, ListingID, Title, ThumbnailContents, Contents, MimeType
			, IsPrimary, DisplayOnListingsPageInd
			, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
		FROM dbo.tblListingImages
		WHERE ImageID = @ImageID;
		RETURN;
	END

	SELECT ImageID, ListingID, Title, ThumbnailContents, Contents, MimeType
		, IsPrimary, DisplayOnListingsPageInd
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblListingImages
	WHERE ListingID = @ListingID
	ORDER BY IsPrimary DESC, [Title] ASC;

END
GO