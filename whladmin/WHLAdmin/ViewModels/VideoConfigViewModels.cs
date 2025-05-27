using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class VideoConfigViewModel : VideoConfig
{
}

[ExcludeFromCodeCoverage]
public class EditableVideoConfigViewModel
{
    public int VideoId { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Display(Name = "Description")]
    [MaxLength(1000)]
    public string Text { get; set; }

    [Display(Name = "Video Link")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Video Link is required")]
    [MaxLength(500)]
    public string Url { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Display On Home Page")]
    public bool DisplayOnHomePageInd { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class VideoConfigsViewModel
{
    public IEnumerable<VideoConfigViewModel> Videos { get; set; }
    public bool CanEdit { get; set; }
}