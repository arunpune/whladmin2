SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @DocumentTypes TABLE (
        [Name] [varchar](100) NOT NULL,
        [Description] [varchar](1000) NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @DocumentTypes ([Name], [Description])
        VALUES (N'Alimony/Child Support Document', NULL)
            , (N'Asset Account Statement (bank, investment, etc.)', NULL)
            , (N'Birth Certificate', NULL)
            , (N'Government ID', NULL)
            , (N'Paystub', NULL)
            , (N'Pension/Retirement Income', NULL)
            , (N'Retirement Fund Statements', NULL)
            , (N'Social Security Income', NULL)
            , (N'Tax Return', NULL)
            , (N'W-2 or 1099', NULL)
            , (N'Other Income or Asset Document', NULL)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @DocumentTypes M
    WHERE M.[Name] NOT IN (SELECT [Name] FROM dbo.tblMasterDocumentTypes);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @DocumentTypes M
    JOIN dbo.tblMasterDocumentTypes T ON T.[Name] = M.[Name] AND ISNULL(RTRIM(T.[Description]), '') <> ISNULL(RTRIM(M.[Description]), '');

    -- Insert new entries
    INSERT INTO dbo.tblMasterDocumentTypes ([Name], [Description], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.DocumentTypeID, 'ADD', 'Added Document Type'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.[Name], M.[Description], N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @DocumentTypes M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Description] = M.[Description]
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.DocumentTypeID, 'UPDATE', 'Updated Document Type'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterDocumentTypes T
    JOIN @DocumentTypes M ON M.[Name] = T.[Name]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'DOCTYPE', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO