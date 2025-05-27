DROP PROCEDURE IF EXISTS [dbo].[uspListingAdd];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Add a new Listing
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspListingAdd @ListingTypeCD = 'RENTAL', @ResaleInd = 0, @ListingAgeTypeCD = 'ALL', @Name = 'NAME', @Description = 'DESCRIPTION'
--							, @StreetLine1 = 'LINE 1', @City = 'CITY', @StateCD = 'ST', @ZipCode = 'ZIPCD', @County = 'COUNTY'
--							, @County = 'COUNTY'
--							, @StatusCD = 'DRAFT'
--							, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspListingAdd]
	@ListingTypeCD				VARCHAR(20)
	, @ResaleInd				BIT = 0
	, @ListingAgeTypeCD			VARCHAR(20) = 'ALL'
	, @Name						VARCHAR(200)
	, @Description				VARCHAR(4000) = NULL
	, @WebsiteUrl				VARCHAR(500) = NULL
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
	, @RentIncludesText			VARCHAR(1000) = NULL
	, @CompletedOrInitialOccupancyYear VARCHAR(4) = NULL
	, @TermOfAffordability		VARCHAR(100) = NULL
	, @EsriX					VARCHAR(20) = NULL
	, @EsriY					VARCHAR(20) = NULL
	, @StatusCD					VARCHAR(20)
	, @CreatedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		DECLARE @ListingID INT;
		SELECT @ListingID = NEXT VALUE FOR [dbo].[seqListingID];

		INSERT INTO dbo.tblListings (ListingID, ListingTypeCD, ResaleInd, ListingAgeTypeCD, [Name], [Description]
										, WebsiteUrl
										, StreetLine1, StreetLine2, StreetLine3, City, StateCD, ZipCode, County
										, Municipality, MunicipalityUrl, SchoolDistrict, SchoolDistrictUrl
										, MapUrl, EsriX, EsriY
										, RentIncludesText, CompletedOrInitialOccupancyYear, TermOfAffordability
										, StatusCD
										, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
			VALUES (@ListingID, @ListingTypeCD, @ResaleInd, @ListingAgeTypeCD, @Name, @Description
						, @WebsiteUrl
						, @StreetLine1, @StreetLine2, @StreetLine3, @City, @StateCD, @ZipCode, @County
						, @Municipality, @MunicipalityUrl, @SchoolDistrict, @SchoolDistrictUrl
						, @MapUrl, @EsriX, @EsriY
						, @RentIncludesText, @CompletedOrInitialOccupancyYear, @TermOfAffordability
						, @StatusCD
						, @CreatedBy, GETDATE(), @CreatedBy, GETDATE(), 1);

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('LISTING', CONVERT(VARCHAR(20), @ListingID), @CreatedBy, 'ADD', 'Added Listing', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ListingID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to add Listing - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO