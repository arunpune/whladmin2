DROP PROCEDURE IF EXISTS [dbo].[uspMasterAmenityUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Update an existing Amenity
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspMasterAmenityUpdate @Name = 'NAME', @Description = 'DESCRIPTION', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAmenityUpdate]
	@AmenityID		INT
	, @Name			VARCHAR(200)
	, @Description	VARCHAR(4000)
	, @Active		BIT
	, @ModifiedBy	VARCHAR(200)
	, @ErrorMessage	VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblMasterAmenities
		SET
			[Name]			= @Name
			, [Description] = @Description
			, Active		= @Active
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE AmenityID = @AmenityID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('AMENITY', CONVERT(VARCHAR(20), @AmenityID), @ModifiedBy, 'UPDATE', 'Updated Amenity', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Amenity - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO