SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHousingApplicationDocumentContents', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHousingApplicationDocumentContents] (
        [DocID] [bigint] NOT NULL,
        [DocContents] [varbinary](max) NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHousingApplicationDocumentContents] PRIMARY KEY CLUSTERED (
            [DocID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY];

    ALTER TABLE [dbo].[tblHousingApplicationDocumentContents] ADD  CONSTRAINT [DF_tblHousingApplicationDocumentContents_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO