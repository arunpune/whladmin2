using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class FaqViewModel : FaqConfig
{
}

[ExcludeFromCodeCoverage]
public class FaqsViewModel
{
    public Dictionary<int, string> Categories { get; set; }
    public IEnumerable<FaqViewModel> Faqs { get; set; }
}