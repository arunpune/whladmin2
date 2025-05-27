SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

IF @Env = 'DEV'
BEGIN

    DECLARE @AMIs TABLE (
        [EffectiveDate] [int] NOT NULL,
        [EffectiveYear] [int] NOT NULL,
        [IncomeAmt] [int] NOT NULL,
        [HH1] [int] NOT NULL,
        [HH2] [int] NOT NULL,
        [HH3] [int] NOT NULL,
        [HH4] [int] NOT NULL,
        [HH5] [int] NOT NULL,
        [HH6] [int] NOT NULL,
        [HH7] [int] NOT NULL,
        [HH8] [int] NOT NULL,
        [HH9] [int] NOT NULL,
        [HH10] [int] NOT NULL,
        [ChangeType] [char](1) NOT NULL DEFAULT ('-')
    );

    DECLARE @Audit TABLE (
        [EntityID] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL
    );

    INSERT INTO @AMIs (EffectiveDate, EffectiveYear, IncomeAmt, HH1, HH2, HH3, HH4, HH5, HH6, HH7, HH8, HH9, HH10)
        VALUES (20240401, 2024, 156200, 70, 80, 90, 100, 108, 116, 124, 132, 140, 148)
        ;

    -- Mark new entries
    UPDATE M
    SET M.ChangeType = 'A'
    FROM @AMIs M
    WHERE M.[EffectiveDate] NOT IN (SELECT [EffectiveDate] FROM dbo.tblMasterAMIs);

    -- Mark updates to existing entries
    UPDATE M
    SET M.ChangeType = 'U'
    FROM @AMIs M
    JOIN dbo.tblMasterAMIs T ON T.[EffectiveDate] = M.[EffectiveDate];

    -- Insert new entries
    INSERT INTO dbo.tblMasterAMIs (EffectiveDate, EffectiveYear, IncomeAmt
                                    , [HH1], [HH2], [HH3], [HH4]
                                    , [HH5], [HH6], [HH7], [HH8]
                                    , [HH9], [HH10]
                                    , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
        OUTPUT INSERTED.EffectiveDate, 'ADD', 'Added AMI'
            INTO @Audit (EntityID, ActionCD, Note)
        SELECT M.EffectiveDate, M.EffectiveYear, M.IncomeAmt
            , M.[HH1], M.[HH2], M.[HH3], M.[HH4]
            , M.[HH5], M.[HH6], M.[HH7], M.[HH8]
            , M.[HH9], M.[HH10]
            , N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
        FROM @AMIs M
        WHERE M.ChangeType = 'A';

    -- Update existing entries
    UPDATE T
    SET
        T.EffectiveYear	= M.EffectiveYear
        , T.IncomeAmt	= M.IncomeAmt
        , T.[HH1]		= M.HH1
        , T.[HH2]		= M.HH2
        , T.[HH3]		= M.HH3
        , T.[HH4]		= M.HH4
        , T.[HH5]		= M.HH5
        , T.[HH6]		= M.HH6
        , T.[HH7]		= M.HH7
        , T.[HH8]		= M.HH8
        , T.[HH9]		= M.HH9
        , T.[HH10]		= M.HH10
        , T.ModifiedBy  = N'SYSTEM'
        , T.ModifiedDate = GETDATE()
    OUTPUT INSERTED.EffectiveDate, 'UPDATE', 'Updated AMI'
        INTO @Audit (EntityID, ActionCD, Note)
    FROM dbo.tblMasterAMIs T
    JOIN @AMIs M ON M.[EffectiveDate] = T.[EffectiveDate]
    WHERE M.ChangeType = 'U';

    INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
        SELECT 'AMI', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
        FROM @Audit A;

END
GO