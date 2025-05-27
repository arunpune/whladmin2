DROP PROCEDURE IF EXISTS [dbo].[uspMasterFAQAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Add a new FAQ
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterFAQAdd @CategoryName = 'CATEGORY', @Title = 'TITLE', @Text = 'TEXT', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFAQAdd]
	@CategoryName	VARCHAR(100)
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
	, @CreatedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @FAQID INT;

		INSERT INTO dbo.tblMasterFAQs (CategoryName, Title, [Text]
										, [Url], Url1, Url2, Url3, Url4, Url5, Url6, Url7, Url8, Url9
										, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@CategoryName, @Title, @Text
					, @Url, @Url1, @Url2, @Url3, @Url4, @Url5, @Url6, @Url7, @Url8, @Url9
					, @DisplayOrder, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @FAQID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('FAQ', CONVERT(VARCHAR(20), @FAQID), @CreatedBy, 'ADD', 'Added FAQ', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add FAQ - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO