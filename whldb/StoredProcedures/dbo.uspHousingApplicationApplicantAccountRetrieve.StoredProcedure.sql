DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationApplicantAccountRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 15-Sep-2024
-- Description:	Retrieve a list of applicant accounts by housing application ID
-- Examples:
--	EXEC dbo.uspHousingApplicationApplicantAccountRetrieve @ApplicationID = 1
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationApplicantAccountRetrieve]
	@ApplicationID	BIGINT
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.AccountID, A.ApplicationID, A.ApplicantID
		, A.AccountNumber
		, A.AccountTypeCD, MDACT.[Description] AS AccountTypeDescription, A.AccountTypeOther
		, A.AccountValueAmt, A.InstitutionName
		, A.CreatedBy, A.CreatedDate, A.ModifiedBy, A.ModifiedDate
		, A.Active
	FROM dbo.tblHousingApplicantAccounts A
	LEFT OUTER JOIN dbo.tblMetadata MDACT ON MDACT.CodeID = 124 AND MDACT.Code = A.AccountTypeCD
	WHERE A.ApplicationID = @ApplicationID
	ORDER BY A.ApplicantID, A.AccountID;

END
GO