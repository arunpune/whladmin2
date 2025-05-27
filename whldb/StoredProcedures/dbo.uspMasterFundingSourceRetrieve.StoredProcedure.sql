DROP PROCEDURE IF EXISTS [dbo].[uspMasterFundingSourceRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of master funding sources, or a single one by FundingSourceID
-- Examples:
--	EXEC dbo.uspMasterFundingSourceRetrieve (Retrieve All)
--	EXEC dbo.uspMasterFundingSourceRetrieve @FundingSourceID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFundingSourceRetrieve]
	@FundingSourceID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	WITH CountsCTE AS (
		SELECT FundingSourceID, COUNT(1) AS UsageCount
		FROM dbo.tblListingFundingSources
		GROUP BY FundingSourceID
	)
		SELECT A.FundingSourceID, A.[Name], A.[Description]
			, ISNULL(C.UsageCount, 0) AS UsageCount
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterFundingSources A
		LEFT OUTER JOIN CountsCTE C ON C.FundingSourceID = A.FundingSourceID
		WHERE @FundingSourceID = 0 OR A.FundingSourceID = @FundingSourceID
		ORDER BY A.[Name] ASC;

END
GO