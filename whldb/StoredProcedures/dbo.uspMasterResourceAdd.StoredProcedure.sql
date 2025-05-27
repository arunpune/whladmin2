DROP PROCEDURE IF EXISTS [dbo].[uspMasterResourceAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Add a new Resource
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterResourceAdd @Title = 'TITLE', @Text = 'TEXT', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterResourceAdd]
	@Title			VARCHAR(200)
	, @Text			VARCHAR(4000) = NULL
	, @Url			VARCHAR(500)
	, @DisplayOrder	INT = 0
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ResourceID INT;

		INSERT INTO dbo.tblMasterResources (Title, [Text], [Url], DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@Title, @Text, @Url, @DisplayOrder, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @ResourceID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('Resource', CONVERT(VARCHAR(20), @ResourceID), @CreatedBy, 'ADD', 'Added Resource', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Resource - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO