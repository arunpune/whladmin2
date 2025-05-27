DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationUpdateHouseholdInfo];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Update houshold information for an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationUpdateHouseholdInfo @ApplicationID = 1
--										, @FirstName = 'John', @LastName = 'Smith'
-- 										, @GenderCD = 'NOANS', @RaceCD = 'NOANS', @EthnicityCD = 'NOANS'
--										, @LeadTypeCD = 'OTHER'
--										, @CreatedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationUpdateHouseholdInfo]
	@ApplicationID					BIGINT
	, @AddressInd					BIT
	, @MailingStreetLine1			VARCHAR(100) = NULL
	, @MailingStreetLine2			VARCHAR(100) = NULL
	, @MailingStreetLine3			VARCHAR(100) = NULL
	, @MailingCity					VARCHAR(100) = NULL
	, @MailingStateCD				VARCHAR(2) = NULL
	, @MailingZipCode				VARCHAR(5) = NULL
	, @MailingCounty				VARCHAR(100) = NULL
	, @DifferentMailingAddressInd	BIT = 0
	, @PhysicalStreetLine1			VARCHAR(100) = NULL
	, @PhysicalStreetLine2			VARCHAR(100) = NULL
	, @PhysicalStreetLine3			VARCHAR(100) = NULL
	, @PhysicalCity					VARCHAR(100) = NULL
	, @PhysicalStateCD				VARCHAR(2) = NULL
	, @PhysicalZipCode				VARCHAR(5) = NULL
	, @PhysicalCounty				VARCHAR(100) = NULL
	, @VoucherInd					BIT
	, @VoucherCDs					VARCHAR(1000) = NULL
	, @VoucherOther					VARCHAR(1000) = NULL
	, @VoucherAdminName				VARCHAR(200) = NULL
	, @LiveInAideInd				BIT
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
			AddressInd						= @AddressInd
			, MailingStreetLine1			= @MailingStreetLine1
			, MailingStreetLine2			= @MailingStreetLine2
			, MailingStreetLine3			= @MailingStreetLine3
			, MailingCity					= @MailingCity
			, MailingStateCD				= @MailingStateCD
			, MailingZipCode				= @MailingZipCode
			, MailingCounty					= @MailingCounty
			, DifferentMailingAddressInd	= @DifferentMailingAddressInd
			, PhysicalStreetLine1			= @PhysicalStreetLine1
			, PhysicalStreetLine2			= @PhysicalStreetLine2
			, PhysicalStreetLine3			= @PhysicalStreetLine3
			, PhysicalCity					= @PhysicalCity
			, PhysicalStateCD				= @PhysicalStateCD
			, PhysicalZipCode				= @PhysicalZipCode
			, PhysicalCounty				= @PhysicalCounty
			, VoucherInd					= @VoucherInd
			, VoucherCDs					= @VoucherCDs
			, VoucherOther					= @VoucherOther
			, VoucherAdminName				= @VoucherAdminName
			, LiveInAideInd					= @LiveInAideInd
			, ModifiedBy					= @ModifiedBy
			, ModifiedDate					= GETDATE()
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Updated Household Info for Housing Application', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ApplicationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update Household Info for Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO