using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class HAViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public int StepNumber { get; set; }

    public ListingViewModel ListingDetails { get; set;}
    public ProfileViewModel ProfileDetails { get; set;}
    public HouseholdViewModel HouseholdDetails { get; set;}

    [Display(Name = "Which units are you applying for?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify the unit(s) you are applying for")]
    public string UnitTypeCds { get; set; }
    public Dictionary<string, string> UnitTypes { get; set; }

    public IEnumerable<HouseholdMemberViewModel> Members { get; set; }
    [Display(Name = "Is there a Co-Applicant?")]
    public bool CoApplicantInd { get; set; }
    [Display(Name = "Co-Applicant")]
    public long CoApplicantMemberId { get; set; }
    public Dictionary<long, string> CoApplicants { get; set; }
    public string MemberIds { get; set; }

    public IEnumerable<HouseholdAccountViewModel> Accounts { get; set; }

    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get; set; }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayIdIssueDate { get; set; }
    public string PhoneNumberTypeDescription { get; set; }
    public string DisplayPhoneNumber { get; set; }
    public string AltPhoneNumberTypeDescription { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string GenderDescription { get; set; }
    public string RaceDescription { get; set; }
    public string EthnicityDescription { get; set; }

    public string DisplayVouchers { get; set; }

    public bool DocumentsReqdInd { get; set; }
    public IEnumerable<ApplicationDocumentViewModel> Documents { get; set; }

    public bool Editable { get; set; }
    public bool CanWithdraw { get; set; }
}