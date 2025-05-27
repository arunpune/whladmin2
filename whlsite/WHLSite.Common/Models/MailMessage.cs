using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class MailMessage
{
    public string Username { get; set; }
    public string NotificationBody { get; set; }
    public string ToAddresses { get; set; }
    public string CcAddresses { get; set; }
    public string BccAddresses { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool UseHtmlBody { get; set; }
}