DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Update a Listing Unit
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUnitUpdate @UnitID = 1, @UnitTypeCD = 'STUDIO', @BedroomCnt = 0, @BathroomCnt = 0, @SquareFootage = 850, @AreaMedianIncomePct = 50
--									, @MonthlyRentAmt  = 979, @AssetLimitAmt = 0, @MonthlyMaintenanceAmt = 0, @EstimatedPriceAmt = 0
--									, @UnitsAvailableCnt = 20
--									, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUnitUpdate]
	@UnitID					INT
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
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT;
		SET @ListingID = ISNULL((SELECT ListingID FROM dbo.tblListingUnits WHERE UnitID = @UnitID), 0);

		UPDATE dbo.tblListingUnits
		SET
			UnitTypeCD			= @UnitTypeCD
			, BedroomCnt		= @BedroomCnt
			, BathroomCnt		= @BathroomCnt
			, BathroomCntPart	= @BathroomCntPart
			, SquareFootage		= @SquareFootage
			, AreaMedianIncomePct = @AreaMedianIncomePct
			, MonthlyRentAmt	= @MonthlyRentAmt
			, AssetLimitAmt		= @AssetLimitAmt
			, EstimatedPriceAmt	= @EstimatedPriceAmt
			, SubsidyAmt		= @SubsidyAmt
			, MonthlyTaxesAmt	= @MonthlyTaxesAmt
			, MonthlyMaintenanceAmt = @MonthlyMaintenanceAmt
			, MonthlyInsuranceAmt = @MonthlyInsuranceAmt
			, UnitsAvailableCnt	= @UnitsAvailableCnt
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE UnitID = @UnitID;

		DELETE FROM dbo.tblListingUnitHouseholds WHERE UnitID = @UnitID;

		INSERT INTO dbo.tblListingUnitHouseholds (UnitID, HouseholdSize, MinHouseholdIncomeAmt, MaxHouseholdIncomeAmt
													, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			SELECT @UnitID, T.HouseholdSize, T.MinHouseholdIncomeAmt, T.MaxHouseholdIncomeAmt
				, @ModifiedBy, GETDATE(), @ModifiedBy, GETDATE(), 1
			FROM @Households T;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('UNIT', CONVERT(VARCHAR(20), @UnitID), @ModifiedBy, 'UPDATE', 'Updated Listing Unit', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Unit: ' + @UnitTypeCD, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @UnitID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Unit - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO