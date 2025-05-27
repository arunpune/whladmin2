using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ResourceViewModel : ResourceConfig
{
}

[ExcludeFromCodeCoverage]
public class ResourcesViewModel
{
    public IEnumerable<ResourceViewModel> Resources { get; set; }
}