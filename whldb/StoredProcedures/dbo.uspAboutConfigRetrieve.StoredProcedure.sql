DROP PROCEDURE IF EXISTS [dbo].[uspAboutConfigRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of about configurations, or a single one by AboutID
-- Examples:
--	EXEC dbo.uspAboutConfigRetrieve (Retrieve All)
--	EXEC dbo.uspAboutConfigRetrieve @AboutID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspAboutConfigRetrieve]
	@AboutID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT AboutID, Title, [Text], [Url], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblAboutConfig
	WHERE @AboutID = 0 OR AboutID = @AboutID
	ORDER BY [Title] ASC;

END
GO