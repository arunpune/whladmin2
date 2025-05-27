SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterMarketingAgents', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterMarketingAgents] (
        [AgentID] [int] IDENTITY(1,1) NOT NULL,
        [Name] [varchar](200) NOT NULL,
        [ContactName] [varchar](200) NULL,
        [PhoneNumber] [varchar](20) NULL,
        [EmailAddress] [varchar](200) NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterMarketingAgents] PRIMARY KEY CLUSTERED (
        	[AgentID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterMarketingAgents] ADD  CONSTRAINT [DF_tblMasterMarketingAgents_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterMarketingAgents] ADD  CONSTRAINT [DF_tblMasterMarketingAgents_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO