SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteUserDocuments', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteUserDocuments] (
        [DocID] [bigint] NOT NULL IDENTITY(1, 1),
        [Username] [varchar](200) NOT NULL,
        [DocTypeID] [int] NOT NULL,
        [DocName] [varchar](250) NULL,
        [FileName] [varchar](250) NOT NULL,
        [MimeType] [varchar](50) NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteUserDocuments] PRIMARY KEY CLUSTERED (
            [DocID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteUserDocuments] ADD  CONSTRAINT [DF_tblSiteUserDocuments_MimeType]  DEFAULT ('application/octet-stream') FOR [MimeType];

    ALTER TABLE [dbo].[tblSiteUserDocuments] ADD  CONSTRAINT [DF_tblSiteUserDocuments_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteUserDocuments] ADD  CONSTRAINT [DF_tblSiteUserDocuments_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO