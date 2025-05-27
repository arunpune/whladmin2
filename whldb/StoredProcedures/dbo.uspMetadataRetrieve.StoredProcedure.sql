DROP PROCEDURE IF EXISTS [dbo].[uspMetadataRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-Jun-2024
-- Description:	Retrieve a list of metadata, or a single one by MetadataID
-- Examples:
--	EXEC dbo.uspMetadataRetrieve (Retrieve All)
--	EXEC dbo.uspMetadataRetrieve @MetadataID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMetadataRetrieve]
	@MetadataID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT MetadataID, CodeID, Code, [Description], AssociatedCodeID, AssociatedCode, Active
	FROM dbo.tblMetadata
	WHERE @MetadataID = 0 OR MetadataID = @MetadataID
	ORDER BY [CodeID] ASC, [Code] ASC, [Description] ASC;

END
GO