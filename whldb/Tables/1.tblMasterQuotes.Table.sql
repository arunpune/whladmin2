SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterQuotes', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterQuotes] (
        [QuoteID] [int] IDENTITY(1,1) NOT NULL,
        [Text] [varchar](1000) NOT NULL,
        [DisplayOnHomePageInd] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterQuotes] PRIMARY KEY CLUSTERED (
        	[QuoteID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterQuotes] ADD  CONSTRAINT [DF_tblMasterQuotes_DisplayOnHomePageInd]  DEFAULT ((0)) FOR [DisplayOnHomePageInd];

    ALTER TABLE [dbo].[tblMasterQuotes] ADD  CONSTRAINT [DF_tblMasterQuotes_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterQuotes] ADD  CONSTRAINT [DF_tblMasterQuotes_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO