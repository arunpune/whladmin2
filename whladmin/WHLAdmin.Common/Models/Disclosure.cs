using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class Disclosure : ModelBase
{
    public long ListingId { get; set; }
    public int DisclosureId { get; set; }
    public int SortOrder { get; set; }
    public string Code { get; set; }
    public string Text { get; set; }
    public int UsageCount { get; set; }
    public bool UserAdded { get; set; }
}