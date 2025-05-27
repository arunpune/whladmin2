DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserDocumentAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 25-Oct-2024
-- Description:	Add a new site user document
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteUserDocumentAdd @Username = 'USERNAME', @DocTypeID = 1
--						, @DocName = 'DOCNAME', @FileName = 'FILENAME'
--						, @MimeType = 'application/octet-stream'
--						, @DocContents = 'WEBSITE'
--						, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserDocumentAdd]
	@Username					VARCHAR(200)
	, @DocTypeID				INT
	, @DocName					VARCHAR(250)
	, @FileName					VARCHAR(250)
	, @MimeType					VARCHAR(50)
	, @DocContents				VARBINARY(MAX)
	, @CreatedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @DocID BIGINT;

		INSERT INTO dbo.tblSiteUserDocuments (Username, DocTypeID, DocName
										, [FileName], MimeType
										, CreatedBy, CreatedDate)
			VALUES (@Username, @DocTypeID, @DocName
						, @FileName, @MimeType
						, @Username, GETDATE());
		SELECT @DocID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblSiteUserDocumentContents (DocID, DocContents, CreatedBy, CreatedDate)
			VALUES (@DocID, @DocContents, @Username, GETDATE());

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @Username, 'UPDATE', 'Added Document ' + @FileName, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Site User Document - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO