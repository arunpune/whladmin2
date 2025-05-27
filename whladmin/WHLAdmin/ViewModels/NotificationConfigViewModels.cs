using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class NotificationConfigViewModel : NotificationConfig
{
    public string FrequencyIntervalDescription { get { return FrequencyCd == "ON" ? FrequencyDescription : $"{FrequencyInterval} {FrequencyDescription}"; } }
}

[ExcludeFromCodeCoverage]
public class EditableNotificationConfigViewModel
{
    public int NotificationId { get; set; }

    [Display(Name = "Category")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required")]
    public string CategoryCd { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Display(Name = "Text")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Text is required")]
    [MaxLength(1000)]
    public string Text { get; set; }

    [Display(Name = "Frequency Interval")]
    public int FrequencyInterval { get; set; }

    [Display(Name = "Frequency Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Frequency Type is required")]
    public string FrequencyCd { get; set; }

    [Display(Name = "Notification List")]
    [MaxLength(200)]
    public string NotificationList { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    public Dictionary<string, string> Categories { get; set; }

    public Dictionary<string, string> Frequencies { get; set; }
}

[ExcludeFromCodeCoverage]
public class NotificationConfigsViewModel
{
    public IEnumerable<NotificationConfigViewModel> Notifications { get; set; }
    public bool CanEdit { get; set; }
}