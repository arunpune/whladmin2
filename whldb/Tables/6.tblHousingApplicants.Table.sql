SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHousingApplicants', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHousingApplicants] (
        [ApplicantID] [bigint] IDENTITY(1, 1) NOT NULL,
        [ApplicationID] [bigint] NOT NULL,
        [CoApplicantInd] [bit] NOT NULL,
        [RelationTypeCD] [varchar](20) NOT NULL,
        [RelationTypeOther] [varchar](100) NULL,
        [MemberID] [bigint] NOT NULL,
        [Title] [varchar](10) NULL,
        [FirstName] [varchar](100) NOT NULL,
        [MiddleName] [varchar](100) NULL,
        [LastName] [varchar](100) NOT NULL,
        [Suffix] [varchar](10) NULL,
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
        [OwnRealEstateInd] [bit] NOT NULL,
        [RealEstateValueAmt] [decimal](17, 2) NULL,
        [AssetValueAmt] [decimal](17, 2) NULL,
        [IncomeValueAmt] [decimal](17, 2) NULL,
        [Last4SSN] [char](4) NULL,
        [DateOfBirth] [datetime] NULL,
        [IDTypeCD] [varchar](20) NULL,
        [IDTypeValue] [varchar](20) NULL,
        [IDIssueDate] [datetime] NULL,
        [ApplicantSortOrder] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHousingApplicants] PRIMARY KEY CLUSTERED (
            [ApplicantID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_CoApplicantInd]  DEFAULT ((0)) FOR [CoApplicantInd];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_EverLivedInWestchesterInd]  DEFAULT ((0)) FOR [EverLivedInWestchesterInd];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_CurrentlyWorkingInWestchesterInd]  DEFAULT ((0)) FOR [CurrentlyWorkingInWestchesterInd];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_OwnRealEstateInd]  DEFAULT ((0)) FOR [OwnRealEstateInd];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHousingApplicants] ADD  CONSTRAINT [DF_tblHousingApplicants_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO