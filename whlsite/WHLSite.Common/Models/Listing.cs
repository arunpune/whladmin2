using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class Listing : ModelBase
{
    public int ListingId { get; set; }
    public string ListingTypeCd { get; set; }
    public bool ResaleInd { get; set; }
    public string ListingTypeDescription { get; set; }
    public string ListingAgeTypeCd { get; set; }
    public string ListingAgeTypeDescription { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string StreetLine1 { get; set; }
    public string StreetLine2 { get; set; }
    public string StreetLine3 { get; set; }
    public string City { get; set; }
    public string StateCd { get; set; }
    public string ZipCode { get; set; }
    public string County { get; set; }
    public string EsriX { get; set; }
    public string EsriY { get; set; }
    public string Municipality { get; set; }
    public string MunicipalityUrl { get; set; }
    public string SchoolDistrict { get; set; }
    public string SchoolDistrictUrl { get; set; }
    public string MapUrl { get; set; }
    public string WebsiteUrl { get; set; }
    public DateTime? ListingStartDate { get; set; }
    public DateTime? ListingEndDate { get; set; }
    public DateTime? ApplicationStartDate { get; set; }
    public DateTime? ApplicationEndDate { get; set; }
    public bool WaitlistEligible { get; set; }
    public DateTime? WaitlistStartDate { get; set; }
    public DateTime? WaitlistEndDate { get; set; }
    public bool LotteryEligible { get; set; }
    public DateTime? LotteryDate { get; set; }
    public decimal MinHouseholdIncomeAmt { get; set; }
    public decimal MaxHouseholdIncomeAmt { get; set; }
    public int MinHouseholdSize { get; set; }
    public int MaxHouseholdSize { get; set; }
    public bool PetsAllowedInd { get; set; }
    public string PetsAllowedText { get; set; }
    public string RentIncludesText { get; set; }
    public string CompletedOrInitialOccupancyYear { get; set; }
    public string TermOfAffordability { get; set; }
    public bool MarketingAgentInd { get; set; }
    public int MarketingAgentId { get; set; }
    public string MarketingAgentName { get; set; }
    public string MarketingAgentApplicationLink { get; set; }
    public string StatusCd { get; set; }
    public string StatusDescription { get; set; }
    public IEnumerable<ListingImage> Images { get; set; }
    public IEnumerable<ListingDocument> Documents { get; set; }
    public IEnumerable<ListingUnit> Units { get; set; }
    public IEnumerable<ListingUnitHousehold> Households { get; set; }
    public IEnumerable<Amenity> Amenities { get; set; }
    public IEnumerable<DocumentType> DocumentTypes { get; set; }
    public IEnumerable<FundingSource> FundingSources { get; set; }
    public Dictionary<string, string> Accessibilities { get; set; }
    public IEnumerable<QuestionConfig> Questions { get; set; }
    public IEnumerable<Declaration> Declarations { get; set; }
    public IEnumerable<Disclosure> Disclosures { get; set; }
    public bool IsRental { get { return (ListingTypeCd ?? "").Trim().Equals("RENTAL", StringComparison.CurrentCultureIgnoreCase); } }
    public bool Is55Plus { get { return (ListingAgeTypeCd ?? "").Trim().Equals("55+", StringComparison.CurrentCultureIgnoreCase); } }
    public bool Is62Plus { get { return (ListingAgeTypeCd ?? "").Trim().Equals("62+", StringComparison.CurrentCultureIgnoreCase); } }

    public string ImageTitle { get; set; }
    public string ImageContents { get; set; }
    public string ImageMimeType { get; set; }
    public bool ImageIsPrimary { get; set; }

    public long LotteryId { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDocument : ModelBase
{
    public int DocumentId { get; set; }
    public int ListingId { get; set; }
    public string Title { get; set; }
    public string FileName { get; set; }
    public string Contents { get; set; }
    public string MimeType { get; set; }
    public bool DisplayOnListingsPageInd { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingImage : ModelBase
{
    public int ImageId { get; set; }
    public int ListingId { get; set; }
    public string Title { get; set; }
    public string ThumbnailContents { get; set; }
    public string Contents { get; set; }
    public string MimeType { get; set; }
    public bool IsPrimary { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnit : ModelBase
{
    public long UnitId { get; set; }
    public int ListingId { get; set; }
    public string UnitTypeCd { get; set; }
    public string UnitTypeDescription { get; set; }
    public int BedroomCnt { get; set; }
    public int BathroomCnt { get; set; }
    public int BathroomCntPart { get; set; }
    public int SquareFootage { get; set; }
    public decimal AreaMedianIncomePct { get; set; }
    public decimal MonthlyRentAmt { get; set; }
    public decimal AssetLimitAmt { get; set; }
    public long EstimatedPriceAmt { get; set; }
    public long SubsidyAmt { get; set; }
    public long NetSalePriceAmt { get; set; }
    public long DownPaymentAmt { get; set; }
    public long MortgageAmt { get; set; }
    public decimal MortgageRatePct { get; set; }
    public decimal LoanRatePct { get; set; }
    public int MortgageTerm { get; set; }
    public long MonthlyPrincipalInterestAmt { get; set; }
    public long MonthlyTaxesAmt { get; set; }
    public long MonthlyMaintenanceAmt { get; set; }
    public long MonthlyInsuranceAmt { get; set; }
    public long MonthlyPmiAmt { get; set; }
    public long MonthlyPaymentAmt { get; set; }
    public int UnitsAvailableCnt { get; set; }

    public List<ListingUnitHousehold> Households { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitHousehold : ModelBase
{
    public int HouseholdId { get; set; }
    public int UnitId { get; set; }
    public int HouseholdSize { get; set; }
    public decimal MinHouseholdIncomeAmt { get; set; }
    public decimal MaxHouseholdIncomeAmt { get; set;}
}

[ExcludeFromCodeCoverage]
public class ListingSearchParameters : PagingInfo
{
    public string AdaptedForDisabilityOptionCd { get; set; }
    public string CityOptionCd { get; set; }
    public string ListingTypeOptionCd { get; set; }
    public string ListingDateFilterOptionCd { get; set; }
    public string PetsAllowedOptionCd { get; set; }
    public string SeniorLivingOptionCd { get; set; }
}