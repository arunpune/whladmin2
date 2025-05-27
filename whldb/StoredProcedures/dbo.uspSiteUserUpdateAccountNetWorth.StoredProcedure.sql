DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserUpdateAccountNetWorth];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update site user net worth
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserUpdateAccountNetWorth @Username = 'USERNAME', @OwnRealEstateInd = 1, @RealEstateValueAmt = 12345.67
--												, @AssetValueAmt = 234.56, @IncomeValueAmt = 345.67
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserUpdateAccountNetWorth]
	@Username						VARCHAR(200)
	, @OwnRealEstateInd				BIT = 0
	, @RealEstateValueAmt			DECIMAL(17, 2) = NULL
	, @AssetValueAmt				DECIMAL(17, 2) = NULL
	, @IncomeValueAmt				DECIMAL(17, 2) = NULL
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteUsers
		SET
			OwnRealEstateInd		= @OwnRealEstateInd
			, RealEstateValueAmt	= @RealEstateValueAmt
			, AssetValueAmt			= @AssetValueAmt
			, IncomeValueAmt		= @IncomeValueAmt
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE Username = @Username AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Updated Site User Net Worth', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Site User Net Worth - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO