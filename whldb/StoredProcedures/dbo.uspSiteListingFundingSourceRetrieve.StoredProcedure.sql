DROP PROCEDURE IF EXISTS [dbo].[uspSiteListingFundingSourceRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of Funding Sources by ListingID
-- Examples:
--	EXEC dbo.uspSiteListingFundingSourceRetrieve @ListingID = 1 (Retrieve All by Listing)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteListingFundingSourceRetrieve]
	@ListingID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.FundingSourceID, A.[Name], A.[Description], LA.CreatedDate, LA.CreatedBy, LA.ModifiedDate, LA.ModifiedBy, LA.Active
	FROM dbo.tblMasterFundingSources A
	JOIN dbo.tblSiteListingFundingSources LA ON LA.ListingID = @ListingID AND LA.FundingSourceID = A.FundingSourceID
	ORDER BY A.[Name] ASC;

END
GO