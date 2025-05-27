using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IProfileService
{
    bool IsIncompleteProfile(string requestId, string correlationId, ProfileViewModel model);
    Task<ProfileViewModel> GetOne(string requestId, string correlationId, string username);
    Task AssignMetadata(EditableProfileViewModel model);
    Task<EditableProfileViewModel> GetForProfileInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveProfileInfo(string requestId, string correlationId, string username, EditableProfileViewModel model);
    Task<EditableAddressInfoViewModel> GetForAddressInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveAddressInfo(string requestId, string correlationId, string username, EditableAddressInfoViewModel model);
    Task<Dictionary<string, string>> GetCountiesByStateCd(string requestId, string correlationId, string username, string stateCd);
    Task<EditableProfileViewModel> GetForContactInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveContactInfo(string requestId, string correlationId, string username, EditableProfileViewModel model);
    Task AssignMetadata(EditablePreferencesViewModel model);
    Task<EditablePreferencesViewModel> GetForPreferencesInfoEdit(string requestId, string correlationId, string username);
    Task<string> SavePreferencesInfo(string requestId, string correlationId, string username, EditablePreferencesViewModel preferences);
    Task<EditableNetWorthViewModel> GetForNetWorthInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveNetWorthInfo(string requestId, string correlationId, string username, EditableNetWorthViewModel netWorth);
    Task<UserNotificationsViewModel> GetNotifications(string requestId, string correlationId, string username, string filterTypeCd = null);
    Task<string> UpdateNotification(string requestId, string correlationId, string username, EditableUserNotificationViewModel notification);
    Task<UserDocumentsViewModel> GetDocuments(string requestId, string correlationId, string username);
    Task<UserDocumentViewModel> GetDocument(string requestId, string correlationId, string username, long docId);
    Task<EditableUserDocumentViewModel> GetForAddDocument(string requestId, string correlationId, string username);
    Task<string> AddDocument(string requestId, string correlationId, string username, EditableUserDocumentViewModel model);
    Task<string> DeleteDocument(string requestId, string correlationId, string username, long docId);

    Task<string> ValidateProfile(string requestId, string correlationId, EditableProfileViewModel model);
    Task<string> ValidateContactInformation(string requestId, string correlationId, EditableProfileViewModel model);
}

public class ProfileService : IProfileService
{
    private readonly ILogger<ProfileService> _logger;
    private readonly IProfileRepository _profileRepository;
    private readonly IMasterConfigService _configService;
    private readonly IEmailService _emailService;
    private readonly IKeyService _keyService;
    private readonly IMetadataService _metadataService;
    private readonly IPhoneService _phoneService;
    private readonly IUiHelperService _uiHelperService;

    public ProfileService(ILogger<ProfileService> logger, IProfileRepository profileRepository
                            , IMasterConfigService configRepository, IEmailService emailService
                            , IKeyService keyService, IMetadataService metadataService
                            , IPhoneService phoneService, IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _profileRepository = profileRepository ?? throw new ArgumentNullException(nameof(profileRepository));
        _configService = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _keyService = keyService ?? throw new ArgumentNullException(nameof(keyService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _phoneService = phoneService ?? throw new ArgumentNullException(nameof(phoneService));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
    }

    public bool IsIncompleteProfile(string requestId, string correlationId, ProfileViewModel model)
    {
        if (model == null) return false;

        model.FirstName = (model.FirstName ?? "").Trim();
        model.LastName = (model.LastName ?? "").Trim();
        model.Last4SSN = (model.Last4SSN ?? "").Trim();
        model.GenderCd = (model.GenderCd ?? "").Trim();
        model.RaceCd = (model.RaceCd ?? "").Trim();
        model.EthnicityCd = (model.EthnicityCd ?? "").Trim();

        model.PhysicalStreetLine1 = (model.PhysicalStreetLine1 ?? "").Trim();
        model.PhysicalCity = (model.PhysicalCity ?? "").Trim();
        model.PhysicalStateCd = (model.PhysicalStateCd ?? "").Trim();
        model.PhysicalZipCode = (model.PhysicalZipCode ?? "").Trim();
        model.PhysicalCounty = (model.PhysicalCounty ?? "").Trim();

        model.PhoneNumber = (model.PhoneNumber ?? "").Trim();
        model.PhoneNumberTypeCd = (model.PhoneNumberTypeCd ?? "").Trim();

        return string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName)
            || string.IsNullOrEmpty(model.DisplayDateOfBirth) || string.IsNullOrEmpty(model.Last4SSN)
                || string.IsNullOrEmpty(model.DisplayIdTypeValue)
            || string.IsNullOrEmpty(model.GenderCd) || string.IsNullOrEmpty(model.RaceCd) || string.IsNullOrEmpty(model.EthnicityCd)
            || string.IsNullOrEmpty(model.PhysicalStreetLine1) || string.IsNullOrEmpty(model.PhysicalCity)
            || string.IsNullOrEmpty(model.PhysicalStateCd) || string.IsNullOrEmpty(model.PhysicalZipCode)
            || string.IsNullOrEmpty(model.PhysicalCounty)
            || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.PhoneNumberTypeCd);
    }

    public async Task<ProfileViewModel> GetOne(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for view";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = profile.ToProfileViewModel();
        model.DisplayName = _uiHelperService.ToDisplayName(model.Title, model.FirstName, model.MiddleName, model.LastName, model.Suffix);
        model.DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(model.IdTypeDescription, model.IdTypeValue);
        model.DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(model.PhoneNumber);
        model.DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(model.AltPhoneNumber);
        model.DisplayLanguagePreference = _uiHelperService.ToOtherAndValueText(model.LanguagePreferenceCd, model.LanguagePreferenceDescription, model.LanguagePreferenceOther);
        model.IsIncomplete = IsIncompleteProfile(requestId, correlationId, model);

        var documents = await GetDocuments(requestId, correlationId, username);
        model.Documents = documents.Documents;

        return model;
    }

    public async Task AssignMetadata(EditableProfileViewModel model)
    {
        if (model == null) return;

        model.IdTypes = await _metadataService.GetIdTypes(true) ?? [];
        model.GenderTypes = await _metadataService.GetGenderTypes(true) ?? [];
        model.RaceTypes = await _metadataService.GetRaceTypes(true) ?? [];
        model.EthnicityTypes = await _metadataService.GetEthnicityTypes(true) ?? [];
        model.PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true) ?? [];
    }

    public async Task<EditableProfileViewModel> GetForProfileInfoEdit(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for edit";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditableProfileViewModel()
        {
            Username = profile.Username,
            AltEmailAddress = profile.AltEmailAddress,
            PhoneNumber = profile.PhoneNumber,
            Title = profile.Title,
            FirstName = profile.FirstName,
            MiddleName = profile.MiddleName,
            LastName = profile.LastName,
            Suffix = profile.Suffix,
            Last4SSN = profile.Last4SSN,
            DateOfBirth = profile.DateOfBirth.ToDateEditorStringFormat(),
            IdTypeCd = profile.IdTypeCd,
            IdTypeValue = profile.IdTypeValue,
            IdIssueDate = profile.IdIssueDate.ToDateEditorStringFormat(),
            GenderCd = profile.GenderCd ?? "NOTSPEC",
            RaceCd = profile.RaceCd ?? "NOTSPEC",
            EthnicityCd = profile.EthnicityCd ?? "NOTSPEC",
            Pronouns = profile.Pronouns,
            CountyLivingIn = profile.CountyLivingIn,
            CountyWorkingIn = profile.CountyWorkingIn,
            StudentInd = profile.StudentInd,
            DisabilityInd = profile.DisabilityInd,
            VeteranInd = profile.VeteranInd,
            EverLivedInWestchesterInd = profile.EverLivedInWestchesterInd,
            CurrentlyWorkingInWestchesterInd = profile.CurrentlyWorkingInWestchesterInd,
            HouseholdSize = profile.HouseholdSize
        };

        await AssignMetadata(model);
        return model;
    }

    public async Task<string> SaveProfileInfo(string requestId, string correlationId, string username, EditableProfileViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to update profile";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "P101";
        }

        var account = await _profileRepository.GetOne(username);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        var validationCode = await ValidateProfile(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Profile validation failed");
            return validationCode;
        }

        var profile = new UserAccount()
        {
            Username = username,
            Title = model.Title,
            FirstName = model.FirstName,
            MiddleName = string.IsNullOrEmpty(model.MiddleName ?? "") ? null : model.MiddleName,
            LastName = model.LastName,
            Suffix = string.IsNullOrEmpty(model.Suffix ?? "") ? null : model.Suffix,
            Last4SSN = model.Last4SSN,
            DateOfBirth = model.DateOfBirth.FromDateEditorStringFormat(),
            IdTypeCd = model.IdTypeCd,
            IdTypeValue = model.IdTypeValue,
            IdIssueDate = model.IdIssueDate.FromDateEditorStringFormat(),
            GenderCd = model.GenderCd,
            RaceCd = model.RaceCd,
            EthnicityCd = model.EthnicityCd,
            Pronouns = string.IsNullOrEmpty(model.Pronouns ?? "") ? null : model.Pronouns,
            CountyLivingIn = string.IsNullOrEmpty(model.CountyLivingIn ?? "") ? null : model.CountyLivingIn,
            CountyWorkingIn = string.IsNullOrEmpty(model.CountyWorkingIn ?? "") ? null : model.CountyWorkingIn,
            StudentInd = model.StudentInd,
            DisabilityInd = model.DisabilityInd,
            VeteranInd = model.VeteranInd,
            EverLivedInWestchesterInd = model.EverLivedInWestchesterInd,
            CurrentlyWorkingInWestchesterInd = model.CurrentlyWorkingInWestchesterInd,
            HouseholdSize = model.HouseholdSize <= 0 ? 1 : model.HouseholdSize,
            ModifiedBy = username
        };

        var updated = await _profileRepository.UpdateProfile(requestId, correlationId, profile);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "P004";
        }

        return "";
    }

    public async Task<EditableAddressInfoViewModel> GetForAddressInfoEdit(string requestId, string correlationId, string username)
    {
        var profile = await _profileRepository.GetOne(username);
        if (profile == null) return null;

        var model = new EditableAddressInfoViewModel()
        {
            Username = profile.Username,
            AddressInd = profile.AddressInd,
            PhysicalStreetLine1 = profile.PhysicalStreetLine1,
            PhysicalStreetLine2 = profile.PhysicalStreetLine2,
            PhysicalStreetLine3 = profile.PhysicalStreetLine3,
            PhysicalCity = profile.PhysicalCity,
            PhysicalStateCd = profile.PhysicalStateCd,
            PhysicalZipCode = profile.PhysicalZipCode,
            PhysicalCounty = profile.PhysicalCounty,
            DifferentMailingAddressInd = profile.DifferentMailingAddressInd,
            MailingStreetLine1 = profile.MailingStreetLine1,
            MailingStreetLine2 = profile.MailingStreetLine2,
            MailingStreetLine3 = profile.MailingStreetLine3,
            MailingCity = profile.MailingCity,
            MailingStateCd = profile.MailingStateCd,
            MailingZipCode = profile.MailingZipCode,
            MailingCounty = profile.MailingCounty,
            Counties = await _metadataService.GetCounties(true),
            UsStates = await _metadataService.GetUsStates(true),
            ArcGisSettings = await _configService.GetArcGisSettings()
        };

        return model;
    }

    public async Task<string> SaveAddressInfo(string requestId, string correlationId, string username, EditableAddressInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to update address information";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _profileRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var validationCode = ValidateAddressInfo(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Address validation failed");
            return validationCode;
        }

        var addressInfo = new Household()
        {
            HouseholdId = model.HouseholdId,
            Username = username,
            AddressInd = model.AddressInd,
            PhysicalStreetLine1 = model.PhysicalStreetLine1,
            PhysicalStreetLine2 = model.PhysicalStreetLine2,
            PhysicalStreetLine3 = model.PhysicalStreetLine3,
            PhysicalCity = model.PhysicalCity,
            PhysicalStateCd = model.PhysicalStateCd,
            PhysicalZipCode = model.PhysicalZipCode,
            PhysicalCounty = model.PhysicalCounty,
            DifferentMailingAddressInd = model.DifferentMailingAddressInd,
            MailingStreetLine1 = model.MailingStreetLine1,
            MailingStreetLine2 = model.MailingStreetLine2,
            MailingStreetLine3 = model.MailingStreetLine3,
            MailingCity = model.MailingCity,
            MailingStateCd = model.MailingStateCd,
            MailingZipCode = model.MailingZipCode,
            MailingCounty = model.MailingCounty,
            ModifiedBy = username
        };

        var updated = await _profileRepository.UpdateAddressInfo(requestId, correlationId, addressInfo);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H004";
        }

        return "";
    }

    public async Task<Dictionary<string, string>> GetCountiesByStateCd(string requestId, string correlationId, string username, string stateCd)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get counties by state for address info edit";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        stateCd = (stateCd ?? "").Trim().ToUpper();
        var counties = await _metadataService.GetCountiesByStateCd(stateCd, true);
        return counties;
    }

    public async Task<EditableProfileViewModel> GetForContactInfoEdit(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for contact info edit";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditableProfileViewModel()
        {
            Username = profile.Username,
            PhoneNumber = profile.PhoneNumber,
            PhoneNumberExtn = profile.PhoneNumberExtn,
            PhoneNumberTypeCd = profile.PhoneNumberTypeCd,
            AltPhoneNumber = profile.AltPhoneNumber,
            AltPhoneNumberExtn = profile.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = profile.AltPhoneNumberTypeCd,
            EmailAddress = profile.EmailAddress,
            AltEmailAddress = profile.AltEmailAddress,
            AuthRepEmailAddressInd = profile.AuthRepEmailAddressInd
        };
        await AssignMetadata(model);
        return model;
    }

    public async Task<string> SaveContactInfo(string requestId, string correlationId, string username, EditableProfileViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to update contact information";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "P101";
        }

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        model.Username = username;
        var validationCode = await ValidateContactInformation(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Profile validation failed");
            return validationCode;
        }

        var contactInformation = new UserAccount()
        {
            Username = username,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberExtn = model.PhoneNumberExtn,
            PhoneNumberTypeCd = model.PhoneNumberTypeCd,
            AltPhoneNumber = model.AltPhoneNumber,
            AltPhoneNumberExtn = model.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = model?.AltPhoneNumberTypeCd,
            EmailAddress = model.EmailAddress,
            AltEmailAddress = model.AltEmailAddress,
            AuthRepEmailAddressInd = model.AuthRepEmailAddressInd,
            ModifiedBy = username
        };

        profile.AltEmailAddress = (profile.AltEmailAddress ?? "").Trim();
        if (profile.AltEmailAddress != model.AltEmailAddress)
        {
            if (model.AltEmailAddress.Length > 0)
            {
                contactInformation.AltEmailAddress = model.AltEmailAddress;
                contactInformation.AltEmailAddressKey = _keyService.GetActivationKey();
                contactInformation.AltEmailAddressKeyExpiry = DateTime.Now.AddMinutes(10);
                contactInformation.AltEmailAddressVerifiedInd = false;
            }
            else
            {
                contactInformation.AltEmailAddress = null;
                contactInformation.AltEmailAddressKey = null;
                contactInformation.AltEmailAddressKeyExpiry = null;
                contactInformation.AltEmailAddressVerifiedInd = false;
            }
        }
        else
        {
            contactInformation.AltEmailAddress = profile.AltEmailAddress;
            contactInformation.AltEmailAddressKey = profile.AltEmailAddressKey;
            contactInformation.AltEmailAddressKeyExpiry = profile.AltEmailAddressKeyExpiry;
            contactInformation.AltEmailAddressVerifiedInd = profile.AltEmailAddressVerifiedInd;
        }

        if (string.IsNullOrEmpty(contactInformation.PhoneNumberExtn)) contactInformation.PhoneNumberExtn = null;
        if (string.IsNullOrEmpty(contactInformation.PhoneNumber))
        {
            contactInformation.PhoneNumber = null;
            contactInformation.PhoneNumberExtn = null;
            contactInformation.PhoneNumberKey = null;
            contactInformation.PhoneNumberKeyExpiry = null;
            contactInformation.PhoneNumberVerifiedInd = false;
        }

        if (string.IsNullOrEmpty(contactInformation.AltPhoneNumberExtn)) contactInformation.AltPhoneNumberExtn = null;
        if (string.IsNullOrEmpty(contactInformation.AltPhoneNumber))
        {
            contactInformation.AltPhoneNumber = null;
            contactInformation.AltPhoneNumberExtn = null;
            contactInformation.AltPhoneNumberKey = null;
            contactInformation.AltPhoneNumberKeyExpiry = null;
            contactInformation.AltPhoneNumberVerifiedInd = false;
        }

        var updated = await _profileRepository.UpdateContactInfo(requestId, correlationId, contactInformation);
        if (!updated)
        {
            _logger.LogError($"Unable to update contact information - System or database exception");
            return "P004";
        }

        return "";
    }

    public async Task AssignMetadata(EditablePreferencesViewModel model)
    {
        if (model == null) return;

        model.LanguageTypes = await _metadataService.GetLanguages(true) ?? [];
        model.ListingTypes = await _metadataService.GetListingTypes(true) ?? [];
    }

    public async Task<EditablePreferencesViewModel> GetForPreferencesInfoEdit(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for preferences info edit";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditablePreferencesViewModel()
        {
            LanguagePreferenceCd = profile.LanguagePreferenceCd,
            LanguagePreferenceOther = profile.LanguagePreferenceOther,
            ListingPreferenceCd = profile.ListingPreferenceCd,
            SmsNotificationsPreferenceInd = profile.SmsNotificationsPreferenceInd,
            CanChangeSmsPreferences = false
            // CanChangeSmsPreferences = (profile.PhoneNumberTypeCd ?? "").Trim().ToUpper().Equals("CELL", StringComparison.CurrentCultureIgnoreCase)
            //                             || (profile.AltPhoneNumberTypeCd ?? "").Trim().ToUpper().Equals("CELL", StringComparison.CurrentCultureIgnoreCase)
        };
        await AssignMetadata(model);
        return model;
    }

    public async Task<string> SavePreferencesInfo(string requestId, string correlationId, string username, EditablePreferencesViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to update preferences";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "P101";
        }

        var account = await _profileRepository.GetOne(username);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        model.LanguagePreferenceCd = (model.LanguagePreferenceCd ?? "").Trim();
        model.LanguagePreferenceOther = (model.LanguagePreferenceOther ?? "").Trim();
        model.ListingPreferenceCd = (model.ListingPreferenceCd ?? "").Trim();

        var languageTypes = await _metadataService.GetLanguages() ?? [];
        if (!languageTypes.ContainsKey(model.LanguagePreferenceCd))
        {
            _logger.LogError($"{logPrefix} - Language Type Preference is invalid");
            return "P311";
        }
        if (model.LanguagePreferenceCd.Equals("OTHER", StringComparison.CurrentCultureIgnoreCase)
                && string.IsNullOrEmpty(model.LanguagePreferenceOther))
        {
            _logger.LogError($"{logPrefix} - Language Type Other is required");
            return "P312";
        }

        var listingTypes = await _metadataService.GetListingTypes() ?? [];
        if (!listingTypes.ContainsKey(model.ListingPreferenceCd))
        {
            _logger.LogError($"{logPrefix} - Listing Type Preference is invalid");
            return "P313";
        }

        var accountPreference = new UserAccount()
        {
            Username = username,
            LanguagePreferenceCd = model.LanguagePreferenceCd,
            LanguagePreferenceOther = string.IsNullOrEmpty(model.LanguagePreferenceOther) ? null : model.LanguagePreferenceOther,
            ListingPreferenceCd = model.ListingPreferenceCd,
            SmsNotificationsPreferenceInd = model.SmsNotificationsPreferenceInd,
            ModifiedBy = username
        };

        var updated = await _profileRepository.UpdatePreferences(requestId, correlationId, accountPreference);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "P304";
        }

        return "";
    }

    public async Task<EditableNetWorthViewModel> GetForNetWorthInfoEdit(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for preferences info edit";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditableNetWorthViewModel()
        {
            AssetValueAmt = profile.AssetValueAmt,
            IncomeValueAmt = profile.IncomeValueAmt,
            OwnRealEstateInd = profile.OwnRealEstateInd,
            RealEstateValueAmt = profile.RealEstateValueAmt
        };
        return model;
    }

    public async Task<string> SaveNetWorthInfo(string requestId, string correlationId, string username, EditableNetWorthViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to update net worth";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "P101";
        }

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        if (model.OwnRealEstateInd)
        {
            model.RealEstateValueAmt = model.RealEstateValueAmt < 0L ? 0L : model.RealEstateValueAmt;
        }
        else
        {
            model.RealEstateValueAmt = 0L;
        }
        model.AssetValueAmt = model.AssetValueAmt < 0L ? 0L : model.AssetValueAmt;
        model.IncomeValueAmt = model.IncomeValueAmt < 0L ? 0L : model.IncomeValueAmt;

        var netWorth = new UserAccount()
        {
            Username = username,
            OwnRealEstateInd = model.OwnRealEstateInd,
            RealEstateValueAmt = model.OwnRealEstateInd ? model.RealEstateValueAmt : 0L,
            AssetValueAmt = model.AssetValueAmt,
            IncomeValueAmt = model.IncomeValueAmt,
            ModifiedBy = username
        };

        var updated = await _profileRepository.UpdateNetWorth(requestId, correlationId, netWorth);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "P404";
        }

        return "";
    }

    public async Task<UserNotificationsViewModel> GetNotifications(string requestId, string correlationId, string username, string filterTypeCd = null)
    {
        var notifications = await _profileRepository.GetNotifications(requestId, correlationId, username, filterTypeCd) ?? [];
        var model = new UserNotificationsViewModel()
        {
            Notifications = notifications.Select(s => s.ToViewModel())
        };
        return model;
    }

    public async Task<string> UpdateNotification(string requestId, string correlationId, string username, EditableUserNotificationViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to update user notification";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "N101";
        }

        if (model.NotificationId <= 0 && model.NotificationId != -999)
        {
            _logger.LogError($"{logPrefix} - Invalid Notification ID");
            return "N102";
        }

        model.Action = (model.Action ?? "").Trim().ToUpper();
        if (!"|R|D|".Contains($"|{model.Action}|"))
        {
            _logger.LogError($"{logPrefix} - Invalid Action");
            return "N103";
        }

        var notification = new UserNotification()
        {
            Username = username,
            NotificationId = model.NotificationId,
            ModifiedBy = username
        };

        if (model.Action.Equals("D"))
        {
            notification.Active = false;
            var deleted = await _profileRepository.DeleteNotification(requestId, correlationId, notification);
            if (!deleted)
            {
                _logger.LogError($"{logPrefix} - System or database exception");
                return "N005";
            }
        }
        else
        {
            notification.ReadInd = true;
            var updated = await _profileRepository.UpdateNotification(requestId, correlationId, notification);
            if (!updated)
            {
                _logger.LogError($"{logPrefix} - System or database exception");
                return "N004";
            }
        }

        return "";
    }

    public async Task<UserDocumentsViewModel> GetDocuments(string requestId, string correlationId, string username)
    {
        var documents = await _profileRepository.GetDocuments(requestId, correlationId, username) ?? [];
        var model = new UserDocumentsViewModel()
        {
            Documents = documents.Select(s => s.ToViewModel())
        };
        return model;
    }

    public async Task<UserDocumentViewModel> GetDocument(string requestId, string correlationId, string username, long docId)
    {
        var document = await _profileRepository.GetDocument(requestId, correlationId, username, docId);
        if (document != null)
        {
            document.DocContents = await _profileRepository.GetDocumentContents(requestId, correlationId, username, docId);
        }
        return document.ToViewModel();
    }

    public async Task<EditableUserDocumentViewModel> GetForAddDocument(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get profile for add document";

        var profile = await _profileRepository.GetOne(username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditableUserDocumentViewModel()
        {
            DocId = 0,
            DocTypeCd = "OTHER",
            DocName = "",
            DocumentTypes = await _metadataService.GetDocumentTypes(),
            FileName = "",
        };
        return model;
    }

    public async Task<string> AddDocument(string requestId, string correlationId, string username, EditableUserDocumentViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to add user document";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "D101";
        }

        model.DocTypeCd = (model.DocTypeCd ?? "").Trim().ToUpper();
        model.DocumentTypes = await _metadataService.GetDocumentTypes() ?? [];
        if (!model.DocumentTypes.ContainsKey(model.DocTypeCd))
        {
            _logger.LogError($"{logPrefix} - Document Type is invalid");
            return "D102";
        }

        model.FileName = (model.FileName ?? "").Trim();
        if (string.IsNullOrEmpty(model.FileName))
        {
            _logger.LogError($"{logPrefix} - File is required");
            return "D103";
        }

        if ((model.DocContents?.Length ?? 0) <= 0)
        {
            _logger.LogError($"{logPrefix} - File is invalid");
            return "D104";
        }

        var fileNameExtension = (Path.GetExtension(model.FileName) ?? "").Replace(".", "").ToUpper();
        var allowedFileTypes = await _metadataService.GetAllowedFileTypes() ?? [];
        if (!allowedFileTypes.ContainsKey(fileNameExtension))
        {
            _logger.LogError($"{logPrefix} - File Type is not allowed");
            return "D105";
        }

        var mimeType = "application/octet-stream";
        switch (fileNameExtension)
        {
            case "PDF": mimeType = "application/pdf"; break;
            case "PNG": mimeType = "image/png"; break;
            case "JPEG": mimeType = "image/jpeg"; break;
            case "JPG": mimeType = "image/jpg"; break;
        }

        var document = new UserDocument()
        {
            Username = username,
            DocTypeCd = model.DocTypeCd,
            DocName = model.DocName,
            DocContents = model.DocContents,
            FileName = model.FileName,
            MimeType = mimeType,
            ModifiedBy = username
        };

        var documents = await _profileRepository.GetDocuments(requestId, correlationId, username);
        var duplicate = documents.FirstOrDefault(f => f.DocTypeCd == document.DocTypeCd
                                                        && f.DocName.Equals(document.DocName, StringComparison.CurrentCultureIgnoreCase)
                                                        && f.FileName.Equals(document.FileName, StringComparison.CurrentCultureIgnoreCase));
        if (duplicate != null)
        {
            _logger.LogError($"{logPrefix} - Duplicate document");
            return "D002";
        }

        var added = await _profileRepository.AddDocument(requestId, correlationId, document);
        if (!added)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "D003";
        }

        return "";
    }

    public async Task<string> DeleteDocument(string requestId, string correlationId, string username, long docId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to add user document";

        if (docId <= 0)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "D101";
        }

        var document = await _profileRepository.GetDocument(requestId, correlationId, username, docId);
        if (document == null)
        {
            _logger.LogError($"{logPrefix} - Document not found");
            return "D001";
        }

        var deleted = await _profileRepository.DeleteDocument(requestId, correlationId, document);
        if (!deleted)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "D005";
        }

        return "";
    }

    public async Task<string> ValidateProfile(string requestId, string correlationId, EditableProfileViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to validate profile";

        model.Title = (model.Title ?? "").Trim();
        model.FirstName = (model.FirstName ?? "").Trim();
        model.MiddleName = (model.MiddleName ?? "").Trim();
        model.LastName = (model.LastName ?? "").Trim();
        model.Suffix = (model.Suffix ?? "").Trim();
        model.Last4SSN = (model.Last4SSN ?? "").Trim();
        model.DateOfBirth = (model.DateOfBirth ?? "").Trim();
        model.IdTypeCd = (model.IdTypeCd ?? "").Trim();
        model.IdTypeValue = (model.IdTypeValue ?? "").Trim();
        model.IdIssueDate = (model.IdIssueDate ?? "").Trim();
        model.GenderCd = (model.GenderCd ?? "").Trim();
        model.RaceCd = (model.RaceCd ?? "").Trim();
        model.EthnicityCd = (model.EthnicityCd ?? "").Trim();
        model.Pronouns = (model.Pronouns ?? "").Trim();
        model.CountyLivingIn = (model.CountyLivingIn ?? "").Trim();
        model.CountyWorkingIn = (model.CountyWorkingIn ?? "").Trim();
        model.HouseholdSize = model.HouseholdSize <= 0 ? 1 : model.HouseholdSize;

        if (string.IsNullOrEmpty(model.FirstName))
        {
            _logger.LogError($"{logPrefix} - First Name is required");
            return "P102";
        }
        if (!_keyService.IsValidNameWithSpecialCharacters(model.FirstName))
        {
            _logger.LogError($"{logPrefix} - First Name is invalid");
            return "P1021";
        }

        if (!string.IsNullOrEmpty(model.MiddleName) && !_keyService.IsValidName(model.MiddleName))
        {
            _logger.LogError($"{logPrefix} - Middle Name is invalid");
            return "P1022";
        }

        if (string.IsNullOrEmpty(model.LastName))
        {
            _logger.LogError($"{logPrefix} - Last Name is required");
            return "P103";
        }
        if (!_keyService.IsValidNameWithSpecialCharacters(model.LastName))
        {
            _logger.LogError($"{logPrefix} - Last Name is invalid");
            return "P1031";
        }

        if (!string.IsNullOrEmpty(model.Suffix) && !_keyService.IsValidName(model.Suffix))
        {
            _logger.LogError($"{logPrefix} - Suffix is invalid");
            return "P1032";
        }

        if (string.IsNullOrEmpty(model.Last4SSN))
        {
            _logger.LogError($"{logPrefix} - Last 4 of SSN/ITIN is required");
            return "P104";
        }

        if (!DateTime.TryParse(model.DateOfBirth, out var dob) || dob.Date < new DateTime(1900, 1, 1).Date)
        {
            _logger.LogError($"{logPrefix} - Date of Birth is required, must be on or after 01/01/1900");
            return "P105";
        }

        DateTime? idIssueDate = null;
        if (!string.IsNullOrEmpty(model.IdTypeCd))
        {
            var idTypes = await _metadataService.GetIdTypes() ?? [];
            if (!idTypes.ContainsKey(model.IdTypeCd))
            {
                _logger.LogError($"{logPrefix} - ID Type is invalid");
                return "P106";
            }

            if (string.IsNullOrEmpty(model.IdTypeValue))
            {
                _logger.LogError($"{logPrefix} - ID Value is invalid");
                return "P107";
            }

            if (DateTime.TryParse(model.IdIssueDate, out var iid))
            {
                idIssueDate = iid;
                if (iid.Date < new DateTime(1900, 1, 1).Date)
                {
                    _logger.LogError($"{logPrefix} - ID Issue Date must be on or after 01/01/1900");
                    return "P108";
                }
            }
        }

        var genderTypes = await _metadataService.GetGenderTypes() ?? [];
        if (!genderTypes.ContainsKey(model.GenderCd))
        {
            _logger.LogError($"{logPrefix} - Gender Type is invalid");
            return "P109";
        }

        var raceTypes = await _metadataService.GetRaceTypes() ?? [];
        if (!raceTypes.ContainsKey(model.RaceCd))
        {
            _logger.LogError($"{logPrefix} - Race Type is invalid");
            return "P110";
        }

        var ethnicityTypes = await _metadataService.GetEthnicityTypes() ?? [];
        if (!ethnicityTypes.ContainsKey(model.EthnicityCd))
        {
            _logger.LogError($"{logPrefix} - Ethnicity Type is invalid");
            return "P111";
        }

        return "";
    }

    public string ValidateAddressInfo(string requestId, string correlationId, EditableAddressInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to validate address information";

        model.PhysicalStreetLine1 = (model.PhysicalStreetLine1 ?? "").Trim();
        model.PhysicalStreetLine2 = (model.PhysicalStreetLine2 ?? "").Trim();
        model.PhysicalStreetLine3 = (model.PhysicalStreetLine3 ?? "").Trim();
        model.PhysicalCity = (model.PhysicalCity ?? "").Trim();
        model.PhysicalStateCd = (model.PhysicalStateCd ?? "").Trim();
        model.PhysicalZipCode = (model.PhysicalZipCode ?? "").Trim();
        model.PhysicalCounty = (model.PhysicalCounty ?? "").Trim();
        model.MailingStreetLine1 = (model.MailingStreetLine1 ?? "").Trim();
        model.MailingStreetLine2 = (model.MailingStreetLine2 ?? "").Trim();
        model.MailingStreetLine3 = (model.MailingStreetLine3 ?? "").Trim();
        model.MailingCity = (model.MailingCity ?? "").Trim();
        model.MailingStateCd = (model.MailingStateCd ?? "").Trim();
        model.MailingZipCode = (model.MailingZipCode ?? "").Trim();
        model.MailingCounty = (model.MailingCounty ?? "").Trim();

        if (!model.AddressInd)
        {
            model.PhysicalStreetLine1 = null;
            model.PhysicalStreetLine2 = null;
            model.PhysicalStreetLine3 = null;
            model.PhysicalCity = null;
            model.PhysicalStateCd = null;
            model.PhysicalZipCode = null;
            model.PhysicalCounty = null;
            model.DifferentMailingAddressInd = false;
            model.MailingStreetLine1 = null;
            model.MailingStreetLine2 = null;
            model.MailingStreetLine3 = null;
            model.MailingCity = null;
            model.MailingStateCd = null;
            model.MailingZipCode = null;
            model.MailingCounty = null;
            return "";
        }

        if (string.IsNullOrEmpty(model.PhysicalStreetLine1))
        {
            _logger.LogError($"{logPrefix} - Physical Street is required");
            return "H102";
        }

        if (string.IsNullOrEmpty(model.PhysicalStreetLine2)) model.PhysicalStreetLine2 = null;
        if (string.IsNullOrEmpty(model.PhysicalStreetLine3)) model.PhysicalStreetLine3 = null;

        if (string.IsNullOrEmpty(model.PhysicalCity))
        {
            _logger.LogError($"{logPrefix} - Physical City is required");
            return "H103";
        }

        if (string.IsNullOrEmpty(model.PhysicalStateCd))
        {
            _logger.LogError($"{logPrefix} - Physical State is required");
            return "H104";
        }

        if (string.IsNullOrEmpty(model.PhysicalZipCode))
        {
            _logger.LogError($"{logPrefix} - Physical Zip Code is required");
            return "H105";
        }

        if (string.IsNullOrEmpty(model.PhysicalCounty))
        {
            _logger.LogError($"{logPrefix} - Physical County is required");
            return "H106";
        }

        if (!model.DifferentMailingAddressInd)
        {
            model.MailingStreetLine1 = null;
            model.MailingStreetLine2 = null;
            model.MailingStreetLine3 = null;
            model.MailingCity = null;
            model.MailingStateCd = null;
            model.MailingZipCode = null;
            model.MailingCounty = null;
            return "";
        }

        if (string.IsNullOrEmpty(model.MailingStreetLine1))
        {
            _logger.LogError($"{logPrefix} - Mailing Street is required");
            return "H112";
        }

        if (string.IsNullOrEmpty(model.MailingStreetLine2)) model.MailingStreetLine2 = null;
        if (string.IsNullOrEmpty(model.MailingStreetLine3)) model.MailingStreetLine3 = null;

        if (string.IsNullOrEmpty(model.MailingCity))
        {
            _logger.LogError($"{logPrefix} - Mailing City is required");
            return "H113";
        }

        if (string.IsNullOrEmpty(model.MailingStateCd))
        {
            _logger.LogError($"{logPrefix} - Mailing State is required");
            return "H114";
        }

        if (string.IsNullOrEmpty(model.MailingZipCode))
        {
            _logger.LogError($"{logPrefix} - Mailing Zip Code is required");
            return "H115";
        }

        if (string.IsNullOrEmpty(model.MailingZipCode))
        {
            _logger.LogError($"{logPrefix} - Mailing County is required");
            return "H116";
        }

        return "";
    }

    public async Task<string> ValidateContactInformation(string requestId, string correlationId, EditableProfileViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to validate contact information";

        model.EmailAddress = (model.EmailAddress ?? "").Trim();
        model.AltEmailAddress = (model.AltEmailAddress ?? "").Trim();
        model.PhoneNumber = (model.PhoneNumber ?? "").Trim();
        model.PhoneNumberExtn = (model.PhoneNumberExtn ?? "").Trim();
        model.PhoneNumberTypeCd = (model.PhoneNumberTypeCd ?? "").Trim();
        model.AltPhoneNumber = (model.AltPhoneNumber ?? "").Trim();
        model.AltPhoneNumberExtn = (model.AltPhoneNumberExtn ?? "").Trim();
        model.AltPhoneNumberTypeCd = (model.AltPhoneNumberTypeCd ?? "").Trim();

        if (!_phoneService.IsValidPhoneNumber(model.PhoneNumber))
        {
            _logger.LogError($"{logPrefix} - Phone Number is invalid");
            return "P211";
        }

        var phoneNumberTypes = await _metadataService.GetPhoneNumberTypes() ?? [];
        if (!phoneNumberTypes.ContainsKey(model.PhoneNumberTypeCd))
        {
            _logger.LogError($"{logPrefix} - Phone Number Type is invalid");
            return "P212";
        }

        if (!string.IsNullOrEmpty(model.AltPhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(model.AltPhoneNumber))
            {
                _logger.LogError($"{logPrefix} - Alternate Phone Number is invalid");
                return "P213";
            }

            if (!phoneNumberTypes.ContainsKey(model.AltPhoneNumberTypeCd))
            {
                _logger.LogError($"{logPrefix} - Alternate Phone Number Type is invalid");
                return "P214";
            }
        }

        if (!_emailService.IsValidEmailAddress(requestId, correlationId, model.EmailAddress))
        {
            _logger.LogError($"{logPrefix} - Email Address is invalid");
            return "P215";
        }

        if (!string.IsNullOrEmpty(model.AltEmailAddress))
        {
            if (!_emailService.IsValidEmailAddress(requestId, correlationId, model.AltEmailAddress))
            {
                _logger.LogError($"{logPrefix} - Alternate Email Address is invalid");
                return "P216";
            }
            if (model.EmailAddress.Equals(model.AltEmailAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogError($"{logPrefix} - Alternate Email Address cannot must be different from the primary email address");
                return "P217";
            }
        }

        return "";
    }
}