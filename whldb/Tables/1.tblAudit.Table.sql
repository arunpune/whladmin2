SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblAudit', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblAudit] (
        [ID] [bigint] IDENTITY(1,1) NOT NULL,
        [EntityTypeCD] [varchar](20) NOT NULL,
        [EntityID] [varchar](200) NOT NULL,
        [Username] [varchar](200) NOT NULL,
        [ActionCD] [varchar](20) NOT NULL,
        [Note] [varchar](max) NOT NULL,
        [Timestamp] [datetime] NOT NULL,
        CONSTRAINT [PK_tblAudit] PRIMARY KEY CLUSTERED (
            [ID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblAudit] ADD  CONSTRAINT [DF_tblAudit_Timestamp]  DEFAULT (getdate()) FOR [Timestamp];

END
GO