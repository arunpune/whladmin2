SET NOCOUNT ON;
GO

DECLARE @Env VARCHAR(20);
SET @Env = '$(ENV)';

DECLARE @AdminUsers TABLE (
    [UserID] [varchar](4) NOT NULL,
    [EmailAddress] [varchar](200) NOT NULL,
    [DisplayName] [varchar](200) NOT NULL,
    [OrganizationCD] [varchar](20) NOT NULL,
    [RoleCD] [varchar](20) NOT NULL,
    [ChangeType] [char](1) NOT NULL DEFAULT ('-')
);

DECLARE @Audit TABLE (
    [EntityID] [varchar](200) NOT NULL,
    [ActionCD] [varchar](20) NOT NULL,
    [Note] [varchar](max) NOT NULL
);

IF @Env = 'DEV'
BEGIN

    INSERT INTO @AdminUsers (UserID, EmailAddress, DisplayName, OrganizationCD, RoleCD)
        VALUES ('raqk', 'raja.kondi@gmail.com', 'Raja Kondi', 'DOP', 'LOTADMIN')
            , ('raqm', 'rakesh.menon@prutech.com', 'Rakesh Menon', 'DOP', 'SYSADMIN')
    ;

END
ELSE IF @Env = 'TEST'
BEGIN

    INSERT INTO @AdminUsers (UserID, EmailAddress, DisplayName, OrganizationCD, RoleCD)
        VALUES ('ajp2', 'ajp2@westchestercountyny.gov', 'Aji Palappillil', 'DOP', 'OPSVIEWER')
            , ('anxs', 'asmith@wroinc.org', 'Andrew Smith', 'WRO', 'LOTADMIN')
            , ('gqs2', 'gqs2@westchestercountyny.gov', 'Girija Kaimal', 'DOP', 'SYSADMIN')
            , ('jvj9', 'jvj9@westchestercountyny.gov', 'Jacob Joseph Vysiantodom', 'DOP', 'OPSVIEWER')
            , ('jee2', 'jee2@westchestercountyny.gov', 'John Estrow', 'DOP', 'SYSADMIN')
            , ('jmlh', 'jmlh@westchestercountyny.gov', 'John Hofstetter', 'DOP', 'SYSADMIN')
            , ('tifa', 'tifa@westchestercountyny.gov', 'Theresa Fleischman', 'DOP', 'SYSADMIN')
            , ('raqk', 'raja.kondi@prutech.com', 'Raja Kondi', 'DOP', 'LOTADMIN')
            , ('raqm', 'rakesh.menon@prutech.com', 'Rakesh Menon', 'DOP', 'SYSADMIN')
    ;

END

-- Mark new entries
UPDATE M
SET M.ChangeType = 'A'
FROM @AdminUsers M
WHERE M.UserID NOT IN (SELECT UserID FROM dbo.tblAdminUsers);

-- Mark updates to existing entries
UPDATE M
SET M.ChangeType = 'U'
FROM @AdminUsers M
JOIN dbo.tblAdminUsers T ON T.UserID = M.UserID
    AND (ISNULL(RTRIM(T.EmailAddress), '') <> ISNULL(RTRIM(M.EmailAddress), '')
        OR ISNULL(RTRIM(T.DisplayName), '') <> ISNULL(RTRIM(M.DisplayName), '')
        OR ISNULL(RTRIM(T.OrganizationCD), '') <> ISNULL(RTRIM(M.OrganizationCD), '')
        OR ISNULL(RTRIM(T.RoleCD), '') <> ISNULL(RTRIM(M.RoleCD), '')
    );

-- Insert new entries
INSERT INTO dbo.tblAdminUsers (UserID, EmailAddress, DisplayName, OrganizationCD, RoleCD
                                , CreatedBy, CreatedDate, ModifiedBy, ModifiedDate, Active)
    OUTPUT INSERTED.UserID, 'ADD', 'Added Admin User'
        INTO @Audit (EntityID, ActionCD, Note)
    SELECT M.UserID, M.EmailAddress, M.DisplayName, M.OrganizationCD, RoleCD
        , N'SYSTEM', GETDATE(), N'SYSTEM', GETDATE(), 1
    FROM @AdminUsers M
    WHERE M.ChangeType = 'A';

-- Update existing entries
UPDATE T
SET
    T.EmailAddress      = M.EmailAddress
    , T.DisplayName     = M.DisplayName
    , T.OrganizationCD  = M.OrganizationCD
    , T.RoleCD          = M.RoleCD
    , T.ModifiedBy      = N'SYSTEM'
    , T.ModifiedDate    = GETDATE()
OUTPUT INSERTED.UserID, 'UPDATE', 'Updated Admin User'
    INTO @Audit (EntityID, ActionCD, Note)
FROM dbo.tblAdminUsers T
JOIN @AdminUsers M ON M.UserID = T.UserID
WHERE M.ChangeType = 'U';

INSERT INTO dbo.tblAudit (EntityTypeCD, EntityID, Username, ActionCD, Note, [Timestamp])
    SELECT 'USER', A.EntityID, 'SYSTEM', A.ActionCD, A.Note, GETDATE()
    FROM @Audit A;
GO