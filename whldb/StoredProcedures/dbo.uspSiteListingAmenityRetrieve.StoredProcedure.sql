DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingAmenityRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Amenities by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingAmenityRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingAmenityRetrieve]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.AmenityID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, LA.Active
	FROM dbo.tblMasterAmenities A
	JOIN dbo.tblSiteListingAmenities LA ON LA.ListingID = @ListingID AND LA.AmenityID = A.AmenityID
	ORDER BY A.[Name] ASC;

END
GO