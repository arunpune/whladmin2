using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class HousingApplicationViewModel : HousingApplication
{
    public bool Editable { get; set; }
    public bool CanWithdraw { get; set; }
    public bool HasComments { get { return (ApplicationComments?.Count() ?? 0) > 0; } }
    public bool CanComment { get; set; }
    public bool ForWaitlist { get; set; }
    public string DisplaySubmittedDate { get { return SubmittedDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? SubmittedDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayWithdrawnDate { get { return WithdrawnDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? WithdrawnDate.Value.ToString("MM/dd/yyyy h:mm tt") : ""; } }
    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get; set; }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayIdIssueDate { get; set;}
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayVouchers { get; set; }
    public string DisplayLeadType { get; set; }
    public bool DocumentsReqdInd { get; set; }
    public IEnumerable<ApplicationDocumentViewModel> ApplicationDocuments { get; set; }
    public IEnumerable<ApplicationCommentViewModel> ApplicationComments { get; set; }
    public ListingViewModel ListingDetails { get; set;}
    public Dictionary<string, string> UnitTypes { get; set;}
    public IEnumerable<HouseholdMemberViewModel> Members { get; set;}
    public IEnumerable<HouseholdAccountViewModel> Accounts { get; set;}
    public bool IsDraft { get { return (StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsSubmitted { get { return (StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsWaitlisted { get { return (StatusCd ?? "").Trim().Equals("WAITLISTED", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsWithdrawn { get { return (StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsExpired { get { return (StatusCd ?? "").Trim().Equals("EXPIRED", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsDuplicate { get { return (StatusCd ?? "").Trim().Equals("DUPLICATE", StringComparison.CurrentCultureIgnoreCase); } }
    public bool IsPotentialDuplicate { get { return (DuplicateCheckCd ?? "").Trim().Equals("P", StringComparison.CurrentCultureIgnoreCase); } }
    public bool DisplayRailroad { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableHousingApplicationViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public int StepNumber { get; set; }

    public ListingViewModel ListingDetails { get; set;}
    public ProfileViewModel ProfileDetails { get; set;}
    public HouseholdViewModel HouseholdDetails { get; set;}
    public HousingApplicationViewModel ApplicationDetails { get; set;}

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
    public string AccountIds { get; set; }

    public string AccessibilityCds { get; set; }

    [Display(Name = "How did you hear about HomeSeeker?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Specify how you heard about us")]
    public string LeadTypeCd { get; set; }
    [Display(Name = "Please specify")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify")]
    public string LeadTypeOther { get; set; }
    public Dictionary<string, string> LeadTypes { get; set; }

    public bool Editable { get; set; }
    public bool CanWithdraw { get; set; }
    public bool ForWaitlist { get; set; }

    public string DisplayName { get; set; }
    public string DisplayDateOfBirth { get; set; }
    public string DisplayIdTypeValue { get; set; }
    public string DisplayIdIssueDate { get; set;}
    public string DisplayPhoneNumber { get; set; }
    public string DisplayAltPhoneNumber { get; set; }
    public string DisplayVouchers { get; set; }
    public string DisplayLeadType { get; set; }
}

[ExcludeFromCodeCoverage]
public class EditableApplicantInfoViewModel : EditableProfileViewModel
{
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }

    public ListingViewModel ListingDetails { get; set; }
    public ProfileViewModel ProfileDetails { get; set; }

    [Display(Name = "Save changes to Profile")]
    public bool UpdateProfileInd { get; set; }

    public bool Editable { get; set; }

    public string SaveMode { get; set; }

    public int StepNumber { get { return 1; } }
}

[ExcludeFromCodeCoverage]
public class EditableHouseholdInfoViewModel : EditableAddressInfoViewModel
{
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }

    [Display(Name = "Which units are you applying for?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify the unit(s) you are applying for")]
    public string UnitTypeCds { get; set; }
    public Dictionary<string, string> UnitTypes { get; set; }


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

    [Display(Name = "Have a Live-in Aide?")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Please specify if your household has a live-in aide")]
    public bool LiveInAideInd { get; set; }

    public ListingViewModel ListingDetails { get; set; }
    public HouseholdViewModel HouseholdDetails { get; set; }

    [Display(Name = "Save changes to Profile")]
    public bool UpdateProfileInd { get; set; }

    public bool Editable { get; set; }

    public string SaveMode { get; set; }

    public int StepNumber { get { return 2; } }
}

[ExcludeFromCodeCoverage]
public class EditableAdditionalMembersInfoViewModel : ErrorViewModel
{
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    public bool CoApplicantInd { get; set; }
    public long CoApplicantMemberId { get; set; }
    public Dictionary<long, string> CoApplicants { get; set; }

    public string MemberIds { get; set; }

    public ListingViewModel ListingDetails { get; set; }
    public IEnumerable<HouseholdMemberViewModel> HouseholdMembers { get; set; }

    public bool Editable { get; set; }

    public string SaveMode { get; set; }

    public int StepNumber { get { return 3; } }
}

[ExcludeFromCodeCoverage]
public class EditableIncomeAssetsInfoViewModel : ErrorViewModel
{
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    public long TotalIncomeValueAmt { get; set; }
    public long TotalAssetValueAmt { get; set; }
    public long TotalRealEstateValueAmt { get; set; }

    public string AccountIds { get; set; }

    public ListingViewModel ListingDetails { get; set; }
    public IEnumerable<HouseholdAccountViewModel> HouseholdAccounts { get; set; }
    public IEnumerable<HouseholdMemberViewModel> HouseholdMembers { get; set; }

    public bool Editable { get; set; }

    public string SaveMode { get; set; }

    public int StepNumber { get { return 4; } }
}

[ExcludeFromCodeCoverage]
public class HousingApplicationReviewSubmitViewModel : ErrorViewModel
{
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    public ListingViewModel ListingDetails { get; set; }

    public EditableApplicantInfoViewModel ApplicantInfo { get; set; }
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

    public EditableHouseholdInfoViewModel HouseholdInfo { get; set; }
    public string DisplayVouchers { get; set; }

    public EditableAdditionalMembersInfoViewModel AdditionalMembersInfo { get; set; }
    public List<HouseholdMemberViewModel> Members { get; set; }

    public EditableIncomeAssetsInfoViewModel IncomeAssetsInfo { get; set; }
    public List<HouseholdAccountViewModel> Accounts { get; set; }

    public bool DocumentsReqdInd { get; set; }
    public Dictionary<string, string> DocumentTypes { get; set; }
    public IEnumerable<ApplicationDocumentViewModel> Documents { get; set; }

    public bool Editable { get; set; }
    public bool CanWithdraw { get; set; }

    public string SaveMode { get; set; }

    public int StepNumber { get { return 5; } }
}

[ExcludeFromCodeCoverage]
public class RailroadViewModel
{
    public int StepNumber { set; get; }
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public string Username { get; set; }
    public bool Complete { get; set; }

    public RailroadViewModel(string username, long applicationId, int listingId, int stepNumber)
    {
        ApplicationId = applicationId;
        ListingId = listingId;
        StepNumber = stepNumber;
        Username = username;
    }

    public RailroadViewModel(string username, long applicationId, int listingId, int stepNumber, bool complete)
    {
        ApplicationId = applicationId;
        ListingId = listingId;
        StepNumber = stepNumber;
        Username = username;
        Complete = complete;
    }
}

[ExcludeFromCodeCoverage]
public class ApplicationActionsViewModel : RailroadViewModel
{
    public string SaveMode { set; get; }
    public bool CanEdit { set; get; }
    public bool CanWithdraw { set; get; }
    public bool DocumentsReqdInd { get; set; }
    public bool HasComments { get; set; }

    public ApplicationActionsViewModel(string username, long applicationId, int listingId, int stepNumber)
        : base(username, applicationId, listingId, stepNumber) { }

    public ApplicationActionsViewModel(string username, long applicationId, int listingId, int stepNumber, bool documentsReqdInd = false, bool canWithdraw = false, bool hasComments = false)
        : base(username, applicationId, listingId, stepNumber)
    {
        DocumentsReqdInd = documentsReqdInd;
        CanWithdraw = canWithdraw;
        HasComments = hasComments;
    }
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
public class EditableApplicationDocumentViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public long DocId { get; set; }
    public byte[] DocContents { get; set; }

    [Display(Name = "Document Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Document type is required")]
    public int DocTypeId { get; set; }

    [Display(Name = "Document Name")]
    public string DocName { get; set; }

    [Display(Name = "File Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "File is required")]
    public string FileName { get; set; }

    [Display(Name = "File Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "File is required")]
    public IFormFile File { get; set; }

    public IEnumerable<DocumentTypeViewModel> DocumentTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class ApplicationCommentViewModel : ApplicationComment
{
}

[ExcludeFromCodeCoverage]
public class ApplicationCommentsViewModel
{
    public IEnumerable<ApplicationCommentViewModel> Comments { get; set; }
    public int Count { get { return Comments?.Count() ?? 0; } }
}

[ExcludeFromCodeCoverage]
public class EditableApplicationCommentViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long ApplicationId { get; set; }
    public int ListingId { get; set; }
    public long CommentId { get; set; }

    [Display(Name = "Comments")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Comment is required")]
    public string Comments { get; set; }
}