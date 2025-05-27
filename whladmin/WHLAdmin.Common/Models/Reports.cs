using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class ApplicationDemographicRecord : Listing
{
    public string GenderCd { get; set; }
    public int GenderCount { get; set; }
    public string RaceCd { get; set; }
    public int RaceCount { get; set; }
    public string EthnicityCd { get; set; }
    public int EthnicityCount { get; set; }
}

[ExcludeFromCodeCoverage]
public class RegistrationSummaryByStateRecord
{
    public string Username { get; set; }
    public string StateCd { get; set; }
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
}

[ExcludeFromCodeCoverage]
public class RegistrationSummaryByCountyRecord : RegistrationSummaryByStateRecord
{
    public string County { get; set; }
}

[ExcludeFromCodeCoverage]
public class CategoryCountPercentage
{
    public string CategoryCd { get; set; }
    public string CategoryKey { get; set; }
    public string CategoryDescription { get; set; }
    public int CategoryCount { get; set; }
    public int CategoryPercentage { get; set; }
    public int SortOrder { get; set; }
}
