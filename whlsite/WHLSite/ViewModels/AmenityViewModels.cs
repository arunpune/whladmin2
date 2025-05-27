using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class AmenityViewModel : Amenity
{
}

public class AmenitiesViewModel
{
    public IEnumerable<AmenityViewModel> Amenities { get; set; }
}
