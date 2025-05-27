using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ChangePasswordViewModel : RecaptchaViewModel
{
    public string Username { get; set; }

    [Display(Name = "Current Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Current password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string CurrentPassword { get; set; }

    [Display(Name = "New Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string NewPassword { get; set; }

    [Display(Name = "Confirm Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Confirmation Password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string ConfirmationPassword { get; set; }

    public string ResetKey { get; set; }
}