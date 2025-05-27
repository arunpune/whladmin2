DROP PROCEDURE IF EXISTS [dbo].[uspMasterVideoAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Add a new Video
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterVideoAdd @Title = 'TITLE', @Url = 'URL', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterVideoAdd]
	@Title					VARCHAR(200)
	, @Text					VARCHAR(1000) = NULL
	, @Url					VARCHAR(500)
	, @DisplayOrder			INT = 0
	, @DisplayOnHomePageInd	BIT = 0
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @VideoID INT;

		INSERT INTO dbo.tblMasterVideos (Title, [Text], [Url], DisplayOrder, DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@Title, @Text, @Url, @DisplayOrder, @DisplayOnHomePageInd, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @VideoID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('VIDEO', CONVERT(VARCHAR(20), @VideoID), @CreatedBy, 'ADD', 'Added Video', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Video - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO