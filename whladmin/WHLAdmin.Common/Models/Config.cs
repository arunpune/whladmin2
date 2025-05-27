using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class Config
{
    public int ConfigId { get; set; }
    public string Category { get; set; }
    public string SubCategory { get; set; }
    public string ConfigKey { get; set; }
    public string ConfigValue { get; set; }
    public bool Active { get; set; }
}