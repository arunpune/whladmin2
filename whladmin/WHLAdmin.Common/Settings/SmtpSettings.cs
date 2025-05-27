using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Settings;

[ExcludeFromCodeCoverage]
public class SmtpSettings
{
    public bool Enabled { get; set; }
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public bool UseSsl { get; set; }
    public bool UseAuthentication { get; set; }
    public string SmtpUsername { get; set; }
    public string SmtpPassword { get; set; }
    public string SmtpFromName { get; set; }    
    public string SmtpFromAddress { get; set; }
}