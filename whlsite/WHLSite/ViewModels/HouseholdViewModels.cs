using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;
using WHLSite.Common.Settings;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class HouseholdViewModel : Household
{
    public bool IsIncomplete { get; set; }
    public string DisplayVouchers { get; set; }
    public List<HouseholdMemberViewModel> Members { get; set; }
    public List<HouseholdAccountViewModel> Accounts { get; set; }
}

[ExcludeFromCodeCoverage]
public class HouseholdMemberViewModel : HouseholdMember
{
    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayRelation { get; set; }
    public int AccountCount { get; set; }

    public string EditorDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("yyyy-MM-dd") : ""; } }
    public string EditorIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("yyyy-MM-dd") : ""; } }
}

[ExcludeFromCodeCoverage]
public class HouseholdAccountViewModel : HouseholdAccount
{
    public string DisplayAccountType { get; set; }
    public string DisplayAccountNumber { get { return "|CASHAPP|CRYPTO|OTHER|".Contains($"|{(AccountTypeCd ?? "").Trim()}|") ? "" : $"XXXX-{(AccountNumber ?? "").Trim()}"; } }
}

[ExcludeFromCodeCoverage]
public class EditableVoucherInfoViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    [Display(Name = "Receive Vouchers?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if you or anyone in your household receive voucher(s)")]
    public bool VoucherInd { get; set; }

    [Display(Name = "Vouchers Types")]
    public string VoucherCds { get; set; }

    [Display(Name = "Please Specify")]
    public string VoucherOther { get; set; }

    [Display(Name = "Voucher Administrator")]
    public string VoucherAdminName { get; set; }

    public Dictionary<string, string> VoucherTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class AddressViewModel
{
    public string StreetLine1 { get; set; }
    public string StreetLine2 { get; set; }
    public string StreetLine3 { get; set; }
    public string City { get; set; }
    public string StateCd { get; set; }
    public string ZipCode { get; set; }
    public string County { get; set; }
    public string CountyDescription { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableAddressInfoViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    // Physical Address
    [Display(Name = "Do you have a current physical address?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if you or anyone in your household has a current physical address")]
    public bool AddressInd { get; set; }

    [Display(Name = "Street Line 1")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify street address")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string PhysicalStreetLine1 { get; set; }

    [Display(Name = "Street Line 2")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string PhysicalStreetLine2 { get; set; }

    [Display(Name = "Street Line 3")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string PhysicalStreetLine3 { get; set; }

    [Display(Name = "City")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify city")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string PhysicalCity { get; set; }

    [Display(Name = "State")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify 2-character state")]
    [Length(2, 2, ErrorMessage = "Must be a valid 2-character domestic US state code")]
    public string PhysicalStateCd { get; set; }

    [Display(Name = "Zip Code")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify 5-digit zip code")]
    [Length(5, 5, ErrorMessage = "Must be a valid 5-digit domestic US zip code")]
    public string PhysicalZipCode { get; set; }

    [Display(Name = "County")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string PhysicalCounty { get; set; }

    // Mailing Address
    [Display(Name = "Do you have a different mailing address?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if you or anyone in your household has a different mailing address")]
    public bool DifferentMailingAddressInd { get; set; }

    [Display(Name = "Street Line 1")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify street address")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string MailingStreetLine1 { get; set; }

    [Display(Name = "Street Line 2")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string MailingStreetLine2 { get; set; }

    [Display(Name = "Street Line 3")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string MailingStreetLine3 { get; set; }

    [Display(Name = "City")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify city")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string MailingCity { get; set; }

    [Display(Name = "State")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify 2-character state")]
    [Length(2, 2, ErrorMessage = "Must be a valid 2-character domestic US state code")]
    public string MailingStateCd { get; set; }

    [Display(Name = "Zip Code")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify 5-digit zip code")]
    [Length(5, 5, ErrorMessage = "Must be a valid 5-digit domestic US zip code")]
    public string MailingZipCode { get; set; }

    [Display(Name = "County")]
    [MaxLength(100, ErrorMessage = "Can be a maximum of 100 characters")]
    public string MailingCounty { get; set; }

    public Dictionary<string, string> Counties { get; set; }

    public Dictionary<string, string> UsStates { get; set; }

    public ArcGisSettings ArcGisSettings { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableHouseholdMemberViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long HouseholdId { get; set; }
    public long MemberId { get; set; }

    [Display(Name = "Relationship Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Relationship Type is required")]
    public string RelationTypeCd { get; set; }

    [Display(Name = "Other Relationship Type")]
    public string RelationTypeOther { get; set; }

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
    public string Last4SSN { get; set; }

    [Display(Name = "Date of Birth")]
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

    [Display(Name = "Email Address")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email Address is required")]
    public string EmailAddress { get; set; }

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

    [Display(Name = "Own Real Estate or Shares in a Co-op?")]
    public bool OwnRealEstateInd { get; set; }

    [Display(Name = "Total Value of all Owned Real Estate or Shares in a Co-op")]
    public long RealEstateValueAmt { get; set; }

    [Display(Name = "Total Value of Assets")]
    public long AssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long IncomeValueAmt { get; set; }

    public Dictionary<string, string> RelationTypes { get; set; }
    public Dictionary<string, string> GenderTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
    public Dictionary<string, string> PhoneNumberTypes { get; set; }
    public Dictionary<string, string> IdTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableHouseholdAccountViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long HouseholdId { get; set; }
    public long AccountId { get; set; }

    [Display(Name = "Account or Asset Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Account or Asset Type is required")]
    public string AccountTypeCd { get; set; }

    [Display(Name = "Other Account or Asset Type")]
    public string AccountTypeOther { get; set; }

    [Display(Name = "Last 4 of Account Number")]
    [MaxLength(4)]
    public string AccountNumber { get; set; }

    [Display(Name = "Name of Institution")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Name of Institution is required")]
    [MaxLength(200)]
    public string InstitutionName { get; set; }

    [Display(Name = "Value of Account")]
    public long AccountValueAmt { get; set; }

    [Display(Name = "Primary Holder")]
    public long PrimaryHolderMemberId { get; set; }

    public Dictionary<string, string> AccountTypes { get; set; }

    public Dictionary<long, string> Members { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableLiveInAideInfoViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    [Display(Name = "Have a Live-in Aide?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if your household has a live-in aide")]
    public bool LiveInAideInd { get; set; }
}