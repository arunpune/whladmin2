DROP PROCEDURE IF EXISTS [dbo].[uspMasterConfigRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of master configurations, or a single one by ConfigID
-- Examples:
--	EXEC dbo.uspMasterConfigRetrieve (Retrieve All)
--	EXEC dbo.uspMasterConfigRetrieve @ConfigID = 1 (Retrieve One)
--	EXEC dbo.uspMasterConfigRetrieve @Category = 'CAT' (Retrieve By Category)
--	EXEC dbo.uspMasterConfigRetrieve @Category = 'CAT', @SubCategory = 'SUBCAT' (Retrieve By Category, SubCategory)
--	EXEC dbo.uspMasterConfigRetrieve @Category = 'CAT', @SubCategory = 'SUBCAT', @ConfigKey = 'KEY' (Retrieve By Category, SubCategory, ConfigKey)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterConfigRetrieve]
	@ConfigID		INT = 0
	, @Category		VARCHAR(20) = NULL
	, @SubCategory	VARCHAR(20) = NULL
	, @ConfigKey	VARCHAR(100) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@ConfigID, 0) > 0
		SELECT ConfigID, Category, SubCategory, ConfigKey, ConfigValue, Active
		FROM dbo.tblMasterConfig
		WHERE ConfigID = @ConfigID;

	ELSE IF ISNULL(@ConfigID, 0) = 0 AND NULLIF(RTRIM(@Category), '') IS NOT NULL
		SELECT ConfigID, Category, SubCategory, ConfigKey, ConfigValue, Active
		FROM dbo.tblMasterConfig
		WHERE Category = @Category AND SubCategory = @SubCategory AND ConfigKey = @ConfigKey
		ORDER BY Category ASC, SubCategory ASC, ConfigKey ASC;

	ELSE
		SELECT ConfigID, Category, SubCategory, ConfigKey, ConfigValue, Active
		FROM dbo.tblMasterConfig
		ORDER BY Category ASC, SubCategory ASC, ConfigKey ASC;

END
GO