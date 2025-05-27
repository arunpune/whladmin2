using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class UserNotificationViewModel : UserNotification
{
}

[ExcludeFromCodeCoverage]
public class UserNotificationsViewModel
{
    public IEnumerable<UserNotificationViewModel> Notifications { get; set; }
    public int Count { get { return Notifications?.Count() ?? 0; } }
}

[ExcludeFromCodeCoverage]
public class EditableUserNotificationViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long NotificationId { get; set; }
    public string Action { get; set; }
}