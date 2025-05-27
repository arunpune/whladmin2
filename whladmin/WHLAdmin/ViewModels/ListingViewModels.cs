using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class ListingViewModel : Listing
{
    public string DisplayAddress { get; set; }
    public string DisplayListingStartDate { get { return ListingStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ListingStartDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayListingEndDate { get { return ListingEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ListingEndDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayApplicationStartDate { get { return ApplicationStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ApplicationStartDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayApplicationEndDate { get { return ApplicationEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ApplicationEndDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayLotteryDate { get { return LotteryEligible && LotteryDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? LotteryDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayWaitlistStartDate { get { return WaitlistStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? WaitlistStartDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayWaitlistEndDate { get { return WaitlistEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? WaitlistEndDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public bool HasUnpublishedChanges { get { return VersionNo != PublishedVersionNo; } }
    public bool CanSubmitPaperApplication { get; set; }
    public bool CanEdit { get; set; }
    public bool CanPublish { get; set; }
    public bool CanUnpublish { get; set; }
    public bool CanRunLottery { get { return LotteryEligible && LotteryId == 0 && DateTime.Now.Ticks >= LotteryDate.GetValueOrDefault(DateTime.MinValue).Ticks; } }

    public IEnumerable<int> AreaMedianIncomePcts { get { return Units?.Select(s => s.AreaMedianIncomePct)?.Distinct()?.OrderBy(o => o) ?? new List<int>().AsEnumerable(); } }
    public ArcGisSettings ArcGisSettings{ get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingViewModel
{
    public long ListingId { get; set; }

    [Display(Name = "Listing Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Listing Type is required")]
    public string ListingTypeCd { get; set; }

    [Display(Name = "Resale?")]
    public bool ResaleInd { get; set; }

    [Display(Name = "Senior Living Opportunities")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Senior Living Opportunities is required")]
    public string ListingAgeTypeCd { get; set; }

    [Display(Name = "Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name is required")]
    public string Name { get; set; }

    [Display(Name = "Description")]
    public string Description { get; set; }

    [Display(Name = "Street Address")]
    public string StreetAddress { get; set; }

    [Display(Name = "Street Line 1")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Street Line 1 is required")]
    public string StreetLine1 { get; set; }

    [Display(Name = "Street Line 2")]
    public string StreetLine2 { get; set; }

    [Display(Name = "Street Line 3")]
    public string StreetLine3 { get; set; }

    [Display(Name = "City")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
    public string City { get; set; }

    [Display(Name = "State")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "State is required")]
    public string StateCd { get; set; }

    [Display(Name = "Zip Code")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code is required")]
    [MinLength(5, ErrorMessage = "Zip code must be at least 5 numbers")]
    public string ZipCode { get; set; }

    [Display(Name = "County")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "County is required")]
    public string County { get; set; }

    [Display(Name = "ESRI X-Coordinate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "ESRI X-Coordinate is required")]
    public string EsriX { get; set; }

    [Display(Name = "ESRI Y-Coordinate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "ESRI Y-Coordinate is required")]
    public string EsriY { get; set; }

    [Display(Name = "Municipality")]
    public string Municipality { get; set; }

    [Display(Name = "Municipality Url")]
    public string MunicipalityUrl { get; set; }

    [Display(Name = "School District")]
    public string SchoolDistrict { get; set; }

    [Display(Name = "School District Url")]
    public string SchoolDistrictUrl { get; set; }

    [Display(Name = "Map Url")]
    public string MapUrl { get; set; }

    [Display(Name = "Website Url")]
    public string WebsiteUrl { get; set; }

    [Display(Name = "Listing Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ListingStartDate { get; set; }

    [Display(Name = "Listing End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ListingEndDate { get; set; }

    [Display(Name = "Application Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ApplicationStartDate { get; set; }

    [Display(Name = "Application Start Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string ApplicationStartTime { get; set; }

    [Display(Name = "Application End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ApplicationEndDate { get; set; }

    [Display(Name = "Application End Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string ApplicationEndTime { get; set; }

    [Display(Name = "Lottery Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string LotteryDate { get; set; }

    [Display(Name = "Lottery Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:ss}")]
    public string LotteryTime { get; set; }

    [Display(Name = "Waitlist Eligible")]
    public bool WaitlistEligible { get; set; }

    [Display(Name = "Waitlist Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string WaitlistStartDate { get; set; }

    [Display(Name = "Waitlist End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string WaitlistEndDate { get; set; }

    [Display(Name = "Minimum Income")]
    public long MinHouseholdIncomeAmt { get; set; }

    [Display(Name = "Maximum Income")]
    public long MaxHouseholdIncomeAmt { get; set; }

    [Display(Name = "Minimum Household Size")]
    public int MinHouseholdSize { get; set; }

    [Display(Name = "Maximum Household Size")]
    public int MaxHouseholdSize { get; set; }

    [Display(Name = "Pet Policy")]
    public bool PetsAllowedInd { get; set; }

    [Display(Name = "Pet Policy Disclaimer")]
    public string PetsAllowedText { get; set; }

    [Display(Name = "Rent Includes")]
    public string RentIncludesText { get; set; }

    [Display(Name = "Completed or Initial Occupancy Year")]
    public string CompletedOrInitialOccupancyYear { get; set; }

    [Display(Name = "Term of Affordability")]
    public string TermOfAffordability { get; set; }

    [Display(Name = "Status")]
    public string StatusCd { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    public Dictionary<string, string> ListingTypes { get; set; }
    public Dictionary<string, string> ListingAgeTypes { get; set; }
    public Dictionary<string, string> Statuses { get; set; }
    public ArcGisSettings ArcGisSettings { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingsViewModel : PagedViewModel
{
    public IEnumerable<ListingViewModel> Listings { get; set; }
    public bool CanEdit { get; set; }
    public ArcGisSettings ArcGisSettings { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingImageViewModel : ListingImage
{
}

[ExcludeFromCodeCoverage]
public class EditableListingImageViewModel
{
    public int ImageId { get; set; }
    public long ListingId { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Display(Name = "Thumbnail")]
    public string ThumbnailContents { get; set; }

    [Display(Name = "Image")]
    public string Contents { get; set; }

    public string MimeType { get; set; }

    [Display(Name = "Is Primary")]
    public bool IsPrimary { get; set; }

    [Display(Name = "Display on Listings Page")]
    public bool DisplayOnListingsPageInd { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingImagesViewModel
{
    public IEnumerable<ListingImageViewModel> Images { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDocumentViewModel : ListingDocument
{
}

[ExcludeFromCodeCoverage]
public class EditableListingDocumentViewModel
{
    public int DocumentId { get; set; }
    public long ListingId { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Display(Name = "File Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "File Name is required")]
    public string FileName { get; set; }

    [Display(Name = "Image")]
    public string Contents { get; set; }

    public string MimeType { get; set; }

    [Display(Name = "Display on Listings Page")]
    public bool DisplayOnListingsPageInd { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDocumentsViewModel
{
    public IEnumerable<ListingDocumentViewModel> Documents { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitViewModel : ListingUnit
{
    public IEnumerable<ListingUnitHouseholdViewModel> UnitHouseholds { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingUnitViewModel
{
    public int UnitId { get; set; }
    public long ListingId { get; set; }
    public bool IsRental { get; set; }

    [Display(Name = "Unit Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    public string UnitTypeCd { get; set; }

    [Display(Name = "Bedrooms")]
    public int BedroomCnt { get; set; }

    [Display(Name = "Bathrooms")]
    public int BathroomCnt { get; set; }
    public int BathroomCntPart { get; set; }

    [Display(Name = "Area (sq. ft.)")]
    public int SquareFootage { get; set; }

    [Display(Name = "Area Median Income %")]
    public int AreaMedianIncomePct { get; set; }

    [Display(Name = "Monthly Rent ($)")]
    public long MonthlyRentAmt { get; set; }

    [Display(Name = "Asset Limit ($)")]
    public long AssetLimitAmt { get; set; }

    [Display(Name = "Estimated Price ($)")]
    public long EstimatedPriceAmt { get; set; }

    [Display(Name = "Subsidy Assumed by Buyer ($)")]
    public long SubsidyAmt { get; set; }

    [Display(Name = "Monthly Taxes ($)")]
    public long MonthlyTaxesAmt { get; set; }

    [Display(Name = "Monthly CC/HOA ($)")]
    public long MonthlyMaintenanceAmt { get; set; }

    [Display(Name = "Monthly Walls In Insurance ($)")]
    public long MonthlyInsuranceAmt { get; set; }

    [Display(Name = "Units Available")]
    public int UnitsAvailableCnt { get; set; }

    [Display(Name = "1 Person")]
    public long MinHouseholdIncomeAmt1 { get; set; }
    public long MaxHouseholdIncomeAmt1 { get; set; }

    [Display(Name = "2 Persons")]
    public long MinHouseholdIncomeAmt2 { get; set; }
    public long MaxHouseholdIncomeAmt2 { get; set; }

    [Display(Name = "3 Persons")]
    public long MinHouseholdIncomeAmt3 { get; set; }
    public long MaxHouseholdIncomeAmt3 { get; set; }

    [Display(Name = "4 Persons")]
    public long MinHouseholdIncomeAmt4 { get; set; }
    public long MaxHouseholdIncomeAmt4 { get; set; }

    [Display(Name = "5 Persons")]
    public long MinHouseholdIncomeAmt5 { get; set; }
    public long MaxHouseholdIncomeAmt5 { get; set; }

    [Display(Name = "6 Persons")]
    public long MinHouseholdIncomeAmt6 { get; set; }
    public long MaxHouseholdIncomeAmt6 { get; set; }

    [Display(Name = "7 Persons")]
    public long MinHouseholdIncomeAmt7 { get; set; }
    public long MaxHouseholdIncomeAmt7 { get; set; }

    [Display(Name = "More than 7 Persons")]
    public long MinHouseholdIncomeAmt8 { get; set; }
    public long MaxHouseholdIncomeAmt8 { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    public Dictionary<string, string> UnitTypes { get; set; }
    public Dictionary<string, string> BathroomPartOptions { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitsViewModel
{
    public IEnumerable<ListingUnitViewModel> Units { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitHouseholdViewModel : ListingUnitHousehold
{
}

[ExcludeFromCodeCoverage]
public class EditableListingAmenitiesViewModel
{
    public long ListingId { get; set; }
    public string AmenityIds { get; set; }
    public IEnumerable<AmenityViewModel> Amenities { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingFundingSourcesViewModel
{
    public long ListingId { get; set; }
    public string FundingSourceIds { get; set; }
    public IEnumerable<FundingSourceViewModel> FundingSources { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingDocumentsReqdViewModel
{
    public long ListingId { get; set; }
    public string DocumentTypeIds { get; set; }
    public IEnumerable<DocumentTypeViewModel> DocumentTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingAccessibilitiesViewModel
{
    public long ListingId { get; set; }
    public string AccessibilityCds { get; set; }
    public Dictionary<string, string> Accessibilities { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDeclarationViewModel : Declaration
{
}

[ExcludeFromCodeCoverage]
public class EditableListingDeclarationViewModel
{
    public int DeclarationId { get; set; }
    public long ListingId { get; set; }

    [Display(Name = "Declaration")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Declaration is required")]
    public string Text { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDisclosureViewModel : Disclosure
{
}

[ExcludeFromCodeCoverage]
public class EditableListingDisclosureViewModel
{
    public int DisclosureId { get; set; }
    public long ListingId { get; set; }

    [Display(Name = "Disclosure")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Disclosure is required")]
    public string Text { get; set; }

    [Display(Name = "Sort Order")]
    public int SortOrder { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingAddressViewModel
{
    public long ListingId { get; set; }

    [Display(Name = "Street Address")]
    public string StreetAddress { get; set; }

    [Display(Name = "Street Line 1")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Street Line 1 is required")]
    public string StreetLine1 { get; set; }

    [Display(Name = "Street Line 2")]
    public string StreetLine2 { get; set; }

    [Display(Name = "Street Line 3")]
    public string StreetLine3 { get; set; }

    [Display(Name = "City")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "City is required")]
    public string City { get; set; }

    [Display(Name = "State")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "State is required")]
    public string StateCd { get; set; }

    [Display(Name = "Zip Code")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Zip Code is required")]
    [MinLength(5, ErrorMessage = "Zip code must be at least 5 numbers")]
    public string ZipCode { get; set; }

    [Display(Name = "County")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "County is required")]
    public string County { get; set; }

    [Display(Name = "ESRI X-Coordinate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "ESRI X-Coordinate is required")]
    public string EsriX { get; set; }

    [Display(Name = "ESRI Y-Coordinate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "ESRI Y-Coordinate is required")]
    public string EsriY { get; set; }

    [Display(Name = "Municipality")]
    public string Municipality { get; set; }

    [Display(Name = "Municipality Url")]
    public string MunicipalityUrl { get; set; }

    [Display(Name = "School District")]
    public string SchoolDistrict { get; set; }

    [Display(Name = "School District Url")]
    public string SchoolDistrictUrl { get; set; }

    [Display(Name = "Map Url")]
    public string MapUrl { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingDatesViewModel
{
    public long ListingId { get; set; }


    [Display(Name = "Listing Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ListingStartDate { get; set; }

    [Display(Name = "Listing End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ListingEndDate { get; set; }

    [Display(Name = "Application Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ApplicationStartDate { get; set; }

    [Display(Name = "Application Start Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string ApplicationStartTime { get; set; }

    [Display(Name = "Application End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string ApplicationEndDate { get; set; }

    [Display(Name = "Application End Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string ApplicationEndTime { get; set; }

    [Display(Name = "Lottery Eligible")]
    public bool LotteryEligible { get; set; }

    [Display(Name = "Lottery Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string LotteryDate { get; set; }

    [Display(Name = "Lottery Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string LotteryTime { get; set; }

    [Display(Name = "Waitlist Eligible")]
    public bool WaitlistEligible { get; set; }

    [Display(Name = "Waitlist Start Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string WaitlistStartDate { get; set; }

    [Display(Name = "Waitlist Start Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string WaitlistStartTime { get; set; }

    [Display(Name = "Waitlist End Date")]
    [DataType(DataType.Date)]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public string WaitlistEndDate { get; set; }

    [Display(Name = "Waitlist End Time")]
    [DataType(DataType.Time)]
    [DisplayFormat(DataFormatString = "{0:HH:mm}")]
    public string WaitlistEndTime { get; set; }
}

[ExcludeFromCodeCoverage]
public class PrintableFormViewModel : ListingViewModel
{
    public Dictionary<string, string> UnitTypes { get; set; }
    public Dictionary<string, string> VoucherTypes { get; set; }
    public Dictionary<string, string> RelationTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
    public string PetDisclosure { get; set; }
    public Dictionary<string, string> LeadTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingPetsAllowedViewModel
{
    public long ListingId { get; set; }

    [Display(Name = "Pet Policy")]
    public bool PetsAllowedInd { get; set; }

    [Display(Name = "Pet Policy Disclaimer")]
    public string PetsAllowedText { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingActionViewModel
{
    public long ListingId { get; set; }

    [Display(Name = "Reason")]
    public string Reason { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableListingMarketingAgentViewModel
{
    public long ListingId { get; set; }

    [Display(Name = "Use Marketing Agent?")]
    public bool MarketingAgentInd { get; set; }

    [Display(Name = "Agent Name")]
    public int MarketingAgentId { get; set; }

    [Display(Name = "Application Link")]
    [MaxLength(500)]
    public string MarketingAgentApplicationLink { get; set; }

    public IEnumerable<MarketingAgentViewModel> Agents { get; set; }
}

[ExcludeFromCodeCoverage]
public class AffordabilityAnalysisViewModel : ListingUnit
{
    public Dictionary<decimal, string> MortgageRates { get; set; }
    public Dictionary<int, string> MortgageTerms { get; set; }
    public Dictionary<int, string> Units { get; set; }
}