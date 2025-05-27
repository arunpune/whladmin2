using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class RegistrationViewModel : RecaptchaViewModel
{
    [Display(Name = "Username")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Username is required")]
    public string Username { get; set; }

    [Display(Name = "Email Address")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is required")]
    public string EmailAddress { get; set; }

    [Display(Name = "Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string Password { get; set; }

    [Display(Name = "Confirm Password")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Confirmation Password is required")]
    [MinLength(14, ErrorMessage = "Password must be at least 14 characters")]
    public string ConfirmationPassword { get; set; }

    [Display(Name = "Phone Number")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number is required")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumberExtn { get; set; }

    [Display(Name = "Phone Number Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Phone Number Type is required")]
    public string PhoneNumberTypeCd { get; set; }

    [Display(Name = "How did you hear about HomeSeeker?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Specify how you heard about us")]
    public string LeadTypeCd { get; set; }

    [Display(Name = "Please specify")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify")]
    public string LeadTypeOther { get; set; }

    [Display(Name = "I consent to receive email notifications")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Consent to receive email notifications is required")]
    public bool ConsentToReceiveEmailNotifications { get; set; }

    [Display(Name = "Accept Terms and Conditions")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Accept Terms and Conditions is required")]
    public bool AcceptTermsAndConditions { get; set; }

    [Display(Name = "Authorized Representative")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "This email address is for the registrant's authorized representative to receive notifications")]
    public bool AuthRepEmailAddressInd { get; set; }

    public Dictionary<string, string> PhoneNumberTypes { get; set; }
    public Dictionary<string, string> LeadTypes { get; set; }
}