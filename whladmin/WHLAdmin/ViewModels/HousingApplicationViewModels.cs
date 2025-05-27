using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class HousingApplicationViewModel : HousingApplication
{
    public ListingViewModel ListingDetails { get; set; }
    public string DisplayName { get; set; }
    public string DisplayPhysicalAddress { get; set; }
    public string DisplayMailingAddress { get; set; }
    public string DisplayDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayLast4SSN { get { return (Last4SSN ?? "").Length > 0 ? $"XXX-XX-{Last4SSN.Trim()}" : ""; } }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("MM/dd/yyyy") : ""; } }
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayVouchers { get; set; }
    public Dictionary<string, string> UnitTypes { get; set; }
    public IEnumerable<HousingApplicantViewModel> ApplicationMembers { get; set; }
    public IEnumerable<HousingApplicantAssetViewModel> ApplicationAccounts { get; set; }
    public string DisplayLeadType { get; set; }
    public bool DocumentsReqdInd { get; set; }
    public IEnumerable<ApplicationDocumentViewModel> ApplicationDocuments { get; set; }
    public string DisplaySubmittedDate { get { return SubmittedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? SubmittedDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public bool IsPaperBased { get { return (SubmissionTypeCd ?? "").Trim().Equals("PAPER", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsDraft { get { return (StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsSubmitted { get { return (StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsWaitlisted { get { return (StatusCd ?? "").Trim().Equals("WAITLISTED", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsWithdrawn { get { return (StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsDuplicate { get { return (StatusCd ?? "").Trim().Equals("DUPLICATE", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsPotentialDuplicate { get { return (DuplicateCheckCd ?? "").Trim().Equals("P", StringComparison.CurrentCultureIgnoreCase); } }
}

[ExcludeFromCodeCoverage]
public class EditableHousingApplicationViewModel : ErrorViewModel
{
    public long ApplicationId { get; set; }

    [Display(Name = "Listing")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Listing is required")]
    public long ListingId { get; set; }

    // Primary Applicant
    [Display(Name = "Title")]
    [MaxLength(20)]
    public string Title { get; set; }

    [Display(Name = "First Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    [MaxLength(100)]
    public string MiddleName { get; set; }

    [Display(Name = "Last Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Display(Name = "Suffix")]
    [MaxLength(20)]
    public string Suffix { get; set; }

    [Display(Name = "Last 4 of SSN/ITIN")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last 4 of SSN/ITIN is required")]
    [MaxLength(4)]
    public string Last4SSN { get; set; }

    [Display(Name = "Date of Birth")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
    public string DateOfBirth { get; set; }

    [Display(Name = "ID Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Type is required")]
    public string IdTypeCd { get; set; }

    [Display(Name = "ID Number/Value")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Number is required")]
    [MaxLength(20)]
    public string IdTypeValue { get; set; }

    [Display(Name = "ID Issue Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Issue Date is required")]
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
    [MaxLength(20)]
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
    public string EmailAddress { get; set; }

    [Display(Name = "Alternate Email Address")]
    public string AltEmailAddress { get; set; }

    [Display(Name = "Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumberExtn { get; set; }

    [Display(Name = "Phone Number Type")]
    public string PhoneNumberTypeCd { get; set; }

    [Display(Name = "Alternate Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumber { get; set; }

    [Display(Name = "Alternate Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumberExtn { get; set; }

    [Display(Name = "Alternate Phone Number Type")]
    public string AltPhoneNumberTypeCd { get; set; }

    [Display(Name = "Do you own any real estate or shares in a Co-op?")]
    public bool OwnRealEstateInd { get; set; }

    [Display(Name = "Total value of all owned real estate or shares in a Co-op")]
    public long RealEstateValueAmt { get; set; }

    [Display(Name = "Total value of all assets owned other than real estate")]
    public long AssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long IncomeValueAmt { get; set; }

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

    // Vouchers
    [Display(Name = "Receive Vouchers?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if you or anyone in your household receive voucher(s)")]
    public bool VoucherInd { get; set; }

    [Display(Name = "Vouchers Types")]
    public string VoucherCds { get; set; }

    [Display(Name = "Please Specify")]
    public string VoucherOther { get; set; }

    [Display(Name = "Voucher Administrator")]
    public string VoucherAdminName { get; set; }

    // Co-Applicant
    [Display(Name = "Is there a Co-applicant?")]
    public bool CoApplicantInd { get; set; }

    [Display(Name = "Relationship Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Relationship Type is required")]
    public string CoRelationTypeCd { get; set; }

    [Display(Name = "Other Relationship Type")]
    public string CoRelationTypeOther { get; set; }

    [Display(Name = "Title")]
    [MaxLength(20)]
    public string CoTitle { get; set; }

    [Display(Name = "First Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
    [MaxLength(100)]
    public string CoFirstName { get; set; }

    [Display(Name = "Middle Name")]
    [MaxLength(100)]
    public string CoMiddleName { get; set; }

    [Display(Name = "Last Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
    [MaxLength(100)]
    public string CoLastName { get; set; }

    [Display(Name = "Suffix")]
    [MaxLength(20)]
    public string CoSuffix { get; set; }

    [Display(Name = "Last 4 of SSN/ITIN")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last 4 of SSN/ITIN is required")]
    [MaxLength(4)]
    public string CoLast4SSN { get; set; }

    [Display(Name = "Date of Birth")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
    public string CoDateOfBirth { get; set; }

    [Display(Name = "ID Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Type is required")]
    public string CoIdTypeCd { get; set; }

    [Display(Name = "ID Number/Value")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Number is required")]
    [MaxLength(20)]
    public string CoIdTypeValue { get; set; }

    [Display(Name = "ID Issue Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Issue Date is required")]
    public string CoIdIssueDate { get; set; }

    [Display(Name = "Gender")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Gender is required")]
    public string CoGenderCd { get; set; }

    [Display(Name = "Race")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Race is required")]
    public string CoRaceCd { get; set; }

    [Display(Name = "Ethnicity")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Ethnicity is required")]
    public string CoEthnicityCd { get; set; }

    [Display(Name = "Pronouns")]
    [MaxLength(20)]
    public string CoPronouns { get; set; }

    [Display(Name = "County Living In")]
    public string CoCountyLivingIn { get; set; }

    [Display(Name = "County Working In")]
    public string CoCountyWorkingIn { get; set; }

    [Display(Name = "Student?")]
    public bool CoStudentInd { get; set; }

    [Display(Name = "Disability?")]
    public bool CoDisabilityInd { get; set; }

    [Display(Name = "Veteran?")]
    public bool CoVeteranInd { get; set; }

    [Display(Name = "Have you ever lived in Westchester County?")]
    public bool CoEverLivedInWestchesterInd { get; set; }

    [Display(Name = "Do you currently work in Westchester County?")]
    public bool CoCurrentlyWorkingInWestchesterInd { get; set; }

    [Display(Name = "Primary Email Address")]
    public string CoEmailAddress { get; set; }

    [Display(Name = "Alternate Email Address")]
    public string CoAltEmailAddress { get; set; }

    [Display(Name = "Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string CoPhoneNumber { get; set; }

    [Display(Name = "Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string CoPhoneNumberExtn { get; set; }

    [Display(Name = "Phone Number Type")]
    public string CoPhoneNumberTypeCd { get; set; }

    [Display(Name = "Alternate Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string CoAltPhoneNumber { get; set; }

    [Display(Name = "Alternate Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string CoAltPhoneNumberExtn { get; set; }

    [Display(Name = "Alternate Phone Number Type")]
    public string CoAltPhoneNumberTypeCd { get; set; }

    [Display(Name = "Do you own any real estate or shares in a Co-op?")]
    public bool CoOwnRealEstateInd { get; set; }

    [Display(Name = "Total value of all owned real estate or shares in a Co-op")]
    public long CoRealEstateValueAmt { get; set; }

    [Display(Name = "Total value of all assets owned other than real estate")]
    public long CoAssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long CoIncomeValueAmt { get; set; }

    // Lead Type
    [Display(Name = "How did you hear about this opportunity")]
    public string LeadTypeCd { get; set; }

    [Display(Name = "Please specicy")]
    public string LeadTypeOther { get; set; }

    public string StatusCd { get; set; }

    public List<long> DuplicateApplicationIds { get; set; }
    public bool OverrideDuplicateWarning { get; set; }

    public ListingViewModel ListingDetails { get; set; }

    public Dictionary<string, string> IdTypes { get; set; }
    public Dictionary<string, string> GenderTypes { get; set; }
    public Dictionary<string, string> RaceTypes { get; set; }
    public Dictionary<string, string> EthnicityTypes { get; set; }
    public Dictionary<string, string> PhoneNumberTypes { get; set; }
    public Dictionary<string, string> VoucherTypes { get; set; }
    public Dictionary<string, string> RelationTypes { get; set; }
    public Dictionary<string, string> LeadTypes { get; set; }

    public string SubmissionTypeCd { get; set; }
    public string SubmissionTypeDescription { get; set; }
    public DateTime? SubmittedDate { get; set; }

    public string IdTypeDescription { get; set; }
    public string GenderDescription { get; set; }
    public string RaceDescription { get; set; }
    public string EthnicityDescription { get; set; }
    public string PhoneNumberTypeDescription { get; set; }
    public string AltPhoneNumberTypeDescription { get; set; }
    public string LeadTypeDescription { get; set; }
    public string StatusDescription { get; set; }
}


[ExcludeFromCodeCoverage]
public class EditableApplicantInfoViewModel : ErrorViewModel
{
    public long ApplicationId { get; set; }

    [Display(Name = "Listing")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Listing is required")]
    public long ListingId { get; set; }

    [Display(Name = "Relationship Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Relationship Type is required")]
    public string RelationTypeCd { get; set; }

    [Display(Name = "Other Relationship Type")]
    public string RelationTypeOther { get; set; }

    [Display(Name = "Title")]
    [MaxLength(20)]
    public string Title { get; set; }

    [Display(Name = "First Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "First Name is required")]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    [MaxLength(100)]
    public string MiddleName { get; set; }

    [Display(Name = "Last Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last Name is required")]
    [MaxLength(100)]
    public string LastName { get; set; }

    [Display(Name = "Suffix")]
    [MaxLength(20)]
    public string Suffix { get; set; }

    [Display(Name = "Last 4 of SSN/ITIN")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Last 4 of SSN/ITIN is required")]
    [MaxLength(4)]
    public string Last4SSN { get; set; }

    [Display(Name = "Date of Birth")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Date of Birth is required")]
    public string DateOfBirth { get; set; }

    [Display(Name = "ID Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Type is required")]
    public string IdTypeCd { get; set; }

    [Display(Name = "ID Number/Value")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Number is required")]
    [MaxLength(20)]
    public string IdTypeValue { get; set; }

    [Display(Name = "ID Issue Date")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Identification Issue Date is required")]
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
    [MaxLength(20)]
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
    public string EmailAddress { get; set; }

    [Display(Name = "Alternate Email Address")]
    public string AltEmailAddress { get; set; }

    [Display(Name = "Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumber { get; set; }

    [Display(Name = "Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string PhoneNumberExtn { get; set; }

    [Display(Name = "Phone Number Type")]
    public string PhoneNumberTypeCd { get; set; }

    [Display(Name = "Alternate Phone Number")]
    [MaxLength(10, ErrorMessage = "Phone Number must be 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumber { get; set; }

    [Display(Name = "Alternate Phone Number Extension")]
    [MaxLength(10, ErrorMessage = "Phone Number Extension can be a maximum of 10 digits without spaces, paranthesis or other special characters")]
    public string AltPhoneNumberExtn { get; set; }

    [Display(Name = "Alternate Phone Number Type")]
    public string AltPhoneNumberTypeCd { get; set; }

    [Display(Name = "Do you own any real estate or shares in a Co-op?")]
    public bool OwnRealEstateInd { get; set; }

    [Display(Name = "Total value of all owned real estate or shares in a Co-op")]
    public long RealEstateValueAmt { get; set; }

    [Display(Name = "Total value of all assets owned other than real estate")]
    public long AssetValueAmt { get; set; }

    [Display(Name = "Total Annual Gross Income")]
    public long IncomeValueAmt { get; set; }
}

[ExcludeFromCodeCoverage]
public class HousingApplicationsViewModel : PagedViewModel
{
    [Display(Name = "Listing")]
    public long ListingId { get; set; }

    [Display(Name = "Listing Name")]
    public string ListingName { get; set; }

    [Display(Name = "Listing Address")]
    public string ListingAddress { get; set; }

    [Display(Name = "Submission Type")]
    public string SubmissionTypeCd { get; set; }

    [Display(Name = "Application Status")]
    public string StatusCd { get; set; }

    public Dictionary<long, string> Listings { get; set; }
    public Dictionary<string, string> SubmissionTypes { get; set; }
    public Dictionary<string, string> Statuses { get; set; }
    public IEnumerable<HousingApplicationViewModel> HousingApplications { get; set; }

    public bool CanEdit { get; set; }
}

[ExcludeFromCodeCoverage]
public class DuplicateViewModel : DuplicateApplication
{
}

[ExcludeFromCodeCoverage]
public class DuplicateApplicationsViewModel : DuplicateViewModel
{
    public long ListingId { get; set; }
    public ListingViewModel ListingDetails { get; set; }
    public string TypeCd { get; set; }
    public string TypeDescription { get; set; }
    public string TypeValue { get; set; }
    public IEnumerable<HousingApplicationViewModel> Applications { get; set; }
}

[ExcludeFromCodeCoverage]
public class DuplicatesViewModel
{
    public Dictionary<long, string> Listings { get; set; }
    public long ListingId { get; set; }
    public IEnumerable<DuplicateViewModel> DuplicatesBySsn { get; set; }
    public IEnumerable<DuplicateViewModel> DuplicatesByName { get; set; }
    public IEnumerable<DuplicateViewModel> DuplicatesByEmailAddress { get; set; }
    public IEnumerable<DuplicateViewModel> DuplicatesByPhoneNumber { get; set; }
    public IEnumerable<DuplicateViewModel> DuplicatesByStreetAddress { get; set; }
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
public class HousingApplicantViewModel : HousingApplicant
{
    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get { return DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? DateOfBirth.Value.ToString("MM-dd-yyyy") : ""; } }
    public string DisplayIdIssueDate { get { return IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? IdIssueDate.Value.ToString("MM-dd-yyyy") : ""; } }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayRelation { get { return RelationTypeCd == "SELF" ? "Primary Applicant" : RelationTypeDescription; } }
    public int AccountCount { get; set; }
}

[ExcludeFromCodeCoverage]
public class HousingApplicantAssetViewModel : HousingApplicantAsset
{
    public string DisplayAccountType { get; set; }
    public string DisplayAccountNumber { get { return "|CASHAPP|CRYPTO|OTHER|".Contains($"|{(AccountTypeCd ?? "").Trim()}|") ? "" : $"XXXX-{(AccountNumber ?? "").Trim()}"; } }
}

[ExcludeFromCodeCoverage]
public class ApplicationDocumentViewModel : ApplicationDocument
{
}

[ExcludeFromCodeCoverage]
public class ApplicationDocumentsViewModel
{
    public IEnumerable<ApplicationDocumentViewModel> Documents { get; set; }
    public int Count { get { return Documents?.Count() ?? 0; } }
}

[ExcludeFromCodeCoverage]
public class EditableDuplicateApplicationViewModel
{
    public long ApplicationId { get; set; }
    public string DuplicateCheckCd { get; set; }
    public string DuplicateReason { get; set; }
    public string DuplicateCheckResponseDueDate { get; set; }
}

[ExcludeFromCodeCoverage]
public class ApplicationCommentViewModel : ApplicationComment
{
}

[ExcludeFromCodeCoverage]
public class ApplicationCommentsViewModel
{
    public long ApplicationId { get; set; }
    public IEnumerable<ApplicationCommentViewModel> Comments { get; set; }
    public int Count { get { return Comments?.Count() ?? 0; } }
}

[ExcludeFromCodeCoverage]
public class EditableApplicationCommentViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public long ListingId { get; set; }
    public long CommentId { get; set; }

    [Display(Name = "Comments")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Comment is required")]
    public string Comments { get; set; }

    [Display(Name = "Internal Only")]
    public bool InternalOnlyInd { get; set; }
}