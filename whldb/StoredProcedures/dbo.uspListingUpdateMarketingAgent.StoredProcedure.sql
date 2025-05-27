DROP PROCEDURE IF EXISTS [dbo].[uspListingUpdateMarketingAgent];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Mar-2025
-- Description:	Update listing marketing agent
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUpdateMarketingAgent @ListingID = 1, @MarketingAgentInd = 1
--											, @MarketingAgentID = 1, @MarketingAgentApplicationLink = 'LINK'
--											, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUpdateMarketingAgent]
	@ListingID							INT
	, @MarketingAgentInd				BIT = 0
	, @MarketingAgentID					INT = NULL
	, @MarketingAgentApplicationLink	VARCHAR(500) = NULL
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListings
		SET
			MarketingAgentInd	= @MarketingAgentInd
			, MarketingAgentID	= NULLIF(@MarketingAgentID, 0)
			, MarketingAgentApplicationLink	= NULLIF(RTRIM(@MarketingAgentApplicationLink), '')
			, ModifiedBy		= @ModifiedBy
			, ModifiedDate		= GETDATE()
		WHERE ListingID = @ListingID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Marketing Agent Details for Listing', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Marketing Agent Details - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO