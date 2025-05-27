SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblMetadata', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblMetadata] (
        [MetadataID] [int] NOT NULL,
        [CodeID] [int] NOT NULL,
        [Code] [varchar](20) NOT NULL,
        [Description] [varchar](1000) NOT NULL,
        [AssociatedCodeID] [int] NULL,
        [AssociatedCode] [varchar](20) NULL,
        [Active] [bit] NOT NULL,
        CONSTRAINT [PK_tblMetadata] PRIMARY KEY CLUSTERED (
            [MetadataID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblMetadata] ADD  CONSTRAINT [DF_tblMetadata_Active]  DEFAULT ((1)) FOR [Active];

END
GO