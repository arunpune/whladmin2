SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterConfig', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterConfig] (
        [ConfigID] [int] NOT NULL,
        [Category] [varchar](20) NOT NULL,
        [SubCategory] [varchar](20) NOT NULL,
        [ConfigKey] [varchar](100) NOT NULL,
        [ConfigValue] [varchar](max) NOT NULL,
        [Active] [bit] NOT NULL,
        CONSTRAINT [PK_tblMasterConfig] PRIMARY KEY CLUSTERED (
            [ConfigID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterConfig] ADD  CONSTRAINT [DF_tblMasterConfig_Active]  DEFAULT ((1)) FOR [Active];

END
GO