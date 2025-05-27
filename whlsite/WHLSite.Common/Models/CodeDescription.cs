using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class CodeDescription
{
    public int MetadataId { get; set; }
    public int CodeId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public int AssociatedCodeId { get; set; }
    public string AssociatedCode { get; set; }
}