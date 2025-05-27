DROP PROCEDURE IF EXISTS [dbo].[uspMasterQuoteRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 23-Sep-2024
-- Description:	Retrieve a list of Quote configurations, or a single one by QuoteID
-- Examples:
--	EXEC dbo.uspMasterQuoteRetrieve (Retrieve All)
--	EXEC dbo.uspMasterQuoteRetrieve @QuoteID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterQuoteRetrieve]
	@QuoteID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT QuoteID, [Text], DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterQuotes
	WHERE @QuoteID = 0 OR QuoteID = @QuoteID
	ORDER BY [Text] ASC;

END
GO