DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingDisclosureRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Disclosures by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingDisclosureRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingDisclosureRetrieve]
	@ListingID			INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @PetsAllowedInd BIT;
	SET @PetsAllowedInd = CONVERT(BIT, ISNULL((SELECT PetsAllowedInd FROM dbo.tblListings WHERE ListingID = @ListingID), 0));

	SELECT MetadataID AS DisclosureID, @ListingID AS ListingID, Code, [Description] AS [Text], 0 AS SortOrder, CONVERT(BIT, 0) AS UserAdded, MetadataID AS DisplayOrder
		, 'SYSTEM' AS CreatedBy, GETDATE() AS CreatedDate, 'SYSTEM' AS ModifiedBy, GETDATE() AS ModifiedDate, CONVERT(BIT, 1) AS Active
	FROM dbo.tblMetadata
	WHERE CodeID = 122 AND Code LIKE 'DISC0%' AND Active = 1

	UNION

	SELECT MetadataID AS DisclosureID, @ListingID AS ListingID, Code, [Description] AS [Text], 0 AS SortOrder, CONVERT(BIT, 0) AS UserAdded, MetadataID AS DisplayOrder
		, 'SYSTEM' AS CreatedBy, GETDATE() AS CreatedDate, 'SYSTEM' AS ModifiedBy, GETDATE() AS ModifiedDate, CONVERT(BIT, 1) AS Active
	FROM dbo.tblMetadata
	WHERE CodeID = 122 AND Code = CASE WHEN @PetsAllowedInd = 1 THEN 'DISCPETYES' ELSE 'DISCPETNO' END AND Active = 1

	UNION

	SELECT DisclosureID, ListingID, '' AS Code, [Text], SortOrder, UserAdded, (1220000 + SortOrder) AS DisplayOrder
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblSiteListingDisclosures
	WHERE ListingID = @ListingID AND Active = 1

	ORDER BY DisplayOrder ASC;

END
GO