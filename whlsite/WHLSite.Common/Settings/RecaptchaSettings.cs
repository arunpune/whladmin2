using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Settings;

[ExcludeFromCodeCoverage]
public class RecaptchaSettings
{
    public string Version { get; set; }
    public string ProjectId { get; set; }
    public string Key { get; set; }
    public string Secret { get; set; }
    public string TokenUrl { get; set; }
    public string VerificationUrl { get; set; }
    public bool Enabled { get; set; }
}