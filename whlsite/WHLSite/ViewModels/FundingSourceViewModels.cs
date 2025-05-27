using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class FundingSourceViewModel : FundingSource
{
}

public class FundingSourcesViewModel
{
    public IEnumerable<FundingSourceViewModel> FundingSources { get; set; }
}
