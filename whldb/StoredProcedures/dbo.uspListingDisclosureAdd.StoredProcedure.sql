DROP PROCEDURE IF EXISTS [dbo].[uspListingDisclosureAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Add a new Listing Disclosure
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDisclosureAdd @ListingID = 1, @Text = 'TITLE', @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDisclosureAdd]
	@ListingID				INT
	, @Text					VARCHAR(1000)
	, @SortOrder			INT = 0
	, @CreatedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @DisclosureID INT;

		INSERT INTO dbo.tblListingDisclosures (ListingID, [Text], SortOrder
																, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ListingID, @Text, @SortOrder
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);
		SELECT @DisclosureID = SCOPE_IDENTITY();

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDSCL', CONVERT(VARCHAR(20), @DisclosureID), @CreatedBy, 'ADD', 'Added Listing Disclosure', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @CreatedBy, 'UPDATE', 'Added Listing Disclosure: ' + @Text, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Listing Disclosure - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO