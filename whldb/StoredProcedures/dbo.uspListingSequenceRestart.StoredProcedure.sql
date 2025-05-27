DROP PROCEDURE IF EXISTS [dbo].[uspListingSequenceRestart];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 02-Aug-2024
-- Description:	Restart the listing sequence identifier
-- Examples:
--	EXEC dbo.uspListingSequenceRestart
-- =============================================
CREATE PROCEDURE [dbo].[uspListingSequenceRestart]
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @LastListingID INT, @LastYYYYMM INT;
	SET @LastListingID = ISNULL((SELECT MAX(ListingID) FROM dbo.tblListings), 0);
	SET @LastYYYYMM = CASE WHEN @LastListingID > 0 THEN CONVERT(INT, LEFT(CONVERT(VARCHAR(9), @LastListingID), 6)) ELSE 0 END;

	DECLARE @RestartYYYYMM INT, @RestartSequenceID INT;
	SET @RestartYYYYMM = CONVERT(INT, (SELECT LEFT(CONVERT(VARCHAR(20), GETDATE(), 112), 6)));

	-- SELECT @LastListingID, @LastYYYYMM, @RestartYYYYMM;

	IF @RestartYYYYMM <= @LastYYYYMM RETURN;

	SET @RestartSequenceID = (@RestartYYYYMM * 1000) + 1;
	-- SELECT 'Will alter to ' + CONVERT(VARCHAR(20), @RestartSequenceID);

	DECLARE @SQL NVARCHAR(1000);
	SET @SQL = N'ALTER SEQUENCE [dbo].[seqListingID] RESTART WITH ' + CONVERT(VARCHAR(20), @RestartSequenceID) + ';';
	EXEC (@SQL);

END
GO