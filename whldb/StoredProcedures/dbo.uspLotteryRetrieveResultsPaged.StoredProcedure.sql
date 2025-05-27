DROP PROCEDURE IF EXISTS [dbo].[uspLotteryRetrieveResultsPaged];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 10-Aug-2024
-- Description:	Retrieve results of a given lottery
-- Examples:
--	EXEC dbo.uspLotteryRetrieveResultsPaged @LotteryID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspLotteryRetrieveResultsPaged]
	@LotteryID		INT
	, @PageNo		INT = 1
	, @PageSize		INT = 1000
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@PageNo, 1) < 1 SET @PageNo = 1;
	IF ISNULL(@PageSize, 100) < 100 SET @PageSize = 100;

	SELECT LOT.LotteryID, LOT.ListingID, LOT.ManualInd, LOT.RunDate, LOT.RunBy
		, LOT.StatusCD AS LotteryStatusCD, MLOTS.[Description] AS LotteryStatusDescription
		, A.ApplicationID, A.LotteryNumber
			, A.Title, A.FirstName, A.MiddleName, A.LastName, A.Suffix
			, A.DateOfBirth, A.Last4SSN
			, A.IDTypeCD, MDIT.[Description] AS IDTypeDescription, A.IDTypeValue, A.IDIssueDate
			, A.GenderCD, A.RaceCD, A.EthnicityCD, A.Pronouns
			, A.StudentInd, A.DisabilityInd, A.VeteranInd
			, A.PhoneNumberTypeCD, A.PhoneNumber, A.PhoneNumberExtn
			, A.AltPhoneNumberTypeCD, A.AltPhoneNumber, A.AltPhoneNumberExtn
			, A.EmailAddress, A.AltEmailAddress
			, A.EverLivedInWestchesterInd, A.CountyLivingIn
			, A.CurrentlyWorkingInWestchesterInd, A.CountyWorkingIn
			, A.OwnRealEstateInd, A.RealEstateValueAmt, A.AssetValueAmt, A.IncomeValueAmt
			, A.LeadTypeCD, A.LeadTypeOther
			, A.SubmissionTypeCD, MDST.[Description] AS SubmissionTypeDescription
	FROM dbo.tblLotteryResults LR
	JOIN dbo.tblLotteries LOT ON LOT.LotteryID = LR.LotteryID
	JOIN dbo.tblHousingApplications A ON A.LotteryID = LR.LotteryID AND A.ListingID = LR.ListingID AND A.ApplicationID = LR.ApplicationID
	LEFT OUTER JOIN dbo.tblMetadata MDST ON MDST.CodeID = 128 AND MDST.Code = A.SubmissionTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MDIT ON MDIT.CodeID = 125 AND MDIT.Code = A.IDTypeCD
	LEFT OUTER JOIN dbo.tblMetadata MLOTS ON MLOTS.CodeID = 127 AND MLOTS.Code = LOT.StatusCD
	WHERE LR.LotteryID = @LotteryID
	ORDER BY A.LotteryNumber ASC
	OFFSET (@PageNo - 1) * @PageSize ROWS
	FETCH NEXT @PageSize ROWS ONLY;

	SELECT @PageNo AS PageNo
		, @PageSize AS PageSize
		, CEILING(COUNT(1) / @PageSize) + 1 AS TotalPages
		, COUNT(1) AS TotalRecords
	FROM dbo.tblLotteryResults LR
	JOIN dbo.tblLotteries LOT ON LOT.LotteryID = LR.LotteryID
	JOIN dbo.tblHousingApplications A ON A.LotteryID = LR.LotteryID AND A.ListingID = LR.ListingID AND A.ApplicationID = LR.ApplicationID
	WHERE LR.LotteryID = @LotteryID;

END
GO