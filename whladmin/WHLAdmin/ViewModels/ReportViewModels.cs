using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class DemographicsReportViewModel : ErrorViewModel
{
    [Display(Name = "From Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "From Date is required")]
    public string FromDate { get; set; }

    [Display(Name = "To Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "To Date is required")]
    public string ToDate { get; set; }

    public bool Searched { get; set; }
    public IEnumerable<ApplicationDemographicRecordViewModel> Data { get; set; }
    public Dictionary<string, string> GenderTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class ApplicationDemographicRecordViewModel
{
    public long ListingId { get; set; }
    public string ListingName { get; set; }
    public string ListingTypeCd { get; set; }
    public string ListingTypeDescription { get; set; }
    public int TotalCount { get; set; }
    public Dictionary<string, int> GenderData { get; set; }
    public Dictionary<string, int> RaceData { get; set; }
    public Dictionary<string, int> EthnicityData { get; set; }
}

[ExcludeFromCodeCoverage]
public class RegistrationsSummaryReportViewModel : ErrorViewModel
{
    [Display(Name = "From Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "From Date is required")]
    public string FromDate { get; set; }

    [Display(Name = "To Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "To Date is required")]
    public string ToDate { get; set; }

    public bool Searched { get; set; }
    public int TotalRegistrationsCount { get; set; }
    public int TotalActiveCount { get; set; }
    public int TotalInactiveCount { get; set; }
    public IEnumerable<RegistrationSummaryByCountyRecord> NewYorkData { get; set; }
    public IEnumerable<RegistrationSummaryByStateRecord> OtherStatesData { get; set; }
}

[ExcludeFromCodeCoverage]
public class MarketEvaluationsReportViewModel : ErrorViewModel
{
    [Display(Name = "From Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "From Date is required")]
    public string FromDate { get; set; }

    [Display(Name = "To Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "To Date is required")]
    public string ToDate { get; set; }

    [Display(Name = "Listing")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Listing is required")]
    public long ListingId { get; set; }

    public IEnumerable<ListingViewModel> Listings { get; set; }

    public bool Searched { get; set; }
    public IEnumerable<CategoryCountPercentage> Data { get; set; }
}