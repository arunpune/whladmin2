DROP PROCEDURE IF EXISTS [dbo].[uspHousingApplicationRetrieveDuplicates];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 20-Dec-2024
-- Description:	Retrieve a list of potential duplicate applications by Listing ID
-- Examples:
--	EXEC dbo.uspHousingApplicationRetrieveDuplicates @ListingID = 1 (Retrieve One)
-- =============================================
CREATE PROCEDURE [dbo].[uspHousingApplicationRetrieveDuplicates]
	@ListingID		INT
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApplicantsTable TABLE (
		ApplicationID BIGINT
		, MemberID BIGINT
		, DateOfBirth DATETIME
		, Last4SSN VARCHAR(4)
		, EmailAddress VARCHAR(200)
		, AltEmailAddress VARCHAR(200)
		, Title VARCHAR(10)
		, FirstName VARCHAR(100)
		, MiddleName VARCHAR(100)
		, LastName VARCHAR(100)
		, Suffix VARCHAR(10)
	);

	DECLARE @EmailsTable TABLE (
		ApplicationID BIGINT
		, EmailAddressTypeCD VARCHAR(20)
		, EmailAddress VARCHAR(200)
	);

	DECLARE @PhoneNumbersTable TABLE (
		ApplicationID BIGINT
		, PhoneNumberTypeCD VARCHAR(20)
		, PhoneNumber VARCHAR(20)
	);

	DECLARE @AddressesTable TABLE (
		ApplicationID BIGINT
		, AddressTypeCD VARCHAR(20)
		, StreetAddress VARCHAR(1000)
	);

	INSERT INTO @ApplicantsTable (ApplicationID, MemberID, DateOfBirth, Last4SSN
									, Title, FirstName, MiddleName, LastName, Suffix)
		SELECT A.ApplicationID, A.MemberID, A.DateOfBirth, A.Last4SSN
			, A.Title, A.FirstName, A.MiddleName, A.LastName, A.Suffix
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE');

	SELECT FORMAT(DateOfBirth, 'MM-dd-yyyy') AS DateOfBirth, Last4SSN, COUNT(DISTINCT ApplicationID) AS [Count]
	FROM @ApplicantsTable
	GROUP BY DateOfBirth, Last4SSN
	HAVING COUNT(DISTINCT ApplicationID) > 1;

	SELECT ISNULL(RTRIM(FirstName), '') + ' ' + ISNULL(RTRIM(LastName), '') AS [Name], COUNT(DISTINCT ApplicationID) AS [Count]
	FROM @ApplicantsTable
	WHERE ISNULL(RTRIM(FirstName), '') + ISNULL(RTRIM(LastName), '') <> ''
	GROUP BY ISNULL(RTRIM(FirstName), '') + ' ' + ISNULL(RTRIM(LastName), '')
	HAVING COUNT(DISTINCT ApplicationID) > 1;

	INSERT INTO @EmailsTable (ApplicationID, EmailAddressTypeCD, EmailAddress)
		SELECT A.ApplicationID, 'PRIMARY', ISNULL(RTRIM(A.EmailAddress), '') AS EmailAddress
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.EmailAddress), '') <> ''
		UNION
		SELECT A.ApplicationID, 'ALTERNATE', ISNULL(RTRIM(A.AltEmailAddress), '') AS EmailAddress
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.AltEmailAddress), '') <> ''

	SELECT EmailAddress, COUNT(DISTINCT ApplicationID) AS [Count]
	FROM @EmailsTable
	WHERE RTRIM(EmailAddress) IS NOT NULL
	GROUP BY EmailAddress
	HAVING COUNT(DISTINCT ApplicationID) > 1;

	INSERT INTO @PhoneNumbersTable (ApplicationID, PhoneNumberTypeCD, PhoneNumber)
		SELECT A.ApplicationID, 'PRIMARY', ISNULL(RTRIM(A.PhoneNumber), '') + ISNULL(RTRIM(A.PhoneNumberExtn), '') AS PhoneNumber
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.PhoneNumber), '') + ISNULL(RTRIM(A.PhoneNumberExtn), '') <> ''
		UNION
		SELECT A.ApplicationID, 'ALTERNATE', ISNULL(RTRIM(A.AltPhoneNumber), '') + ISNULL(RTRIM(A.AltPhoneNumberExtn), '') AS PhoneNumber
		FROM dbo.tblHousingApplicants A
		JOIN dbo.tblHousingApplications HA ON HA.ApplicationID = A.ApplicationID
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(A.AltPhoneNumber), '') + ISNULL(RTRIM(A.AltPhoneNumberExtn), '') <> '';

	SELECT PhoneNumber, COUNT(DISTINCT ApplicationID) AS [Count]
	FROM @PhoneNumbersTable
	WHERE ISNULL(RTRIM(PhoneNumber), '') <> ''
	GROUP BY PhoneNumber
	HAVING COUNT(DISTINCT ApplicationID) > 1;

	INSERT INTO @AddressesTable (ApplicationID, AddressTypeCD, StreetAddress)
		SELECT HA.ApplicationID, 'PHYSICAL', ISNULL(RTRIM(HA.PhysicalStreetLine1), '')
											+ CASE WHEN ISNULL(RTRIM(HA.PhysicalStreetLine2), '') <> '' THEN ', ' + RTRIM(HA.PhysicalStreetLine2) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.PhysicalStreetLine3), '') <> '' THEN ', ' + RTRIM(HA.PhysicalStreetLine3) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.PhysicalCity), '') <> '' THEN ', ' + RTRIM(HA.PhysicalCity) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.PhysicalStateCD), '') <> '' THEN ', ' + RTRIM(HA.PhysicalStateCD) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.PhysicalZipCode), '') <> '' THEN ' ' + RTRIM(HA.PhysicalZipCode) ELSE '' END
											AS StreetAddress
		FROM dbo.tblHousingApplications HA
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(HA.PhysicalStreetLine1), '') + ISNULL(RTRIM(HA.PhysicalCity), '') + ISNULL(RTRIM(HA.PhysicalStateCD), '') + ISNULL(RTRIM(HA.PhysicalZipCode), '') <> ''
		UNION
		SELECT HA.ApplicationID, 'MAILING', ISNULL(RTRIM(HA.MailingStreetLine1), '')
											+ CASE WHEN ISNULL(RTRIM(HA.MailingStreetLine2), '') <> '' THEN ', ' + RTRIM(HA.MailingStreetLine2) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.MailingStreetLine3), '') <> '' THEN ', ' + RTRIM(HA.MailingStreetLine3) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.MailingCity), '') <> '' THEN ', ' + RTRIM(HA.MailingCity) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.MailingStateCD), '') <> '' THEN ', ' + RTRIM(HA.MailingStateCD) ELSE '' END
											+ CASE WHEN ISNULL(RTRIM(HA.MailingZipCode), '') <> '' THEN ' ' + RTRIM(HA.MailingZipCode) ELSE '' END
											AS StreetAddress
		FROM dbo.tblHousingApplications HA
		WHERE HA.ListingID = @ListingID
			AND HA.StatusCD IN ('SUBMITTED', 'WAITLISTED', 'ASSIGNED', 'DUPLICATE')
			AND ISNULL(RTRIM(HA.MailingStreetLine1), '') + ISNULL(RTRIM(HA.MailingCity), '') + ISNULL(RTRIM(HA.MailingStateCD), '') + ISNULL(RTRIM(HA.MailingZipCode), '') <> '';

	SELECT StreetAddress, COUNT(DISTINCT ApplicationID) AS [Count]
	FROM @AddressesTable
	WHERE ISNULL(RTRIM(StreetAddress), '') <> ''
	GROUP BY StreetAddress
	HAVING COUNT(DISTINCT ApplicationID) > 1;

END
GO