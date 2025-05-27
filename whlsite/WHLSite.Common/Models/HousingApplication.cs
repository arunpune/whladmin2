using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class HousingApplication : Listing
{
    public string ListingAddress { get; set; }

    public long ApplicationId { get; set; }
    public string Username { get; set; }
    public long HouseholdId { get; set; }

    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }

    public string GenderCd { get; set; }
    public string GenderDescription { get; set; }
    public string RaceCd { get; set; }
    public string RaceDescription { get; set; }
    public string EthnicityCd { get; set; }
    public string EthnicityDescription { get; set; }
    public string Pronouns { get; set; }

    public bool StudentInd { get; set; }
    public bool DisabilityInd { get; set; }
    public bool VeteranInd { get; set; }

    public bool EverLivedInWestchesterInd { get; set; }
    public string CountyLivingIn { get; set; }
    public bool CurrentlyWorkingInWestchesterInd { get; set; }
    public string CountyWorkingIn { get; set; }
    public string OccupationCd { get; set; }
    public string OccupationDescription { get; set; }
    public string OccupationOther { get; set; }

    public string LanguagePreferenceCd { get; set; }
    public string LanguagePreferenceDescription { get; set; }
    public string LanguagePreferenceOther { get; set; }

    public string ListingPreferenceCd { get; set; }
    public string ListingPreferenceDescription { get; set; }
    public bool SmsNotificationsPreferenceInd { get; set; }

    public bool OwnRealEstateInd { get; set; }
    public long RealEstateValueAmt { get; set; }
    public long AssetValueAmt { get; set; }
    public long IncomeValueAmt { get; set; }
    public long RentAndPaymentsAmt { get; set; }

    public string Last4SSN { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string IdTypeCd { get; set; }
    public string IdTypeDescription { get; set; }
    public string IdTypeValue { get; set; }
    public DateTime? IdIssueDate { get; set; }

    public string EmailAddress { get; set; }
    public string AltEmailAddress { get; set; }

    public string PhoneNumber { get; set; }
    public string PhoneNumberExtn { get; set; }
    public string PhoneNumberTypeCd { get; set; }
    public string PhoneNumberTypeDescription { get; set; }
    public string AltPhoneNumber { get; set; }
    public string AltPhoneNumberExtn { get; set; }
    public string AltPhoneNumberTypeCd { get; set; }
    public string AltPhoneNumberTypeDescription { get; set; }

    public bool AddressInd { get; set; }
    public string PhysicalStreetLine1 { get; set; }
    public string PhysicalStreetLine2 { get; set; }
    public string PhysicalStreetLine3 { get; set; }
    public string PhysicalCity { get; set; }
    public string PhysicalStateCd { get; set; }
    public string PhysicalZipCode { get; set; }
    public string PhysicalCounty { get; set; }
    public bool DifferentMailingAddressInd { get; set; }
    public string MailingStreetLine1 { get; set; }
    public string MailingStreetLine2 { get; set; }
    public string MailingStreetLine3 { get; set; }
    public string MailingCity { get; set; }
    public string MailingStateCd { get; set; }
    public string MailingZipCode { get; set; }
    public string MailingCounty { get; set; }
    public bool VoucherInd { get; set; }
    public string VoucherCds { get; set; }
    public string VoucherOther { get; set; }
    public string VoucherAdminName { get; set; }
    public bool LiveInAideInd { get; set; }

    public string LeadTypeCd { get; set; }
    public string LeadTypeDescription { get; set; }
    public string LeadTypeOther { get; set; }

    public string UnitTypeCds { get; set; }
    public string UnitTypeDescriptions { get; set; }

    public bool CoApplicantInd { get; set; }
    public long CoApplicantMemberId { get; set; }
    public string MemberIds { get; set; }
    public string AccountIds { get; set; }

    public DateTime? DueDate { get; set; }
    public DateTime? SubmittedDate { get; set; }
    public DateTime? WithdrawnDate { get; set; }
    public DateTime? ReceivedDate { get; set; }

    public string DuplicateCheckCd { get; set; }
    public DateTime? DuplicateCheckResponseDueDate { get; set; }

    public string LotteryNumber { get; set; }

    public bool UpdateProfileInd { get; set;}

    public bool DisqualifiedInd { get; set; }
    public string DisqualificationCd { get; set; }
    public string DisqualificationDescription { get; set; }
    public string DisqualificationOther { get; set; }
    public string DisqualificationReason { get; set; }

    public string AccessibilityCds { get; set; }
    public string AccessibilityDescriptions { get; set; }
}