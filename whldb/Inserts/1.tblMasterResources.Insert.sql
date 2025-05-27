SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Resources TABLE (
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](4000) NULL,
        [Url] [varchar](500) NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @Resources (Title, [Text], [Url], DisplayOrder)
        VALUES ('Exploring Westchester County (westchestergov.com)', NULL, 'https://gis.westchestergov.com/', 1)
            , ('Residential Tenants'' Rights Guide | New York State Attorney General (ny.gov)', 'In New York State, there are several different laws governing this relationship, which can vary depending on the county or town where you live. This booklet explains many of these laws you need to know and provides resources where you can find more information about landlord and tenant issues.', 'https://ag.ny.gov/publications/residential-tenants-rights-guide', 2)
            , ('Reports and Resources (westchestergov.com)', 'Westchester County publishes free informational booklets, available below, to help with a whole range of housing topics including the very fundamental, "A Roof Over Your Head" to well researched reports and plans.', 'https://homes.westchestergov.com/resources/', 3)
            , ('Westchester County 2024 Income & Rent Limits Program Guidelines (westchestergov.com)', 'The federal Housing and Urban Development Income Guidelines.  This will cross reference household size and income thresholds for home rent and purchase in Westchester County.', 'https://homes.westchestergov.com/images/stories/pdfs/24inclimguide53124.pdf', 4)
            , ('New York | HUD.gov / U.S. Department of Housing and Urban Development (HUD)', 'US Department of Housing and Urban Development NYS Resource Page.', 'https://www.hud.gov/states/new_york', 5)
            , ('Let''s Make Home the Goal | HUD.gov / U.S. Department of Housing and Urban Development (HUD)', 'The HUD Office of Housing Counselling provides resources for first time homebuyers.', 'https://www.hud.gov/makehomethegoal?gad_source=1&gclid=EAIaIQobChMInJHFpvC9iAMVxTEIBR2rDwYgEAAYASAAEgL28_D_BwE', 6)
            , ('A Roof over Your Head (westchestergov.com)', 'A Question and Answer Guide to the Westchester County Housing Market for Tenants, Landlords, Homeowners, Elderly and People with Disabilities.', 'https://planning.westchestergov.com/images/stories/pdfs/royh820.pdf', 7)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Resources M
    WHERE M.Title NOT IN (SELECT Title FROM dbo.tblMasterResources);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Resources M
    JOIN dbo.tblMasterResources T ON T.Title = M.Title
        AND (ISNULL(RTRIM(T.Text), '') <> ISNULL(RTRIM(M.Text), '')
            OR ISNULL(RTRIM(T.[Url]), '') <> ISNULL(RTRIM(M.[Url]), '')
            OR T.[DisplayOrder] <> M.[DisplayOrder]
        );

    -- Insert new entries
    INSERT INTO dbo.tblMasterResources (Title, [Text], [Url], DisplayOrder, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.ResourceID, 'ADD', 'Added Resource'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.Title, M.[Text], M.[Url], M.DisplayOrder, N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Resources M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Text]        = M.Text
        , T.[Url]       = M.[Url]
        , T.DisplayOrder = M.DisplayOrder
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.ResourceID, 'UPDATE', 'Updated Resource'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterResources T
    JOIN @Resources M ON M.Title = T.Title
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'RESOURCE', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO