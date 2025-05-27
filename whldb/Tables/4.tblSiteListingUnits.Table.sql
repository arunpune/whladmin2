SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingUnits', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingUnits] (
        [UnitID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [UnitTypeCD] [varchar](20) NULL,
        [BedroomCnt] [int] NOT NULL,
        [BathroomCnt] [int] NOT NULL,
        [BathroomCntPart] [int] NOT NULL,
        [SquareFootage] [int] NOT NULL,
        [AreaMedianIncomePct] [int] NOT NULL,
        [MonthlyRentAmt] [decimal](7, 2) NOT NULL,
        [AssetLimitAmt] [decimal](12, 2) NOT NULL,
        [EstimatedPriceAmt] [decimal](12, 2) NOT NULL,
        [SubsidyAmt] [decimal](12, 2) NOT NULL,
        [MonthlyTaxesAmt] [decimal](7, 2) NOT NULL,
        [MonthlyMaintenanceAmt] [decimal](7, 2) NOT NULL,
        [MonthlyInsuranceAmt] [decimal](12, 2) NOT NULL,
        [UnitsAvailableCnt] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingUnits] PRIMARY KEY CLUSTERED (
        	[UnitID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_BedroomCnt]  DEFAULT ((0)) FOR [BedroomCnt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_BathroomCnt]  DEFAULT ((0)) FOR [BathroomCnt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_BathroomCntPart]  DEFAULT ((0)) FOR [BathroomCntPart];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_AreaMedianIncomePct]  DEFAULT ((100)) FOR [AreaMedianIncomePct];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_MonthlyRentAmt]  DEFAULT ((0)) FOR [MonthlyRentAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_AssetLimitAmt]  DEFAULT ((0)) FOR [AssetLimitAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_EstimatedPriceAmt]  DEFAULT ((0)) FOR [EstimatedPriceAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_SubsidyAmt]  DEFAULT ((0)) FOR [SubsidyAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_MonthlyTaxesAmt]  DEFAULT ((0)) FOR [MonthlyTaxesAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_MonthlyMaintenanceAmt]  DEFAULT ((0)) FOR [MonthlyMaintenanceAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_MonthlyInsuranceAmt]  DEFAULT ((0)) FOR [MonthlyInsuranceAmt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_UnitsAvailableCnt]  DEFAULT ((0)) FOR [UnitsAvailableCnt];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingUnits] ADD  CONSTRAINT [DF_tblSiteListingUnits_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO