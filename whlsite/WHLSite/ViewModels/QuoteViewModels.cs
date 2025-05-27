using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class QuoteViewModel : QuoteConfig
{
}

[ExcludeFromCodeCoverage]
public class QuotesViewModel
{
    public IEnumerable<QuoteViewModel> Quotes { get; set; }
}