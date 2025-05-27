SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingDocuments', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingDocuments] (
        [DocumentID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [FileName] [varchar](250) NOT NULL,
        [MimeType] [varchar](30) NOT NULL,
        [Contents] [varchar](MAX) NOT NULL,
        [DisplayOnListingsPageInd] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingDocuments] PRIMARY KEY CLUSTERED (
        	[DocumentID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingDocuments] ADD  CONSTRAINT [DF_tblSiteListingDocuments_DisplayOnListingsPageInd]  DEFAULT ((1)) FOR [DisplayOnListingsPageInd];

    ALTER TABLE [dbo].[tblSiteListingDocuments] ADD  CONSTRAINT [DF_tblSiteListingDocuments_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingDocuments] ADD  CONSTRAINT [DF_tblSiteListingDocuments_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO