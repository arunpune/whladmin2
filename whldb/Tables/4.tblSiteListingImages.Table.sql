SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingImages', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingImages] (
        [ImageID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [ThumbnailContents] [varchar](MAX) NULL,
        [MimeType] [varchar](30) NOT NULL,
        [Contents] [varchar](MAX) NOT NULL,
        [IsPrimary] [bit] NOT NULL,
        [DisplayOnListingsPageInd] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingImages] PRIMARY KEY CLUSTERED (
        	[ImageID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingImages] ADD  CONSTRAINT [DF_tblSiteListingImages_IsPrimary]  DEFAULT ((0)) FOR [IsPrimary];

    ALTER TABLE [dbo].[tblSiteListingImages] ADD  CONSTRAINT [DF_tblSiteListingImages_DisplayOnListingsPageInd]  DEFAULT ((1)) FOR [DisplayOnListingsPageInd];

    ALTER TABLE [dbo].[tblSiteListingImages] ADD  CONSTRAINT [DF_tblSiteListingImages_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingImages] ADD  CONSTRAINT [DF_tblSiteListingImages_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO