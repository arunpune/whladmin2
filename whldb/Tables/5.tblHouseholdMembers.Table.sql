SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHouseholdMembers', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHouseholdMembers] (
        [MemberID] [bigint] IDENTITY(1, 1) NOT NULL,
        [HouseholdID] [bigint] NOT NULL,
        [RelationTypeCD] [varchar](20) NOT NULL,
        [RelationTypeOther] [varchar](100) NULL,
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
        [CurrentlyWorkingInWestchesterInd] [bit] NOT NULL,
        [CountyLivingIn] [varchar](100) NULL,
        [CountyWorkingIn] [varchar](100) NULL,
        [OwnRealEstateInd] [bit] NOT NULL,
        [RealEstateValueAmt] [decimal](17, 2) NULL,
        [AssetValueAmt] [decimal](17, 2) NULL,
        [IncomeValueAmt] [decimal](17, 2) NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHouseholdMembers] PRIMARY KEY CLUSTERED (
            [MemberID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_FirstName]  DEFAULT (('')) FOR [FirstName];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_LastName]  DEFAULT (('')) FOR [LastName];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_RelationTypeCD]  DEFAULT (('OTHER')) FOR [RelationTypeCD];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_GenderCD]  DEFAULT (('')) FOR [GenderCD];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_RaceCD]  DEFAULT (('')) FOR [RaceCD];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_EthnicityCD]  DEFAULT (('')) FOR [EthnicityCD];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_StudentInd]  DEFAULT ((0)) FOR [StudentInd];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_DisabilityInd]  DEFAULT ((0)) FOR [DisabilityInd];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_VeteranInd]  DEFAULT ((0)) FOR [VeteranInd];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_EverLivedInWestchesterInd]  DEFAULT ((0)) FOR [EverLivedInWestchesterInd];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_CurrentlyWorkingInWestchesterInd]  DEFAULT ((0)) FOR [CurrentlyWorkingInWestchesterInd];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHouseholdMembers] ADD  CONSTRAINT [DF_tblHouseholdMembers_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO