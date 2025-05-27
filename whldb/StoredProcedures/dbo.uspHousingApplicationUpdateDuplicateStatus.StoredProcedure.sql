DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationUpdateDuplicateStatus];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Update an existing Housing Application
-- Examples:
--	DECLARE @ErrorMessage VARCHAR(1000);
--	EXEC dbo.uspHousingApplicationUpdateDuplicateStatus @ApplicationID = 1, @DuplicateCheckCD = 'N', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	EXEC dbo.uspHousingApplicationUpdateDuplicateStatus @ApplicationID = 1, @DuplicateCheckCD = 'D', @DuplicateReason = 'REASON', @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	EXEC dbo.uspHousingApplicationUpdateDuplicateStatus @ApplicationID = 1, @DuplicateCheckCD = 'P', @DuplicateReason = 'REASON', @DuplicateCheckResponseDueDate = GETDATE(), @ModifiedBy = 'USERNAME', @ErrorMessage = @ErrorMessage OUTPUT
--	SELECT @ErrorMessage;
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationUpdateDuplicateStatus]
	@ApplicationID						BIGINT
	, @DuplicateCheckCD					CHAR(1)
	, @DuplicateReason					VARCHAR(1000) = NULL
	, @DuplicateCheckResponseDueDate	DATETIME = NULL
	, @ModifiedBy						VARCHAR(200)
	, @ErrorMessage						VARCHAR(1000) = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	BEGIN TRY

		BEGIN TRAN;

		UPDATE dbo.tblHousingApplications
		SET
			DuplicateCheckCD					= CASE WHEN @DuplicateCheckCD = 'N' THEN '-' ELSE @DuplicateCheckCD END
			, DuplicateCheckResponseDueDate		= @DuplicateCheckResponseDueDate
			, ModifiedBy						= @ModifiedBy
			, ModifiedDate						= GETDATE()
		WHERE ApplicationID = @ApplicationID;

		IF @DuplicateCheckCD = 'D'
			UPDATE dbo.tblHousingApplications
			SET
				OriginalStatusCD	= StatusCD
				, StatusCD			= 'DUPLICATE'
				, WithdrawnDate		= GETDATE()
			WHERE ApplicationID = @ApplicationID;
		ELSE IF @DuplicateCheckCD = 'N'
			UPDATE dbo.tblHousingApplications
			SET
				StatusCD		= OriginalStatusCD
				, WithdrawnDate	= NULL
			WHERE ApplicationID = @ApplicationID;

		INSERT INTO dbo.tblHousingApplicationComments (ApplicationID, Comments, InternalOnlyInd, Active, CreatedDate, CreatedBy)
			VALUES (@ApplicationID, @DuplicateReason, 0, 1, GETDATE(), @ModifiedBy);

		DECLARE @AuditNote VARCHAR(1000);
		SET @AuditNote = CASE @DuplicateCheckCD
								WHEN 'D' THEN 'Updated Housing Application as Duplicate, application automatically Withdrawn - Eliminated'
								WHEN 'P' THEN 'Updated Housing Application as a Potential Duplicate, pending applicant response'
								WHEN 'N' THEN 'Removed duplicate flag from Housing Application'
								ELSE ''
							END
		IF LEN(ISNULL(RTRIM(@AuditNote), '')) > 0
			INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
				VALUES ('APPLICATION', CONVERT(VARCHAR(20), @ApplicationID), @ModifiedBy, 'UPDATE', 'Updated Housing Application as Potential Duplicate', GETDATE());

		COMMIT TRAN;

		SET @ErrorMessage = '';
		SELECT @ApplicationID;

	END TRY
	BEGIN CATCH

		IF XACT_STATE() <> 0 ROLLBACK TRAN;
		SET @ErrorMessage = 'Failed to update duplicate status for Housing Application - ' + CONVERT(VARCHAR(20), ERROR_NUMBER()) + ': ' + ERROR_MESSAGE();
		SELECT -1;

	END CATCH

END
GO