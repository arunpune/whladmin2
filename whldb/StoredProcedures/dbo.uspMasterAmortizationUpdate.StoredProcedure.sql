DROP PROCEDURE IF EXISTS [dbo].[uspMasterAmortizationUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Mar-2025
-- Description:	Update an existing Amortization
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterAmortizationUpdate @Rate = 2.0, @RateInterestOnly = 0.16667, @Rate10Year = 9.20135
--										, @Rate15Year = 6.43509, @Rate20Year = 5.05883, @Rate25Year = 4.23854
--										, @Rate30Year = 3.69619, @Rate40Year = 3.02826
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAmortizationUpdate]
	@Rate				DECIMAL(7, 5)
	, @RateInterestOnly		DECIMAL(7, 5) = 0
	, @Rate10Year		DECIMAL(7, 5) = 0
	, @Rate15Year		DECIMAL(7, 5) = 0
	, @Rate20Year		DECIMAL(7, 5) = 0
	, @Rate25Year		DECIMAL(7, 5) = 0
	, @Rate30Year		DECIMAL(7, 5) = 0
	, @Rate40Year		DECIMAL(7, 5) = 0
	, @ModifiedBy		VARCHAR(200)
	, @ErrorMessage		VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterAmortizations
		SET
			RateInterestOnly = @RateInterestOnly
			, Rate10Year	= @Rate10Year
			, Rate15Year	= @Rate15Year
			, Rate20Year	= @Rate20Year
			, Rate25Year	= @Rate25Year
			, Rate30Year	= @Rate30Year
			, Rate40Year	= @Rate40Year
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE [Rate] = @Rate;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('AMORTIZATION', CONVERT(VARCHAR(20), @Rate), @ModifiedBy, 'UPDATE', 'Updated Amortization', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Amortization - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO