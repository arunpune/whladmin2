DROP PROCEDURE IF EXISTS [dbo].[uspListingUpdateAddress];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Jan-2025
-- Description:	Update address for an existing Listing
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingUpdateAddress @ListingID = 1
--							, @StreetLine1 = 'LINE 1', @City = 'CITY', @StateCD = 'ST', @ZipCode = 'ZIPCD', @County = 'COUNTY'
--							, @StatusCD = 'DRAFT'
--							, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingUpdateAddress]
	@ListingID					INT
	, @StreetLine1				VARCHAR(250)
	, @StreetLine2				VARCHAR(250) = NULL
	, @StreetLine3				VARCHAR(250) = NULL
	, @City						VARCHAR(100)
	, @StateCD					VARCHAR(2)
	, @ZipCode					VARCHAR(9)
	, @County					VARCHAR(100)
	, @Municipality				VARCHAR(250) = NULL
	, @MunicipalityUrl			VARCHAR(500) = NULL
	, @SchoolDistrict			VARCHAR(250) = NULL
	, @SchoolDistrictUrl		VARCHAR(500) = NULL
	, @MapUrl					VARCHAR(500) = NULL
	, @EsriX					VARCHAR(20) = NULL
	, @EsriY					VARCHAR(20) = NULL
	, @ModifiedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblListings
		SET
			StreetLine1				= @StreetLine1
			, StreetLine2			= @StreetLine2
			, StreetLine3			= @StreetLine3
			, City					= @City
			, StateCD				= @StateCD
			, ZipCode				= @ZipCode
			, County				= @County
			, Municipality			= @Municipality
			, MunicipalityUrl		= @MunicipalityUrl
			, SchoolDistrict		= @SchoolDistrict
			, SchoolDistrictUrl		= @SchoolDistrictUrl
			, MapUrl				= @MapUrl
			, EsriX					= @EsriX
			, EsriY					= @EsriY
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE ListingID = @ListingID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @ModifiedBy, 'UPDATE', 'Updated Listing Address', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Listing Address - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO