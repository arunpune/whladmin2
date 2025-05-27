DROP PROCEDURE IF EXISTS [dbo].[uspSystemInfoRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 27-Sep-2024
-- Description:	Retrieve system information
-- Examples:
--	EXEC dbo.uspSystemInfoRetrieve
-- =============================================
CREATE PROCEDURE [dbo].[uspSystemInfoRetrieve]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT TOP 1 ReleaseVersion, [Timestamp]
	FROM dbo.tblReleaseConfig;

END
GO