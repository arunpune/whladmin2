using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class ResourceConfigViewModel : ResourceConfig
{
}

[ExcludeFromCodeCoverage]
public class EditableResourceConfigViewModel
{
    public int ResourceId { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Display(Name = "Text")]
    [MaxLength(4000)]
    public string Text { get; set; }

    [Display(Name = "Website")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Website is required")]
    [MaxLength(500)]
    public string Url { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class ResourceConfigsViewModel
{
    public IEnumerable<ResourceConfigViewModel> Resources { get; set; }
    public bool CanEdit { get; set; }
}