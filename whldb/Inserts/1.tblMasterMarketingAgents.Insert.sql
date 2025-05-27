SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @MarketingAgents TABLE (
        [Name] [varchar](200) NOT NULL,
        [ContactName] [varchar](200) NULL,
        [PhoneNumber] [varchar](20) NULL,
        [EmailAddress] [varchar](200) NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @MarketingAgents ([Name], [ContactName], [PhoneNumber], [EmailAddress])
        VALUES (N'Marketing Agent 1', NULL, NULL, NULL)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @MarketingAgents M
    WHERE M.[Name] NOT IN (SELECT [Name] FROM dbo.tblMasterMarketingAgents);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @MarketingAgents M
    JOIN dbo.tblMasterMarketingAgents T ON T.[Name] = M.[Name];

    -- Insert new entries
    INSERT INTO dbo.tblMasterMarketingAgents ([Name], [ContactName], [PhoneNumber], [EmailAddress]
                                                , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.AgentID, 'ADD', 'Added Marketing Agent'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.[Name], M.[ContactName], M.[PhoneNumber], M.[EmailAddress]
            , N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @MarketingAgents M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[ContactName]     = M.[ContactName]
        , T.[PhoneNumber]   = M.[PhoneNumber]
        , T.[EmailAddress]  = M.[EmailAddress]
        , T.ModifiedBy      = N'SYSTEM'
        , T.ModifiedDate    = GETDATE()
    OUTPUT INSERTED.AgentID, 'UPDATE', 'Updated Marketing Agent'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterMarketingAgents T
    JOIN @MarketingAgents M ON M.[Name] = T.[Name]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'MKTAGENT', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO