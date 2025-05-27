SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterAMIs', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterAMIs] (
        [EffectiveDate] [int] NOT NULL,
        [EffectiveYear] [int] NOT NULL,
        [IncomeAmt] [int] NOT NULL,
        [HH1] [int] NOT NULL,
        [HH2] [int] NOT NULL,
        [HH3] [int] NOT NULL,
        [HH4] [int] NOT NULL,
        [HH5] [int] NOT NULL,
        [HH6] [int] NOT NULL,
        [HH7] [int] NOT NULL,
        [HH8] [int] NOT NULL,
        [HH9] [int] NOT NULL,
        [HH10] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterAMIs] PRIMARY KEY CLUSTERED (
        	[EffectiveDate] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterAMIs] ADD  CONSTRAINT [DF_tblMasterAMIs_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterAMIs] ADD  CONSTRAINT [DF_tblMasterAMIs_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO