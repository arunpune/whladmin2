using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class AmiConfigViewModel : AmiConfig
{
    public List<AmiHhPctAmt> HhPctAmts { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableAmiConfigViewModel
{
    [Display(Name = "Effective Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Effective Date is required")]
    [MaxLength(10)]
    public string EffectiveDate { get; set; }

    [Display(Name = "Effective Year")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Effective Year is required")]
    public int EffectiveYear { get; set; }

    [Display(Name = "4P Household AMI")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "4 Person Household AMI is required")]
    public long IncomeAmt { get; set; }

    public List<AmiHhPctAmt> HhPctAmts { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
    public string Mode { get; set; }
}

[ExcludeFromCodeCoverage]
public class AmiHhPctAmt
{
    public int Size { get; set ;}
    public int Pct { get; set ;}
    public long Amt { get; set ;}
}

[ExcludeFromCodeCoverage]
public class AmiConfigsViewModel
{
    public IEnumerable<AmiConfigViewModel> Amis { get; set; }
    public bool CanEdit { get; set; }
}