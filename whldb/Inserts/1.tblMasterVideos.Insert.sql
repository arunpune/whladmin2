SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Videos TABLE (
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](1000) NULL,
        [Url] [varchar](500) NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [DisplayOnHomePageInd] [bit] NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @Videos (Title, [Text], [Url], [DisplayOrder], DisplayOnHomePageInd)
        VALUES ('CE George Latimer on Building Affordable Homes', NULL, 'https://www.youtube.com/watch?v=puk_pORe-dc', 1, 0)
            , ('CE George Latimer Allocates Millions to Create Affordable Rental Units for Seniors', NULL, 'https://www.youtube.com/watch?v=n7KSAMmHmHQ', 2, 0)
            , ('Affordable Housing Highlights 2022', NULL, 'https://www.youtube.com/watch?v=_panTdui5LU', 3, 0)
            , ('CE George Latimer on Affordable Housing 2022', NULL, 'https://www.youtube.com/watch?v=tRqS7YoxF74', 4, 0)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Videos M
    WHERE M.Title NOT IN (SELECT Title FROM dbo.tblMasterVideos);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Videos M
    JOIN dbo.tblMasterVideos T ON T.Title = M.Title
        AND (ISNULL(RTRIM(T.[Text]), '') <> ISNULL(RTRIM(M.[Text]), '')
            OR ISNULL(RTRIM(T.[Url]), '') <> ISNULL(RTRIM(M.[Url]), '')
            OR ISNULL(T.DisplayOrder, 0) <> ISNULL(M.DisplayOrder, 0)
            OR ISNULL(T.DisplayOnHomePageInd, 0) <> ISNULL(M.DisplayOnHomePageInd, 0)
        );

    -- Insert new entries
    INSERT INTO dbo.tblMasterVideos (Title, [Text], [Url], DisplayOrder, DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.VideoID, 'ADD', 'Added Video'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.Title, M.[Text], M.[Url], M.DisplayOrder, M.DisplayOnHomePageInd, N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Videos M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Text]                    = M.Text
        , T.[Url]                   = M.[Url]
        , T.DisplayOrder            = M.DisplayOrder
        , T.DisplayOnHomePageInd    = M.DisplayOnHomePageInd
        , T.ModifiedBy              = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.VideoID, 'UPDATE', 'Updated Video'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterVideos T
    JOIN @Videos M ON M.Title = T.Title
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'VIDEO', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO