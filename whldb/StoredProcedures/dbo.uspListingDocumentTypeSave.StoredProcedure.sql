DROP PROCEDURE IF EXISTS [dbo].[uspListingDocumentTypeSave];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Save Listing Document Types
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDocumentTypeSave @ListingID = 1, @DocumentTypeIDs = '1,2,3', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDocumentTypeSave]
	@ListingID				INT
	, @DocumentTypeIDs		VARCHAR(MAX)
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		IF ISNULL(RTRIM(@DocumentTypeIDs), '') = ''
		BEGIN

			DELETE FROM dbo.tblListingDocumentTypes WHERE ListingID = @ListingID;

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Deleted Listing Document Types.', GETDATE());

		END
		ELSE
		BEGIN

			DECLARE @DocumentTypes TABLE (
				ListingID		INT
				, DocumentTypeID		INT
				, ChangeType	CHAR(1) DEFAULT('-')
			);

			INSERT INTO @DocumentTypes (ListingID, DocumentTypeID)
				SELECT @ListingID, [Value] FROM STRING_SPLIT(@DocumentTypeIDs, ',');

			-- Add new entries
			INSERT INTO dbo.tblListingDocumentTypes (ListingID, DocumentTypeID, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
				SELECT A.ListingID, A.DocumentTypeID, @ModifiedBy, GETDATE(), @ModifiedBy, GETDATE(), 1
				FROM @DocumentTypes A
				WHERE A.DocumentTypeID NOT IN (
					SELECT DocumentTypeID FROM dbo.tblListingDocumentTypes WHERE ListingID = @ListingID
				);

			-- Delete old entries
			DELETE FROM dbo.tblListingDocumentTypes
			WHERE ListingID = @ListingID AND DocumentTypeID NOT IN (
				SELECT DocumentTypeID FROM @DocumentTypes
			);

			DECLARE @Note VARCHAR(MAX);
			SET @Note = ISNULL((SELECT STRING_AGG(A.[Name], ', ') AS DocumentTypes
								FROM dbo.tblMasterDocumentTypes A
								JOIN dbo.tblListingDocumentTypes LA ON LA.ListingID = @ListingID AND LA.DocumentTypeID = A.DocumentTypeID), '');
			IF LEN(@Note) = 0 SET @Note = 'Updated Listing Document Types.'
			ELSE SET @Note = 'Updated Listing Document Types - ' + @Note + '.';

			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', @Note, GETDATE());

		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Listing Document Types - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO