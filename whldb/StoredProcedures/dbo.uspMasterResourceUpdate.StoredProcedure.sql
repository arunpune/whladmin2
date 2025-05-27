DROP PROCEDURE IF EXISTS [dbo].[uspMasterResourceUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 16-sep-2024
-- Description:	Update an existing Resource
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterResourceUpdate @ResourceID = 1, @Title = 'TITLE', @Text = 'TEXT', @Active = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterResourceUpdate]
	@ResourceID			INT
	, @Title		VARCHAR(200)
	, @Text			VARCHAR(4000) = NULL
	, @Url			VARCHAR(500)
	, @DisplayOrder	INT = 0
	, @Active		BIT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterResources
		SET
			Title			= @Title
			, [Text]		= @Text
			, [Url]			= @Url
			, DisplayOrder	= @DisplayOrder
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE ResourceID = @ResourceID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('Resource', CONVERT(VARCHAR(20), @ResourceID), @ModifiedBy, 'UPDATE', 'Updated Resource', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Resource - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO