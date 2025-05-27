using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class FundingSourceViewModel : FundingSource
{
}

[ExcludeFromCodeCoverage]
public class EditableFundingSourceViewModel
{
    public int FundingSourceId { get; set; }

    [Display(Name = "Funding Source Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Funding Source Name is required")]
    [MaxLength(100)]
    public string FundingSourceName { get; set; }

    [Display(Name = "Funding Source Description")]
    [MaxLength(1000)]
    public string FundingSourceDescription { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class FundingSourcesViewModel
{
    public IEnumerable<FundingSourceViewModel> FundingSources { get; set; }
    public bool CanEdit { get; set; }
}
