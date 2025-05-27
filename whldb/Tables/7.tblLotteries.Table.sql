SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblLotteries', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblLotteries] (
        [LotteryID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [ManualInd] [bit] NOT NULL,
        [RunDate] [datetime] NOT NULL,
        [RunBy] [varchar](200) NOT NULL,
        [StatusCD] [varchar](20) NOT NULL,
        CONSTRAINT [PK_tblLotteries] PRIMARY KEY CLUSTERED (
            [LotteryID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblLotteries] ADD  CONSTRAINT [DF_tblLotteries_ManualInd]  DEFAULT ((0)) FOR [ManualInd];

    ALTER TABLE [dbo].[tblLotteries] ADD  CONSTRAINT [DF_tblLotteries_RunDate]  DEFAULT (getdate()) FOR [RunDate];

    ALTER TABLE [dbo].[tblLotteries] ADD  CONSTRAINT [DF_tblLotteries_StatusCD]  DEFAULT ('NEW') FOR [StatusCD];

END
GO