DROP PROCEDURE IF EXISTS [dbo].[uspListingDisclosureUpdate];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Update an existing Listing Disclosure
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingDisclosureUpdate @DisclosureID = 1, @Text = 'TITLE', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingDisclosureUpdate]
	@DisclosureID			INT
	, @Text					VARCHAR(1000)
	, @SortOrder			INT = 0
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT;
		SET @ListingID = ISNULL((SELECT ListingID FROM dbo.tblListingDisclosures WHERE DisclosureID = @DisclosureID), 0);

		UPDATE dbo.tblListingDisclosures
		SET
			[Text]			= @Text
			, SortOrder		= @SortOrder
			, ModifiedBy	= @ModifiedBy
			, ModifiedDate	= GETDATE()
		WHERE DisclosureID = @DisclosureID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTINGDECL', CONVERT(VARCHAR(20), @DisclosureID), @ModifiedBy, 'UPDATE', 'Updated Listing Disclosure', GETDATE())
				, ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Disclosure: ' + @Text, GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Disclosure - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO