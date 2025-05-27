DROP PROCEDURE IF EXISTS [dbo].[uspMasterMarketingAgentRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 14-Mar-2025
-- Description:	Retrieve a list of master marketing agents, or a single one by AgentID
-- Examples:
--	EXEC dbo.uspMasterMarketingAgentRetrieve (Retrieve All)
--	EXEC dbo.uspMasterMarketingAgentRetrieve @AgentID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterMarketingAgentRetrieve]
	@AgentID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	WITH CountsCTE AS (
		SELECT MarketingAgentID AS AgentID, COUNT(1) AS UsageCount
		FROM dbo.tblListings
		GROUP BY MarketingAgentID
	)
		SELECT A.AgentID, A.[Name], A.ContactName, A.PhoneNumber, A.EmailAddress
			, ISNULL(C.UsageCount, 0) AS UsageCount
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterMarketingAgents A
		LEFT OUTER JOIN CountsCTE C ON C.AgentID = A.AgentID
		WHERE @AgentID = 0 OR A.AgentID = @AgentID
		ORDER BY A.[Name] ASC;

END
GO