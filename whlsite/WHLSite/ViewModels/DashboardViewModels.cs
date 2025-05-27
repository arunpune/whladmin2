using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class DashboardViewModel : HouseholdViewModel
{
    public IEnumerable<HousingApplicationViewModel> Applications { get; set; }
    public int ApplicationsCount { get { return Applications?.Count() ?? 0; } }
}