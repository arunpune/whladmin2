using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class LogInViewModel : RecaptchaViewModel
{
    [Display(Name = "Username")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    [MinLength(8, ErrorMessage = "Username must be at least 8 characters")]
    public string Username { get; set; }

    [Display(Name = "Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string Password { get; set; }

    public string ReturnUrl { get; set; }
}