using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class AmortizationViewModel : Amortization
{
}

[ExcludeFromCodeCoverage]
public class EditableAmortizationViewModel
{
    [Display(Name = "Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate { get; set; }

    [Display(Name = "Interest Only Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Interest Only Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal RateInterestOnly { get; set; }

    [Display(Name = "10 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "10 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate10Year { get; set; }

    [Display(Name = "15 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "15 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate15Year { get; set; }

    [Display(Name = "20 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "20 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate20Year { get; set; }

    [Display(Name = "25 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "25 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate25Year { get; set; }

    [Display(Name = "30 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "30 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate30Year { get; set; }

    [Display(Name = "40 Year Rate")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "40 Year Rate is required")]
    [Range(0, 100, ErrorMessage = "Rate must be between 0 and 100")]
    public decimal Rate40Year { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    public string Mode { get; set; }
}

[ExcludeFromCodeCoverage]
public class AmortizationsViewModel
{
    public IEnumerable<AmortizationViewModel> Amortizations { get; set; }
    public bool CanEdit { get; set; }
}