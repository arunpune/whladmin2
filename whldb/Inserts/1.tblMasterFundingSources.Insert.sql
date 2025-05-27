SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @FundingSources TABLE (
        [Name] [varchar](100) NOT NULL,
        [Description] [varchar](1000) NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @FundingSources ([Name], [Description])
        VALUES (N'HOME', NULL)
            , (N'NHLA', NULL)
            , (N'CDBG', NULL)
            , (N'LIHTC', NULL)
            , (N'NYS HFA (HCNP 101)', NULL)
            , (N'County Health Funds', NULL)
            , (N'NYS HHAP', NULL)
            , (N'FHLB', NULL)
            , (N'HOME (4 units only)', NULL)
            , (N'HUD Section 8', NULL)
            , (N'SONYMA', NULL)
            , (N'Refinanced through NYS', NULL)
            , (N'HUD Section 8 - New Construction', NULL)
            , (N'County HHAP', NULL)
            , (N'Freddie Mac', NULL)
            , (N'HUD Section 8 - Substantial Rehabilitation', NULL)
            , (N'Section 811', NULL)
            , (N'Section 8 New Construciton', NULL)
            , (N'MTV HOME', NULL)
            , (N'HUD Project-Based Section 8', NULL)
            , (N'HUD Section 11b', NULL)
            , (N'HUD Section 221 (d)(4)', NULL)
            , (N'PB section 8', NULL)
            , (N'LIHTC (NYS HCR)', NULL)
            , (N'Mitchell-Lama', NULL)
            , (N'HHAP', NULL)
            , (N'NYS HFA', NULL)
            , (N'LIHTC 4%', NULL)
            , (N'LIHTC 9%', NULL)
            , (N'NYS-3 (7) (105)', NULL)
            , (N'Housing Development Fund', NULL)
            , (N'PRAC/202', NULL)
            , (N'Rental Rehab', NULL)
            , (N'Urban Development Corporation', NULL)
            , (N'HUD Section 220 UR', NULL)
            , (N'LIHTC (4% HFA)', NULL)
            , (N'HUD Section 220', NULL)
            , (N'NYS HTF', NULL)
            , (N'Purchase Rehab', NULL)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @FundingSources M
    WHERE M.[Name] NOT IN (SELECT [Name] FROM dbo.tblMasterFundingSources);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @FundingSources M
    JOIN dbo.tblMasterFundingSources T ON T.[Name] = M.[Name] AND ISNULL(RTRIM(T.[Description]), '') <> ISNULL(RTRIM(M.[Description]), '');

    -- Insert new entries
    INSERT INTO dbo.tblMasterFundingSources ([Name], [Description], CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.FundingSourceID, 'ADD', 'Added Funding Source'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.[Name], M.[Description], N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @FundingSources M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.[Description] = M.[Description]
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.FundingSourceID, 'UPDATE', 'Updated Funding Source'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterFundingSources T
    JOIN @FundingSources M ON M.[Name] = T.[Name]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'FUNDSRC', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO