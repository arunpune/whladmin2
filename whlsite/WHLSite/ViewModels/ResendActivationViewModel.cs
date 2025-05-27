using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ResendActivationViewModel : RecaptchaViewModel
{
    [Display(Name = "Username")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    public string Username { get; set; }
}