SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHouseholds', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHouseholds] (
        [HouseholdID] [bigint] IDENTITY(1, 1) NOT NULL,
        [Username] [varchar](200) NOT NULL,
        [AddressInd] [bit] NOT NULL,
        [PhysicalStreetLine1] [varchar](100) NULL,
        [PhysicalStreetLine2] [varchar](100) NULL,
        [PhysicalStreetLine3] [varchar](100) NULL,
        [PhysicalCity] [varchar](100) NULL,
        [PhysicalStateCD] [varchar](2) NULL,
        [PhysicalZipCode] [varchar](5) NULL,
        [PhysicalCounty] [varchar](100) NULL,
        [DifferentMailingAddressInd] [bit] NOT NULL,
        [MailingStreetLine1] [varchar](100) NULL,
        [MailingStreetLine2] [varchar](100) NULL,
        [MailingStreetLine3] [varchar](100) NULL,
        [MailingCity] [varchar](100) NULL,
        [MailingStateCD] [varchar](2) NULL,
        [MailingZipCode] [varchar](5) NULL,
        [MailingCounty] [varchar](100) NULL,
        [VoucherInd] [bit] NOT NULL,
        [VoucherCDs] [varchar](1000) NULL,
        [VoucherOther] [varchar](1000) NULL,
        [VoucherAdminName] [varchar](200) NULL,
        [LiveInAideInd] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHouseholds] PRIMARY KEY CLUSTERED (
            [HouseholdID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_AddressInd]  DEFAULT ((0)) FOR [AddressInd];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_DifferentMailingAddressInd]  DEFAULT ((0)) FOR [DifferentMailingAddressInd];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_VoucherInd]  DEFAULT ((0)) FOR [VoucherInd];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_LiveInAideInd]  DEFAULT ((0)) FOR [LiveInAideInd];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHouseholds] ADD  CONSTRAINT [DF_tblHouseholds_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO