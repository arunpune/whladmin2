DROP PROCEDURE IF EXISTS [dbo].[uspMasterAMIAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 17-Oct-2024
-- Description:	Add a new AMI
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterAMIAdd @EffectiveDate = 20240401, @EffectiveDate = 2024, @IncomeAmt = 156200
--								, @HH1 = 60, @HH2 = 70, @HH3 = 80
--								, @HH4 = 100, @HH5 = 108, @HH6 = 116
--								, @HH7 = 124, @HH8 = 132, @HH9 = 168
--								, @HH10 = 192
--								, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAMIAdd]
	@EffectiveDate		INT
	, @EffectiveYear	INT
	, @IncomeAmt		INT
	, @HH1				INT = 0
	, @HH2				INT = 0
	, @HH3				INT = 0
	, @HH4				INT = 0
	, @HH5				INT = 0
	, @HH6				INT = 0
	, @HH7				INT = 0
	, @HH8				INT = 0
	, @HH9				INT = 0
	, @HH10				INT = 0
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		INSERT INTO dbo.tblMasterAMIs (EffectiveDate, EffectiveYear, IncomeAmt
										, [HH1], [HH2], [HH3], [HH4]
										, [HH5], [HH6], [HH7], [HH8]
										, [HH9], [HH10]
										, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@EffectiveDate, @EffectiveYear, @IncomeAmt
						, @HH1, @HH2, @HH3, @HH4
						, @HH5, @HH6, @HH7, @HH8
						, @HH9, @HH10
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);


		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('AMI', CONVERT(VARCHAR(20), @EffectiveDate), @CreatedBy, 'ADD', 'Added AMI', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add AMI - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO