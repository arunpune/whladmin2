using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class LoginHelpViewModel : RecaptchaViewModel
{
    public Dictionary<string, string> HelpTypes { get; set; }

    [Display(Name = "Help Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Help Type is required")]
    public string HelpTypeCd { get; set; }

    [Display(Name = "Username")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Display(Name = "Email Address")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is required")]
    public string EmailAddress { get; set; }
}