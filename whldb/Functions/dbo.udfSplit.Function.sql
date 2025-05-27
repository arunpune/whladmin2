DROP FUNCTION IF EXISTS [dbo].[udfSplit];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Split the input string by the specified delimiter
-- Examples:
--	SELECT Item FROM dbo.udfSplit('1,2,3', ',')
-- =============================================
CREATE FUNCTION [dbo].[udfSplit] (
	@Input VARCHAR(MAX)
	, @Delimiter CHAR(1)
) RETURNS @Items TABLE (
	Item VARCHAR(MAX)
)
AS
BEGIN

	DECLARE @SanitizedInput VARCHAR(MAX);
	SET @SanitizedInput = ISNULL(RTRIM(@Input), '');

	IF LEN(@SanitizedInput) = 0 RETURN;

	;WITH XTE AS (
		SELECT CAST('<Item>' + REPLACE(@SanitizedInput, @Delimiter, '</Item><Item>') + '</Item>' AS XML) AS ColValue
	)
		INSERT INTO @Items (Item)
			SELECT col.value('.', 'VARCHAR(MAX)')
			FROM XTE
			CROSS APPLY ColValue.nodes('/Item') CA(col);
	
	RETURN;
END
GO