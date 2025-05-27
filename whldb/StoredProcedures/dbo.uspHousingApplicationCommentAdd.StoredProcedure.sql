DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationCommentAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Dec-2024
-- Description:	Add a new comment to an application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationCommentAdd @Username = 'USERNAME', @ApplicationID = 1
--												, @Comments = 'COMMENT', @InternalOnlyInd = 0
--												, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationCommentAdd]
	@ApplicationID			BIGINT
	, @Comments				VARCHAR(4000)
	, @InternalOnlyInd		BIT
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @CommentID BIGINT;

		INSERT INTO dbo.tblHousingApplicationComments (
				ApplicationID, Comments, InternalOnlyInd
				, CreatedBy, CreatedDate
		) VALUES (
			@ApplicationID, @Comments, @InternalOnlyInd
			, @CreatedBy, GETDATE()
		);
		SELECT @CommentID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @CreatedBy, 'UPDATE', 'Added Comments - ' + @Comments, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Housing Application Comments - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO