using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class UserAccount : UserProfile
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string ActivationKey { get; set; }
    public DateTime? ActivationKeyExpiry { get; set; }
    public bool UsernameVerifiedInd { get; set; }

    public string EmailAddress { get; set; }
    public bool AuthRepEmailAddressInd { get; set; }
    public string AltEmailAddress { get; set; }
    public string AltEmailAddressKey { get; set; }
    public DateTime? AltEmailAddressKeyExpiry { get; set; }
    public bool AltEmailAddressVerifiedInd { get; set; }

    public string PhoneNumber { get; set; }
    public string PhoneNumberExtn { get; set; }
    public string PhoneNumberTypeCd { get; set; }
    public string PhoneNumberTypeDescription { get; set; }
    public string PhoneNumberKey { get; set; }
    public DateTime? PhoneNumberKeyExpiry { get; set; }
    public bool PhoneNumberVerifiedInd { get; set; }

    public string AltPhoneNumber { get; set; }
    public string AltPhoneNumberExtn { get; set; }
    public string AltPhoneNumberTypeCd { get; set; }
    public string AltPhoneNumberTypeDescription { get; set; }
    public string AltPhoneNumberKey { get; set; }
    public DateTime? AltPhoneNumberKeyExpiry { get; set; }
    public bool AltPhoneNumberVerifiedInd { get; set; }

    public string LeadTypeCd { get; set; }
    public string LeadTypeDescription { get; set; }
    public string LeadTypeOther { get; set; }

    public DateTime? LastLoggedIn { get; set; }

    public string DeactivatedBy { get; set; }
    public DateTime? DeactivatedDate { get; set; }

    public string PasswordResetKey { get; set; }
    public DateTime? PasswordResetKeyExpiry { get; set; }
}