using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class FundingSource : ModelBase
{
    public int FundingSourceId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UsageCount { get; set; }
    public bool Selected { get; set; }
}