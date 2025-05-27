SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterNotifications', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterNotifications] (
        [NotificationID] [int] IDENTITY(1,1) NOT NULL,
        [CategoryCD] [varchar](20) NOT NULL,
        [Title] [varchar](200) NOT NULL,
        [Text] [varchar](1000) NOT NULL,
        [FrequencyCD] [varchar](20) NOT NULL,
        [FrequencyInterval] [int] NOT NULL,
        [NotificationList] [varchar](2000) NULL,
        [InternalNotificationList] [varchar](2000) NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterNotifications] PRIMARY KEY CLUSTERED (
            [NotificationID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterNotifications] ADD  CONSTRAINT [DF_tblMasterNotifications_FrequencyInterval]  DEFAULT ((0)) FOR [FrequencyInterval];

    ALTER TABLE [dbo].[tblMasterNotifications] ADD  CONSTRAINT [DF_tblMasterNotifications_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterNotifications] ADD  CONSTRAINT [DF_tblMasterNotifications_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO