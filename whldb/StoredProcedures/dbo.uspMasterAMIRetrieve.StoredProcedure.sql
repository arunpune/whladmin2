DROP PROCEDURE IF EXISTS [dbo].[uspMasterAMIRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 17-Oct-2024
-- Description:	Retrieve a list of master AMIs, or a single one by Effective Date, or the most recent by year
-- Examples:
--	EXEC dbo.uspMasterAMIRetrieve (Retrieve All)
--	EXEC dbo.uspMasterAMIRetrieve @EffectiveDate = 2024041 (Retrieve One)
--	EXEC dbo.uspMasterAMIRetrieve @EffectiveYear = 2024 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAMIRetrieve]
	@EffectiveDate		INT = 0
	, @EffectiveYear	INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@EffectiveYear, 0) > 0
		SELECT A.EffectiveDate, A.EffectiveYear
			, A.IncomeAmt
			, A.HH1, A.HH2, A.HH3, A.HH4
			, A.HH5, A.HH6, A.HH7, A.HH8
			, A.HH9, A.HH10
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterAMIs A
		JOIN (
			SELECT MAX(EffectiveDate) AS EffectiveDate
			FROM dbo.tblMasterAMIs
			WHERE EffectiveYear = @EffectiveYear
		) T ON T.EffectiveDate = A.EffectiveDate
		ORDER BY A.EffectiveDate DESC;
	ELSE
		SELECT A.EffectiveDate, A.EffectiveYear
			, A.IncomeAmt
			, A.HH1, A.HH2, A.HH3, A.HH4
			, A.HH5, A.HH6, A.HH7, A.HH8
			, A.HH9, A.HH10
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterAMIs A
		WHERE ISNULL(@EffectiveDate, 0) = 0 OR A.EffectiveDate = @EffectiveDate
		ORDER BY A.EffectiveDate DESC;

END
GO