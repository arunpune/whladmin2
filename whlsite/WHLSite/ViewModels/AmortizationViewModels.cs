using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class AmortizationViewModel : Amortization
{
}

[ExcludeFromCodeCoverage]
public class AmortizationsViewModel
{
    public IEnumerable<AmortizationViewModel> Amortizations { get; set; }
}