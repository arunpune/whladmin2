SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblLotteryResults', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblLotteryResults] (
        [ResultID] [bigint] IDENTITY(1, 1) NOT NULL,
        [LotteryID] [int] NOT NULL,
        [ListingID] [int] NOT NULL,
        [ApplicationID] [bigint] NOT NULL,
        [SortOrder] [int] NOT NULL,
        [LotteryNumber] [varchar](20) NULL,
        CONSTRAINT [PK_tblLotteryResults] PRIMARY KEY CLUSTERED (
            [ResultID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

END
GO