SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @Quotes TABLE (
        [Text] [varchar](1000) NOT NULL,
        [DisplayOnHomePageInd] [bit] NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @Quotes ([Text], DisplayOnHomePageInd)
        VALUES ('For decades, Westchester County has been at the forefront of fair and affordable housing development. Westchester is one of the most diverse counties in New York State, which is one of our greatest strengths. Diversity enhances our quality of life every day by attracting talented people, stimulating creative thinking, and promoting tolerance and understanding.', 1)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @Quotes M
    WHERE M.[Text] NOT IN (SELECT [Text] FROM dbo.tblMasterQuotes);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @Quotes M
    JOIN dbo.tblMasterQuotes T ON T.[Text] = M.[Text]
        AND ISNULL(T.DisplayOnHomePageInd, 0) <> ISNULL(M.DisplayOnHomePageInd, 0);

    -- Insert new entries
    INSERT INTO dbo.tblMasterQuotes ([Text], DisplayOnHomePageInd, CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.QuoteID, 'ADD', 'Added Quote'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.[Text], M.DisplayOnHomePageInd, N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @Quotes M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.DisplayOnHomePageInd  = M.DisplayOnHomePageInd
        , T.ModifiedBy          = N'SYSTEM'
        , T.ModifiedDate        = GETDATE()
    OUTPUT INSERTED.QuoteID, 'UPDATE', 'Updated Quote'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterQuotes T
    JOIN @Quotes M ON M.[Text] = T.[Text]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'QUOTE', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO