using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ListingViewModel : Listing
{
    public string AddressText { get; set; }
    public IEnumerable<decimal> AreaMedianIncomePcts { get { return Units?.Select(s => s.AreaMedianIncomePct)?.Distinct()?.OrderBy(o => o) ?? new List<decimal>().AsEnumerable(); } }
    public string DisplayListingStartDate { get { return ListingStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ListingStartDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayListingEndDate { get { return ListingEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ListingEndDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayApplicationStartDate { get { return ApplicationStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ApplicationStartDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayApplicationEndDate { get { return ApplicationEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? ApplicationEndDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayLotteryDate { get { return LotteryDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? LotteryDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayWaitlistStartDate { get { return WaitlistStartDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? WaitlistStartDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayWaitlistEndDate { get { return WaitlistEndDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? WaitlistEndDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }

    public HousingApplicationViewModel Application { get; set; }

    public bool CanApply { get { return DateTime.Now.Ticks >= ApplicationStartDate.GetValueOrDefault(DateTime.MinValue).Ticks
                                            && (ApplicationEndDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue
                                                    || DateTime.Now.Ticks < ApplicationEndDate.GetValueOrDefault(DateTime.MinValue).Ticks); } }
    public bool CanApplyWaitlist { get { return WaitlistEligible
                                            && DateTime.Now.Ticks >= WaitlistStartDate.GetValueOrDefault(DateTime.MinValue).Ticks
                                            && (WaitlistEndDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue
                                                    || DateTime.Now.Ticks < WaitlistEndDate.GetValueOrDefault(DateTime.MinValue).Ticks); } }
}

[ExcludeFromCodeCoverage]
public class ListingsViewModel : ListingSearchViewModel
{
    public IEnumerable<ListingViewModel> Listings { get; set; }
    public IEnumerable<ListingViewModel> SaleListings { get; set; }
    public IEnumerable<ListingViewModel> RentalListings { get; set; }

    public Dictionary<string, string> AdaptedForDisabilityOptions { get; set; }
    public Dictionary<string, string> CityOptions { get; set; }
    public Dictionary<string, string> ListingDateFilterOptions { get; set; }
    public Dictionary<string, string> ListingTypeOptions { get; set; }
    public Dictionary<string, string> PetsAllowedOptions { get; set; }
    public Dictionary<string, string> SeniorLivingOptions { get; set; }

    public Dictionary<string, string> CurrentFilters { get; set; }

    public List<int> PageSizes { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingDocumentViewModel : ListingDocument
{
}

public class ListingDocumentsViewModel
{
    public IEnumerable<ListingDocumentViewModel> Documents { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingImageViewModel : ListingImage
{
}

public class ListingImagesViewModel
{
    public IEnumerable<ListingImageViewModel> Images { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitViewModel : ListingUnit
{
    public IEnumerable<ListingUnitHouseholdViewModel> UnitHouseholds { get; set; }
}

public class ListingUnitsViewModel
{
    public IEnumerable<ListingUnitViewModel> Units { get; set; }
}

[ExcludeFromCodeCoverage]
public class ListingUnitHouseholdViewModel : ListingUnitHousehold
{
}

[ExcludeFromCodeCoverage]
public class ListingSearchViewModel : ListingSearchParameters
{
    public bool Searched { get; set; }
}

[ExcludeFromCodeCoverage]
public class PrintableFormViewModel : ListingViewModel
{
    public string ApplicationEndDateText { get; set; }
    public string LotteryDateText { get; set; }
    public Dictionary<string, string> UnitTypes { get; set; }
    public Dictionary<string, string> VoucherTypes { get; set; }
    public Dictionary<string, string> RelationTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
    public string PetDisclosure { get; set; }
    public Dictionary<string, string> LeadTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class AffordabilityAnalysisViewModel : ListingUnit
{
    public Dictionary<decimal, string> MortgageRates { get; set; }
    public Dictionary<int, string> MortgageTerms { get; set; }
    public Dictionary<long, string> Units { get; set; }
}