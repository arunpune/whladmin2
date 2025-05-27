SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMasterAmortizations', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMasterAmortizations] (
        [Rate] [decimal](8,5) NOT NULL,
        [RateInterestOnly] [decimal](8,5) NOT NULL,
        [Rate10Year] [decimal](8,5) NOT NULL,
        [Rate15Year] [decimal](8,5) NOT NULL,
        [Rate20Year] [decimal](8,5) NOT NULL,
        [Rate25Year] [decimal](8,5) NOT NULL,
        [Rate30Year] [decimal](8,5) NOT NULL,
        [Rate40Year] [decimal](8,5) NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblMasterAmortizations] PRIMARY KEY CLUSTERED (
        	[Rate] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMasterAmortizations] ADD  CONSTRAINT [DF_tblMasterAmortizations_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblMasterAmortizations] ADD  CONSTRAINT [DF_tblMasterAmortizations_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO