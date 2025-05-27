SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHousingApplications', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHousingApplications] (
        [ApplicationID] [bigint] NOT NULL,
        [ListingID] [int] NOT NULL,
        [Username] [varchar](200) NULL,
        [HouseholdID] [bigint] NOT NULL,
        [UnitTypeCDs] [varchar](200) NULL,
        [CoApplicantMemberID] [bigint] NULL,
        [MemberIDs] [varchar](1000) NULL,
        [AccountIDs] [varchar](1000) NULL,
        [AccessibilityCDs] [varchar](1000) NULL,
        [StatusCD] [varchar](20) NOT NULL,
        [OriginalStatusCD] [varchar](20) NULL,
        [SubmittedDate] [datetime] NULL,
        [ReceivedDate] [datetime] NULL,
        [WithdrawnDate] [datetime] NULL,
        [LotteryID] [int] NULL,
        [LotteryDate] [datetime] NULL,
        [LotteryNumber] [varchar](20) NULL,
        [DuplicateCheckCD] [char](1) NOT NULL,
        [DuplicateCheckResponseDueDate] [datetime] NULL,
        [DisqualifiedInd] [bit] NOT NULL,
        [DisqualificationCD] [varchar](20) NULL,
        [DisqualificationOther] [varchar](500) NULL,
        [DisqualificationReason] [varchar](500) NULL,
        [Title] [varchar](10) NULL,
        [FirstName] [varchar](100) NOT NULL,
        [MiddleName] [varchar](100) NULL,
        [LastName] [varchar](100) NOT NULL,
        [Suffix] [varchar](10) NULL,
        [DateOfBirth] [datetime] NULL,
        [Last4SSN] [char](4) NULL,
        [IDTypeCD] [varchar](20) NULL,
        [IDTypeValue] [varchar](20) NULL,
        [IDIssueDate] [datetime] NULL,
        [GenderCD] [varchar](20) NOT NULL,
        [RaceCD] [varchar](20) NOT NULL,
        [EthnicityCD] [varchar](20) NOT NULL,
        [Pronouns] [varchar](20) NULL,
        [StudentInd] [bit] NOT NULL,
        [DisabilityInd] [bit] NOT NULL,
        [VeteranInd] [bit] NOT NULL,
        [PhoneNumberTypeCD] [varchar](20) NULL,
        [PhoneNumber] [varchar](10) NULL,
        [PhoneNumberExtn] [varchar](10) NULL,
        [AltPhoneNumberTypeCD] [varchar](20) NULL,
        [AltPhoneNumber] [varchar](10) NULL,
        [AltPhoneNumberExtn] [varchar](10) NULL,
        [EmailAddress] [varchar](200) NULL,
        [AltEmailAddress] [varchar](200) NULL,
        [EverLivedInWestchesterInd] [bit] NOT NULL,
        [CountyLivingIn] [varchar](100) NULL,
        [CurrentlyWorkingInWestchesterInd] [bit] NOT NULL,
        [CountyWorkingIn] [varchar](100) NULL,
        [HouseholdSize] [int] NOT NULL,
        [OwnRealEstateInd] [bit] NOT NULL,
        [RealEstateValueAmt] [decimal](17, 2) NULL,
        [AssetValueAmt] [decimal](17, 2) NULL,
        [IncomeValueAmt] [decimal](17, 2) NULL,
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
        [LeadTypeCD] [varchar](20) NOT NULL,
        [LeadTypeOther] [varchar](500) NULL,
        [SubmissionTypeCD] [varchar](20) NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHousingApplications] PRIMARY KEY CLUSTERED (
            [ApplicationID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_DuplicateCheckCD]  DEFAULT (('-')) FOR [DuplicateCheckCD];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_DisqualifiedInd]  DEFAULT ((0)) FOR [DisqualifiedInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_StudentInd]  DEFAULT ((0)) FOR [StudentInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_DisabilityInd]  DEFAULT ((0)) FOR [DisabilityInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_VeteranInd]  DEFAULT ((0)) FOR [VeteranInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_EverLivedInWestchesterInd]  DEFAULT ((0)) FOR [EverLivedInWestchesterInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_CurrentlyWorkingInWestchesterInd]  DEFAULT ((0)) FOR [CurrentlyWorkingInWestchesterInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_OwnRealEstateInd]  DEFAULT ((0)) FOR [OwnRealEstateInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_AddressInd]  DEFAULT ((0)) FOR [AddressInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_DifferentMailingAddressInd]  DEFAULT ((0)) FOR [DifferentMailingAddressInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_VoucherInd]  DEFAULT ((0)) FOR [VoucherInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_LiveInAideInd]  DEFAULT ((0)) FOR [LiveInAideInd];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_StatusCD]  DEFAULT ('DRAFT') FOR [StatusCD];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_SubmissionTypeCD]  DEFAULT ('') FOR [SubmissionTypeCD];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHousingApplications] ADD  CONSTRAINT [DF_tblHousingApplications_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO