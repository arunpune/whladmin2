DROP PROCEDURE IF EXISTS [dbo].[uspListingFundingSourceRetrieveForEdit];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of all Funding Sources available in the system (selected by ListingID)
-- Examples:
--	EXEC dbo.uspListingFundingSourceRetrieveForEdit @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspListingFundingSourceRetrieveForEdit]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.FundingSourceID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, A.Active
		, CASE WHEN LA.CreatedDate IS NOT NULL THEN 1 ELSE 0 END AS Selected
	FROM dbo.tblMasterFundingSources A
	LEFT OUTER JOIN dbo.tblListingFundingSources LA ON LA.ListingID = @ListingID AND LA.FundingSourceID = A.FundingSourceID
	ORDER BY A.[Name] ASC;

END
GO