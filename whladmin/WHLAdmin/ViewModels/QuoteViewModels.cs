using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class QuoteConfigViewModel : QuoteConfig
{
}

[ExcludeFromCodeCoverage]
public class EditableQuoteConfigViewModel
{
    public int QuoteId { get; set; }

    [Display(Name = "Quote")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Quote is required")]
    [MaxLength(100)]
    public string Text { get; set; }

    [Display(Name = "Display On Home Page")]
    public bool DisplayOnHomePageInd { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class QuoteConfigsViewModel
{
    public IEnumerable<QuoteConfigViewModel> Quotes { get; set; }
    public bool CanEdit { get; set; }
}
