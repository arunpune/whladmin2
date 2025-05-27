using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class CodeDescription
{
    public int MetadataId { get; set; }
    public int CodeId { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }

    public CodeDescription() {}

    public CodeDescription(string code, string description)
    {
        Code = code;
        Description = description;
    }
}