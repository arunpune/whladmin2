DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationUpdateHouseholdInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Update household information for an existing housing application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationUpdateHouseholdInfo @ApplicationID = 1, @HouseholdID = 1, @Username = 'USERNAME'
--												, @AddressInd = 1, @MailingStreetLine1 = 'Line 1', @MailingCity = 'CITY'
--												, @MailingStateCD = 'ST', @MailingZipCode = '12345'
--												, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationUpdateHouseholdInfo]
	@ApplicationID					BIGINT
	, @HouseholdID					BIGINT
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
	, @VoucherInd					BIT = 0
	, @VoucherCDs					VARCHAR(1000) = NULL
	, @VoucherOther					VARCHAR(1000) = NULL
	, @VoucherAdminName				VARCHAR(200) = NULL
	, @LiveInAideInd				BIT = 0
	, @UnitTypeCDs					VARCHAR(200) = NULL
	, @UpdateProfileInd				BIT = 0
	, @ModifiedBy					VARCHAR(200)
	, @ErrorMessage					VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
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
			, VoucherInd			= @VoucherInd
			, VoucherCDs			= @VoucherCDs
			, VoucherOther			= @VoucherOther
			, VoucherAdminName		= @VoucherAdminName
			, LiveInAideInd			= @LiveInAideInd
			, UnitTypeCDs			= @UnitTypeCDs
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE ApplicationID = @ApplicationID AND Active = 1;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Updated household information', GETDATE());

		IF ISNULL(@UpdateProfileInd, 0) = 1
		BEGIN
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
				VALUES ('SITEUSER', @Username, @ModifiedBy, 'UPDATE', 'Updated household information', GETDATE())
					, ('HOUSEHOLD', CONVERT(VARCHAR(200), @HouseholdID), @ModifiedBy, 'UPDATE', 'Updated household information', GETDATE());
		END

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Household Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO