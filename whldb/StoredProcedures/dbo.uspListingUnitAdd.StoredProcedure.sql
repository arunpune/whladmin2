DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Add a new Listing Unit
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUnitAdd @ListingID = 1, @UnitTypeCD = 'STUDIO', @BedroomCnt = 0, @BathroomCnt = 0, @SquareFootage = 850, @AreaMedianIncomePct = 50
--												, @MonthlyRentAmt  = 979, @AssetLimitAmt = 0, @MonthlyMaintenanceAmt = 0, @EstimatedPriceAmt = 0
--												, @UnitsAvailableCnt = 20
--												, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUnitAdd]
	@ListingID				INT
	, @UnitTypeCD			VARCHAR(20)
	, @BedroomCnt			INT = 0
	, @BathroomCnt			INT = 0
	, @BathroomCntPart		INT = 0
	, @SquareFootage		INT
	, @AreaMedianIncomePct	INT
	, @MonthlyRentAmt		DECIMAL(7, 2)
	, @AssetLimitAmt		DECIMAL(12, 2)
	, @EstimatedPriceAmt	DECIMAL(12, 2)
	, @SubsidyAmt			DECIMAL(12, 2)
	, @MonthlyTaxesAmt		DECIMAL(7, 2)
	, @MonthlyMaintenanceAmt DECIMAL(7, 2)
	, @MonthlyInsuranceAmt	DECIMAL(7, 2)
	, @UnitsAvailableCnt	INT
	, @Households			udtListingUnitHousehold READONLY
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @UnitID INT;

		INSERT INTO dbo.tblListingUnits (ListingID, UnitTypeCD, BedroomCnt, BathroomCnt, BathroomCntPart, SquareFootage, AreaMedianIncomePct
											, MonthlyRentAmt, AssetLimitAmt, EstimatedPriceAmt, SubsidyAmt
											, MonthlyTaxesAmt, MonthlyMaintenanceAmt, MonthlyInsuranceAmt
											, UnitsAvailableCnt
											, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ListingID, @UnitTypeCD, @BedroomCnt, @BathroomCnt, @BathroomCntPart, @SquareFootage, @AreaMedianIncomePct
						, @MonthlyRentAmt, @AssetLimitAmt, @EstimatedPriceAmt, @SubsidyAmt
						, @MonthlyTaxesAmt, @MonthlyMaintenanceAmt, @MonthlyInsuranceAmt
						, @UnitsAvailableCnt
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @UnitID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblListingUnitHouseholds (UnitID, HouseholdSize, MinHouseholdIncomeAmt, MaxHouseholdIncomeAmt
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT @UnitID, T.HouseholdSize, T.MinHouseholdIncomeAmt, T.MaxHouseholdIncomeAmt
				, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1
			FROM @Households T;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('UNIT', CONVERT(VARCHAR(20), @UnitID), @CreatedBy, 'ADD', 'Added Listing Unit', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @CreatedBy, 'UPDATE', 'Added Listing Unit: ' + @UnitTypeCD, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Listing Unit - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO