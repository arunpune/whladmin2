SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

IF OBJECT_ID(N'tblSiteListingAmenities', N'U') IS NULL
BEGIN

    CREATE TABLE [dbo].[tblSiteListingAmenities] (
        [ListingID] [int] NOT NULL,
        [AmenityID] [int] NOT NULL,
        [Active] [bit] NOT NULL,
        [CreatedDate] [datetime] NOT NULL,
        [CreatedBy] [varchar](200) NOT NULL,
        [ModifiedDate] [datetime] NULL,
        [ModifiedBy] [varchar](200) NULL,
        CONSTRAINT [PK_tblSiteListingAmenities] PRIMARY KEY CLUSTERED (
        	[ListingID] ASC,
            [AmenityID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
    ) ON [PRIMARY];

    ALTER TABLE [dbo].[tblSiteListingAmenities] ADD  CONSTRAINT [DF_tblSiteListingAmenities_Active]  DEFAULT ((1)) FOR [Active];

    ALTER TABLE [dbo].[tblSiteListingAmenities] ADD  CONSTRAINT [DF_tblSiteListingAmenities_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate];

END
GO