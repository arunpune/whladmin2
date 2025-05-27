using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IHouseholdService
{
    bool IsIncompleteHousehold(string requestId, string correlationId, Household household);
    Task<HouseholdViewModel> GetOne(string requestId, string correlationId, string username);
    Task<Dictionary<long, string>> GetMembers(string requestId, string correlationId, string username);
    Task<EditableAddressInfoViewModel> GetForAddressInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveAddressInfo(string requestId, string correlationId, string username, EditableAddressInfoViewModel model);
    Task<EditableVoucherInfoViewModel> GetForVoucherInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveVoucherInfo(string requestId, string correlationId, string username, EditableVoucherInfoViewModel model);
    Task<EditableLiveInAideInfoViewModel> GetForLiveInAideInfoEdit(string requestId, string correlationId, string username);
    Task<string> SaveLiveInAideInfo(string requestId, string correlationId, string username, EditableLiveInAideInfoViewModel model);
    Task AssignMetadata(EditableHouseholdMemberViewModel model);
    Task<EditableHouseholdMemberViewModel> GetForMemberInfoEdit(string requestId, string correlationId, string username, long memberId);
    Task<string> SaveMemberInfo(string requestId, string correlationId, string username, EditableHouseholdMemberViewModel model);
    Task<string> DeleteMemberInfo(string requestId, string correlationId, string username, long memberId);
    Task AssignMetadata(EditableHouseholdAccountViewModel model);
    Task<EditableHouseholdAccountViewModel> GetForAccountInfoEdit(string requestId, string correlationId, string username, long accountId);
    Task<string> SaveAccountInfo(string requestId, string correlationId, string username, EditableHouseholdAccountViewModel model);
    Task<string> DeleteAccountInfo(string requestId, string correlationId, string username, long accountId);

    string ValidateAddressInfo(string requestId, string correlationId, EditableAddressInfoViewModel model);
    Task<string> ValidateVoucherInfo(string requestId, string correlationId, EditableVoucherInfoViewModel model);
}

public class HouseholdService : IHouseholdService
{
    private readonly ILogger<HouseholdService> _logger;
    private readonly IHouseholdRepository _householdRepository;
    private readonly IEmailService _emailService;
    private readonly IMetadataService _metadataService;
    private readonly IPhoneService _phoneService;
    private readonly IUiHelperService _uiHelperService;

    public HouseholdService(ILogger<HouseholdService> logger, IHouseholdRepository householdRepository, IEmailService emailService, IMetadataService metadataService, IPhoneService phoneService, IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _householdRepository = householdRepository ?? throw new ArgumentNullException(nameof(householdRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _phoneService = phoneService ?? throw new ArgumentNullException(nameof(phoneService));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
    }

    public bool IsIncompleteHousehold(string requestId, string correlationId, Household household)
    {
        return false;
        // if (household == null) return false;

        // household.PhysicalStreetLine1 = (household.PhysicalStreetLine1 ?? "").Trim();
        // household.PhysicalCity = (household.PhysicalCity ?? "").Trim();
        // household.PhysicalStateCd = (household.PhysicalStateCd ?? "").Trim();
        // household.PhysicalZipCode = (household.PhysicalZipCode ?? "").Trim();
        // household.PhysicalCounty = (household.PhysicalCounty ?? "").Trim();
        // return string.IsNullOrEmpty(household.PhysicalStreetLine1) || string.IsNullOrEmpty(household.PhysicalCity)
        //     || string.IsNullOrEmpty(household.PhysicalStateCd) || string.IsNullOrEmpty(household.PhysicalZipCode);
    }

    public async Task<HouseholdViewModel> GetOne(string requestId, string correlationId, string username)
    {
        var household = await _householdRepository.GetOne(username);

        var model = household.ToViewModel();
        if (model == null) return null;

        model.IsIncomplete = IsIncompleteHousehold(requestId, correlationId, model);

        if (model.VoucherInd && !string.IsNullOrEmpty(model.VoucherCds))
        {
            var voucherCds = model.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var displayVouchers = "";
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {model.VoucherOther}";
            }
            model.DisplayVouchers = displayVouchers;
        }
        else
        {
            model.DisplayVouchers = "";
        }

        model.Members = [];
        var members = await _householdRepository.GetMembers(model.HouseholdId);
        if (members.Any())
        {
            foreach (var member in members)
            {
                var memberModel = member.ToViewModel();
                memberModel.DisplayName = _uiHelperService.ToDisplayName(memberModel.Title, memberModel.FirstName, memberModel.MiddleName, memberModel.LastName, memberModel.Suffix);
                memberModel.DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(memberModel.PhoneNumber);
                memberModel.DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(memberModel.AltPhoneNumber);
                memberModel.DisplayRelation = _uiHelperService.ToOtherAndValueText(memberModel.RelationTypeCd, memberModel.RelationTypeDescription, memberModel.RelationTypeOther);
                model.Members.Add(memberModel);
            }
        }

        model.Accounts = [];
        var householdAccounts = await _householdRepository.GetAccounts(model.HouseholdId);
        if (householdAccounts.Any())
        {
            foreach (var householdAccount in householdAccounts)
            {
                var accountModel = householdAccount.ToViewModel();
                accountModel.DisplayAccountType = _uiHelperService.ToOtherAndValueText(accountModel.AccountTypeCd, accountModel.AccountTypeDescription, accountModel.AccountTypeOther);
                accountModel.PrimaryHolderMemberName = model.Members.FirstOrDefault(f => f.MemberId == accountModel.PrimaryHolderMemberId)?.DisplayName ?? "Not Specified";
                model.Accounts.Add(accountModel);
            }
        }

        foreach (var member in model.Members)
        {
            member.AssetValueAmt = model.Accounts.Where(c => c.PrimaryHolderMemberId == member.MemberId).Sum(s => s.AccountValueAmt);
            member.AccountCount = model.Accounts.Count(c => c.PrimaryHolderMemberId == member.MemberId);
        }

        return model;
    }

    public async Task<Dictionary<long, string>> GetMembers(string requestId, string correlationId, string username)
    {
        var list = new Dictionary<long, string>();
        var household = await _householdRepository.GetOne(username);
        if (household == null) return list;

        var members = await _householdRepository.GetMembers(household.HouseholdId);
        if (members.Any())
        {
            foreach (var member in members)
            {
                var memberModel = member.ToViewModel();
                memberModel.DisplayName = _uiHelperService.ToDisplayName(memberModel.Title, memberModel.FirstName, memberModel.MiddleName, memberModel.LastName, memberModel.Suffix);
                list.Add(memberModel.MemberId, memberModel.DisplayName);
            }
        }
        return list;
    }

    public async Task<EditableAddressInfoViewModel> GetForAddressInfoEdit(string requestId, string correlationId, string username)
    {
        var household = await _householdRepository.GetOne(username);
        if (household == null) return null;

        var model = new EditableAddressInfoViewModel()
        {
            HouseholdId = household.HouseholdId,
            Username = household.Username,
            AddressInd = household.AddressInd,
            PhysicalStreetLine1 = household.PhysicalStreetLine1,
            PhysicalStreetLine2 = household.PhysicalStreetLine2,
            PhysicalStreetLine3 = household.PhysicalStreetLine3,
            PhysicalCity = household.PhysicalCity,
            PhysicalStateCd = household.PhysicalStateCd,
            PhysicalZipCode = household.PhysicalZipCode,
            DifferentMailingAddressInd = household.DifferentMailingAddressInd,
            MailingStreetLine1 = household.MailingStreetLine1,
            MailingStreetLine2 = household.MailingStreetLine2,
            MailingStreetLine3 = household.MailingStreetLine3,
            MailingCity = household.MailingCity,
            MailingStateCd = household.MailingStateCd,
            MailingZipCode = household.MailingZipCode,
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

        var household = await _householdRepository.GetOne(username);
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

        var updated = await _householdRepository.UpdateAddressInfo(requestId, correlationId, addressInfo);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H004";
        }

        return "";
    }

    public async Task<EditableVoucherInfoViewModel> GetForVoucherInfoEdit(string requestId, string correlationId, string username)
    {
        var household = await _householdRepository.GetOne(username);
        if (household == null) return null;

        var model = new EditableVoucherInfoViewModel()
        {
            HouseholdId = household.HouseholdId,
            Username = household.Username,
            VoucherAdminName = household.VoucherAdminName,
            VoucherCds = household.VoucherCds,
            VoucherInd = household.VoucherInd,
            VoucherOther = household.VoucherOther,
            VoucherTypes = await _metadataService.GetVoucherTypes()
        };

        return model;
    }

    public async Task<string> SaveVoucherInfo(string requestId, string correlationId, string username, EditableVoucherInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to update voucher information";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var validationCode = await ValidateVoucherInfo(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Address validation failed");
            return validationCode;
        }

        var voucherInfo = new Household()
        {
            HouseholdId = model.HouseholdId,
            Username = username,
            VoucherAdminName = string.IsNullOrEmpty(model.VoucherAdminName) ? null : model.VoucherAdminName,
            VoucherCds = string.IsNullOrEmpty(model.VoucherCds) ? null : model.VoucherCds,
            VoucherInd = model.VoucherInd,
            VoucherOther = string.IsNullOrEmpty(model.VoucherOther) ? null : model.VoucherOther,
            ModifiedBy = username
        };

        var updated = await _householdRepository.UpdateVoucherInfo(requestId, correlationId, voucherInfo);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H204";
        }

        return "";
    }

    public async Task<EditableLiveInAideInfoViewModel> GetForLiveInAideInfoEdit(string requestId, string correlationId, string username)
    {
        var household = await _householdRepository.GetOne(username);
        if (household == null) return null;

        var model = new EditableLiveInAideInfoViewModel()
        {
            HouseholdId = household.HouseholdId,
            Username = household.Username,
            LiveInAideInd = household.LiveInAideInd
        };

        return model;
    }

    public async Task<string> SaveLiveInAideInfo(string requestId, string correlationId, string username, EditableLiveInAideInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to update live-in aide information";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var liveInAideInfo = new Household()
        {
            HouseholdId = model.HouseholdId,
            Username = username,
            LiveInAideInd = model.LiveInAideInd,
            ModifiedBy = username
        };

        var updated = await _householdRepository.UpdateLiveInAideInfo(requestId, correlationId, liveInAideInfo);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H304";
        }

        return "";
    }

    public async Task AssignMetadata(EditableHouseholdMemberViewModel model)
    {
        model.GenderTypes = await _metadataService.GetGenderTypes(true) ?? [];
        model.RaceTypes = await _metadataService.GetRaceTypes(true) ?? [];
        model.EthnicityTypes = await _metadataService.GetEthnicityTypes(true) ?? [];
        model.PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true) ?? [];
        model.RelationTypes = await _metadataService.GetRelationTypes(true) ?? [];
        model.IdTypes = await _metadataService.GetIdTypes(true) ?? [];
    }

    public async Task<EditableHouseholdMemberViewModel> GetForMemberInfoEdit(string requestId, string correlationId, string username, long memberId)
    {
        var household = await _householdRepository.GetOne(username);
        if (household == null) return null;

        var model = new EditableHouseholdMemberViewModel()
        {
            HouseholdId = household.HouseholdId,
            Username = household.Username,
        };
        await AssignMetadata(model);

        if (memberId > 0)
        {
            var member = await _householdRepository.GetMember(household.HouseholdId, memberId);
            if (member == null)
            {
                return null;
            }

            model.MemberId = memberId;
            model.RelationTypeCd = member.RelationTypeCd;
            model.RelationTypeOther = member.RelationTypeOther;
            model.Title = member.Title;
            model.FirstName = member.FirstName;
            model.MiddleName = member.MiddleName;
            model.LastName = member.LastName;
            model.Suffix = member.Suffix;
            model.DateOfBirth = member.DateOfBirth.ToDateEditorStringFormat();
            model.Last4SSN = member.Last4SSN;
            model.IdTypeCd = member.IdTypeCd;
            model.IdTypeValue = member.IdTypeValue;
            model.IdIssueDate = member.IdIssueDate.ToDateEditorStringFormat();
            model.GenderCd = member.GenderCd;
            model.RaceCd = member.RaceCd;
            model.EthnicityCd = member.EthnicityCd;
            model.Pronouns = member.Pronouns;
            model.CountyLivingIn = member.CountyLivingIn;
            model.CountyWorkingIn = member.CountyWorkingIn;
            model.StudentInd = member.StudentInd;
            model.DisabilityInd = member.DisabilityInd;
            model.VeteranInd = member.VeteranInd;
            model.EverLivedInWestchesterInd = member.EverLivedInWestchesterInd;
            model.CurrentlyWorkingInWestchesterInd = member.CurrentlyWorkingInWestchesterInd;
            model.PhoneNumber = member.PhoneNumber;
            model.PhoneNumberExtn = member.PhoneNumberExtn;
            model.PhoneNumberTypeCd = member.PhoneNumberTypeCd;
            model.AltPhoneNumber = member.AltPhoneNumber;
            model.AltPhoneNumberExtn = member.AltPhoneNumberExtn;
            model.AltPhoneNumberTypeCd = member.AltPhoneNumberTypeCd;
            model.EmailAddress = member.EmailAddress;
            model.AltEmailAddress = member.AltEmailAddress;
            model.OwnRealEstateInd = member.OwnRealEstateInd;
            model.RealEstateValueAmt = member.RealEstateValueAmt;
            model.IncomeValueAmt = member.IncomeValueAmt;
            model.AssetValueAmt = member.AssetValueAmt;
        }

        return model;
    }

    public async Task<string> SaveMemberInfo(string requestId, string correlationId, string username, EditableHouseholdMemberViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to save household member";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        model.RelationTypeCd = (model.RelationTypeCd ?? "").Trim().ToUpper();
        model.RelationTypeOther = (model.RelationTypeOther ?? "").Trim();
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
        model.PhoneNumber = (model.PhoneNumber ?? "").Trim();
        model.PhoneNumberExtn = (model.PhoneNumberExtn ?? "").Trim();
        model.PhoneNumberTypeCd = (model.PhoneNumberTypeCd ?? "").Trim();
        model.AltPhoneNumber = (model.AltPhoneNumber ?? "").Trim();
        model.AltPhoneNumberExtn = (model.AltPhoneNumberExtn ?? "").Trim();
        model.AltPhoneNumberTypeCd = (model.AltPhoneNumberTypeCd ?? "").Trim();
        model.EmailAddress = (model.EmailAddress ?? "").Trim();
        model.AltEmailAddress = (model.AltEmailAddress ?? "").Trim();
        model.RealEstateValueAmt = (model.RealEstateValueAmt < 0L) ? 0L : model.RealEstateValueAmt;
        model.AssetValueAmt = (model.AssetValueAmt < 0L) ? 0L : model.AssetValueAmt;
        model.IncomeValueAmt = (model.IncomeValueAmt < 0L) ? 0L : model.IncomeValueAmt;

        var existingMembers = await _householdRepository.GetMembers(model.HouseholdId);

        var existingMember = existingMembers.FirstOrDefault(f => f.MemberId == model.MemberId);
        if (model.MemberId > 0 && existingMember == null)
        {
            _logger.LogError($"{logPrefix} - Member information is incorrect, or Member does not exist");
            return "H401";
        }

        var relationTypes = await _metadataService.GetRelationTypes() ?? [];
        if (!relationTypes.ContainsKey(model.RelationTypeCd))
        {
            _logger.LogError($"{logPrefix} - Relation type is required");
            return "H411";
        }
        if (model.RelationTypeCd.Equals("OTHER", StringComparison.CurrentCultureIgnoreCase)
                && string.IsNullOrEmpty(model.RelationTypeOther))
        {
            _logger.LogError($"{logPrefix} - Relation type other is required");
            return "H412";
        }

        if (string.IsNullOrEmpty(model.FirstName))
        {
            _logger.LogError($"{logPrefix} - First Name is required");
            return "P102";
        }

        if (string.IsNullOrEmpty(model.LastName))
        {
            _logger.LogError($"{logPrefix} - Last Name is required");
            return "P103";
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
            var idTypes = await _metadataService.GetIdTypes();
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
        else
        {
            model.IdTypeCd = null;
            model.IdTypeValue = null;
            model.IdIssueDate = null;
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

        var phoneNumberTypes = await _metadataService.GetPhoneNumberTypes() ?? [];
        if (!string.IsNullOrEmpty(model.PhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(model.PhoneNumber))
            {
                _logger.LogError($"{logPrefix} - Phone Number is invalid");
                return "P211";
            }

            if (string.IsNullOrEmpty(model.PhoneNumberExtn)) model.PhoneNumberExtn = null;

            if (!phoneNumberTypes.ContainsKey(model.PhoneNumberTypeCd))
            {
                _logger.LogError($"{logPrefix} - Phone Number Type is invalid");
                return "P212";
            }
        }
        else
        {
            model.PhoneNumber = null;
            model.PhoneNumberExtn = null;
            model.PhoneNumberTypeCd = null;
        }

        if (!string.IsNullOrEmpty(model.AltPhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(model.AltPhoneNumber))
            {
                _logger.LogError($"{logPrefix} - Alternate Phone Number is invalid");
                return "P213";
            }

            if (string.IsNullOrEmpty(model.AltPhoneNumberExtn)) model.AltPhoneNumberExtn = null;

            if (!phoneNumberTypes.ContainsKey(model.AltPhoneNumberTypeCd))
            {
                _logger.LogError($"{logPrefix} - Alternate Phone Number Type is invalid");
                return "P214";
            }
        }
        else
        {
            model.AltPhoneNumber = null;
            model.AltPhoneNumberExtn = null;
            model.AltPhoneNumberTypeCd = null;
        }

        if (!string.IsNullOrEmpty(model.EmailAddress))
        {
            if (!_emailService.IsValidEmailAddress(requestId, correlationId, model.EmailAddress))
            {
                _logger.LogError($"{logPrefix} - Email Address is invalid");
                return "P215";
            }
        }
        else
        {
            model.EmailAddress = null;
        }

        if (!string.IsNullOrEmpty(model.AltEmailAddress))
        {
            if (!_emailService.IsValidEmailAddress(requestId, correlationId, model.AltEmailAddress))
            {
                _logger.LogError($"{logPrefix} - Alternate Email Address is invalid");
                return "P216";
            }
        }
        else
        {
            model.AltEmailAddress = null;
        }

        if (!string.IsNullOrEmpty(model.EmailAddress) && !string.IsNullOrEmpty(model.AltEmailAddress))
        {
            if (model.EmailAddress.Equals(model.AltEmailAddress, StringComparison.CurrentCultureIgnoreCase))
            {
                _logger.LogError($"{logPrefix} - Alternate Email Address cannot must be different from the primary email address");
                return "P217";
            }
        }

        if (!model.OwnRealEstateInd) model.RealEstateValueAmt = 0L;

        HouseholdMember duplicateMember = null;
        if (model.MemberId > 0)
        {
            duplicateMember = existingMembers.FirstOrDefault(f => f.RelationTypeCd.Equals(model.RelationTypeCd, StringComparison.CurrentCultureIgnoreCase)
                                                                        && f.FirstName.Equals(model.FirstName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.MiddleName.Length == 0 || (model.MiddleName.Length > 0 && (f.MiddleName ?? "").Trim().Equals(model.MiddleName, StringComparison.CurrentCultureIgnoreCase)))
                                                                        && f.LastName.Equals(model.LastName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.Suffix.Length == 0 || (model.Suffix.Length > 0 && (f.Suffix ?? "").Trim().Equals(model.Suffix, StringComparison.CurrentCultureIgnoreCase)))
                                                                        && f.DateOfBirth.ToDateEditorStringFormat().Equals(model.DateOfBirth, StringComparison.CurrentCultureIgnoreCase)
                                                                        && f.Last4SSN.Equals(model.Last4SSN, StringComparison.CurrentCultureIgnoreCase)
                                                                        && f.MemberId != model.MemberId);
        }
        else
        {
            duplicateMember = existingMembers.FirstOrDefault(f => f.RelationTypeCd.Equals(model.RelationTypeCd, StringComparison.CurrentCultureIgnoreCase)
                                                                        && f.FirstName.Equals(model.FirstName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.MiddleName.Length == 0 || (model.MiddleName.Length > 0 && (f.MiddleName ?? "").Trim().Equals(model.MiddleName, StringComparison.CurrentCultureIgnoreCase)))
                                                                        && f.LastName.Equals(model.LastName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.Suffix.Length == 0 || (model.Suffix.Length > 0 && (f.Suffix ?? "").Trim().Equals(model.Suffix, StringComparison.CurrentCultureIgnoreCase)))
                                                                        && f.DateOfBirth.ToDateEditorStringFormat().Equals(model.DateOfBirth, StringComparison.CurrentCultureIgnoreCase)
                                                                        && f.Last4SSN.Equals(model.Last4SSN, StringComparison.CurrentCultureIgnoreCase));
        }
        if (duplicateMember != null)
        {
            _logger.LogError($"{logPrefix} - Duplicate household member");
            return "H402";
        }

        if (string.IsNullOrEmpty(model.Title)) model.Title = null;
        if (string.IsNullOrEmpty(model.MiddleName)) model.MiddleName = null;
        if (string.IsNullOrEmpty(model.Suffix)) model.Suffix = null;
        if (string.IsNullOrEmpty(model.Pronouns)) model.Pronouns = null;
        if (string.IsNullOrEmpty(model.CountyLivingIn)) model.CountyLivingIn = null;
        if (string.IsNullOrEmpty(model.CountyWorkingIn)) model.CountyWorkingIn = null;

        var member = new HouseholdMember()
        {
            Username = username,
            HouseholdId = model.HouseholdId,
            MemberId = model.MemberId,
            RelationTypeCd = model.RelationTypeCd,
            RelationTypeOther = string.IsNullOrEmpty(model.RelationTypeOther ?? "") ? null : model.RelationTypeOther,
            Title = string.IsNullOrEmpty(model.Title ?? "") ? null : model.Title,
            FirstName = model.FirstName,
            MiddleName = string.IsNullOrEmpty(model.MiddleName ?? "") ? null : model.MiddleName,
            LastName = model.LastName,
            Suffix = string.IsNullOrEmpty(model.Suffix ?? "") ? null : model.Suffix,
            Last4SSN = model.Last4SSN,
            DateOfBirth = dob,
            IdTypeCd = model.IdTypeCd,
            IdTypeValue = model.IdTypeValue,
            IdIssueDate = idIssueDate,
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
            PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber ?? "") ? null : model.PhoneNumber,
            PhoneNumberExtn = string.IsNullOrEmpty(model.PhoneNumberExtn ?? "") ? null : model.PhoneNumberExtn,
            PhoneNumberTypeCd = string.IsNullOrEmpty(model.PhoneNumberTypeCd ?? "") ? null : model.PhoneNumberTypeCd,
            AltPhoneNumber = string.IsNullOrEmpty(model.AltPhoneNumber ?? "") ? null : model.AltPhoneNumber,
            AltPhoneNumberExtn = string.IsNullOrEmpty(model.AltPhoneNumberExtn ?? "") ? null : model.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = string.IsNullOrEmpty(model.AltPhoneNumberTypeCd ?? "") ? null : model.AltPhoneNumberTypeCd,
            EmailAddress = string.IsNullOrEmpty(model.EmailAddress ?? "") ? null : model.EmailAddress,
            AltEmailAddress = string.IsNullOrEmpty(model.AltEmailAddress ?? "") ? null : model.AltEmailAddress,
            OwnRealEstateInd = model.OwnRealEstateInd,
            RealEstateValueAmt = model.OwnRealEstateInd ? model.RealEstateValueAmt : 0L,
            AssetValueAmt = model.AssetValueAmt,
            IncomeValueAmt = model.IncomeValueAmt,
            ModifiedBy = username
        };

        var updated = await _householdRepository.SaveMemberProfile(requestId, correlationId, member);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H403";
        }

        return "";
    }

    public async Task<string> DeleteMemberInfo(string requestId, string correlationId, string username, long memberId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to delete household member";

        if (memberId <= 0)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var member = await _householdRepository.GetMember(household.HouseholdId, memberId);
        if (member == null)
        {
            _logger.LogError($"{logPrefix} - Member not found");
            return "H401";
        }

        member.Username = username;
        member.ModifiedBy = username;
        var deleted = await _householdRepository.DeleteMemberInfo(requestId, correlationId, member);
        if (!deleted)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H405";
        }

        return "";
    }

    public async Task AssignMetadata(EditableHouseholdAccountViewModel model)
    {
        model.AccountTypes = await _metadataService.GetAccountTypes(true);
        model.Members = await GetMembers("", "", model.Username);
    }

    public async Task<EditableHouseholdAccountViewModel> GetForAccountInfoEdit(string requestId, string correlationId, string username, long accountId)
    {
        var household = await _householdRepository.GetOne(username);
        if (household == null) return null;

        var model = new EditableHouseholdAccountViewModel()
        {
            HouseholdId = household.HouseholdId,
            Username = household.Username,
        };
        await AssignMetadata(model);

        if (accountId > 0)
        {
            var account = await _householdRepository.GetAccount(household.HouseholdId, accountId);
            if (account == null)
            {
                return null;
            }

            model.AccountId = accountId;
            model.AccountNumber = account.AccountNumber;
            model.AccountTypeCd = account.AccountTypeCd;
            model.AccountTypeOther = account.AccountTypeOther;
            model.AccountValueAmt = account.AccountValueAmt;
            model.InstitutionName = account.InstitutionName;
            model.PrimaryHolderMemberId = account.PrimaryHolderMemberId;
        }

        return model;
    }

    public async Task<string> SaveAccountInfo(string requestId, string correlationId, string username, EditableHouseholdAccountViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to save household account";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        model.AccountNumber = (model.AccountNumber ?? "").Trim();
        model.AccountTypeCd = (model.AccountTypeCd ?? "").Trim().ToUpper();
        model.AccountTypeOther = (model.AccountTypeOther ?? "").Trim();
        model.AccountValueAmt = (model.AccountValueAmt < 0L) ? 0L : model.AccountValueAmt;
        model.InstitutionName = (model.InstitutionName ?? "").Trim();
        model.PrimaryHolderMemberId = model.PrimaryHolderMemberId < 0 ? 0 : model.PrimaryHolderMemberId;

        var existingAccounts = await _householdRepository.GetAccounts(model.HouseholdId);

        var existingAccount = existingAccounts.FirstOrDefault(f => f.AccountId == model.AccountId);
        if (model.AccountId > 0 && existingAccount == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return "H501";
        }

        if (string.IsNullOrEmpty(model.AccountTypeCd))
        {
            _logger.LogError($"{logPrefix} - Account type is required");
            return "H512";
        }

        var accountTypes = await _metadataService.GetAccountTypes() ?? [];
        if (!accountTypes.ContainsKey(model.AccountTypeCd))
        {
            _logger.LogError($"{logPrefix} - Account Type is invalid");
            return "H513";
        }

        if (model.AccountTypeCd.Equals("OTHER", StringComparison.CurrentCultureIgnoreCase)
                && string.IsNullOrEmpty(model.AccountTypeOther))
        {
            _logger.LogError($"{logPrefix} - Account type other is required");
            return "H514";
        }

        if ("|CHECKING|SAVINGS|BROKERAGE|CERTIFICATE|MUTUALFUND|RETIREMENT|".Contains($"|{model.AccountTypeCd}|"))
        {
            if (string.IsNullOrEmpty(model.AccountNumber))
            {
                _logger.LogError($"{logPrefix} - Account Number is required");
                return "H511";
            }
        }

        if (string.IsNullOrEmpty(model.InstitutionName))
        {
            _logger.LogError($"{logPrefix} - Institution or financial instrument name is required");
            return "H515";
        }

        HouseholdAccount duplicateAccount = null;
        if (model.AccountId > 0)
        {
            duplicateAccount = existingAccounts.FirstOrDefault(f => (f.AccountTypeCd ?? "").Trim().Equals(model.AccountTypeCd, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (f.AccountTypeOther ?? "").Trim().Equals(model.AccountTypeOther, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (f.InstitutionName ?? "").Trim().Equals(model.InstitutionName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.AccountNumber.Length > 0 && (f.AccountNumber ?? "").Trim().Equals(model.AccountNumber, StringComparison.CurrentCultureIgnoreCase))
                                                                        && f.AccountId != model.AccountId);
        }
        else
        {
            duplicateAccount = existingAccounts.FirstOrDefault(f => (f.AccountTypeCd ?? "").Trim().Equals(model.AccountTypeCd, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (f.AccountTypeOther ?? "").Trim().Equals(model.AccountTypeOther, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (f.InstitutionName ?? "").Trim().Equals(model.InstitutionName, StringComparison.CurrentCultureIgnoreCase)
                                                                        && (model.AccountNumber.Length > 0 && (f.AccountNumber ?? "").Trim().Equals(model.AccountNumber, StringComparison.CurrentCultureIgnoreCase)));
        }
        if (duplicateAccount != null)
        {
            _logger.LogError($"{logPrefix} - Duplicate household account");
            return "H502";
        }

        var account = new HouseholdAccount()
        {
            Username = username,
            HouseholdId = model.HouseholdId,
            AccountId = model.AccountId,
            AccountNumber = string.IsNullOrEmpty(model.AccountNumber ?? "") ? "" : model.AccountNumber,
            AccountTypeCd = model.AccountTypeCd,
            AccountTypeOther = string.IsNullOrEmpty(model.AccountTypeOther ?? "") ? null : model.AccountTypeOther,
            AccountValueAmt = model.AccountValueAmt,
            InstitutionName = string.IsNullOrEmpty(model.InstitutionName ?? "") ? null : model.InstitutionName,
            ModifiedBy = username,
            PrimaryHolderMemberId = model.PrimaryHolderMemberId
        };

        var updated = await _householdRepository.SaveAccountInfo(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H504";
        }

        return "";
    }

    public async Task<string> DeleteAccountInfo(string requestId, string correlationId, string username, long accountId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message - Unable to delete household account";

        if (accountId <= 0)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "H101";
        }

        var household = await _householdRepository.GetOne(username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var account = await _householdRepository.GetAccount(household.HouseholdId, accountId);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return "H501";
        }

        account.Username = username;
        account.ModifiedBy = username;
        var deleted = await _householdRepository.DeleteAccountInfo(requestId, correlationId, account);
        if (!deleted)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "H505";
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
            _logger.LogError($"{logPrefix} - Street is required");
            return "H102";
        }

        if (string.IsNullOrEmpty(model.PhysicalStreetLine2)) model.PhysicalStreetLine2 = null;
        if (string.IsNullOrEmpty(model.PhysicalStreetLine3)) model.PhysicalStreetLine3 = null;

        if (string.IsNullOrEmpty(model.PhysicalCity))
        {
            _logger.LogError($"{logPrefix} - City is required");
            return "H103";
        }

        if (string.IsNullOrEmpty(model.PhysicalStateCd))
        {
            _logger.LogError($"{logPrefix} - State is required");
            return "H104";
        }

        if (string.IsNullOrEmpty(model.PhysicalZipCode))
        {
            _logger.LogError($"{logPrefix} - Zip Code is required");
            return "H105";
        }

        if (string.IsNullOrEmpty(model.PhysicalCounty)) model.PhysicalCounty = null;

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
            _logger.LogError($"{logPrefix} - Street is required");
            return "H112";
        }

        if (string.IsNullOrEmpty(model.MailingStreetLine2)) model.MailingStreetLine2 = null;
        if (string.IsNullOrEmpty(model.MailingStreetLine3)) model.MailingStreetLine3 = null;

        if (string.IsNullOrEmpty(model.MailingCity))
        {
            _logger.LogError($"{logPrefix} - City is required");
            return "H113";
        }

        if (string.IsNullOrEmpty(model.MailingStateCd))
        {
            _logger.LogError($"{logPrefix} - State is required");
            return "H114";
        }

        if (string.IsNullOrEmpty(model.MailingZipCode))
        {
            _logger.LogError($"{logPrefix} - Zip Code is required");
            return "H115";
        }

        if (string.IsNullOrEmpty(model.MailingCounty)) model.MailingCounty = null;

        return "";
    }

    public async Task<string> ValidateVoucherInfo(string requestId, string correlationId, EditableVoucherInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to validate voucher information";

        model.VoucherCds = (model.VoucherCds ?? "").Trim();
        model.VoucherOther = (model.VoucherOther ?? "").Trim();
        model.VoucherAdminName = (model.VoucherAdminName ?? "").Trim();

        if (!model.VoucherInd)
        {
            model.VoucherCds = null;
            model.VoucherOther = null;
            model.VoucherAdminName = null;
            return "";
        }

        if (string.IsNullOrEmpty(model.VoucherCds))
        {
            _logger.LogError($"{logPrefix} - One or more voucher types is required");
            return "H211";
        }

        var voucherTypes = await _metadataService.GetVoucherTypes() ?? [];
        var voucherTypeCds = model.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
        var errorCode = "";
        foreach (var voucherTypeCd in voucherTypeCds)
        {
            if (!voucherTypes.ContainsKey(voucherTypeCd))
            {
                _logger.LogError($"{logPrefix} - One or more voucher types is invalid");
                errorCode = "H212";
                break;
            }
            else if (voucherTypeCd == "OTHER")
            {
                if (string.IsNullOrEmpty(model.VoucherOther))
                {
                    _logger.LogError($"{logPrefix} - Other voucher type is required");
                    errorCode = "H213";
                    break;
                }
            }
        }
        if (!string.IsNullOrEmpty(errorCode)) return errorCode;

        if (string.IsNullOrEmpty(model.VoucherAdminName))
        {
            _logger.LogError($"{logPrefix} - Voucher administrator name is required");
            return "H214";
        }

        return "";
    }
}