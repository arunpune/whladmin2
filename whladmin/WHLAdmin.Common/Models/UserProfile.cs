using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class UserProfile : ModelBase
{
    public string Title { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Suffix { get; set; }

    public string Last4SSN { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string IdTypeCd { get; set; }
    public string IdTypeDescription { get; set; }
    public string IdTypeValue { get; set; }
    public DateTime? IdIssueDate { get; set; }

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
}