DROP PROCEDURE IF EXISTS [dbo].[uspListingAmenityRetrieveForEdit];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of all Amenities available in the system (selected by ListingID)
-- Examples:
--	EXEC dbo.uspListingAmenityRetrieveForEdit @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingAmenityRetrieveForEdit]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.AmenityID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, A.Active
		, CASE WHEN LA.CreatedDate IS NOT NULL THEN 1 ELSE 0 END AS Selected
	FROM dbo.tblMasterAmenities A
	LEFT OUTER JOIN dbo.tblListingAmenities LA ON LA.ListingID = @ListingID AND LA.AmenityID = A.AmenityID
	ORDER BY A.[Name] ASC;

END
GO