DROP PROCEDURE IF EXISTS [dbo].[uspSiteHouseholdRetrieveAccounts];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Sep-2024
-- Description:	Retrieve household accounts by householdid, or a given one by account ID
-- Examples:
--	EXEC dbo.uspSiteHouseholdRetrieveAccounts @HouseholdID = 1 (Retrieve All by Household)
--	EXEC dbo.uspSiteHouseholdRetrieveAccounts @HouseholdID = 1, @AccountID = 1 (Retrieve by Household Member)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteHouseholdRetrieveAccounts]
	@HouseholdID	BIGINT
	, @AccountID	BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.AccountID, A.HouseholdID, A.AccountNumber, A.AccountValueAmt, A.InstitutionName
		, A.AccountTypeCD, MDACT.[Description] AS AccountTypeDescription, A.AccountTypeOther
		, A.PrimaryHolderMemberID
		, A.CreatedBy, A.CreatedDate, A.ModifiedBy, A.ModifiedDate
		, A.Active
	FROM dbo.tblHouseholdAccounts A
	JOIN dbo.tblHouseholds H ON H.HouseholdID = A.HouseholdID
	LEFT OUTER JOIN dbo.tblMetadata MDACT ON MDACT.CodeID = 124 AND MDACT.Code = A.AccountTypeCD
	WHERE A.HouseholdID = @HouseholdID
		AND (ISNULL(@AccountID, 0) = 0 OR A.AccountID = @AccountID)
		AND A.Active = 1
	ORDER BY A.AccountNumber;

END
GO