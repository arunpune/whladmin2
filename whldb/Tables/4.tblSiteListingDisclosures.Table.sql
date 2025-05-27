SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingDisclosures', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingDisclosures] (
        [DisclosureID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [Text] [varchar](1000) NOT NULL,
        [SortOrder] [int] NOT NULL,
        [UserAdded] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingDisclosures] PRIMARY KEY CLUSTERED (
            [DisclosureID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingDisclosures] ADD  CONSTRAINT [DF_tblSiteListingDisclosures_SortOrder]  DEFAULT ((0)) FOR [SortOrder];

    ALTER TABLE [dbo].[tblSiteListingDisclosures] ADD  CONSTRAINT [DF_tblSiteListingDisclosures_UserAdded]  DEFAULT ((1)) FOR [UserAdded];

    ALTER TABLE [dbo].[tblSiteListingDisclosures] ADD  CONSTRAINT [DF_tblSiteListingDisclosures_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingDisclosures] ADD  CONSTRAINT [DF_tblSiteListingDisclosures_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO