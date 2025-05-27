DROP PROCEDURE IF EXISTS [dbo].[uspSiteHousingApplicationUpdateMembers];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Update site user information
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspSiteHousingApplicationUpdateMembers @ApplicationID = 1, @Username = 'USERNAME', @Last4SSN = '1234', @DateOfBirth = '1/1/2000'
--										, @IDTypeCD = 'STATEDL', @IDTypeValue = 'NY 1234', @IDIssueDate = '1/1/2000'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHousingApplicationUpdateMembers]
	@ApplicationID			BIGINT
	, @Username				VARCHAR(200)
	, @CoApplicantMemberID	BIGINT = NULL
	, @MemberIDs			VARCHAR(1000) = NULL
	, @ModifiedBy			VARCHAR(200)
	, @ErrorMessage			VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			CoApplicantMemberID	= @CoApplicantMemberID
			, MemberIDs			= @MemberIDs
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @Username, 'ADD', 'Updated additional members information', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to save Additional Members Information - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO