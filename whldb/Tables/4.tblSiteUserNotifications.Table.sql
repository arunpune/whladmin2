SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteUserNotifications', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteUserNotifications] (
        [NotificationID] [bigint] IDENTITY(1,1) NOT NULL,
        [Username] [varchar](200) NOT NULL,
        [Subject] [varchar](1000) NULL,
        [Body] [varchar](max) NULL,
        [ReadInd] [bit] NOT NULL,
        [ReadTimestamp] [datetime] NULL,
        [EmailSentInd] [bit] NOT NULL,
        [EmailTimestamp] [datetime] NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteUserNotifications] PRIMARY KEY CLUSTERED (
            [NotificationID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteUserNotifications] ADD  CONSTRAINT [DF_tblSiteUserNotifications_ReadInd]  DEFAULT ((0)) FOR [ReadInd];

    ALTER TABLE [dbo].[tblSiteUserNotifications] ADD  CONSTRAINT [DF_tblSiteUserNotifications_EmailSentInd]  DEFAULT (0) FOR [EmailSentInd];

    ALTER TABLE [dbo].[tblSiteUserNotifications] ADD  CONSTRAINT [DF_tblSiteUserNotifications_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteUserNotifications] ADD  CONSTRAINT [DF_tblSiteUserNotifications_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO