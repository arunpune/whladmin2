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

public interface IHousingApplicationService
{
    Task<DashboardViewModel> GetForDashboard(string requestId, string correlationId, string username);
    Task<EditableApplicantInfoViewModel> GetForApplicationEdit(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0);
    Task<string> SaveApplicantInfo(string requestId, string correlationId, string username, EditableApplicantInfoViewModel model);
    Task<EditableHouseholdInfoViewModel> GetForHouseholdEdit(string requestId, string correlationId, string username, long applicationId);
    Task<string> SaveHouseholdInfo(string requestId, string correlationId, string username, EditableHouseholdInfoViewModel model);
    Task<EditableAdditionalMembersInfoViewModel> GetForAdditionalMembersEdit(string requestId, string correlationId, string username, long applicationId);
    Task<string> SaveAdditionalMembersInfo(string requestId, string correlationId, string username, EditableAdditionalMembersInfoViewModel model);
    Task<EditableIncomeAssetsInfoViewModel> GetForIncomeAssetsEdit(string requestId, string correlationId, string username, long applicationId);
    Task<string> SaveIncomeAssetsInfo(string requestId, string correlationId, string username, EditableIncomeAssetsInfoViewModel model);
    Task<HousingApplicationReviewSubmitViewModel> GetForReviewSubmit(string requestId, string correlationId, string username, long applicationId);
    Task<string> SubmitApplication(string requestId, string correlationId, string username, string siteUrl, HousingApplicationReviewSubmitViewModel model);
    Task<string> WithdrawApplication(string requestId, string correlationId, string username, string siteUrl, long applicationId);

    Task<IEnumerable<ApplicationDocumentViewModel>> GetDocuments(string requestId, string correlationId, string username, long applicationId);
    Task<ApplicationDocumentViewModel> GetDocument(string requestId, string correlationId, string username, long applicationId, long docId);
    Task<EditableApplicationDocumentViewModel> GetForAddDocument(string requestId, string correlationId, string username, long applicationId);
    Task<string> AddDocument(string requestId, string correlationId, string username, EditableApplicationDocumentViewModel model);
    Task<string> DeleteDocument(string requestId, string correlationId, string username, long applicationId, long docId);

    Task<HAViewModel> GetApplicantInfo(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0);

    Task<EditableHousingApplicationViewModel> GetForEdit(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0);
    Task<string> ValidateApplicationInfo(string requestId, string correlationId, string username, EditableHousingApplicationViewModel model);
    Task<string> SubmitApplication(string requestId, string correlationId, string username, string siteUrl, EditableHousingApplicationViewModel model);
    Task<HousingApplicationViewModel> GetSubmitted(string requestId, string correlationId, string username, long applicationId);

    Task<IEnumerable<ApplicationCommentViewModel>> GetComments(string requestId, string correlationId, string username, long applicationId);
    Task<EditableApplicationCommentViewModel> GetForAddComment(string requestId, string correlationId, string username, long applicationId);
    Task<string> AddComment(string requestId, string correlationId, string username, string siteUrl, EditableApplicationCommentViewModel model);
}

public class HousingApplicationService : IHousingApplicationService
{
    private readonly ILogger<HousingApplicationService> _logger;
    private readonly IHousingApplicationRepository _applicationRepository;
    private readonly IEmailService _emailService;
    private readonly IListingService _listingService;
    private readonly IProfileService _profileService;
    private readonly IHouseholdService _householdService;
    private readonly IMetadataService _metadataService;
    private readonly IUiHelperService _uiHelperService;

    private const int MaxFileSizeBytes = 1048576;
    private const int MaxFileSizeKilobytes = 1024;
    private const int MaxFileSizeMegabytes = 1;

    public HousingApplicationService(ILogger<HousingApplicationService> logger, IHousingApplicationRepository applicationRepository, IEmailService emailService, IListingService listingService, IProfileService profileService, IHouseholdService householdService, IMetadataService metadataService, IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _listingService = listingService ?? throw new ArgumentNullException(nameof(listingService));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
        _householdService = householdService ?? throw new ArgumentNullException(nameof(householdService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
    }

    public async Task<DashboardViewModel> GetForDashboard(string requestId, string correlationId, string username)
    {
        var applications = await _applicationRepository.GetAll(requestId, correlationId, username) ?? [];
        var model = new DashboardViewModel
        {
            Applications = applications.Select(s => s.ToViewModel())
        };
        return model;
    }

    public async Task<EditableApplicantInfoViewModel> GetForApplicationEdit(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var model = new EditableApplicantInfoViewModel
        {
            ProfileDetails = profile,
            IdTypes = await _metadataService.GetIdTypes(true),
            GenderTypes = await _metadataService.GetGenderTypes(true),
            RaceTypes = await _metadataService.GetRaceTypes(true),
            EthnicityTypes = await _metadataService.GetEthnicityTypes(true),
            PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true),
        };

        HousingApplication application = null;
        if (applicationId > 0)
        {
            application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
            if (application == null)
            {
                _logger.LogError($"{logPrefix} - Application not found");
                return null;
            }
        }
        else
        {
            var applications = await _applicationRepository.GetAllByListing(requestId, correlationId, username, listingId);
            var eligibleApplications = applications?.Where(w => !(w.StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase));
            if (eligibleApplications?.Count() == 1)
            {
                application = eligibleApplications.First();
            }
        }

        if (application == null)
        {
            model.ApplicationId = 0;
            model.ListingId = listingId;
            model.Username = username;

            // Applicant Info
            model.Title = profile.Title;
            model.FirstName = profile.FirstName;
            model.MiddleName = profile.MiddleName;
            model.LastName = profile.LastName;
            model.Suffix = profile.Suffix;
            model.GenderCd = profile.GenderCd;
            model.RaceCd = profile.RaceCd;
            model.EthnicityCd = profile.EthnicityCd;
            model.StudentInd = profile.StudentInd;
            model.DisabilityInd = profile.DisabilityInd;
            model.VeteranInd = profile.VeteranInd;
            model.Pronouns = profile.Pronouns;
            model.EverLivedInWestchesterInd = profile.EverLivedInWestchesterInd;
            model.CountyLivingIn = profile.CountyLivingIn;
            model.CurrentlyWorkingInWestchesterInd = profile.CurrentlyWorkingInWestchesterInd;
            model.CountyWorkingIn = profile.CountyWorkingIn;

            // ID Info
            model.Last4SSN = profile.Last4SSN;
            model.DateOfBirth = profile.DateOfBirth.ToDateEditorStringFormat();
            model.IdTypeCd = profile.IdTypeCd;
            model.IdTypeValue = profile.IdTypeValue;
            model.IdIssueDate = profile.IdIssueDate.ToDateEditorStringFormat();

            // Contact Info
            model.EmailAddress = profile.EmailAddress;
            model.AltEmailAddress = profile.AltEmailAddress;
            model.PhoneNumberTypeCd = profile.PhoneNumberTypeCd;
            model.PhoneNumber = profile.PhoneNumber;
            model.PhoneNumberExtn = profile.PhoneNumberExtn;
            model.AltPhoneNumberTypeCd = profile.AltPhoneNumberTypeCd;
            model.AltPhoneNumber = profile.AltPhoneNumber;
            model.AltPhoneNumberExtn = profile.AltPhoneNumberExtn;

            // Net Worth
            model.AssetValueAmt = profile.IncomeValueAmt;
            model.IncomeValueAmt = profile.IncomeValueAmt;
            model.OwnRealEstateInd = profile.OwnRealEstateInd;
            model.RealEstateValueAmt = profile.RealEstateValueAmt;
        }
        else
        {
            applicationId = application.ApplicationId;
            model.ApplicationId = application.ApplicationId;
            model.ListingId = application.ListingId;
            model.Username = application.Username;

            // Applicant Info
            model.Title = application.Title;
            model.FirstName = application.FirstName;
            model.MiddleName = application.MiddleName;
            model.LastName = application.LastName;
            model.Suffix = application.Suffix;
            model.Last4SSN = application.Last4SSN;
            model.DateOfBirth = application.DateOfBirth.ToDateEditorStringFormat();
            model.IdTypeCd = application.IdTypeCd;
            model.IdTypeValue = application.IdTypeValue;
            model.IdIssueDate = application.IdIssueDate.ToDateEditorStringFormat();
            model.GenderCd = application.GenderCd;
            model.RaceCd = application.RaceCd;
            model.EthnicityCd = application.EthnicityCd;
            model.StudentInd = application.StudentInd;
            model.DisabilityInd = application.DisabilityInd;
            model.VeteranInd = application.VeteranInd;
            model.Pronouns = application.Pronouns;
            model.EverLivedInWestchesterInd = application.EverLivedInWestchesterInd;
            model.CountyLivingIn = application.CountyLivingIn;
            model.CurrentlyWorkingInWestchesterInd = application.CurrentlyWorkingInWestchesterInd;
            model.CountyWorkingIn = application.CountyWorkingIn;

            // Contact Info
            model.EmailAddress = application.EmailAddress;
            model.AltEmailAddress = application.AltEmailAddress;
            model.PhoneNumberTypeCd = application.PhoneNumberTypeCd;
            model.PhoneNumber = application.PhoneNumber;
            model.PhoneNumberExtn = application.PhoneNumberExtn;
            model.AltPhoneNumberTypeCd = application.AltPhoneNumberTypeCd;
            model.AltPhoneNumber = application.AltPhoneNumber;
            model.AltPhoneNumberExtn = application.AltPhoneNumberExtn;

            // Net Worth
            model.AssetValueAmt = application.AssetValueAmt;
            model.IncomeValueAmt = application.IncomeValueAmt;
            model.OwnRealEstateInd = application.OwnRealEstateInd;
            model.RealEstateValueAmt = application.RealEstateValueAmt;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }
        model.ListingDetails = listing;

        model.Editable = application == null || (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);

        return model;
    }

    public async Task<string> SaveApplicantInfo(string requestId, string correlationId, string username, EditableApplicantInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to save applicant info";

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return "L001";
        }
        model.ListingDetails = listing;

        model.EmailAddress = profile.EmailAddress;
        model.ProfileDetails = profile;
        model.IdTypes = await _metadataService.GetIdTypes(true);
        model.GenderTypes = await _metadataService.GetGenderTypes(true);
        model.RaceTypes = await _metadataService.GetRaceTypes(true);
        model.EthnicityTypes = await _metadataService.GetEthnicityTypes(true);
        model.PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true);

        // Validations
        var validationCode = await _profileService.ValidateProfile(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Profile validation failed");
            return validationCode;
        }
        validationCode = await _profileService.ValidateContactInformation(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Profile validation failed");
            return validationCode;
        }

        if (model.ApplicationId > 0)
        {
            var existingApplication = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
            if (existingApplication == null)
            {
                _logger.LogError($"{logPrefix} - Application not found");
                return "A001";
            }
        }

        var application = new HousingApplication()
        {
            ApplicationId = model.ApplicationId,
            ListingId = model.ListingId,
            Username = username,
            Title = model.Title,
            FirstName = model.FirstName,
            MiddleName = model.MiddleName,
            LastName = model.LastName,
            Suffix = model.Suffix,
            GenderCd = model.GenderCd,
            RaceCd = model.RaceCd,
            EthnicityCd = model.EthnicityCd,
            StudentInd = model.StudentInd,
            DisabilityInd = model.DisabilityInd,
            VeteranInd = model.VeteranInd,
            Pronouns = model.Pronouns,
            EverLivedInWestchesterInd = model.EverLivedInWestchesterInd,
            CountyLivingIn = model.CountyLivingIn,
            CurrentlyWorkingInWestchesterInd = model.CurrentlyWorkingInWestchesterInd,
            CountyWorkingIn = model.CountyWorkingIn,
            PhoneNumberTypeCd = model.PhoneNumberTypeCd,
            PhoneNumber = model.PhoneNumber,
            PhoneNumberExtn = model.PhoneNumberExtn,
            AltPhoneNumberTypeCd = model.AltPhoneNumberTypeCd,
            AltPhoneNumber = model.AltPhoneNumber,
            AltPhoneNumberExtn = model.AltPhoneNumberExtn,
            EmailAddress = model.EmailAddress,
            AltEmailAddress = model.AltEmailAddress,
            OwnRealEstateInd = model.OwnRealEstateInd,
            RealEstateValueAmt = model.RealEstateValueAmt,
            AssetValueAmt = model.AssetValueAmt,
            IncomeValueAmt = model.IncomeValueAmt,
            Last4SSN = model.Last4SSN,
            DateOfBirth = model.DateOfBirth.FromDateEditorStringFormat(),
            IdTypeCd = model.IdTypeCd,
            IdTypeValue = model.IdTypeValue,
            IdIssueDate = model.IdIssueDate.FromDateEditorStringFormat(),
            LeadTypeCd = "",
            UpdateProfileInd = model.UpdateProfileInd,
            CreatedBy = username,
            ModifiedBy = username
        };
        var saved = model.ApplicationId > 0 ?
                        await _applicationRepository.Update(requestId, correlationId, application)
                            : await _applicationRepository.Add(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A003";
        }

        model.ApplicationId = application.ApplicationId;

        return "";
    }

    public async Task<EditableHouseholdInfoViewModel> GetForHouseholdEdit(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var model = new EditableHouseholdInfoViewModel
        {
            ApplicationId = application.ApplicationId,
            ListingId = application.ListingId,
            ListingDetails = listing,
            Username = username,
            HouseholdDetails = household,
            AddressInd = application.AddressInd,
            PhysicalStreetLine1 = application.PhysicalStreetLine1,
            PhysicalStreetLine2 = application.PhysicalStreetLine2,
            PhysicalStreetLine3 = application.PhysicalStreetLine3,
            PhysicalCity = application.PhysicalCity,
            PhysicalStateCd = application.PhysicalStateCd,
            PhysicalZipCode = application.PhysicalZipCode,
            PhysicalCounty = application.PhysicalCounty,
            DifferentMailingAddressInd = application.DifferentMailingAddressInd,
            MailingStreetLine1 = application.MailingStreetLine1,
            MailingStreetLine2 = application.MailingStreetLine2,
            MailingStreetLine3 = application.MailingStreetLine3,
            MailingCity = application.MailingCity,
            MailingStateCd = application.MailingStateCd,
            MailingZipCode = application.MailingZipCode,
            MailingCounty = application.MailingCounty,
            VoucherInd = application.VoucherInd,
            VoucherCds = application.VoucherCds,
            VoucherOther = application.VoucherOther,
            VoucherAdminName = application.VoucherAdminName,
            LiveInAideInd = application.LiveInAideInd,
            VoucherTypes = await _metadataService.GetVoucherTypes(true),
            UnitTypeCds = application.UnitTypeCds
        };

        model.UnitTypes = [];
        if (listing.Units.Any())
        {
            foreach (var unit in listing.Units)
            {
                if (!model.UnitTypes.ContainsKey(unit.UnitTypeCd))
                {
                    model.UnitTypes.Add(unit.UnitTypeCd, unit.UnitTypeDescription);
                }
            }
        }
        else
        {
            model.UnitTypes = await _metadataService.GetUnitTypes();
        }

        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);

        return model;
    }

    public async Task<string> SaveHouseholdInfo(string requestId, string correlationId, string username, EditableHouseholdInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to save co-applicant info";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return "A001";
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        model.HouseholdDetails = household;
        model.VoucherTypes = await _metadataService.GetVoucherTypes(true);

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return "L001";
        }
        model.ListingDetails = listing;

        model.UnitTypes = [];
        if (listing.Units.Any())
        {
            foreach (var unit in listing.Units)
            {
                if (!model.UnitTypes.ContainsKey(unit.UnitTypeCd))
                {
                    model.UnitTypes.Add(unit.UnitTypeCd, unit.UnitTypeDescription);
                }
            }
        }
        else
        {
            model.UnitTypes = await _metadataService.GetUnitTypes();
        }

        // Validations
        model.UnitTypeCds = (model.UnitTypeCds ?? "").Trim();
        if (model.UnitTypeCds.Length == 0)
        {
            _logger.LogError($"{logPrefix} - No unit types selected");
            return "A103";
        }
        var validationCode = _householdService.ValidateAddressInfo(requestId, correlationId, model);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Address validation failed");
            return validationCode;
        }
        var voucherModel = new EditableVoucherInfoViewModel()
        {
            VoucherAdminName = model.VoucherAdminName,
            VoucherCds = model.VoucherCds,
            VoucherInd = model.VoucherInd,
            VoucherOther = model.VoucherOther
        };
        validationCode = await _householdService.ValidateVoucherInfo(requestId, correlationId, voucherModel);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"{logPrefix} - Voucher validation failed");
            return validationCode;
        }

        application = new HousingApplication()
        {
            ApplicationId = model.ApplicationId,
            ListingId = model.ListingId,
            Username = username,
            HouseholdId = household.HouseholdId,
            UnitTypeCds = model.UnitTypeCds,
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
            VoucherInd = model.VoucherInd,
            VoucherCds = model.VoucherCds,
            VoucherOther = model.VoucherOther,
            VoucherAdminName = model.VoucherAdminName,
            LiveInAideInd = model.LiveInAideInd,
            UpdateProfileInd = model.UpdateProfileInd,
            CreatedBy = username,
            ModifiedBy = username
        };
        var saved = await _applicationRepository.UpdateHouseholdInfo(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A015";
        }

        return "";
    }

    public async Task<EditableAdditionalMembersInfoViewModel> GetForAdditionalMembersEdit(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var coapplicants = new Dictionary<long, string>()
        {
            { 0, "None" }
        };
        application.MemberIds = (application.MemberIds ?? "").Trim();
        if (application.MemberIds.Length > 0)
        {
            foreach (var id in application.MemberIds.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (long.TryParse(id, out var memberId) && memberId > 0)
                {
                    var member = household.Members.FirstOrDefault(f => f.MemberId == memberId);
                    if (member != null)
                    {
                        coapplicants.Add(memberId, member.DisplayName);
                    }
                }
            }
        }

        var model = new EditableAdditionalMembersInfoViewModel
        {
            ApplicationId = application.ApplicationId,
            ListingId = application.ListingId,
            ListingDetails = listing,
            Username = username,
            HouseholdMembers = household.Members.Where(w => !w.RelationTypeCd.Equals("SELF")),
            CoApplicantInd = application.CoApplicantMemberId > 0,
            CoApplicantMemberId = application.CoApplicantMemberId,
            CoApplicants = coapplicants,
            MemberIds = application.MemberIds
        };

        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);

        return model;
    }

    public async Task<string> SaveAdditionalMembersInfo(string requestId, string correlationId, string username, EditableAdditionalMembersInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to save additional members info";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }
        model.ListingDetails = listing;

        model.HouseholdMembers = household.Members.Where(w => !w.RelationTypeCd.Equals("SELF"));
        model.MemberIds = (model.MemberIds ?? "").Trim();
        var coapplicants = new Dictionary<long, string>()
        {
            { 0, "None" }
        };
        if (model.MemberIds.Length > 0)
        {
            foreach (var id in model.MemberIds.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (long.TryParse(id, out var memberId) && memberId > 0)
                {
                    var member = household.Members.FirstOrDefault(f => f.MemberId == memberId);
                    if (member != null)
                    {
                        coapplicants.Add(memberId, member.DisplayName);
                    }
                }
            }
        }
        model.CoApplicants = coapplicants;

        model.MemberIds = (model.MemberIds ?? "").Trim();
        if (model.MemberIds.Length == 0) model.MemberIds = null;
        model.CoApplicantMemberId = (model.CoApplicantMemberId < 0) ? 0 : model.CoApplicantMemberId;

        application = new HousingApplication()
        {
            ApplicationId = model.ApplicationId,
            ListingId = model.ListingId,
            Username = username,
            HouseholdId = household.HouseholdId,
            MemberIds = model.MemberIds,
            CoApplicantMemberId = model.CoApplicantMemberId
        };
        var saved = await _applicationRepository.UpdateMembers(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A016";
        }

        return "";
    }

    public async Task<EditableIncomeAssetsInfoViewModel> GetForIncomeAssetsEdit(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        // Filter members and accounts by members selected within application
        var selectedMembers = new List<HouseholdMemberViewModel>();
        var selectedMemberAccounts = new List<HouseholdAccountViewModel>();
        selectedMembers.Add(household.Members.FirstOrDefault(f => f.MemberId == 0)); // Primary Applicant
        selectedMemberAccounts.AddRange(household.Accounts.Where(w => w.PrimaryHolderMemberId == 0)); // Primary Applicant
        application.MemberIds = (application.MemberIds ?? "").Trim();
        if (application.MemberIds.Length > 0)
        {
            foreach (var id in application.MemberIds.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (long.TryParse(id, out var memberId) && memberId > 0)
                {
                    selectedMembers.Add(household.Members.FirstOrDefault(f => f.MemberId == memberId));
                    var memberAccounts = household.Accounts.Where(w => w.PrimaryHolderMemberId == memberId);
                    if (memberAccounts.Any())
                    {
                        selectedMemberAccounts.AddRange(memberAccounts);
                    }
                }
            }
        }
        household.Members = selectedMembers;
        household.Accounts = selectedMemberAccounts;

        var model = new EditableIncomeAssetsInfoViewModel
        {
            ApplicationId = application.ApplicationId,
            ListingId = application.ListingId,
            ListingDetails = listing,
            Username = username,
            HouseholdAccounts = household.Accounts,
            HouseholdMembers = household.Members,
            AccountIds = application.AccountIds,
            TotalAssetValueAmt = selectedMemberAccounts?.Sum(s => s.AccountValueAmt) ?? 0,
            TotalIncomeValueAmt = selectedMembers?.Sum(s => s.IncomeValueAmt) ?? 0,
            TotalRealEstateValueAmt = selectedMembers?.Sum(s => s.RealEstateValueAmt) ?? 0,
            Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase)
        };

        return model;
    }

    public async Task<string> SaveIncomeAssetsInfo(string requestId, string correlationId, string username, EditableIncomeAssetsInfoViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to save income and assets";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        model.HouseholdAccounts = household.Accounts;

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }
        model.ListingDetails = listing;

        model.AccountIds = (model.AccountIds ?? "").Trim();
        if (model.AccountIds.Length == 0) model.AccountIds = null;

        application = new HousingApplication()
        {
            ApplicationId = model.ApplicationId,
            ListingId = model.ListingId,
            Username = username,
            HouseholdId = household.HouseholdId,
            AccountIds = model.AccountIds
        };
        var saved = await _applicationRepository.UpdateAccounts(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A017";
        }

        return "";
    }

    public async Task<HousingApplicationReviewSubmitViewModel> GetForReviewSubmit(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var model = new HousingApplicationReviewSubmitViewModel
        {
            ApplicationId = application.ApplicationId,
            ListingId = application.ListingId,
            ListingDetails = listing,
            Username = username,

            ApplicantInfo = await GetForApplicationEdit(requestId, correlationId, username, applicationId: applicationId),
            DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix),
            DisplayDateOfBirth = application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.DateOfBirth.Value.ToString("MM/dd/yyyy") : "",
            DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue),
            DisplayIdIssueDate = application.IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.IdIssueDate.Value.ToString("MM/dd/yyyy") : "",
            PhoneNumberTypeDescription = application.PhoneNumberTypeDescription,
            DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber),
            AltPhoneNumberTypeDescription = application.AltPhoneNumberTypeDescription,
            DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber),
            GenderDescription = application.GenderDescription,
            RaceDescription = application.RaceDescription,
            EthnicityDescription = application.EthnicityDescription,

            HouseholdInfo = await GetForHouseholdEdit(requestId, correlationId, username, applicationId),

            AdditionalMembersInfo = await GetForAdditionalMembersEdit(requestId, correlationId, username, applicationId),

            IncomeAssetsInfo = await GetForIncomeAssetsEdit(requestId, correlationId, username, applicationId),

            DocumentsReqdInd = listing.DocumentTypes.Any(),
            Documents = await GetDocuments(requestId, correlationId, username, applicationId),
        };

        if (application.VoucherInd && !string.IsNullOrEmpty(application.VoucherCds))
        {
            var voucherCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var displayVouchers = "";
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {application.VoucherOther}";
            }
            model.DisplayVouchers = displayVouchers;
        }
        else
        {
            model.DisplayVouchers = "";
        }

        model.Members = (model.IncomeAssetsInfo.HouseholdMembers ?? []).ToList();
        // foreach (var memberId in (model.AdditionalMembersInfo.MemberIds ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries))
        // {
        //     if (long.TryParse(memberId, out long id))
        //     {
        //         var member = household.Members.FirstOrDefault(f => f.MemberId == id);
        //         if (member != null)
        //         {
        //             model.Members.Add(member);
        //         }
        //     }
        // }

        model.Accounts = (model.IncomeAssetsInfo.HouseholdAccounts ?? []).ToList();

        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);
        model.CanWithdraw = ((application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase))
                                && (string.IsNullOrEmpty(model.ListingDetails.DisplayApplicationEndDate)
                                        || model.ListingDetails.ApplicationEndDate.Value.Ticks >= DateTime.Now.Ticks);

        return model;
    }

    public async Task<string> SubmitApplication(string requestId, string correlationId, string username, string siteUrl, HousingApplicationReviewSubmitViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to submit application";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        model = new HousingApplicationReviewSubmitViewModel
        {
            ApplicationId = application.ApplicationId,
            ListingId = application.ListingId,
            ListingDetails = listing,
            Username = username,

            ApplicantInfo = await GetForApplicationEdit(requestId, correlationId, username, applicationId: model.ApplicationId),
            DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix),
            DisplayDateOfBirth = application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.DateOfBirth.Value.ToString("MM/dd/yyyy") : "",
            DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue),
            DisplayIdIssueDate = application.IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.IdIssueDate.Value.ToString("MM/dd/yyyy") : "",
            PhoneNumberTypeDescription = application.PhoneNumberTypeDescription,
            DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber),
            AltPhoneNumberTypeDescription = application.AltPhoneNumberTypeDescription,
            DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber),
            GenderDescription = application.GenderDescription,
            RaceDescription = application.RaceDescription,
            EthnicityDescription = application.EthnicityDescription,

            HouseholdInfo = await GetForHouseholdEdit(requestId, correlationId, username, model.ApplicationId),

            AdditionalMembersInfo = await GetForAdditionalMembersEdit(requestId, correlationId, username, model.ApplicationId),

            IncomeAssetsInfo = await GetForIncomeAssetsEdit(requestId, correlationId, username, model.ApplicationId),
        };

        if (application.VoucherInd && !string.IsNullOrEmpty(application.VoucherCds))
        {
            var voucherCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var displayVouchers = "";
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {application.VoucherOther}";
            }
            model.DisplayVouchers = displayVouchers;
        }
        else
        {
            model.DisplayVouchers = "";
        }

        model.Members = (model.IncomeAssetsInfo.HouseholdMembers ?? []).ToList();
        // foreach (var memberId in (model.AdditionalMembersInfo.MemberIds ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries))
        // {
        //     if (long.TryParse(memberId, out long id))
        //     {
        //         var member = household.Members.FirstOrDefault(f => f.MemberId == id);
        //         if (member != null)
        //         {
        //             model.Members.Add(member);
        //         }
        //     }
        // }

        model.Accounts = (model.IncomeAssetsInfo.HouseholdAccounts ?? []).ToList();

        var saved = await _applicationRepository.Submit(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A006";
        }

        application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        application.ListingAddress = listing.AddressText;

        var emailSent = await _emailService.SendApplicationSubmittedEmail(requestId, correlationId, siteUrl, username, profile.EmailAddress, application);

        return "";
    }

    public async Task<string> WithdrawApplication(string requestId, string correlationId, string username, string siteUrl, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to withdraw application";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        // model = new HousingApplicationReviewSubmitViewModel
        // {
        //     ApplicationId = application.ApplicationId,
        //     ListingId = application.ListingId,
        //     ListingDetails = listing,
        //     Username = username,

        //     ApplicantInfo = await GetForApplicationEdit(requestId, correlationId, username, applicationId: model.ApplicationId),
        //     DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix),
        //     DisplayDateOfBirth = application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.DateOfBirth.Value.ToString("MM/dd/yyyy") : "",
        //     DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue),
        //     DisplayIdIssueDate = application.IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.IdIssueDate.Value.ToString("MM/dd/yyyy") : "",
        //     PhoneNumberTypeDescription = application.PhoneNumberTypeDescription,
        //     DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber),
        //     AltPhoneNumberTypeDescription = application.AltPhoneNumberTypeDescription,
        //     DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber),
        //     GenderDescription = application.GenderDescription,
        //     RaceDescription = application.RaceDescription,
        //     EthnicityDescription = application.EthnicityDescription,

        //     HouseholdInfo = await GetForHouseholdEdit(requestId, correlationId, username, model.ApplicationId),

        //     AdditionalMembersInfo = await GetForAdditionalMembersEdit(requestId, correlationId, username, model.ApplicationId),

        //     IncomeAssetsInfo = await GetForIncomeAssetsEdit(requestId, correlationId, username, model.ApplicationId),
        // };

        // if (application.VoucherInd && !string.IsNullOrEmpty(application.VoucherCds))
        // {
        //     var voucherCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
        //     var displayVouchers = "";
        //     var voucherTypes = await _metadataService.GetVoucherTypes(false);
        //     foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
        //     {
        //         var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
        //         displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
        //     }
        //     if (voucherCds.Contains("OTHER"))
        //     {
        //         displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {application.VoucherOther}";
        //     }
        //     model.DisplayVouchers = displayVouchers;
        // }
        // else
        // {
        //     model.DisplayVouchers = "";
        // }

        // model.Members = (model.IncomeAssetsInfo.HouseholdMembers ?? []).ToList();
        // foreach (var memberId in (model.AdditionalMembersInfo.MemberIds ?? "").Split(",", StringSplitOptions.RemoveEmptyEntries))
        // {
        //     if (long.TryParse(memberId, out long id))
        //     {
        //         var member = household.Members.FirstOrDefault(f => f.MemberId == id);
        //         if (member != null)
        //         {
        //             model.Members.Add(member);
        //         }
        //     }
        // }

        // model.Accounts = (model.IncomeAssetsInfo.HouseholdAccounts ?? []).ToList();

        application.ModifiedBy = username;
        application.ListingAddress = listing.AddressText;
        var saved = await _applicationRepository.Withdraw(requestId, correlationId, application);
        if (!saved)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A007";
        }

        var emailSent = await _emailService.SendApplicationWithdrawnEmail(requestId, correlationId, siteUrl, username, profile.EmailAddress, application);

        return "";
    }

    public async Task<IEnumerable<ApplicationDocumentViewModel>> GetDocuments(string requestId, string correlationId, string username, long applicationId)
    {
        var documents = await _applicationRepository.GetDocuments(requestId, correlationId, username, applicationId) ?? [];
        return documents?.Select(s => s.ToViewModel()) ?? [];
    }

    public async Task<ApplicationDocumentViewModel> GetDocument(string requestId, string correlationId, string username, long applicationId, long docId)
    {
        var document = await _applicationRepository.GetDocument(requestId, correlationId, username, applicationId, docId);
        if (document != null)
        {
            document.DocContents = await _applicationRepository.GetDocumentContents(requestId, correlationId, username, applicationId, docId);
        }
        return document.ToViewModel();
    }

    public async Task<EditableApplicationDocumentViewModel> GetForAddDocument(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for add document";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var model = new EditableApplicationDocumentViewModel()
        {
            Username = username,
            ListingId = application.ListingId,
            ApplicationId = applicationId,
            DocId = 0,
            DocTypeId = 0,
            DocName = "",
            DocumentTypes = await _listingService.GetDocumentTypes(requestId, correlationId, application.ListingId),
            FileName = "",
        };
        return model;
    }

    public async Task<string> AddDocument(string requestId, string correlationId, string username, EditableApplicationDocumentViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to add user document";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "D101";
        }

        model.DocTypeId = model.DocTypeId < 0 ? 0 : model.DocTypeId;
        model.DocumentTypes = await _listingService.GetDocumentTypes(requestId, correlationId, model.ListingId) ?? [];
        if (model.DocumentTypes.FirstOrDefault(f => f.DocumentTypeId == model.DocTypeId) == null)
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

        var docContentsLength = model.DocContents?.Length ?? 0;
        if (docContentsLength <= 0)
        {
            _logger.LogError($"{logPrefix} - File is invalid");
            return "D104";
        }

        if (docContentsLength > MaxFileSizeBytes)
        {
            _logger.LogError($"{logPrefix} - File size {docContentsLength} exceeds limit {MaxFileSizeBytes}");
            return "D105";
        }

        var fileNameExtension = (Path.GetExtension(model.FileName) ?? "").Replace(".", "").ToUpper();
        var allowedFileTypes = await _metadataService.GetAllowedFileTypes() ?? [];
        if (!allowedFileTypes.ContainsKey(fileNameExtension))
        {
            _logger.LogError($"{logPrefix} - File Type is not allowed");
            return "D106";
        }

        var mimeType = "application/octet-stream";
        switch (fileNameExtension)
        {
            case "PDF": mimeType = "application/pdf"; break;
            case "PNG": mimeType = "image/png"; break;
            case "JPEG": mimeType = "image/jpeg"; break;
            case "JPG": mimeType = "image/jpg"; break;
        }

        var document = new ApplicationDocument()
        {
            Username = username,
            ApplicationId = model.ApplicationId,
            DocTypeId = model.DocTypeId,
            DocName = model.DocName,
            DocContents = model.DocContents,
            FileName = model.FileName,
            MimeType = mimeType,
            ModifiedBy = username
        };

        var documents = await _applicationRepository.GetDocuments(requestId, correlationId, username, model.ApplicationId);
        var duplicate = documents.FirstOrDefault(f => f.DocTypeId == document.DocTypeId
                                                        && f.DocName.Equals(document.DocName, StringComparison.CurrentCultureIgnoreCase)
                                                        && f.FileName.Equals(document.FileName, StringComparison.CurrentCultureIgnoreCase));
        if (duplicate != null)
        {
            _logger.LogError($"{logPrefix} - Duplicate document");
            return "D002";
        }

        var added = await _applicationRepository.AddDocument(requestId, correlationId, document);
        if (!added)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "D003";
        }

        return "";
    }

    public async Task<string> DeleteDocument(string requestId, string correlationId, string username, long applicationId, long docId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to add user document";

        if (docId <= 0)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "D101";
        }

        var document = await _applicationRepository.GetDocument(requestId, correlationId, username, applicationId, docId);
        if (document == null)
        {
            _logger.LogError($"{logPrefix} - Document not found");
            return "D001";
        }

        var deleted = await _applicationRepository.DeleteDocument(requestId, correlationId, document);
        if (!deleted)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "D005";
        }

        return "";
    }

    public async Task<HAViewModel> GetApplicantInfo(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        HousingApplication application = null;
        if (applicationId > 0)
        {
            application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
            if (application == null)
            {
                _logger.LogError($"{logPrefix} - Application not found");
                return null;
            }
        }
        else
        {
            var applications = await _applicationRepository.GetAllByListing(requestId, correlationId, username, listingId);
            var eligibleApplications = applications?.Where(w => !(w.StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase));
            if (eligibleApplications?.Count() == 1)
            {
                application = eligibleApplications.First();
            }
        }

        application ??= new HousingApplication()
        {
            ListingId = listingId,
            StatusCd = "DRAFT"
        };

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var model = new HAViewModel
        {
            Username = username,
            ListingId = application.ListingId,
            ApplicationId = application.ApplicationId,
            StepNumber = 1,

            ListingDetails = listing,

            ProfileDetails = profile,
            DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix),
            DisplayDateOfBirth = application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.DateOfBirth.Value.ToString("MM/dd/yyyy") : "",
            DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue),
            DisplayIdIssueDate = application.IdIssueDate.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? application.IdIssueDate.Value.ToString("MM/dd/yyyy") : "",
            PhoneNumberTypeDescription = application.PhoneNumberTypeDescription,
            DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber),
            AltPhoneNumberTypeDescription = application.AltPhoneNumberTypeDescription,
            DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber),
            GenderDescription = application.GenderDescription,
            RaceDescription = application.RaceDescription,
            EthnicityDescription = application.EthnicityDescription,

            HouseholdDetails = household,

            DocumentsReqdInd = listing.DocumentTypes.Any(),
            Documents = await GetDocuments(requestId, correlationId, username, applicationId),
            UnitTypes = []
        };

        if (listing.Units.Any())
        {
            foreach (var unit in listing.Units)
            {
                if (!model.UnitTypes.ContainsKey(unit.UnitTypeCd))
                {
                    model.UnitTypes.Add(unit.UnitTypeCd, unit.UnitTypeDescription);
                }
            }
        }
        else
        {
            model.UnitTypes = await _metadataService.GetUnitTypes();
        }

        if (household.VoucherInd && !string.IsNullOrEmpty(household.VoucherCds))
        {
            var voucherCds = household.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var displayVouchers = "";
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {household.VoucherOther}";
            }
            model.DisplayVouchers = displayVouchers;
        }
        else
        {
            model.DisplayVouchers = "";
        }

        var coapplicants = new Dictionary<long, string>()
        {
            { 0, "None" }
        };
        model.Members = (household.Members ?? []).ToList().Where(w => w.RelationTypeCd != "SELF");
        foreach (var member in model.Members)
        {
            coapplicants.Add(member.MemberId, member.DisplayName);
        }
        model.CoApplicants = coapplicants;

        model.Accounts = (household.Accounts ?? []).ToList();

        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);
        model.CanWithdraw = ((application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase))
                                && (string.IsNullOrEmpty(model.ListingDetails.DisplayApplicationEndDate)
                                        || model.ListingDetails.ApplicationEndDate.Value.Ticks >= DateTime.Now.Ticks);

        return model;
    }

    public async Task<EditableHousingApplicationViewModel> GetForEdit(string requestId, string correlationId, string username, int listingId = 0, long applicationId = 0)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for {(applicationId > 0 ? "edit" : "add")}";

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        HousingApplication application = null;
        if (applicationId > 0)
        {
            application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
            if (application == null)
            {
                _logger.LogError($"{logPrefix} - Application not found");
                return null;
            }
        }
        else
        {
            var applications = await _applicationRepository.GetAllByListing(requestId, correlationId, username, listingId);
            var eligibleApplications = applications?.Where(w => !(w.StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase));
            if (eligibleApplications?.Count() == 1)
            {
                application = eligibleApplications.First();
            }
        }

        application ??= new HousingApplication()
        {
            ListingId = listingId,
            StatusCd = "DRAFT",

            // Profile
            Title = profile.Title,
            FirstName = profile.FirstName,
            MiddleName = profile.MiddleName,
            LastName = profile.LastName,
            Suffix = profile.Suffix,
            DateOfBirth = profile.DateOfBirth,
            Last4SSN = profile.Last4SSN,
            IdTypeCd = profile.IdTypeCd,
            IdTypeDescription = profile.IdTypeDescription,
            IdTypeValue = profile.IdTypeValue,
            IdIssueDate = profile.IdIssueDate,
            GenderCd = profile.GenderCd,
            GenderDescription = profile.GenderDescription,
            RaceCd = profile.RaceCd,
            RaceDescription = profile.RaceDescription,
            EthnicityCd = profile.EthnicityCd,
            EthnicityDescription = profile.EthnicityDescription,
            CountyLivingIn = profile.CountyLivingIn,
            EverLivedInWestchesterInd = profile.EverLivedInWestchesterInd,
            CountyWorkingIn = profile.CountyWorkingIn,
            CurrentlyWorkingInWestchesterInd = profile.CurrentlyWorkingInWestchesterInd,

            // Address
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

            // Contact Information
            EmailAddress = profile.EmailAddress,
            AltEmailAddress = profile.AltEmailAddress,
            PhoneNumber = profile.PhoneNumber,
            PhoneNumberExtn = profile.PhoneNumberExtn,
            PhoneNumberTypeCd = profile.PhoneNumberTypeCd,
            PhoneNumberTypeDescription = profile.PhoneNumberTypeDescription,
            AltPhoneNumber = profile.AltPhoneNumber,
            AltPhoneNumberExtn = profile.AltPhoneNumberExtn,
            AltPhoneNumberTypeCd = profile.AltPhoneNumberTypeCd,
            AltPhoneNumberTypeDescription = profile.AltPhoneNumberTypeDescription,

            // Household Information
            VoucherInd = household.VoucherInd,
            VoucherCds = household.VoucherCds,
            VoucherOther = household.VoucherOther,
            VoucherAdminName = household.VoucherAdminName,
            LiveInAideInd = household.LiveInAideInd,

            // Application Information
            UnitTypeCds = "",
            MemberIds = "",
            CoApplicantMemberId = 0,
            AccountIds = "",
            AccessibilityCds = "",
            LeadTypeCd = "",
            LeadTypeOther = "",
        };

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var unitTypes = new Dictionary<string, string>();
        if (listing.Units.Any())
        {
            foreach (var unit in listing.Units)
            {
                if (!unitTypes.ContainsKey(unit.UnitTypeCd))
                {
                    unitTypes.Add(unit.UnitTypeCd, unit.UnitTypeDescription);
                }
            }
        }
        else
        {
            unitTypes = await _metadataService.GetUnitTypes();
        }

        var members = new List<HouseholdMemberViewModel>();
        var coapplicants = new Dictionary<long, string>()
        {
            { 0, "None" }
        };
        var accounts = new List<HouseholdAccountViewModel>();

        application.MemberIds = (application.MemberIds ?? "").Trim();
        if (application.MemberIds.Length > 0)
        {
            foreach (var id in application.MemberIds.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (long.TryParse(id, out var memberId) && memberId > 0)
                {
                    var member = household.Members.FirstOrDefault(f => f.MemberId == memberId);
                    if (member != null)
                    {
                        members.Add(member);
                        coapplicants.Add(memberId, member.DisplayName);
                        var memberAccounts = household.Accounts.Where(w => w.PrimaryHolderMemberId == memberId);
                        if (memberAccounts.Any())
                        {
                            accounts.AddRange(memberAccounts);
                        }
                    }
                }
            }
        }
        application.AccountIds = string.Join(",", accounts.Select(s => s.AccountId));

        var displayVouchers = "";
        if (application.VoucherInd && !string.IsNullOrEmpty(application.VoucherCds))
        {
            var voucherCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {household.VoucherOther}";
            }
        }

        var model = new EditableHousingApplicationViewModel
        {
            Username = username,
            ListingId = application.ListingId,
            ApplicationId = application.ApplicationId,
            StepNumber = 1,

            ListingDetails = listing,
            ProfileDetails = profile,
            HouseholdDetails = household,
            ApplicationDetails = application.ToViewModel(),

            UnitTypes = unitTypes,
            UnitTypeCds = application.UnitTypeCds,

            Members = members,
            MemberIds = application.MemberIds,
            CoApplicants = coapplicants,
            CoApplicantInd = application.CoApplicantInd,
            CoApplicantMemberId = application.CoApplicantMemberId,

            Accounts = accounts,
            AccountIds = application.AccountIds,

            AccessibilityCds = application.AccessibilityCds,

            LeadTypeCd = application.LeadTypeCd,
            LeadTypeOther = application.LeadTypeOther,
            LeadTypes = await _metadataService.GetLeadTypes(true),

            DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix),
            DisplayDateOfBirth = _uiHelperService.ToDateTimeDisplayText(application.DateOfBirth, "MM/dd/yyyy"),
            DisplayIdIssueDate = _uiHelperService.ToDateTimeDisplayText(application.IdIssueDate, "MM/dd/yyyy"),
            DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue),
            DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber),
            DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber),
            DisplayVouchers = displayVouchers,

            ForWaitlist = listing.CanApplyWaitlist
        };

        model.Members = (household.Members ?? []).ToList().Where(w => w.RelationTypeCd != "SELF");
        model.CoApplicants = coapplicants;

        model.Accounts = (household.Accounts ?? []).ToList();

        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);
        model.CanWithdraw = ((application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("WAITLISTED", StringComparison.CurrentCultureIgnoreCase))
                                && (listing.CanApply || listing.CanApplyWaitlist);

        return model;
    }

    public async Task<string> ValidateApplicationInfo(string requestId, string correlationId, string username, EditableHousingApplicationViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to validate application for {(model.ApplicationId > 0 ? "edit" : "add")}";

        model.UnitTypeCds = (model.UnitTypeCds ?? "").Trim().ToUpper();
        model.MemberIds = (model.MemberIds ?? "").Trim();
        model.CoApplicantMemberId = model.CoApplicantMemberId < 0 ? 0 : model.CoApplicantMemberId;
        model.LeadTypeCd = (model.LeadTypeCd ?? "").Trim().ToUpper();
        model.LeadTypeOther = (model.LeadTypeOther ?? "").Trim();

        // Validations
        if (model.UnitTypeCds.Length == 0)
        {
            _logger.LogError($"{logPrefix} - No unit types selected");
            return "A103";
        }

        var leadTypes = await _metadataService.GetLeadTypes();
        if (!leadTypes.ContainsKey(model.LeadTypeCd))
        {
            _logger.LogError($"{logPrefix} - Lead Type is invalid");
            return "A108";
        }
        if (model.LeadTypeCd == "WEBSITE" || model.LeadTypeCd == "NEWSPAPERART" || model.LeadTypeCd == "OTHER")
        {
            if (string.IsNullOrEmpty(model.LeadTypeOther))
            {
                _logger.LogError($"{logPrefix} - Lead Type Other is required");
                return "A109";
            }
        }

        return "";
    }

    public async Task<string> SubmitApplication(string requestId, string correlationId, string username, string siteUrl, EditableHousingApplicationViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to submit application";

        HousingApplication application = null;
        if (model.ApplicationId > 0)
        {
            application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
            if (application == null)
            {
                _logger.LogError($"{logPrefix} - Application not found");
                return "A001";
            }
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return "H001";
        }

        var listing = await _listingService.GetOne(requestId, correlationId, model.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return "L001";
        }

        if (!listing.CanApply && !listing.CanApplyWaitlist)
        {
            _logger.LogError($"{logPrefix} - Past due date");
            return "A104";
        }

        application = new HousingApplication()
        {
            ApplicationId = model.ApplicationId,
            ListingId = model.ListingId,
            ListingAddress = listing.AddressText,
            Username = username,
            CreatedBy = username,
            ModifiedBy = username,
            UnitTypeCds = model.UnitTypeCds,
            MemberIds = model.MemberIds,
            CoApplicantInd = model.CoApplicantInd,
            CoApplicantMemberId = model.CoApplicantMemberId,
            AccountIds = model.AccountIds,
            AccessibilityCds = model.AccessibilityCds,
            LeadTypeCd = model.LeadTypeCd,
            LeadTypeOther = model.LeadTypeOther,
            StatusCd = listing.CanApplyWaitlist ? "WAITLISTED" : "SUBMITTED"
        };

        model.ApplicationId = await _applicationRepository.SubmitEx(requestId, correlationId, application);
        if (model.ApplicationId <= 0)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "A006";
        }

        application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        application.ListingAddress = listing.AddressText;

        var emailSent = await _emailService.SendApplicationSubmittedEmail(requestId, correlationId, siteUrl, username, profile.EmailAddress, application);

        return "";
    }

    public async Task<HousingApplicationViewModel> GetSubmitted(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to retrieve submitted application";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return null;
        }

        var household = await _householdService.GetOne(requestId, correlationId, username);
        if (household == null)
        {
            _logger.LogError($"{logPrefix} - Household not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }

        var unitTypes = new Dictionary<string, string>();
        if (listing.Units.Any())
        {
            foreach (var unit in listing.Units)
            {
                if (!unitTypes.ContainsKey(unit.UnitTypeCd))
                {
                    unitTypes.Add(unit.UnitTypeCd, unit.UnitTypeDescription);
                }
            }
        }
        else
        {
            unitTypes = await _metadataService.GetUnitTypes();
        }

        var members = new List<HouseholdMemberViewModel>();
        var accounts = new List<HouseholdAccountViewModel>();
        if (household.Accounts.Count != 0)
        {
            var applicantAccounts = household.Accounts.Where(w => w.PrimaryHolderMemberId == 0);
            if (applicantAccounts.Any())
            {
                accounts.AddRange(applicantAccounts);
            }
        }

        application.MemberIds = (application.MemberIds ?? "").Trim();
        if (application.MemberIds.Length > 0)
        {
            foreach (var id in application.MemberIds.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                if (long.TryParse(id, out var memberId) && memberId > 0)
                {
                    var member = household.Members.FirstOrDefault(f => f.MemberId == memberId);
                    if (member != null)
                    {
                        members.Add(member);
                        var memberAccounts = household.Accounts.Where(w => w.PrimaryHolderMemberId == memberId);
                        if (memberAccounts.Any())
                        {
                            accounts.AddRange(memberAccounts);
                        }
                    }
                }
            }
        }
        application.AccountIds = string.Join(",", accounts.Select(s => s.AccountId));

        var displayVouchers = "";
        if (application.VoucherInd && !string.IsNullOrEmpty(application.VoucherCds))
        {
            var voucherCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var voucherTypes = await _metadataService.GetVoucherTypes(false);
            foreach (var voucherTypeCd in voucherCds.Where(w => !w.Equals("OTHER", StringComparison.OrdinalIgnoreCase)))
            {
                var displayVoucherText = voucherTypes.TryGetValue(voucherTypeCd, out string value) ? value : voucherTypeCd;
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + displayVoucherText;
            }
            if (voucherCds.Contains("OTHER"))
            {
                displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {household.VoucherOther}";
            }
        }

        var model = application.ToViewModel();
        model.ListingAddress = listing.AddressText;
        model.UnitTypes = unitTypes;
        model.Members = members;
        model.Accounts = accounts;
        model.DisplayName = _uiHelperService.ToDisplayName(application.Title, application.FirstName, application.MiddleName, application.LastName, application.Suffix);
        model.DisplayDateOfBirth = _uiHelperService.ToDateTimeDisplayText(application.DateOfBirth, "MM/dd/yyyy");
        model.DisplayIdIssueDate = _uiHelperService.ToDateTimeDisplayText(application.IdIssueDate, "MM/dd/yyyy");
        model.DisplayIdTypeValue = _uiHelperService.ToDisplayIdTypeValue(application.IdTypeDescription, application.IdTypeValue);
        model.DisplayPhoneNumber = _uiHelperService.ToPhoneNumberText(application.PhoneNumber);
        model.DisplayAltPhoneNumber = _uiHelperService.ToPhoneNumberText(application.AltPhoneNumber);
        model.DisplayVouchers = displayVouchers;
        model.DisplayLeadType = _uiHelperService.ToOtherAndValueText(application.LeadTypeCd, application.LeadTypeDescription, application.LeadTypeOther, "Not Specified");
        model.Editable = (application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase);
        model.CanWithdraw = ((application.StatusCd ?? "").Trim().Equals("DRAFT", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("SUBMITTED", StringComparison.CurrentCultureIgnoreCase)
                                    || (application.StatusCd ?? "").Trim().Equals("WAITLISTED", StringComparison.CurrentCultureIgnoreCase))
                                && (listing.CanApply || listing.CanApplyWaitlist);

        model.DocumentsReqdInd = listing.DocumentTypes.Any();
        model.ApplicationComments = await GetComments(requestId, correlationId, username, applicationId);
        model.ApplicationDocuments = await GetDocuments(requestId, correlationId, username, applicationId);
        model.ListingDetails = listing;
        model.CanComment = (model.IsSubmitted || model.IsWaitlisted)
                            && model.IsPotentialDuplicate
                            && model.HasComments
                            && DateTime.Now.Ticks <= application.DuplicateCheckResponseDueDate.GetValueOrDefault(DateTime.MaxValue).Ticks;
        return model;
    }

    public async Task<IEnumerable<ApplicationCommentViewModel>> GetComments(string requestId, string correlationId, string username, long applicationId)
    {
        var comments = await _applicationRepository.GetComments(requestId, correlationId, username, applicationId) ?? [];
        return comments?.Select(s => s.ToViewModel()) ?? [];
    }

    public async Task<EditableApplicationCommentViewModel> GetForAddComment(string requestId, string correlationId, string username, long applicationId)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to get application for add comment";

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, applicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var model = new EditableApplicationCommentViewModel()
        {
            Username = username,
            ListingId = application.ListingId,
            ApplicationId = applicationId,
            CommentId = 0,
            Comments = ""
        };
        return model;
    }

    public async Task<string> AddComment(string requestId, string correlationId, string username, string siteUrl, EditableApplicationCommentViewModel model)
    {
        var logPrefix = $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Unable to add user comment";

        if (model == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "C101";
        }

        model.Comments = (model.Comments ?? "").Trim();
        if (string.IsNullOrEmpty(model.Comments))
        {
            _logger.LogError($"{logPrefix} - Comment is required");
            return "C102";
        }

        var application = await _applicationRepository.GetOne(requestId, correlationId, username, model.ApplicationId);
        if (application == null)
        {
            _logger.LogError($"{logPrefix} - Application not found");
            return null;
        }

        var listing = await _listingService.GetOne(requestId, correlationId, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"{logPrefix} - Listing not found");
            return null;
        }
        application.ListingAddress = listing.AddressText;

        var profile = await _profileService.GetOne(requestId, correlationId, username);
        if (profile == null)
        {
            _logger.LogError($"{logPrefix} - Profile not found");
            return "P001";
        }

        var comment = new ApplicationComment()
        {
            Username = username,
            ApplicationId = model.ApplicationId,
            CommentId = 0,
            Comments = model.Comments,
            ModifiedBy = username
        };

        var added = await _applicationRepository.AddComment(requestId, correlationId, comment);
        if (!added)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "C003";  
        }

        var emailSent = await _emailService.SendApplicationCommentEmail(requestId, correlationId, siteUrl, username, profile.EmailAddress, application, model.Comments);

        return "";
    }
}