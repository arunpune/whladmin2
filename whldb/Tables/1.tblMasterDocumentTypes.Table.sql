SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterDocumentTypes', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterDocumentTypes] (
        [DocumentTypeID] [int] IDENTITY(1,1) NOT NULL,
        [Name] [varchar](100) NOT NULL,
        [Description] [varchar](1000) NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterDocumentTypes] PRIMARY KEY CLUSTERED (
        	[DocumentTypeID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterDocumentTypes] ADD  CONSTRAINT [DF_tblMasterDocumentTypes_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterDocumentTypes] ADD  CONSTRAINT [DF_tblMasterDocumentTypes_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO