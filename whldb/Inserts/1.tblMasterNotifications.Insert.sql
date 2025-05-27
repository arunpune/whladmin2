SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Notifications TABLE (
        [CategoryCD] [varchar](20) NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](1000) NOT NULL,
        [FrequencyCD] [varchar](20) NOT NULL,
        [FrequencyInterval] [int] NOT NULL,
        [NotificationList] [varchar](2000) NULL,
        [InternalNotificationList] [varchar](2000) NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    DECLARE @InternalNotificationList VARCHAR(1000);
    SET @InternalNotificationList = 'tifa@westchestercountyny.gov, jee2@westchestercountyny.gov, jmlh@westchestercountyny.gov, asmith@wroinc.org';

    INSERT INTO @Notifications (CategoryCd, Title, [Text], FrequencyCD, FrequencyInterval, NotificationList, InternalNotificationList)
        VALUES ('APPLICANT', 'Incomplete Application - First Reminder', 'You have one or more incomplete Housing Lottery Applications. Please visit the site to complete them.', 'BEFORE', 15, NULL, NULL)
            , ('APPLICANT', 'Incomplete Application - Second Reminder', 'You have one or more incomplete Housing Lottery Applications. Please visit the site to complete them.', 'BEFORE', 7, NULL, NULL)
            , ('APPLICANT', 'Incomplete Application - Final Reminder', 'You have one or more incomplete Housing Lottery Applications. Please visit the site to complete them.', 'BEFORE', 2, NULL, NULL)
            , ('APPLICANT', 'New Listing Available', 'There are one or more new Housing Lottery Listings available now. Please visit the site to review and apply.', 'ON', 0, NULL, NULL)
            , ('APPLICANT', 'Successful Application Submission', 'You have submitted a new Housing Lottery Application - Reference [#APPLID].', 'ON', 0, NULL, NULL)
            , ('APPLICANT', 'Lottery or Waitlist Selection', 'Congratulations! Your application number [#APPLID] has been selected for further action. A representative will contact you shortly for next steps. Please make sure to collate and provide all required documents in a timely manner.', 'ON', 0, NULL, NULL)
            , ('APPLICANT', 'Potential Duplicate Housing Application Notification', 'Your application number [#APPLID] for Listing [#LISTID] - [#NAME], [#ADDRESS] submitted on [#SUBDATE] has been identified as a Potential Duplicate application, which means we believe one or more of your household members are included on another application for that same listing. Your household members may each only appear on one application for each listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('APPLICANT', 'Duplicate Housing Application Notification', 'Your application number [#APPLID] for Listing [#LISTID] - [#NAME], [#ADDRESS] submitted on [#SUBDATE] has been identified as a Duplicate application and removed from consideration. As a reminder, your household members may each only appear on one application for each listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Listing Ready for Review', 'Housing Lottery Listing #[#LISTID] for [#NAME], [#ADDRESS] was submitted by [#USERNAME] and is ready for review. Copy and paste the following URL into your browser window to review and publish the listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Listing Requires Revisions', 'Housing Lottery Listing #[#LISTID] for [#NAME], [#ADDRESS] has been sent back for revisions by [#USERNAME]. Reason: [#REASON]. Copy and paste the following URL into your browser window to review and publish the listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Listing Published', 'Housing Lottery Listing #[#LISTID] for [#NAME], [#ADDRESS] has been published by [#USERNAME]. Copy and paste the following URL into your browser window to view the published listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Listing Unpublished', 'Housing Lottery Listing #[#LISTID] for [#NAME], [#ADDRESS] has been unpublished for revisions by [#USERNAME]. Reason: [#REASON]. Copy and paste the following URL into your browser window to view the published listing.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Lottery Closing Deadline', 'Application deadline for Housing Lottery Listing - [#LISTID] for [#NAME], [#ADDRESS] - is [#APPLENDDATE]. Please update the deadline if more time is required.', 'BEFORE', 7, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Waitlist Closing Deadline', 'Waitlist deadline for Housing Lottery Listing - [#LISTID] for [#NAME], [#ADDRESS] - is [#WAITENDDATE]. Please update the deadline if more time is required.', 'BEFORE', 7, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Paper-based Housing Application Submitted', 'Paper-based Housing Application [#APPLID] for [#LISTID] - [#NAME], [#ADDRESS] has been submitted. Copy and paste the following URL into your browser window to view the submission details.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Potential Duplicate Paper-based Housing Application Notification', 'Housing Lottery Application [#APPLID] - for Listing [#LISTID] - [#NAME], [#ADDRESS] has been identified as a potential duplicate housing application due to the following:[#BR][#BR][#REASON][#P]Please reach out to [#APPLNAME] via email [#APPLEMAIL] or phone [#APPLPHONE] to resolve this matter on or before [#DUEDATE] for this application to be considered for further processing.[#P]If you are unable to resolve this matter by the due date indicated above, this application will automatically be withdrawn from consideration.', 'ON', 0, NULL, @InternalNotificationList)
            , ('INTERNAL', 'Duplicate Paper-based Housing Application Notification', 'Housing Lottery Application [#APPLID] - for Listing [#LISTID] - [#NAME], [#ADDRESS] - has been deemed a duplicate housing application due to the following:[#BR][#BR][#REASON][#P]Please reach out to [#APPLNAME] via email [#APPLEMAIL] or phone [#APPLPHONE] to let the applicant know of this decision.', 'ON', 0, NULL, @InternalNotificationList)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Notifications M
    WHERE M.CategoryCD = 'APPLICANT' AND M.Title NOT IN (SELECT Title FROM dbo.tblMasterNotifications WHERE CategoryCD = 'APPLICANT');
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Notifications M
    WHERE M.CategoryCD = 'INTERNAL' AND M.Title NOT IN (SELECT Title FROM dbo.tblMasterNotifications WHERE CategoryCD = 'INTERNAL');

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Notifications M
    JOIN dbo.tblMasterNotifications T ON T.CategoryCD = M.CategoryCD AND T.Title = M.Title
        AND (ISNULL(RTRIM(T.[Text]), '') <> ISNULL(RTRIM(M.[Text]), '')
            OR ISNULL(RTRIM(T.FrequencyCD), '') <> ISNULL(RTRIM(M.FrequencyCD), '')
            OR ISNULL(T.FrequencyInterval, 0) <> ISNULL(M.FrequencyInterval, 0)
            OR ISNULL(RTRIM(T.NotificationList), '') <> ISNULL(RTRIM(M.NotificationList), '')
        );

    -- Insert new entries
    INSERT INTO dbo.tblMasterNotifications (CategoryCD, Title, [Text], FrequencyCD, FrequencyInterval
                                                , NotificationList, InternalNotificationList
                                                , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.NotificationID, 'ADD', 'Added Notification'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.CategoryCD, M.Title, M.[Text], M.FrequencyCD, M.FrequencyInterval
            , M.NotificationList, M.InternalNotificationList
            , N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Notifications M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Text]                = M.[Text]
        , T.FrequencyCD         = M.FrequencyCD
        , T.FrequencyInterval   = M.FrequencyInterval
        , T.NotificationList    = M.NotificationList
        , T.InternalNotificationList = M.InternalNotificationList
        , T.ModifiedBy          = N'SYSTEM'
        , T.ModifiedDate        = GETDATE()
    OUTPUT INSERTED.NotificationID, 'UPDATE', 'Updated Notification'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterNotifications T
    JOIN @Notifications M ON M.Title = T.Title
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'NOTIFICATION', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO