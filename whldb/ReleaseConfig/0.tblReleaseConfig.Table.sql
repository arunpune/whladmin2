SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblReleaseConfig', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblReleaseConfig] (
        [ReleaseVersion] [varchar](20) NOT NULL,
        [Timestamp] [datetime] NOT NULL
    ) ON [PRIMARY];

END
GO

SET NOCOUNT ON;
GO

DECLARE @ReleaseVersion VARCHAR(20);
SET @ReleaseVersion = REPLACE('$(VER)', '_', '.');

TRUNCATE TABLE [dbo].[tblReleaseConfig];
INSERT INTO [dbo].[tblReleaseConfig] (ReleaseVersion, [Timestamp])
    VALUES (@ReleaseVersion, GETDATE());
GO