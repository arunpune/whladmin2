SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterResources', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterResources] (
        [ResourceID] [int] IDENTITY(1,1) NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](4000) NULL,
        [Url] [varchar](500) NOT NULL,
        [DisplayOrder] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterResources] PRIMARY KEY CLUSTERED (
            [ResourceID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterResources] ADD  CONSTRAINT [DF_tblMasterResources_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterResources] ADD  CONSTRAINT [DF_tblMasterResources_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO