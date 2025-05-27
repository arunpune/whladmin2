DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingAccessibilityRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Accessibilities by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingAccessibilityRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingAccessibilityRetrieve]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.Code, A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, LA.Active
	FROM dbo.tblMetadata A
	JOIN dbo.tblSiteListingAccessibilities LA ON LA.ListingID = @ListingID AND LA.AccessibilityCD = A.Code
	WHERE A.CodeID = 119
	ORDER BY A.[Description] ASC;

END
GO