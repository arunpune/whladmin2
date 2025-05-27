DROP PROCEDURE IF EXISTS [dbo].[uspMasterAmenityRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of master amenities, or a single one by AmenityID
-- Examples:
--	EXEC dbo.uspMasterAmenityRetrieve (Retrieve All)
--	EXEC dbo.uspMasterAmenityRetrieve @AmenityID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterAmenityRetrieve]
	@AmenityID		INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	WITH CountsCTE AS (
		SELECT AmenityID, COUNT(1) AS UsageCount
		FROM dbo.tblListingAmenities
		GROUP BY AmenityID
	)
		SELECT A.AmenityID, A.[Name], A.[Description]
			, ISNULL(C.UsageCount, 0) AS UsageCount
			, A.CreatedDate, A.CreatedBy, A.ModifiedDate, A.ModifiedBy, A.Active
		FROM dbo.tblMasterAmenities A
		LEFT OUTER JOIN CountsCTE C ON C.AmenityID = A.AmenityID
		WHERE @AmenityID = 0 OR A.AmenityID = @AmenityID
		ORDER BY A.[Name] ASC;

END
GO