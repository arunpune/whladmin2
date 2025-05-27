SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteUsers', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteUsers] (
        [Username] [varchar](200) NOT NULL,
        [PasswordHash] [varchar](1024) NOT NULL,
        [UsernameVerifiedInd] [bit] NOT NULL,
        [EmailAddress] [varchar](200) NOT NULL,
        [AuthRepEmailAddressInd] [bit] NOT NULL,
        [AltEmailAddress] [varchar](200) NULL,
        [AltEmailAddressVerifiedInd] [bit] NOT NULL,
        [PhoneNumberTypeCD] [varchar](20) NOT NULL,
        [PhoneNumber] [varchar](10) NOT NULL,
        [PhoneNumberExtn] [varchar](10) NULL,
        [PhoneNumberVerifiedInd] [bit] NOT NULL,
        [AltPhoneNumberTypeCD] [varchar](20) NULL,
        [AltPhoneNumber] [varchar](10) NULL,
        [AltPhoneNumberExtn] [varchar](10) NULL,
        [AltPhoneNumberVerifiedInd] [bit] NOT NULL,
        [LeadTypeCD] [varchar](20) NOT NULL,
        [LeadTypeOther] [varchar](500) NULL,
        [LastLoggedIn] [datetime] NULL,
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
        [LanguagePreferenceCD] [varchar](20) NOT NULL,
        [LanguagePreferenceOther] [varchar](100) NULL,
        [ListingPreferenceCD] [varchar](20) NOT NULL,
        [RentalsListingAgePreferenceCD] [varchar](20) NOT NULL,
        [SalesListingAgePreferenceCD] [varchar](20) NOT NULL,
        [SmsNotificationsPreferenceInd] [bit] NOT NULL,
        [EverLivedInWestchesterInd] [bit] NOT NULL,
        [CountyLivingIn] [varchar](100) NULL,
        [CurrentlyWorkingInWestchesterInd] [bit] NOT NULL,
        [CountyWorkingIn] [varchar](100) NULL,
        [OwnRealEstateInd] [bit] NOT NULL,
        [RealEstateValueAmt] [decimal](17, 2) NULL,
        [AssetValueAmt] [decimal](17, 2) NULL,
        [IncomeValueAmt] [decimal](17, 2) NULL,
        [HouseholdSize] [int] NOT NULL,
        [EmfluenceContactID] [bigint] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        [DeactivatedDate] [datetime] NULL,
        [DeactivatedBy] [varchar](200) NULL,
        [ActivationKey] [varchar](32) NULL,
        [ActivationKeyExpiry] [datetime] NULL,
        [PasswordResetKey] [varchar](32) NULL,
        [PasswordResetKeyExpiry] [datetime] NULL,
        [AltEmailAddressKey] [varchar](32) NULL,
        [AltEmailAddressKeyExpiry] [datetime] NULL,
        [PhoneNumberKey] [varchar](32) NULL,
        [PhoneNumberKeyExpiry] [datetime] NULL,
        [AltPhoneNumberKey] [varchar](32) NULL,
        [AltPhoneNumberKeyExpiry] [datetime] NULL,
        CONSTRAINT [PK_tblSiteUsers] PRIMARY KEY CLUSTERED (
            [Username] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_UsernameVerifiedInd]  DEFAULT ((0)) FOR [UsernameVerifiedInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_AuthRepEmailAddressInd]  DEFAULT ((0)) FOR [AuthRepEmailAddressInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_AltEmailAddressVerifiedInd]  DEFAULT ((0)) FOR [AltEmailAddressVerifiedInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_PhoneNumberTypeCD]  DEFAULT (('OTHER')) FOR [PhoneNumberTypeCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_PhoneNumber]  DEFAULT (('')) FOR [PhoneNumber];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_PhoneNumberVerifiedInd]  DEFAULT ((0)) FOR [PhoneNumberVerifiedInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_AltPhoneNumberVerifiedInd]  DEFAULT ((0)) FOR [AltPhoneNumberVerifiedInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_FirstName]  DEFAULT (('')) FOR [FirstName];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_LastName]  DEFAULT (('')) FOR [LastName];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_GenderCD]  DEFAULT (('')) FOR [GenderCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_RaceCD]  DEFAULT (('')) FOR [RaceCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_EthnicityCD]  DEFAULT (('')) FOR [EthnicityCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_StudentInd]  DEFAULT ((0)) FOR [StudentInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_DisabilityInd]  DEFAULT ((0)) FOR [DisabilityInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_VeteranInd]  DEFAULT ((0)) FOR [VeteranInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_LanguagePreferenceCD]  DEFAULT (('EN')) FOR [LanguagePreferenceCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_ListingPreferenceCD]  DEFAULT (('BOTH')) FOR [ListingPreferenceCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_RentalsListingAgePreferenceCD]  DEFAULT (('ALL')) FOR [RentalsListingAgePreferenceCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_SalesListingAgePreferenceCD]  DEFAULT (('ALL')) FOR [SalesListingAgePreferenceCD];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_SmsNotificationsPreferenceInd]  DEFAULT ((0)) FOR [SmsNotificationsPreferenceInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_EverLivedInWestchesterInd]  DEFAULT ((0)) FOR [EverLivedInWestchesterInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_CurrentlyWorkingInWestchesterInd]  DEFAULT ((0)) FOR [CurrentlyWorkingInWestchesterInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_OwnRealEstateInd]  DEFAULT ((0)) FOR [OwnRealEstateInd];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_HouseholdSize]  DEFAULT ((1)) FOR [HouseholdSize];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_EmfluenceContactID]  DEFAULT ((0)) FOR [EmfluenceContactID];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO

-- HouseholdSize
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblSiteUsers' AND [column_name] = 'HouseholdSize')
    ALTER TABLE [dbo].[tblSiteUsers] ADD [HouseholdSize] [bigint] NULL;
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblSiteUsers' AND [column_name] = 'HouseholdSize' AND [is_nullable] = 'YES')
BEGIN
    UPDATE [dbo].[tblSiteUsers] SET [HouseholdSize] = 0 WHERE [HouseholdSize] IS NULL;
    ALTER TABLE [dbo].[tblSiteUsers] ALTER COLUMN [HouseholdSize] [bigint] NOT NULL;
END
GO

-- DF_tblSiteUsers_HouseholdSize
IF OBJECT_ID('DF_tblSiteUsers_HouseholdSize', 'D') IS NULL
    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_HouseholdSize]  DEFAULT ((1)) FOR [HouseholdSize];
GO
-- EmfluenceContactID
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblSiteUsers' AND [column_name] = 'EmfluenceContactID')
    ALTER TABLE [dbo].[tblSiteUsers] ADD [EmfluenceContactID] [bigint] NULL;
GO
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE [table_name] = 'tblSiteUsers' AND [column_name] = 'EmfluenceContactID' AND [is_nullable] = 'YES')
BEGIN
    UPDATE [dbo].[tblSiteUsers] SET [EmfluenceContactID] = 0 WHERE [EmfluenceContactID] IS NULL;
    ALTER TABLE [dbo].[tblSiteUsers] ALTER COLUMN [EmfluenceContactID] [bigint] NOT NULL;
END
GO

-- DF_tblSiteUsers_EmfluenceContactID
IF OBJECT_ID('DF_tblSiteUsers_EmfluenceContactID', 'D') IS NULL
    ALTER TABLE [dbo].[tblSiteUsers] ADD  CONSTRAINT [DF_tblSiteUsers_EmfluenceContactID]  DEFAULT ((0)) FOR [EmfluenceContactID];
GO