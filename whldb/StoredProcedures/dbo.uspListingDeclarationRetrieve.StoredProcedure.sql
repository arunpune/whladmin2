DROP PROCEDURE IF EXISTS [dbo].[uspListingDeclarationRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Declarations by ListingID, or a single one by DeclarationID
-- Examples:
--	EXEC dbo.uspListingDeclarationRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspListingDeclarationRetrieve @DeclarationID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDeclarationRetrieve]
	@ListingID			INT = 0
	, @DeclarationID	INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@DeclarationID, 0) > 0
	BEGIN
		SELECT DeclarationID, ListingID, '' AS Code, [Text], SortOrder, UserAdded, (1210000 + SortOrder) AS DisplayOrder
			, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
		FROM dbo.tblListingDeclarations
		WHERE DeclarationID = @DeclarationID;
		RETURN;
	END

	SELECT MetadataID AS DeclarationID, @ListingID AS ListingID, Code, [Description] AS [Text], 0 AS SortOrder, CONVERT(BIT, 0) AS UserAdded, MetadataID AS DisplayOrder
		, 'SYSTEM' AS CreatedBy, GETDATE() AS CreatedDate, 'SYSTEM' AS ModifiedBy, GETDATE() AS ModifiedDate, CONVERT(BIT, 1) AS Active
	FROM dbo.tblMetadata
	WHERE CodeID = 121

	UNION

	SELECT DeclarationID, ListingID, '' AS Code, [Text], SortOrder, UserAdded, (1210000 + SortOrder) AS DisplayOrder
		, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblListingDeclarations
	WHERE ListingID = @ListingID

	ORDER BY DisplayOrder ASC;

END
GO