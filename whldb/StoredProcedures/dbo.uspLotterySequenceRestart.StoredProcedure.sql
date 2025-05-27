DROP PROCEDURE IF EXISTS [dbo].[uspLotterySequenceRestart];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Aug-2024
-- Description:	Restart the lottery sequence identifier
-- Examples:
--	EXEC dbo.uspLotterySequenceRestart
-- =============================================
CREATE PROCEDURE [dbo].[uspLotterySequenceRestart]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastLotteryID INT, @LastYYYY INT;
	SET @LastLotteryID = ISNULL((SELECT MAX(LotteryID) FROM dbo.tblLotteries), 0);
	SET @LastYYYY = CASE WHEN @LastLotteryID > 0 THEN CONVERT(INT, LEFT(CONVERT(VARCHAR(9), @LastLotteryID), 4)) ELSE 0 END;

	DECLARE @RestartYYYY INT, @RestartSequenceID INT;
	SET @RestartYYYY = YEAR(GETDATE());
	SET @RestartSequenceID = (@RestartYYYY * 100000) + 1;

	DECLARE @CurrentSequenceID INT;
	SET @CurrentSequenceID = ISNULL((SELECT CONVERT(INT, current_value) FROM sys.sequences WHERE [name] = 'seqLotteryID'), 0);

	-- SELECT @LastLotteryID, @LastYYYY, @RestartYYYY, @CurrentSequenceID, @RestartSequenceID;

	IF @RestartYYYY <= @LastYYYY RETURN;
	IF @CurrentSequenceID >= @RestartSequenceID RETURN;

	-- SELECT 'Will alter to ' + CONVERT(VARCHAR(20), @RestartSequenceID);

	DECLARE @SQL NVARCHAR(1000);
	SET @SQL = N'ALTER SEQUENCE [dbo].[seqLotteryID] RESTART WITH ' + CONVERT(VARCHAR(20), @RestartSequenceID) + ';';
	EXEC (@SQL);

END
GO