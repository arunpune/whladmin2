DROP PROCEDURE IF EXISTS [dbo].[uspMasterAmortizationRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Mar-2025
-- Description:	Retrieve a list of master Amortizations, or a single one by Rate
-- Examples:
--	EXEC dbo.uspMasterAmortizationRetrieve (Retrieve All)
--	EXEC dbo.uspMasterAmortizationRetrieve @Rate = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAmortizationRetrieve]
	@Rate		DECIMAL(7, 5) = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT A.Rate, A.RateInterestOnly, A.Rate10Year, A.Rate15Year
		, A.Rate20Year, A.Rate25Year, A.Rate30Year, A.Rate40Year
		, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
	FROM dbo.tblMasterAmortizations A
	WHERE ISNULL(@Rate, 0) = 0 OR A.Rate = @Rate
	ORDER BY A.Rate ASC;

END
GO