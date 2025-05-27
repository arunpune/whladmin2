SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblHousingApplicationComments', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblHousingApplicationComments] (
        [CommentID] [bigint] IDENTITY(1, 1) NOT NULL,
        [ApplicationID] [bigint] NOT NULL,
        [Comments] [varchar](4000) NOT NULL,
        [InternalOnlyInd] [bit] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblHousingApplicationComments] PRIMARY KEY CLUSTERED (
            [CommentID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblHousingApplicationComments] ADD  CONSTRAINT [DF_tblHousingApplicationComments_InternalOnlyInd]  DEFAULT ((0)) FOR [InternalOnlyInd];

    ALTER TABLE [dbo].[tblHousingApplicationComments] ADD  CONSTRAINT [DF_tblHousingApplicationComments_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblHousingApplicationComments] ADD  CONSTRAINT [DF_tblHousingApplicationComments_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO