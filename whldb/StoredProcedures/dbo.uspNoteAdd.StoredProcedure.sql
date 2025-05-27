DROP PROCEDURE IF EXISTS [dbo].[uspNoteAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Add a new Amenity
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspNoteAdd @EntityTypeCD = 'LISTING', @EntityID = '1', @Note = 'A user note', @Username = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspNoteAdd]
	@EntityTypeCD	VARCHAR(20)
	, @EntityID		VARCHAR(20)
	, @Note			VARCHAR(1000)
	, @Username	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ID INT;

		INSERT INTO dbo.tblNotes (EntityTypeCD, EntityID, Note, Username, [Timestamp])
			VALUES (@EntityTypeCD, @EntityID, @Note, @Username, GETDATE());
		SELECT @ID = SCOPE_IDENTITY();

		-- INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
		-- 	VALUES ('AMENITY', CONVERT(VARCHAR(20), @ID), @Username, 'ADD', 'Added Amenity', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Note - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO