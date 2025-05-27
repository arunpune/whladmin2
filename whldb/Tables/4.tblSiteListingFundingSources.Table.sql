SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingFundingSources', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingFundingSources] (
        [ListingID] [int] NOT NULL,
        [FundingSourceID] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingFundingSources] PRIMARY KEY CLUSTERED (
        	[ListingID] ASC,
            [FundingSourceID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingFundingSources] ADD  CONSTRAINT [DF_tblSiteListingFundingSources_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingFundingSources] ADD  CONSTRAINT [DF_tblSiteListingFundingSources_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO