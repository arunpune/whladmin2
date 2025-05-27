using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class UserNotification : ModelBase
{
    public string Username { get; set; }
    public long NotificationId { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public bool ReadInd { get; set; }
    public bool EmailSentInd { get; set; }
    public DateTime? EmailTimestamp { get; set; }
}