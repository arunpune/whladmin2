using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class HousingApplication : Household
{
    public long ApplicationId { get; set; }
    public long ListingId { get; set; }
    public string Username { get; set; }
    public string StatusCd { get; set; }
    public string StatusDescription { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? ReceivedDate { get; set; }
    public DateTime? WithdrawnDate { get; set; }

    public string DuplicateCheckCd { get; set ;}
    public string DuplicateReason { get; set; }
    public DateTime? DuplicateCheckResponseDueDate { get; set; }

    public string LeadTypeCd { get; set; }
    public string LeadTypeDescription { get; set; }
    public string LeadTypeOther { get; set; }

    public string SubmissionTypeCd { get; set; }
    public string SubmissionTypeDescription { get; set; }

    public string ListingName { get; set; }
    public string ListingAddress { get; set; }

    public long LotteryId { get; set; }
    public DateTime? LotteryDate { get; set; }
    public string LotteryNumber { get; set; }

    public bool DisqualifiedInd { get; set; }
    public string DisqualificationCd { get; set; }
    public string DisqualificationDescription { get; set; }
    public string DisqualificationOther { get; set; }
    public string DisqualificationReason { get; set; }

    public List<HousingApplicant> Members { get; set; }
    public List<HousingApplicantAsset> Accounts { get; set; }

    public string UnitTypeCds { get; set; }
    public string MemberIds { get; set; }
    public int CoApplicantMemberId { get; set; }
    public string AccountIds { get; set; }
    public string AccessibilityCds { get; set; }
}

[ExcludeFromCodeCoverage]
public class HousingApplicant : HouseholdMember
{
    public long ApplicantId { get; set; }
    public long ApplicationId { get; set; }
    public bool CoApplicantInd { get; set; }
}

[ExcludeFromCodeCoverage]
public class HousingApplicantAsset : HouseholdAccount
{
    public long ApplicantId { get; set; }
    public long ApplicationId { get; set; }
}