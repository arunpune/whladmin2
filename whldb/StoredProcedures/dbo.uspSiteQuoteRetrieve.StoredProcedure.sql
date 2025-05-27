DROP PROCEDURE IF EXISTS [dbo].[uspSiteQuoteRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Retrieve a list of Quote configurations, or a single one by QuoteID
-- Examples:
--	EXEC dbo.uspSiteQuoteRetrieve (Retrieve All)
--	EXEC dbo.uspSiteQuoteRetrieve @QuoteID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteQuoteRetrieve]
	@QuoteID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT QuoteID, [Text], DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterQuotes
	WHERE (@QuoteID = 0 OR QuoteID = @QuoteID) AND Active = 1
	ORDER BY [Text] ASC;

END
GO