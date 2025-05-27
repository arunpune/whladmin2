SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterFAQs', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterFAQs] (
        [FAQID] [int] IDENTITY(1,1) NOT NULL,
        [CategoryName] [varchar](100) NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](4000) NOT NULL,
        [Url] [varchar](500) NULL,
        [Url1] [varchar](500) NULL,
        [Url2] [varchar](500) NULL,
        [Url3] [varchar](500) NULL,
        [Url4] [varchar](500) NULL,
        [Url5] [varchar](500) NULL,
        [Url6] [varchar](500) NULL,
        [Url7] [varchar](500) NULL,
        [Url8] [varchar](500) NULL,
        [Url9] [varchar](500) NULL,
        [DisplayOrder] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterFAQs] PRIMARY KEY CLUSTERED (
            [FAQID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterFAQs] ADD  CONSTRAINT [DF_tblMasterFAQs_CategoryName]  DEFAULT (('General')) FOR [CategoryName];

    ALTER TABLE [dbo].[tblMasterFAQs] ADD  CONSTRAINT [DF_tblMasterFAQs_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterFAQs] ADD  CONSTRAINT [DF_tblMasterFAQs_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO