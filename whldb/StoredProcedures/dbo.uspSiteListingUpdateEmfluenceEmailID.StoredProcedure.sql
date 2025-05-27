DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingUpdateEmfluenceEmailID];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update site listing Emfluence Email ID
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteListingUpdateEmfluenceEmailID @ListingID = 1, @EmfluenceEmailID = 1
--													, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingUpdateEmfluenceEmailID]
	@ListingID				INT
	, @EmfluenceEmailID		BIGINT
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblSiteListings
		SET
			EmfluenceEmailID	= @EmfluenceEmailID
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE ListingID = @ListingID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Emfluence Email ID - ' + CONVERT(VARCHAR(20), @EmfluenceEmailID), GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Emfluence Email ID - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO