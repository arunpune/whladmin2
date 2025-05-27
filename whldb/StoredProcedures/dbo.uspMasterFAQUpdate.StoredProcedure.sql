DROP PROCEDURE IF EXISTS [dbo].[uspMasterFAQUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Update an existing FAQ
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterFAQUpdate @FAQID = 1, @CategoryName = 'CATEGORY', @Title = 'TITLE', @Text = 'TEXT', @Active = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFAQUpdate]
	@FAQID			INT
	, @CategoryName	VARCHAR(100)
	, @Title		VARCHAR(200)
	, @Text			VARCHAR(4000)
	, @Url			VARCHAR(500) = NULL
	, @Url1			VARCHAR(500) = NULL
	, @Url2			VARCHAR(500) = NULL
	, @Url3			VARCHAR(500) = NULL
	, @Url4			VARCHAR(500) = NULL
	, @Url5			VARCHAR(500) = NULL
	, @Url6			VARCHAR(500) = NULL
	, @Url7			VARCHAR(500) = NULL
	, @Url8			VARCHAR(500) = NULL
	, @Url9			VARCHAR(500) = NULL
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

		UPDATE dbo.tblMasterFAQs
		SET
			CategoryName	= @CategoryName
			, Title			= @Title
			, [Text]		= @Text
			, [Url]			= @Url
			, Url1			= @Url1
			, Url2			= @Url2
			, Url3			= @Url3
			, Url4			= @Url4
			, Url5			= @Url5
			, Url6			= @Url6
			, Url7			= @Url7
			, Url8			= @Url8
			, Url9			= @Url9
			, DisplayOrder	= @DisplayOrder
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE FAQID = @FAQID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('FAQ', CONVERT(VARCHAR(20), @FAQID), @ModifiedBy, 'UPDATE', 'Updated FAQ', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update FAQ - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO