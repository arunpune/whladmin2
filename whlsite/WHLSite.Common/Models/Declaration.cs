using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class Declaration : ModelBase
{
    public int ListingId { get; set; }
    public int DeclarationId { get; set; }
    public int SortOrder { get; set; }
    public string Code { get; set; }
    public string Text { get; set; }
    public int UsageCount { get; set; }
    public bool UserAdded { get; set; }
}