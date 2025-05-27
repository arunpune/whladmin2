using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class Amenity : ModelBase
{
    public int AmenityId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UsageCount { get; set; }
    public bool Selected { get; set; }
}