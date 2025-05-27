DROP PROCEDURE IF EXISTS [dbo].[uspMasterFAQRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of FAQ configurations, or a single one by FAQID
-- Examples:
--	EXEC dbo.uspMasterFAQRetrieve (Retrieve All)
--	EXEC dbo.uspMasterFAQRetrieve @FAQID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterFAQRetrieve]
	@FAQID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT FAQID, CategoryName, Title, [Text]
		, [Url], Url1, Url2, Url3, Url4, Url5, Url6, Url7, Url8, Url9
		, DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active
	FROM dbo.tblMasterFAQs
	WHERE @FAQID = 0 OR FAQID = @FAQID
	ORDER BY DisplayOrder ASC;

END
GO