SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHousingApplicantAccounts', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHousingApplicantAccounts] (
        [AccountID] [bigint] IDENTITY(1, 1) NOT NULL,
        [ApplicationID] [bigint] NOT NULL,
        [ApplicantID] [bigint] NOT NULL,
        [AccountNumber] [varchar](4) NOT NULL,
        [AccountTypeCD] [varchar](20) NOT NULL,
        [AccountTypeOther] [varchar](100) NULL,
        [AccountValueAmt] [decimal](17, 2) NOT NULL,
        [InstitutionName] [varchar](200) NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHousingApplicantAccounts] PRIMARY KEY CLUSTERED (
            [AccountID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHousingApplicantAccounts] ADD  CONSTRAINT [DF_tblHousingApplicantAccounts_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHousingApplicantAccounts] ADD  CONSTRAINT [DF_tblHousingApplicantAccounts_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO