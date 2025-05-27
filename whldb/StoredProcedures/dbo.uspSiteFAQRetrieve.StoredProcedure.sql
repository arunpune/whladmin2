DROP PROCEDURE IF EXISTS [dbo].[uspSiteFAQRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 07-Jul-2024
-- Description:	Retrieve a list of FAQ configurations, or a single one by FAQID
-- Examples:
--	EXEC dbo.uspSiteFAQRetrieve (Retrieve All)
--	EXEC dbo.uspSiteFAQRetrieve @FAQID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteFAQRetrieve]
	@FAQID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FAQID, CategoryName, Title, [Text]
		, [Url], Url1, Url2, Url3, Url4, Url5, Url6, Url7, Url8, Url9
		, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterFAQs
	WHERE (@FAQID = 0 OR FAQID = @FAQID) AND Active = 1
	ORDER BY DisplayOrder ASC;

END
GO