DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingDeclarationRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 02-Sep-2024
-- Description:	Retrieve a list of Listing Declarations by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingDeclarationRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingDeclarationRetrieve]
	@ListingID			INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MetadataID AS DeclarationID, @ListingID AS ListingID, Code, [Description] AS [Text], 0 AS SortOrder, CONVERT(BIT, 0) AS UserAdded, MetadataID AS DisplayOrder
		, 'SYSTEM' AS CreatedBy, GETDATE() AS CreatedDate, 'SYSTEM' AS ModifiedBy, GETDATE() AS ModifiedDate, CONVERT(BIT, 1) AS Active
	FROM dbo.tblMetadata
	WHERE CodeID = 121 AND Active = 1

	UNION

	SELECT DeclarationID, ListingID, '' AS Code, [Text], SortOrder, UserAdded, (1210000 + SortOrder) AS DisplayOrder
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblSiteListingDeclarations
	WHERE ListingID = @ListingID AND Active = 1

	ORDER BY DisplayOrder ASC;

END
GO