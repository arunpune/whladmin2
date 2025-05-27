using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class ProfileViewModel : AccountViewModel
{
    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayLanguagePreference { get; set; }
    public bool IsIncomplete { get; set; }

    public IEnumerable<UserDocumentViewModel> Documents { get; set; }
    public int DocumentsCount { get { return Documents?.Count() ?? 0; } }

    public string EditorDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("yyyy-MM-dd") : ""; } }
    public string EditorIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("yyyy-MM-dd") : ""; } }
}

[ExcludeFromCodeCoverage]
public class EditableProfileViewModel : ErrorViewModel
{
    public string Username { get; set; }

    [Display(Name = "Email Address")]
    public string EmailAddress { get; set; }

    [Display(Name = "Authorized Representative")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "This email address is for the registrant's authorized representative to receive notifications")]
    public bool AuthRepEmailAddressInd { get; set; }

    [Display(Name = "Alternate Email Address")]
    public string AltEmailAddress { get; set; }

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

    [Display(Name = "Alternate Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumber { get; set; }

    [Display(Name = "Alternate Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumberExtn { get; set; }

    [Display(Name = "Alternate Phone Number Type")]
    public string AltPhoneNumberTypeCd { get; set; }

    [Display(Name = "Title")]
    public string Title { get; set; }

    [Display(Name = "First Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    public string MiddleName { get; set; }

    [Display(Name = "Last Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }

    [Display(Name = "Suffix")]
    public string Suffix { get; set; }

    [Display(Name = "Last 4 of SSN/ITIN")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last 4 of SSN/ITIN is required")]
    public string Last4SSN { get; set; }

    [Display(Name = "Date of Birth")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required in the format MM/dd/yyyy")]
    public string DateOfBirth { get; set; }

    [Display(Name = "ID Type")]
    public string IdTypeCd { get; set; }

    [Display(Name = "ID Number")]
    public string IdTypeValue { get; set; }

    [Display(Name = "ID Issue Date")]
    public string IdIssueDate { get; set; }

    [Display(Name = "Gender")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is required")]
    public string GenderCd { get; set; }

    [Display(Name = "Race")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Race is required")]
    public string RaceCd { get; set; }

    [Display(Name = "Ethnicity")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Ethnicity is required")]
    public string EthnicityCd { get; set; }

    [Display(Name = "Pronouns")]
    public string Pronouns { get; set; }

    [Display(Name = "County Living In")]
    public string CountyLivingIn { get; set; }

    [Display(Name = "County Working In")]
    public string CountyWorkingIn { get; set; }

    [Display(Name = "Student?")]
    public bool StudentInd { get; set; }

    [Display(Name = "Disability?")]
    public bool DisabilityInd { get; set; }

    [Display(Name = "Veteran?")]
    public bool VeteranInd { get; set; }

    [Display(Name = "Have you ever lived in Westchester County?")]
    public bool EverLivedInWestchesterInd { get; set; }

    [Display(Name = "Do you currently work in Westchester County?")]
    public bool CurrentlyWorkingInWestchesterInd { get; set; }

    [Display(Name = "Do you own any real estate or shares in a Co-op?")]
    public bool OwnRealEstateInd { get; set; }

    [Display(Name = "Total value of all owned real estate or shares in a Co-op")]
    public long RealEstateValueAmt { get; set; }

    [Display(Name = "Total value of all assets owned other than real estate")]
    public long AssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long IncomeValueAmt { get; set; }

    [Display(Name = "Size of Household")]
    public int HouseholdSize { get; set; }

    public Dictionary<string, string> IdTypes { get; set; }
    public Dictionary<string, string> GenderTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
    public Dictionary<string, string> PhoneNumberTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditablePreferencesViewModel : ErrorViewModel
{
    public string Username { get; set; }

    [Display(Name = "Preferred Language")]
    public string LanguagePreferenceCd { get; set; }

    [Display(Name = "Please Specify")]
    public string LanguagePreferenceOther { get; set; }

    [Display(Name = "Listings Interested In")]
    public string ListingPreferenceCd { get; set; }

    [Display(Name = "SMS Notifications?")]
    public bool SmsNotificationsPreferenceInd { get; set; }

    public bool CanChangeSmsPreferences { get; set; }

    public Dictionary<string, string> LanguageTypes { get; set; }
    public Dictionary<string, string> ListingTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableNetWorthViewModel : ErrorViewModel
{
    public string Username { get; set; }

    [Display(Name = "Do you own any real estate or shares in a Co-op?")]
    public bool OwnRealEstateInd { get; set; }

    [Display(Name = "Total value of all owned real estate or shares in a Co-op")]
    public long RealEstateValueAmt { get; set; }

    [Display(Name = "Total value of all assets owned other than real estate")]
    public long AssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long IncomeValueAmt { get; set; }
}