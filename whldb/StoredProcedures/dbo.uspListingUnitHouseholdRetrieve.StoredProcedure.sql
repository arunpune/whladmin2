DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitHouseholdRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Unit Households by ListingID, or a single one by UnitID
-- Examples:
--	EXEC dbo.uspListingUnitHouseholdRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspListingUnitHouseholdRetrieve @UnitID = 1 (Retrieve All by Unit)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUnitHouseholdRetrieve]
	@ListingID		INT = 0
	, @UnitID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@UnitID, 0) > 0
	BEGIN
		SELECT H.UnitHouseholdID, H.UnitID, H.HouseholdSize, H.MinHouseholdIncomeAmt, H.MaxHouseholdIncomeAmt
			, H.CreatedBy, H.CreatedDate, H.ModifiedBy, H.ModifiedDate, H.Active
		FROM dbo.tblListingUnitHouseholds H
		WHERE H.UnitID = @UnitID
		ORDER BY H.HouseholdSize ASC;
		RETURN;
	END

	SELECT H.UnitHouseholdID, H.UnitID, H.HouseholdSize, H.MinHouseholdIncomeAmt, H.MaxHouseholdIncomeAmt
		, H.CreatedBy, H.CreatedDate, H.ModifiedBy, H.ModifiedDate, H.Active
	FROM dbo.tblListingUnitHouseholds H
	JOIN dbo.tblListingUnits U ON U.UnitID = H.UnitID
	WHERE U.ListingID = @ListingID
	ORDER BY U.AreaMedianIncomePct ASC, U.BedroomCnt ASC, U.BathroomCnt ASC, U.BathroomCntPart ASC, H.HouseholdSize ASC

END
GO