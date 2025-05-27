DROP PROCEDURE IF EXISTS [dbo].[uspSiteUserNotificationRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 09-Oct-2024
-- Description:	Retrieve a list of notifications for a given site user, optionally filter by read, unread, all
-- Examples:
--	EXEC dbo.uspSiteUserNotificationRetrieve @Username = 'USERNAME' (Retrieve All)
--	EXEC dbo.uspSiteUserNotificationRetrieve @Username = 'USERNAME', @FilterTypeCD = 'R' (Retrieve All Read)
--	EXEC dbo.uspSiteUserNotificationRetrieve @Username = 'USERNAME', @FilterTypeCD = 'U' (Retrieve All Unread)
-- =============================================
CREATE PROCEDURE [dbo].[uspSiteUserNotificationRetrieve]
	@Username		VARCHAR(200) = NULL
	, @FilterTypeCD	VARCHAR(1) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	SET @FilterTypeCD = NULLIF(ISNULL(RTRIM(@FilterTypeCD), ''), '');

	SELECT N.NotificationID, N.Username, N.Subject, N.Body
		, N.ReadInd, N.ReadTimestamp
		, N.EmailSentInd, N.EmailTimestamp
		, N.CreatedBy, N.CreatedDate, N.ModifiedBy, N.ModifiedDate
		, N.Active
	FROM dbo.tblSiteUserNotifications N
	WHERE N.Username = @Username
		AND N.ReadInd = CASE @FilterTypeCD WHEN 'R' THEN 1 WHEN 'U' THEN 0 ELSE N.ReadInd END
		AND N.Active = 1
	ORDER BY N.CreatedDate DESC;

END
GO