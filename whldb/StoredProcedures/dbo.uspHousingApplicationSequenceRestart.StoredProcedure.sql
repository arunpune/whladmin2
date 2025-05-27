DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationSequenceRestart];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Restart the housing application sequence identifier
-- Examples:
--	EXEC dbo.uspHousingApplicationSequenceRestart
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationSequenceRestart]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastApplicationID BIGINT, @LastYY INT;
	SET @LastApplicationID = ISNULL((SELECT MAX(ApplicationID) FROM dbo.tblHousingApplications), 0);
	SET @LastYY = CASE WHEN @LastApplicationID > 0 THEN CONVERT(INT, LEFT(CONVERT(VARCHAR(20), @LastApplicationID), 2)) ELSE 0 END;

	DECLARE @RestartYY INT, @RestartSequenceID BIGINT;
	SET @RestartYY = CONVERT(INT, FORMAT(GETDATE(), 'yy'));

	-- SELECT @LastApplicationID, @LastYY, @RestartYY;

	IF @RestartYY <= @LastYY RETURN;

	SET @RestartSequenceID = (CONVERT(BIGINT, @RestartYY) * 1000000000) + 1001;
	-- SELECT 'Will alter to ' + CONVERT(VARCHAR(20), @RestartSequenceID);

	DECLARE @SQL NVARCHAR(1000);
	SET @SQL = N'ALTER SEQUENCE [dbo].[seqApplicationID] RESTART WITH ' + CONVERT(VARCHAR(20), @RestartSequenceID) + ';';
	EXEC (@SQL);

END
GO