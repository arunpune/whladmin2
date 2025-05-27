DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdUpdateAddressInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update address information for an existing household
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHouseholdUpdateAddressInfo @HouseholdID = 1, @Username = 'USERNAME'
--												, @AddressInd = 1, @MailingStreetLine1 = 'Line 1', @MailingCity = 'CITY'
--												, @MailingStateCD = 'ST', @MailingZipCode = '12345'
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdUpdateAddressInfo]
	@HouseholdID					BIGINT
	, @Username						VARCHAR(200)
	, @AddressInd					BIT
	, @PhysicalStreetLine1			VARCHAR(100) = NULL
	, @PhysicalStreetLine2			VARCHAR(100) = NULL
	, @PhysicalStreetLine3			VARCHAR(100) = NULL
	, @PhysicalCity					VARCHAR(100) = NULL
	, @PhysicalStateCD				VARCHAR(2) = NULL
	, @PhysicalZipCode				VARCHAR(5) = NULL
	, @PhysicalCounty				VARCHAR(100) = NULL
	, @DifferentMailingAddressInd	BIT = 0
	, @MailingStreetLine1			VARCHAR(100) = NULL
	, @MailingStreetLine2			VARCHAR(100) = NULL
	, @MailingStreetLine3			VARCHAR(100) = NULL
	, @MailingCity					VARCHAR(100) = NULL
	, @MailingStateCD				VARCHAR(2) = NULL
	, @MailingZipCode				VARCHAR(5) = NULL
	, @MailingCounty				VARCHAR(100) = NULL
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHouseholds
		SET
			AddressInd				= @AddressInd
			, PhysicalStreetLine1	= @PhysicalStreetLine1
			, PhysicalStreetLine2	= @PhysicalStreetLine2
			, PhysicalStreetLine3	= @PhysicalStreetLine3
			, PhysicalCity			= @PhysicalCity
			, PhysicalStateCD		= @PhysicalStateCD
			, PhysicalZipCode		= @PhysicalZipCode
			, PhysicalCounty		= @PhysicalCounty
			, DifferentMailingAddressInd = @DifferentMailingAddressInd
			, MailingStreetLine1	= @MailingStreetLine1
			, MailingStreetLine2	= @MailingStreetLine2
			, MailingStreetLine3	= @MailingStreetLine3
			, MailingCity			= @MailingCity
			, MailingStateCD		= @MailingStateCD
			, MailingZipCode		= @MailingZipCode
			, MailingCounty			= @MailingCounty
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE HouseholdID = @HouseholdID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated Household Address Information', GETDATE())
				, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated Household Address Information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Household Address Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO