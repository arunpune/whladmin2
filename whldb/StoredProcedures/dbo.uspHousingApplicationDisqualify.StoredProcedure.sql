DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationDisqualify];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Disqualify an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationDisqualify @ApplicationID = 1, @DisqualificationCD = 'CODE', @DisqualificationReason = 'REASON'
--										, @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationDisqualify]
	@ApplicationID				BIGINT
	, @DisqualificationCD		VARCHAR(20)
	, @DisqualificationOther	VARCHAR(500) = NULL
	, @DisqualificationReason	VARCHAR(500)
	, @ModifiedBy				VARCHAR(200)
	, @ErrorMessage				VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			StatusCD				= 'WITHDRAWNDUPE'
			, WithdrawnDate			= GETDATE()
			, DisqualifiedInd		= 1
			, DisqualificationCD	= 'DUPLICATE'
			, DisqualificationOther	= @DisqualificationOther
			, DisqualificationReason = @DisqualificationReason
			, ModifiedBy			= @ModifiedBy
			, ModifiedDate			= GETDATE()
		WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
			VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Disqualified Housing Application - ' + RTRIM(@DisqualificationReason), GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT 1;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to disqualify Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO