using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Settings;

[ExcludeFromCodeCoverage]
public class ArcGisSettings
{
    public bool Enabled { get; set; }
    public string ApiUrl { get; set; }
    public string ApiMethod { get; set; }
    public string ApiKey { get; set; }
    public string ApiKeyExpiry { get; set; }
}