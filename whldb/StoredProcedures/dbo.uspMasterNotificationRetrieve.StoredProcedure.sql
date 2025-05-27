DROP PROCEDURE IF EXISTS [dbo].[uspMasterNotificationRetrieve];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- =============================================
-- Author:		Prutech
-- Create date: 05-May-2024
-- Description:	Retrieve a list of master notifications, or a single one by NotificationID
-- Examples:
--	EXEC dbo.uspMasterNotificationRetrieve (Retrieve All)
--	EXEC dbo.uspMasterNotificationRetrieve @NotificationID = 1 (Retrieve One)
--	EXEC dbo.uspMasterNotificationRetrieve @CategoryCD = 'CAT' (Retrieve by Category)
-- =============================================
CREATE PROCEDURE [dbo].[uspMasterNotificationRetrieve]
	@NotificationID		INT = 0
	, @CategoryCD		VARCHAR(20) = NULL
	, @Title			VARCHAR(200) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF ISNULL(@NotificationID, 0) > 0
		SELECT N.NotificationID, N.CategoryCD, MC.[Description] AS CategoryDescription
			, N.Title, N.[Text]
			, N.FrequencyCD, MF.[Description] AS FrequencyDescription, N.FrequencyInterval
			, N.NotificationList, N.InternalNotificationList
			, N.CreatedBy, N.CreatedDate, N.ModifiedBy, N.ModifiedDate, N.Active
		FROM dbo.tblMasterNotifications N
		LEFT OUTER JOIN dbo.tblMetadata MC ON MC.CodeID = 103 AND MC.Code = N.CategoryCD
		LEFT OUTER JOIN dbo.tblMetadata MF ON MF.CodeID = 104 AND MF.Code = N.FrequencyCD
		WHERE N.NotificationID = @NotificationID;

	ELSE IF ISNULL(@NotificationID, 0) = 0 AND NULLIF(RTRIM(@CategoryCD), '') IS NOT NULL
		SELECT N.NotificationID, N.CategoryCD, MC.[Description] AS CategoryDescription
			, N.Title, N.[Text]
			, N.FrequencyCD, MF.[Description] AS FrequencyDescription, N.FrequencyInterval
			, N.NotificationList, N.InternalNotificationList
			, N.CreatedBy, N.CreatedDate, N.ModifiedBy, N.ModifiedDate, N.Active
		FROM dbo.tblMasterNotifications N
		LEFT OUTER JOIN dbo.tblMetadata MC ON MC.CodeID = 103 AND MC.Code = N.CategoryCD
		LEFT OUTER JOIN dbo.tblMetadata MF ON MF.CodeID = 104 AND MF.Code = N.FrequencyCD
		WHERE N.CategoryCD = @CategoryCD
			AND (NULLIF(ISNULL(RTRIM(@Title), ''), '') IS NULL OR N.Title = RTRIM(@Title))
		ORDER BY N.Title ASC;

	ELSE
		SELECT N.NotificationID, N.CategoryCD, MC.[Description] AS CategoryDescription
			, N.Title, N.[Text]
			, N.FrequencyCD, MF.[Description] AS FrequencyDescription, N.FrequencyInterval
			, N.NotificationList, N.InternalNotificationList
			, N.CreatedBy, N.CreatedDate, N.ModifiedBy, N.ModifiedDate, N.Active
		FROM dbo.tblMasterNotifications N
		LEFT OUTER JOIN dbo.tblMetadata MC ON MC.CodeID = 103 AND MC.Code = N.CategoryCD
		LEFT OUTER JOIN dbo.tblMetadata MF ON MF.CodeID = 104 AND MF.Code = N.FrequencyCD
		ORDER BY CategoryDescription ASC, N.Title ASC;

END
GO