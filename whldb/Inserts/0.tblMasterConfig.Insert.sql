SET NOCOUNT ON;
GO

DECLARE @MasterConfig TABLE (
    [ConfigID] [int] NOT NULL,
    [Category] [varchar](20) NOT NULL,
    [SubCategory] [varchar](20) NOT NULL,
    [ConfigKey] [varchar](100) NOT NULL,
    [ConfigValue] [varchar](max) NOT NULL,
    [Active] [bit] NOT NULL,
    [ChangeType] [char](1) NOT NULL DEFAULT ('-')
);

-- Lottery Settings
INSERT INTO @MasterConfig (ConfigID, Category, SubCategory, ConfigKey, ConfigValue, Active)
    VALUES (1001, 'LOTTERY', 'DEFAULT', 'RUNMODE', 'MANUAL', 1)
        , (1002, 'LOTTERY', 'DEFAULT', 'REVIEWMODE', 'MANUAL', 1)
        , (2001, 'ESRIAPI', 'DEFAULT', 'ENABLED', 'YES', 1)
        , (2002, 'ESRIAPI', 'DEFAULT', 'APIURL', 'https://geocode.arcgis.com/arcgis/rest/services/World/GeocodeServer', 1)
        , (2003, 'ESRIAPI', 'DEFAULT', 'APIKEY', 'AAPTxy8BH1VEsoebNVZXo8HurGdaik2el18NEKTjnLHO6ByvM1PxVuHzA4EuGdFwsMknzjNB_3d43oMCYzv4nIpyvvRR8TaP-WDdg8lTN_ndjIvdavW2mjFNno5yKHxqQdlANI79wyf8Gh8udCR8u4PIIUsUJdc11VCdYTiIWqNqjmtMbeYM3b5tq_HlpQupRVt6EBrLPWX3sKvNqLNQBEZFOfARMmu0e8iBuIclVEfsuSE.AT1_F9fsodNZ', 1)
        , (2004, 'ESRIAPI', 'DEFAULT', 'APIKEYEXPIRY', '2026-02-05', 1)
        , (2005, 'ESRIAPI', 'DEFAULT', 'METHOD', 'findAddressCandidates', 1)
        , (3001, 'EMFLUENCE', 'DEFAULT', 'ENABLED', 'YES', 1)
        , (3002, 'EMFLUENCE', 'DEFAULT', 'APIURL', 'https://api.emailer.emfluence.com/v1', 1)
        , (3003, 'EMFLUENCE', 'DEFAULT', 'APIKEY', '1024E3DB-374C-4DEB-916B-BDC9F5EC68AA', 1)
        , (3004, 'EMFLUENCE', 'DEFAULT', 'DOMAIN', 'list.westchestergov.com', 1)
        , (3101, 'EMFLUENCE', 'DEFAULT', 'RENTALGROUPID', '824314', 1)
        , (3102, 'EMFLUENCE', 'DEFAULT', 'RENTALGROUPNAME', 'Homeseeker - Rental - Test', 1)
        , (3103, 'EMFLUENCE', 'DEFAULT', 'RENTALTEMPLATEID', '22709', 1)
        , (3104, 'EMFLUENCE', 'DEFAULT', 'RENTALTEMPLATENAME', 'Homeseeker Rental Test', 1)
        , (3901, 'EMFLUENCE', 'DEFAULT', 'CUSTOMFIELD1NAME', 'Listing Link', 1)
        , (3902, 'EMFLUENCE', 'DEFAULT', 'CUSTOMFIELD1ATTRIBUTE', '$$custom01', 1)
    ;

-- Mark new entries
UPDATE M
SET M.ChangeType = 'A'
FROM @MasterConfig M
WHERE M.ConfigID NOT IN (SELECT ConfigID FROM dbo.tblMasterConfig);

-- Mark updates to existing entries
UPDATE M
SET M.ChangeType = 'U'
FROM @MasterConfig M
JOIN dbo.tblMasterConfig T ON T.ConfigID = M.ConfigID;

-- Insert new entries
INSERT INTO dbo.tblMasterConfig (ConfigID, Category, SubCategory, ConfigKey, ConfigValue, Active)
    SELECT M.ConfigID, M.Category, M.SubCategory, M.ConfigKey, M.ConfigValue, M.Active
    FROM @MasterConfig M
    WHERE M.ChangeType = 'A';

-- Update existing entries
UPDATE T
SET T.Category = M.Category, T.SubCategory = M.SubCategory, T.ConfigKey = M.ConfigKey, T.ConfigValue = M.ConfigValue, T.Active = M.Active
FROM dbo.tblMasterConfig T
JOIN @MasterConfig M ON M.ConfigID = T.ConfigID
WHERE M.ChangeType = 'U';

-- Delete old entries
DELETE M
FROM dbo.tblMasterConfig M
WHERE M.ConfigID NOT IN (SELECT ConfigID FROM @MasterConfig);

GO