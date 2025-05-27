DROP PROCEDURE IF EXISTS [dbo].[uspLotteryRetrieveResults];
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
--	EXEC dbo.uspLotteryRetrieveResults @LotteryID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspLotteryRetrieveResults]
	@LotteryID		INT
AS
BEGIN
	SET NOCOUNT ON;

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
	ORDER BY A.LotteryNumber ASC;

END
GO