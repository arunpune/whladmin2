DROP PROCEDURE IF EXISTS [dbo].[uspMasterAmenityDelete];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Delete an existing Amenity
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterAmenityDelete @AmenityID = 1, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAmenityDelete]
	@AmenityID		INT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterAmenities SET Active = 0 WHERE AmenityID = @AmenityID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('AMENITY', CONVERT(VARCHAR(20), @AmenityID), @ModifiedBy, 'DELETE', 'Deleted Amenity', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @AmenityID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to delete Amenity - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO