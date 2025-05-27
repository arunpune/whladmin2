SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingUnitHouseholds', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingUnitHouseholds] (
        [UnitHouseholdID] [int] NOT NULL,
        [UnitID] [int] NOT NULL,
        [HouseholdSize] INT NOT NULL,
        [MinHouseholdIncomeAmt] [decimal](12, 2) NOT NULL,
        [MaxHouseholdIncomeAmt] [decimal](12, 2) NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingUnitHouseholds] PRIMARY KEY CLUSTERED (
        	[UnitHouseholdID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingUnitHouseholds] ADD  CONSTRAINT [DF_tblSiteListingUnitHouseholds_HouseholdSize]  DEFAULT ((1)) FOR [HouseholdSize];

    ALTER TABLE [dbo].[tblSiteListingUnitHouseholds] ADD  CONSTRAINT [DF_tblSiteListingUnitHouseholds_MinHouseholdIncomeAmt]  DEFAULT ((0)) FOR [MinHouseholdIncomeAmt];

    ALTER TABLE [dbo].[tblSiteListingUnitHouseholds] ADD  CONSTRAINT [DF_tblSiteListingUnitHouseholds_MaxHouseholdIncomeAmt]  DEFAULT ((100)) FOR [MaxHouseholdIncomeAmt];

    ALTER TABLE [dbo].[tblSiteListingUnitHouseholds] ADD  CONSTRAINT [DF_tblSiteListingUnitHouseholds_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingUnitHouseholds] ADD  CONSTRAINT [DF_tblSiteListingUnitHouseholds_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO