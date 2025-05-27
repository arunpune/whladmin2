SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblAdminUsers', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblAdminUsers] (
        [UserID] [varchar](4) NOT NULL,
        [EmailAddress] [varchar](200) NOT NULL,
        [DisplayName] [varchar](200) NOT NULL,
        [OrganizationCD] [varchar](20) NOT NULL,
        [RoleCD] [varchar](20) NOT NULL,
        [OTP] [varchar](16) NULL,
        [OTPExpiry] [datetime] NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        [DeactivatedDate] [datetime] NULL,
        [DeactivatedBy] [varchar](200) NULL,
        [LastLoggedIn] [datetime] NULL,
        CONSTRAINT [PK_tblAdminUsers] PRIMARY KEY CLUSTERED (
            [UserID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblAdminUsers] ADD  CONSTRAINT [DF_tblAdminUsers_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblAdminUsers] ADD  CONSTRAINT [DF_tblAdminUsers_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO