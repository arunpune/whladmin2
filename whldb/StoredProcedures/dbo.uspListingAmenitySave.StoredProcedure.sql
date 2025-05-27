DROP PROCEDURE IF EXISTS [dbo].[uspListingAmenitySave];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Save Listing Amenities
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingAmenitySave @ListingID = 1, @AmenityIDs = '1,2,3', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingAmenitySave]
	@ListingID				INT
	, @AmenityIDs			VARCHAR(MAX)
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF ISNULL(RTRIM(@AmenityIDs), '') = ''
		BEGIN

			DELETE FROM dbo.tblListingAmenities WHERE ListingID = @ListingID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Amenities.', GETDATE());

		END
		ELSE
		BEGIN

			DECLARE @Amenities TABLE (
				ListingID		INT
				, AmenityID		INT
				, ChangeType	CHAR(1) DEFAULT('-')
			);

			INSERT INTO @Amenities (ListingID, AmenityID)
				SELECT @ListingID, [Value] FROM STRING_SPLIT(@AmenityIDs, ',');

			-- Add new entries
			INSERT INTO dbo.tblListingAmenities (ListingID, AmenityID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
				SELECT A.ListingID, A.AmenityID, @ModifiedBy, GETDATE(), @ModifiedBy, GETDATE(), 1
				FROM @Amenities A
				WHERE A.AmenityID NOT IN (
					SELECT AmenityID FROM dbo.tblListingAmenities WHERE ListingID = @ListingID
				);

			-- Delete old entries
			DELETE FROM dbo.tblListingAmenities
			WHERE ListingID = @ListingID AND AmenityID NOT IN (
				SELECT AmenityID FROM @Amenities
			);

			DECLARE @Note VARCHAR(MAX);
			SET @Note = ISNULL((SELECT STRING_AGG(A.[Name], ', ') AS Amenities
								FROM dbo.tblMasterAmenities A
								JOIN dbo.tblListingAmenities LA ON LA.ListingID = @ListingID AND LA.AmenityID = A.AmenityID), '');
			IF LEN(@Note) = 0 SET @Note = 'Updated Listing Amenities.'
			ELSE SET @Note = 'Updated Listing Amenities - ' + @Note + '.';

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Listing Amenities - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO