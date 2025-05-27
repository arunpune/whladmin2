DROP PROCEDURE IF EXISTS [dbo].[uspMasterVideoUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Update an existing Video
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterVideoUpdate @VideoID = 1, @Title = 'TITLE', @Url = 'URL', @Active = 1, @ModifiedBy = 'USERNAME'
--									, @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterVideoUpdate]
	@VideoID				INT
	, @Title				VARCHAR(200)
	, @Text					VARCHAR(1000) = NULL
	, @Url					VARCHAR(500)
	, @DisplayOrder			INT = 0
	, @DisplayOnHomePageInd	BIT = 0
	, @Active				BIT
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF @DisplayOnHomePageInd = 1
			UPDATE dbo.tblMasterVideos SET DisplayOnHomePageInd = 0 WHERE VideoID <> @VideoID;

		UPDATE dbo.tblMasterVideos
		SET
			Title					= @Title
			, [Text]				= @Text
			, [Url]					= @Url
			, DisplayOrder			= @DisplayOrder
			, DisplayOnHomePageInd	= @DisplayOnHomePageInd
			, Active				= @Active
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE VideoID = @VideoID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('VIDEO', CONVERT(VARCHAR(20), @VideoID), @ModifiedBy, 'UPDATE', 'Updated Video', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Video - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO