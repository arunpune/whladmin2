using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class NotificationConfig : ModelBase
{
    public int NotificationId { get; set; }
    public string CategoryCd { get; set; }
    public string CategoryDescription { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
    public int FrequencyInterval { get; set; }
    public string FrequencyCd { get; set; }
    public string FrequencyDescription { get; set; }
    public string NotificationList { get; set; }
    public string InternalNotificationList { get; set; }
}