DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Delete a listing unit
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUnitDelete @UnitID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUnitDelete]
	@UnitID			INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT, @UnitTypeCD VARCHAR(20);
		SET @ListingID = ISNULL((SELECT ListingID FROM dbo.tblListingUnits WHERE UnitID = @UnitID), 0);
		SET @UnitTypeCD = ISNULL((SELECT UnitTypeCD FROM dbo.tblListingUnits WHERE UnitID = @UnitID), '');

		UPDATE dbo.tblListingUnits
		SET
			Active			= 0
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE UnitID = @UnitID;

		UPDATE dbo.tblListingUnitHouseholds
		SET
			Active			= 0
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE UnitID = @UnitID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('UNIT', CONVERT(VARCHAR(20), @UnitID), @ModifiedBy, 'DELETE', 'Deleted Listing Unit', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Unit: ' + @UnitTypeCD, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Listing Unit - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO