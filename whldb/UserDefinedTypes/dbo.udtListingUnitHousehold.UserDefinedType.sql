SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

-- Drop SPs that use this UDT
DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitAdd];
DROP PROCEDURE IF EXISTS [dbo].[uspListingUnitUpdate];
GO

-- Drop UDT if exists
DROP TYPE IF EXISTS [dbo].[udtListingUnitHousehold];
GO

CREATE TYPE [dbo].[udtListingUnitHousehold] AS TABLE (
    [HouseholdSize] INT NOT NULL,
    [MinHouseholdIncomeAmt] [decimal](12, 2) NOT NULL,
    [MaxHouseholdIncomeAmt] [decimal](12, 2) NOT NULL
);
GO