DROP PROCEDURE IF EXISTS [dbo].[uspListingAccessibilitySave];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Save Listing Accessibilities
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingAccessibilitySave @ListingID = 1, @AccessibilityCDs = '1,2,3', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingAccessibilitySave]
	@ListingID				INT
	, @AccessibilityCDs		VARCHAR(MAX)
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF ISNULL(RTRIM(@AccessibilityCDs), '') = ''
		BEGIN

			DELETE FROM dbo.tblListingAccessibilities WHERE ListingID = @ListingID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Accessibilities.', GETDATE());

		END
		ELSE
		BEGIN

			DECLARE @Accessibilities TABLE (
				ListingID			INT
				, AccessibilityCD	VARCHAR(20)
				, ChangeType		CHAR(1) DEFAULT('-')
			);

			INSERT INTO @Accessibilities (ListingID, AccessibilityCD)
				SELECT @ListingID, [Value] FROM STRING_SPLIT(@AccessibilityCDs, ',');

			-- Add new entries
			INSERT INTO dbo.tblListingAccessibilities (ListingID, AccessibilityCD, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
				SELECT A.ListingID, A.AccessibilityCD, @ModifiedBy, GETDATE(), @ModifiedBy, GETDATE(), 1
				FROM @Accessibilities A
				WHERE A.AccessibilityCD NOT IN (
					SELECT AccessibilityCD FROM dbo.tblListingAccessibilities WHERE ListingID = @ListingID
				);

			-- Delete old entries
			DELETE FROM dbo.tblListingAccessibilities
			WHERE ListingID = @ListingID AND AccessibilityCD NOT IN (
				SELECT AccessibilityCD FROM @Accessibilities
			);

			DECLARE @Note VARCHAR(MAX);
			SET @Note = ISNULL((SELECT STRING_AGG(A.[Description], ', ') AS Accessibilities
								FROM dbo.tblMetadata A
								JOIN dbo.tblListingAccessibilities LA ON LA.ListingID = @ListingID AND LA.AccessibilityCD = A.Code
								WHERE A.CodeID = 119), '');
			IF LEN(@Note) = 0 SET @Note = 'Updated Listing Accessibilities.'
			ELSE SET @Note = 'Updated Listing Accessibilities - ' + @Note + '.';

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Listing Accessibilities - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO