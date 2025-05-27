using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class FaqConfigViewModel : FaqConfig
{
}

[ExcludeFromCodeCoverage]
public class EditableFaqConfigViewModel
{
    public int FaqId { get; set; }

    [Display(Name = "Category")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required")]
    [MaxLength(100)]
    public string CategoryName { get; set; }

    [Display(Name = "Title")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Title is required")]
    [MaxLength(200)]
    public string Title { get; set; }

    [Display(Name = "Text")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Text is required")]
    [MaxLength(4000)]
    public string Text { get; set; }

    [Display(Name = "Link 1")]
    [MaxLength(500)]
    public string Url { get; set; }

    [Display(Name = "Link 2")]
    [MaxLength(500)]
    public string Url1 { get; set; }

    [Display(Name = "Link 3")]
    [MaxLength(500)]
    public string Url2 { get; set; }

    [Display(Name = "Link 4")]
    [MaxLength(500)]
    public string Url3 { get; set; }

    [Display(Name = "Link 5")]
    [MaxLength(500)]
    public string Url4 { get; set; }

    [Display(Name = "Link 6")]
    [MaxLength(500)]
    public string Url5 { get; set; }

    [Display(Name = "Link 7")]
    [MaxLength(500)]
    public string Url6 { get; set; }

    [Display(Name = "Link 8")]
    [MaxLength(500)]
    public string Url7 { get; set; }

    [Display(Name = "Link 9")]
    [MaxLength(500)]
    public string Url8 { get; set; }

    [Display(Name = "Link 10")]
    [MaxLength(500)]
    public string Url9 { get; set; }

    [Display(Name = "Display Order")]
    public int DisplayOrder { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class FaqConfigsViewModel
{
    public IEnumerable<FaqConfigViewModel> Faqs { get; set; }
    public bool CanEdit { get; set; }
}