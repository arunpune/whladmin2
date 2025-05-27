DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of Listing Units by ListingID, or a single one by UnitID
-- Examples:
--	EXEC dbo.uspListingUnitRetrieve @ListingID = 1 (Retrieve All by Listing)
--	EXEC dbo.uspListingUnitRetrieve @UnitID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUnitRetrieve]
	@ListingID		INT = 0
	, @UnitID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@UnitID, 0) > 0
	BEGIN
		SELECT U.UnitID, U.ListingID, U.UnitTypeCD, MLU.[Description] AS UnitTypeDescription
			, U.BedroomCnt, U.BathroomCnt, U.BathroomCntPart, U.SquareFootage, U.AreaMedianIncomePct
			, U.MonthlyRentAmt, U.AssetLimitAmt, U.EstimatedPriceAmt, U.SubsidyAmt
			, U.MonthlyTaxesAmt, U.MonthlyMaintenanceAmt, U.MonthlyInsuranceAmt
			, U.UnitsAvailableCnt
			, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate, U.Active
		FROM dbo.tblListingUnits U
		LEFT OUTER JOIN dbo.tblMetadata MLU ON MLU.CodeID = 108 AND MLU.Code = U.UnitTypeCD
		WHERE U.UnitID = @UnitID;
		RETURN;
	END

	SELECT U.UnitID, U.ListingID, U.UnitTypeCD, MLU.[Description] AS UnitTypeDescription
		, U.BedroomCnt, U.BathroomCnt, U.BathroomCntPart, U.SquareFootage, U.AreaMedianIncomePct
		, U.MonthlyRentAmt, U.AssetLimitAmt, U.EstimatedPriceAmt, U.SubsidyAmt
		, U.MonthlyTaxesAmt, U.MonthlyMaintenanceAmt, U.MonthlyInsuranceAmt
		, U.UnitsAvailableCnt
		, U.CreatedBy, U.CreatedDate, U.ModifiedBy, U.ModifiedDate, U.Active
	FROM dbo.tblListingUnits U
	LEFT OUTER JOIN dbo.tblMetadata MLU ON MLU.CodeID = 108 AND MLU.Code = U.UnitTypeCD
	WHERE U.ListingID = @ListingID
	ORDER BY U.AreaMedianIncomePct ASC, U.BedroomCnt ASC, U.BathroomCnt ASC, U.BathroomCntPart ASC;

END
GO