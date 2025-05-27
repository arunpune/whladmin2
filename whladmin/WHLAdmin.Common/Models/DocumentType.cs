using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class DocumentType : ModelBase
{
    public int DocumentTypeId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int UsageCount { get; set; }
    public bool Selected { get; set; }
}