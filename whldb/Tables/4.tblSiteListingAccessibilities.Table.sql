SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingAccessibilities', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingAccessibilities] (
        [ListingID] [int] NOT NULL,
        [AccessibilityCD] [varchar](20) NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingAccessibilities] PRIMARY KEY CLUSTERED (
        	[ListingID] ASC,
            [AccessibilityCD] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingAccessibilities] ADD  CONSTRAINT [DF_tblSiteListingAccessibilities_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingAccessibilities] ADD  CONSTRAINT [DF_tblSiteListingAccessibilities_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO