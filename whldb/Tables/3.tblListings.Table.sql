SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblListings', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblListings] (
        [ListingID] [int] NOT NULL,
        [ListingTypeCD] [varchar](20) NOT NULL,
        [ResaleInd] [bit] NOT NULL,
        [ListingAgeTypeCD] [varchar](20) NOT NULL,
        [Name] [varchar](500) NOT NULL,
        [Description] [varchar](4000) NULL,
        [StreetLine1] [varchar](250) NOT NULL,
        [StreetLine2] [varchar](250) NULL,
        [StreetLine3] [varchar](250) NULL,
        [City] [varchar](100) NOT NULL,
        [StateCD] [varchar](2) NOT NULL,
        [ZipCode] [varchar](9) NOT NULL,
        [County] [varchar](100) NOT NULL,
        [Municipality] [varchar](250) NULL,
        [MunicipalityUrl] [varchar](500) NULL,
        [SchoolDistrict] [varchar](250) NULL,
        [SchoolDistrictUrl] [varchar](500) NULL,
        [MapUrl] [varchar](500) NULL,
        [WebsiteUrl] [varchar](500) NULL,
        [EsriX] [varchar](20) NULL,
        [EsriY] [varchar](20) NULL,
        [ListingStartDate] [datetime] NULL,
        [ListingEndDate] [datetime] NULL,
        [ApplicationStartDate] [datetime] NULL,
        [ApplicationEndDate] [datetime] NULL,
        [LotteryEligible] [bit] NOT NULL,
        [LotteryDate] [datetime] NULL,
        [WaitlistEligible] [bit] NOT NULL,
        [WaitlistStartDate] [datetime] NULL,
        [WaitlistEndDate] [datetime] NULL,
        [MinHouseholdIncomeAmt] [decimal](12, 0) NOT NULL,
        [MaxHouseholdIncomeAmt] [decimal](12, 0) NOT NULL,
        [MinHouseholdSize] [int] NOT NULL,
        [MaxHouseholdSize] [int] NOT NULL,
        [PetsAllowedInd] [bit] NOT NULL,
        [PetsAllowedText] [varchar](1000) NULL,
        [RentIncludesText] [varchar](1000) NULL,
        [CompletedOrInitialOccupancyYear] [varchar](4) NULL,
        [TermOfAffordability] [varchar](100) NULL,
        [MarketingAgentInd] [bit] NOT NULL,
        [MarketingAgentID] [int] NULL,
        [MarketingAgentApplicationLink] [varchar](500) NULL,
        [StatusCD] [varchar](20) NOT NULL,
        [VersionNo] [int] NOT NULL,
        [LotteryID] [int] NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblListings] PRIMARY KEY CLUSTERED (
        	[ListingID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_ResaleInd]  DEFAULT ((0)) FOR [ResaleInd];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_ListingAgeTypeCD]  DEFAULT (('ALL')) FOR [ListingAgeTypeCD];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_LotteryEligible]  DEFAULT ((0)) FOR [LotteryEligible];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_WaitlistEligible]  DEFAULT ((0)) FOR [WaitlistEligible];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_MinHouseholdIncomeAmt]  DEFAULT ((0)) FOR [MinHouseholdIncomeAmt];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_MaxHouseholdIncomeAmt]  DEFAULT ((0)) FOR [MaxHouseholdIncomeAmt];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_MinHouseholdSize]  DEFAULT ((0)) FOR [MinHouseholdSize];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_MaxHouseholdSize]  DEFAULT ((0)) FOR [MaxHouseholdSize];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_PetsAllowedInd]  DEFAULT ((1)) FOR [PetsAllowedInd];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_MarketingAgentInd]  DEFAULT ((0)) FOR [MarketingAgentInd];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_StatusCD]  DEFAULT ('DRAFT') FOR [StatusCD];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_VersionNo]  DEFAULT ((1)) FOR [VersionNo];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO

-- ResaleInd
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblListings' AND [column_name] = 'ResaleInd')
    ALTER TABLE [dbo].[tblListings] ADD [ResaleInd] [bit] NULL;
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblListings' AND [column_name] = 'ResaleInd' AND [is_nullable] = 'YES')
BEGIN
    UPDATE [dbo].[tblListings] SET [ResaleInd] = 0 WHERE [ResaleInd] IS NULL;
    ALTER TABLE [dbo].[tblListings] ALTER COLUMN [ResaleInd] [bit] NOT NULL;
END
GO

-- DF_tblListings_ResaleInd
IF OBJECT_ID('DF_tblListings_ResaleInd', 'D') IS NULL
    ALTER TABLE [dbo].[tblListings] ADD  CONSTRAINT [DF_tblListings_ResaleInd]  DEFAULT ((0)) FOR [ResaleInd];
GO
