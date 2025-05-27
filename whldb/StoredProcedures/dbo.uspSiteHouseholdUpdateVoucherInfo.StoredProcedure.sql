DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdUpdateVoucherInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update voucher information for an existing household
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdUpdateVoucherInfo @HouseholdID = 1, @Username = 'USERNAME'
--												, @VoucherInd = 1, @VoucherCDs = 'CODE1,CODE2'
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdUpdateVoucherInfo]
	@HouseholdID					BIGINT
	, @Username						VARCHAR(200)
	, @VoucherInd					BIT
	, @VoucherCDs					VARCHAR(1000) = NULL
	, @VoucherOther					VARCHAR(1000) = NULL
	, @VoucherAdminName				VARCHAR(200) = NULL
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHouseholds
		SET
			VoucherInd				= @VoucherInd
			, VoucherCDs			= @VoucherCDs
			, VoucherOther			= @VoucherOther
			, VoucherAdminName		= @VoucherAdminName
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE HouseholdID = @HouseholdID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated Household Voucher Information', GETDATE())
				, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated Household Voucher Information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Household Voucher Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO