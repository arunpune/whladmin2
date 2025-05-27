using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IHousingApplicationsService
{
    Task<Dictionary<long, string>> GetListings(string requestId, string correlationId, string username);
    Task<HousingApplicationsViewModel> GetData(string requestId, string correlationId, string username, long listingId = 0, string submissionTypeCd = "ALL", string statusCd = "ALL", int pageNo = 1, int pageSize = 1000);
    Task<string> GetDownloadData(string requestId, string correlationId, string username, long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL");
    Task<IEnumerable<HousingApplicationViewModel>> GetAll(string requestId, string correlationId, long listingId = 0, string submissionTypeCd = "ALL", string statusCd = "ALL");
    Task<HousingApplicationViewModel> GetOne(string requestId, string correlationId, string username, long applicationId);
    Task<EditableHousingApplicationViewModel> GetOneForAdd(string requestId, string correlationId, long listingId);
    Task<EditableHousingApplicationViewModel> GetOneForEdit(string requestId, string correlationId, long applicationId);
    Task<string> Add(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item);
    Task<string> Update(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item);
    Task<string> UpdateHouseholdInfo(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item);
    Task<string> Delete(string requestId, string correlationId, string username, long applicationId);
    void Sanitize(string requestId, string correlationId, string username, HousingApplication application);
    Task<string> ValidateApplication(string requestId, string correlationId, string username, HousingApplication application);
    Task<string> ValidateApplicantInfo(string requestId, string correlationId, HousingApplication application);
    Task<string> ValidateHouseholdInfo(string requestId, string correlationId, HousingApplication application);
    Task<string> ValidateHouseholdMemberInfo(string requestId, string correlationId, HouseholdMember member);

    Task<DuplicatesViewModel> GetPotentialDuplicates(string requestId, string correlationId, string username, long listingId);
    Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByDoBLast4Ssn(string requestId, string correlationId, string username, long listingId, string dateOfBirth, string last4Ssn);
    Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByName(string requestId, string correlationId, string username, long listingId, string name);
    Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByEmailAddress(string requestId, string correlationId, string username, long listingId, string emailAddress);
    Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByPhoneNumber(string requestId, string correlationId, string username, long listingId, string phoneNumber);
    Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByStreetAddress(string requestId, string correlationId, string username, long listingId, string streetAddress);
    Task<string> UpdateDuplicateStatus(string requestId, string correlationId, string username, string siteUrl, EditableDuplicateApplicationViewModel model);
    Task<ApplicationCommentsViewModel> GetComments(string requestId, string correlationId, string username, long applicationId);
    EditableApplicationCommentViewModel GetOneForAddComment(string requestId, string correlationId, string username, long applicationId);
    Task<string> AddComment(string requestId, string correlationId, string username, EditableApplicationCommentViewModel model);
}

public class HousingApplicationsService : IHousingApplicationsService
{
    private readonly ILogger<HousingApplicationsService> _logger;
    private readonly IHousingApplicationRepository _applicationRepository;
    private readonly IListingsService _listingsService;
    private readonly IMetadataService _metadataService;
    private readonly IEmailService _emailService;
    private readonly IPhoneService _phoneService;
    private readonly IUsersService _usersService;
    private readonly string _publicSiteUrl;

    public HousingApplicationsService(ILogger<HousingApplicationsService> logger, IConfiguration configuration, IHousingApplicationRepository housingApplicationRepository, IListingsService listingsService, IMetadataService metadataService, IEmailService emailService, IPhoneService phoneService, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationRepository = housingApplicationRepository ?? throw new ArgumentNullException(nameof(housingApplicationRepository));
        _listingsService = listingsService ?? throw new ArgumentNullException(nameof(listingsService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _phoneService = phoneService ?? throw new ArgumentNullException(nameof(phoneService));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        _publicSiteUrl = configuration.GetValue<string>("PublicSiteUrl");
    }

    public async Task<Dictionary<long, string>> GetListings(string requestId, string correlationId, string username)
    {
        var listings = await _listingsService.GetPublishedListings(requestId, correlationId, username);
        var results = new Dictionary<long, string>();
        // {
        //     { 0, "All Listings" }
        // };
        foreach (var listing in listings?.Where(w => (w.StatusCd ?? "").Equals("PUBLISHED", StringComparison.CurrentCultureIgnoreCase)))
        {
            results.Add(listing.ListingId, listing.Name);
        }
        return results;
    }

    public async Task<HousingApplicationsViewModel> GetData(string requestId, string correlationId, string username, long listingId = 0, string submissionTypeCd = "ALL", string statusCd = "ALL", int pageNo = 1, int pageSize = 1000)
    {
        var userRole = await _usersService.GetUserRole(correlationId, username);
        var pagedApplications = await _applicationRepository.GetAllPaged(listingId, submissionTypeCd, statusCd, pageNo, pageSize);
        var model = new HousingApplicationsViewModel
        {
            ListingId = listingId,
            Listings = await GetListings(requestId, correlationId, username),
            HousingApplications = pagedApplications.Records.Select(s => s.ToViewModel()),
            PageNo = pagedApplications.PagingInfo.PageNo,
            PageSize = pagedApplications.PagingInfo.PageSize,
            TotalPages = pagedApplications.PagingInfo.TotalPages,
            TotalRecords = pagedApplications.PagingInfo.TotalRecords,
            PageSizes = [ 100, 250, 500, 1000 ],
            SubmissionTypeCd = submissionTypeCd,
            SubmissionTypes = await _metadataService.GetSubmissionTypes(),
            StatusCd = statusCd,
            Statuses = await _metadataService.GetApplicationStatuses(),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
        if (model.ListingId <= 0 && (model.Listings?.Count ?? 0) > 0)
        {
            model.ListingId = model.Listings.First().Key;
        }
        return model;
    }

    public async Task<string> GetDownloadData(string requestId, string correlationId, string username, long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL")
    {
        var dataTable = await _applicationRepository.GetDownload(listingId, submissionTypeCd, statusCd);
        return dataTable.ToCsvFileContents();
    }

    public async Task<IEnumerable<HousingApplicationViewModel>> GetAll(string requestId, string correlationId, long listingId = 0, string submissionTypeCd = "ALL", string statusCd = "ALL")
    {
        var applications = await _applicationRepository.GetAll(listingId, submissionTypeCd, statusCd);
        if ((applications?.Count() ?? 0) == 0) applications = new List<HousingApplicationViewModel>();
        return applications.Select(s => s.ToViewModel());
    }

    public async Task<HousingApplicationViewModel> GetOne(string requestId, string correlationId, string username, long applicationId)
    {
        var application = await _applicationRepository.GetOne(new HousingApplication() { ApplicationId = applicationId });
        if (application != null)
        {
            var model = application.ToViewModel();

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
                    displayVouchers += (displayVouchers.Length > 0 ? ", " : "") + $"Other: {application.VoucherOther}";
                }
            }
            model.DisplayVouchers = displayVouchers;

            var listing = await _listingsService.GetOne(requestId, correlationId, username, application.ListingId, true);
            model.ListingDetails = listing;

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
            model.UnitTypes = unitTypes;

            var members = await _applicationRepository.GetApplicants(applicationId) ?? [];
            var applicants = members.Select(s => s.ToViewModel());
            model.ApplicationMembers = applicants.Where(w => !w.RelationTypeCd.Equals("SELF"));

            var accounts = await _applicationRepository.GetApplicantAccounts(applicationId) ?? [];
            var applicantAccounts = accounts.Select(s => s.ToViewModel());
            foreach (var applicantAccount in applicantAccounts)
            {
                applicantAccount.PrimaryHolderMemberName = applicants.FirstOrDefault(f => f.ApplicantId == applicantAccount.ApplicantId)?.DisplayName ?? "";
            }
            model.ApplicationAccounts = applicantAccounts;

            return model;
        }
        return null;
    }

    public async Task<EditableHousingApplicationViewModel> GetOneForAdd(string requestId, string correlationId, long listingId)
    {
        return new EditableHousingApplicationViewModel()
        {
            ListingId = listingId,

            ApplicationId = 0,
            Title = "",
            FirstName = "",
            MiddleName = "",
            LastName = "",
            Suffix = "",
            Last4SSN = "",
            DateOfBirth = "",
            IdTypeCd = "",
            IdTypeValue = "",
            IdIssueDate = "",
            GenderCd = "NOTSPEC",
            RaceCd = "NOTSPEC",
            EthnicityCd = "NOTSPEC",
            Pronouns = "",
            CountyLivingIn = "",
            CountyWorkingIn = "",
            StudentInd = false,
            DisabilityInd = false,
            VeteranInd = false,
            EverLivedInWestchesterInd = false,
            CurrentlyWorkingInWestchesterInd = false,
            PhoneNumberTypeCd = "",
            PhoneNumber = "",
            PhoneNumberExtn = "",
            AltPhoneNumberTypeCd = "",
            AltPhoneNumber = "",
            AltPhoneNumberExtn = "",
            EmailAddress = "",
            AltEmailAddress = "",
            OwnRealEstateInd = false,
            RealEstateValueAmt = 0,
            AssetValueAmt = 0,
            IncomeValueAmt = 0,

            AddressInd = false,
            PhysicalStreetLine1 = "",
            PhysicalStreetLine2 = "",
            PhysicalStreetLine3 = "",
            PhysicalCity = "",
            PhysicalStateCd = "",
            PhysicalZipCode = "",
            PhysicalCounty = "",
            DifferentMailingAddressInd = false,
            MailingStreetLine1 = "",
            MailingStreetLine2 = "",
            MailingStreetLine3 = "",
            MailingCity = "",
            MailingStateCd = "",
            MailingZipCode = "",
            MailingCounty = "",
            VoucherInd = false,
            VoucherCds = "",
            VoucherOther = "",
            VoucherAdminName = "",

            CoApplicantInd = false,
            CoRelationTypeCd = "",
            CoRelationTypeOther = "",
            CoTitle = "",
            CoFirstName = "",
            CoMiddleName = "",
            CoLastName = "",
            CoSuffix = "",
            CoLast4SSN = "",
            CoDateOfBirth = "",
            CoIdTypeCd = "",
            CoIdTypeValue = "",
            CoIdIssueDate = "",
            CoGenderCd = "NOTSPEC",
            CoRaceCd = "NOTSPEC",
            CoEthnicityCd = "NOTSPEC",
            CoPronouns = "",
            CoCountyLivingIn = "",
            CoCountyWorkingIn = "",
            CoStudentInd = false,
            CoDisabilityInd = false,
            CoVeteranInd = false,
            CoEverLivedInWestchesterInd = false,
            CoCurrentlyWorkingInWestchesterInd = false,
            CoPhoneNumberTypeCd = "",
            CoPhoneNumber = "",
            CoPhoneNumberExtn = "",
            CoAltPhoneNumberTypeCd = "",
            CoAltPhoneNumber = "",
            CoAltPhoneNumberExtn = "",
            CoEmailAddress = "",
            CoAltEmailAddress = "",
            CoOwnRealEstateInd = false,
            CoRealEstateValueAmt = 0,
            CoAssetValueAmt = 0,
            CoIncomeValueAmt = 0,

            LeadTypeCd = "",
            LeadTypeOther = "",

            IdTypes = await _metadataService.GetIdTypes(true),
            GenderTypes = await _metadataService.GetGenderTypes(true),
            RaceTypes = await _metadataService.GetRaceTypes(true),
            EthnicityTypes = await _metadataService.GetEthnicityTypes(true),
            PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true),
            VoucherTypes = await _metadataService.GetVoucherTypes(),
            RelationTypes = await _metadataService.GetRelationTypes(true),
            LeadTypes = await _metadataService.GetLeadTypes(true)
        };
    }

    public async Task<EditableHousingApplicationViewModel> GetOneForEdit(string requestId, string correlationId, long applicationId)
    {
        var application = await _applicationRepository.GetOne(new HousingApplication() { ApplicationId = applicationId });
        var model = application.ToEditableViewModel();
        if (model != null)
        {
            model.IdTypes = await _metadataService.GetIdTypes(true);
            model.GenderTypes = await _metadataService.GetGenderTypes(true);
            model.RaceTypes = await _metadataService.GetRaceTypes(true);
            model.EthnicityTypes = await _metadataService.GetEthnicityTypes(true);
            model.PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true);
            model.VoucherTypes = await _metadataService.GetVoucherTypes();
            model.RelationTypes = await _metadataService.GetRelationTypes(true);
            model.LeadTypes = await _metadataService.GetLeadTypes(true);
        }
        return model;
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item)
    {
        if (item == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        var application = new HousingApplication()
        {
            ApplicationId = item.ApplicationId,
            ListingId = item.ListingId,
            Title = item.Title,
            FirstName = item.FirstName,
            MiddleName = string.IsNullOrEmpty(item.MiddleName ?? "") ? null : item.MiddleName,
            LastName = item.LastName,
            Suffix = string.IsNullOrEmpty(item.Suffix ?? "") ? null : item.Suffix,
            Last4SSN = item.Last4SSN,
            DateOfBirth = DateTime.TryParse(item.DateOfBirth, out var dob) ? dob : null,
            IdTypeCd = item.IdTypeCd,
            IdTypeValue = item.IdTypeValue,
            IdIssueDate = DateTime.TryParse(item.IdIssueDate, out var isd) ? isd : null,
            GenderCd = item.GenderCd,
            RaceCd = item.RaceCd,
            EthnicityCd = item.EthnicityCd,
            Pronouns = item.Pronouns,
            CountyLivingIn = item.CountyLivingIn,
            CountyWorkingIn = item.CountyWorkingIn,
            StudentInd = item.StudentInd,
            DisabilityInd = item.DisabilityInd,
            VeteranInd = item.VeteranInd,
            EverLivedInWestchesterInd = item.EverLivedInWestchesterInd,
            CurrentlyWorkingInWestchesterInd = item.CurrentlyWorkingInWestchesterInd,
            PhoneNumberTypeCd = item.PhoneNumberTypeCd,
            PhoneNumber = item.PhoneNumber,
            PhoneNumberExtn = item.PhoneNumberExtn,
            AltPhoneNumberTypeCd = item.AltPhoneNumberTypeCd,
            AltPhoneNumber = item.AltPhoneNumber,
            AltPhoneNumberExtn = item.AltPhoneNumberExtn,
            EmailAddress = item.EmailAddress,
            AltEmailAddress = item.AltEmailAddress,
            OwnRealEstateInd = item.OwnRealEstateInd,
            RealEstateValueAmt = item.RealEstateValueAmt,
            AssetValueAmt = item.AssetValueAmt,
            IncomeValueAmt = item.IncomeValueAmt,
            LeadTypeCd = "OTHER",
            LeadTypeOther = item.LeadTypeOther,
            SubmissionTypeCd = "PAPER",
            CreatedBy = username
        };
        Sanitize(requestId, correlationId, username, application);

        var validationCode = await ValidateApplication(requestId, correlationId, username, application);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for application");
            return validationCode;
        }

        validationCode = await ValidateApplicantInfo(requestId, correlationId, application);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for primary applicant information");
            return validationCode;
        }

        if (!item.OverrideDuplicateWarning)
        {
            var applications = await _applicationRepository.GetAllByLast4SSN(application.ListingId, application.Last4SSN);
            if (applications.Any())
            {
                item.DuplicateApplicationIds = applications.Where(s => s.ApplicationId != item.ApplicationId).Select(s => s.ApplicationId).ToList();
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Potential duplicate applications");
                return "H002";
            }
        }

        var added = await _applicationRepository.Add(correlationId, application);
        if (!added)
        {
            return "H003";
        }

        item.ApplicationId = application.ApplicationId;
        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item)
    {
        if (item == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        var application = new HousingApplication()
        {
            ApplicationId = item.ApplicationId,
            ListingId = item.ListingId,
            Title = item.Title,
            FirstName = item.FirstName,
            MiddleName = string.IsNullOrEmpty(item.MiddleName ?? "") ? null : item.MiddleName,
            LastName = item.LastName,
            Suffix = string.IsNullOrEmpty(item.Suffix ?? "") ? null : item.Suffix,
            Last4SSN = item.Last4SSN,
            DateOfBirth = DateTime.TryParse(item.DateOfBirth, out var dob) ? dob : null,
            IdTypeCd = item.IdTypeCd,
            IdTypeValue = item.IdTypeValue,
            IdIssueDate = DateTime.TryParse(item.IdIssueDate, out var isd) ? isd : null,
            GenderCd = item.GenderCd,
            RaceCd = item.RaceCd,
            EthnicityCd = item.EthnicityCd,
            Pronouns = item.Pronouns,
            CountyLivingIn = item.CountyLivingIn,
            CountyWorkingIn = item.CountyWorkingIn,
            StudentInd = item.StudentInd,
            DisabilityInd = item.DisabilityInd,
            VeteranInd = item.VeteranInd,
            EverLivedInWestchesterInd = item.EverLivedInWestchesterInd,
            CurrentlyWorkingInWestchesterInd = item.CurrentlyWorkingInWestchesterInd,
            PhoneNumberTypeCd = item.PhoneNumberTypeCd,
            PhoneNumber = item.PhoneNumber,
            PhoneNumberExtn = item.PhoneNumberExtn,
            AltPhoneNumberTypeCd = item.AltPhoneNumberTypeCd,
            AltPhoneNumber = item.AltPhoneNumber,
            AltPhoneNumberExtn = item.AltPhoneNumberExtn,
            EmailAddress = item.EmailAddress,
            AltEmailAddress = item.AltEmailAddress,
            OwnRealEstateInd = item.OwnRealEstateInd,
            RealEstateValueAmt = item.RealEstateValueAmt,
            AssetValueAmt = item.AssetValueAmt,
            IncomeValueAmt = item.IncomeValueAmt,
            LeadTypeCd = "OTHER",
            LeadTypeOther = item.LeadTypeOther,
            SubmissionTypeCd = "PAPER",
            CreatedBy = username
        };
        Sanitize(requestId, correlationId, username, application);

        var validationCode = await ValidateApplication(requestId, correlationId, username, application);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for application");
            return validationCode;
        }

        validationCode = await ValidateApplicantInfo(requestId, correlationId, application);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for primary applicant information");
            return validationCode;
        }

        if (!item.OverrideDuplicateWarning)
        {
            var applications = await _applicationRepository.GetAllByLast4SSN(application.ListingId, application.Last4SSN);
            if (applications.Any())
            {
                item.DuplicateApplicationIds = applications.Where(s => s.ApplicationId != item.ApplicationId).Select(s => s.ApplicationId).ToList();
                _logger.LogWarning($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Potential duplicate applications");
                return "H002";
            }
        }

        var added = await _applicationRepository.Update(correlationId, application);
        if (!added)
        {
            return "H004";
        }

        item.ApplicationId = application.ApplicationId;
        return "";
    }

    public async Task<string> UpdateHouseholdInfo(string requestId, string correlationId, string username, EditableHousingApplicationViewModel item)
    {
        if (item == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        var application = new HousingApplication()
        {
            ApplicationId = item.ApplicationId,
            AddressInd = item.AddressInd,
            PhysicalStreetLine1 = item.PhysicalStreetLine1,
            PhysicalStreetLine2 = item.PhysicalStreetLine2,
            PhysicalStreetLine3 = item.PhysicalStreetLine3,
            PhysicalCity = item.PhysicalCity,
            PhysicalStateCd = item.PhysicalStateCd,
            PhysicalZipCode = item.PhysicalZipCode,
            PhysicalCounty = item.PhysicalCounty,
            DifferentMailingAddressInd = item.DifferentMailingAddressInd,
            MailingStreetLine1 = item.MailingStreetLine1,
            MailingStreetLine2 = item.MailingStreetLine2,
            MailingStreetLine3 = item.MailingStreetLine3,
            MailingCity = item.MailingCity,
            MailingStateCd = item.MailingStateCd,
            MailingZipCode = item.MailingZipCode,
            MailingCounty = item.MailingCounty,
            VoucherInd = item.VoucherInd,
            VoucherCds = item.VoucherCds,
            VoucherOther = item.VoucherOther,
            VoucherAdminName = item.VoucherAdminName,
            ModifiedBy = username
        };
        Sanitize(requestId, correlationId, username, application);

        var validationCode = await ValidateHouseholdInfo(requestId, correlationId, application);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for household information");
            return validationCode;
        }

        var updated = await _applicationRepository.UpdateHouseholdInfo(correlationId, application);
        if (!updated)
        {
            return "H004";
        }

        item.ApplicationId = application.ApplicationId;
        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, long applicationId)
    {
        var existingApplication = await _applicationRepository.GetOne(new HousingApplication() { ApplicationId = applicationId });
        if (existingApplication == null)
        {
            _logger.LogError($"Unable to find housing application - {applicationId}");
            return "H001";
        }

        existingApplication.ModifiedBy = username;
        var deleted = await _applicationRepository.Delete(correlationId, existingApplication);
        if (!deleted)
        {
            return "H005";
        }

        return "";
    }

    public void Sanitize(string requestId, string correlationId, string username, HousingApplication application)
    {
        if (application == null) return;

        if (application.ListingId <= 0) application.ListingId = 0;

        application.Title = (application.Title ?? "").Trim();
        if (string.IsNullOrEmpty(application.Title)) application.Title = null;

        application.FirstName = (application.FirstName ?? "").Trim();
        if (string.IsNullOrEmpty(application.FirstName)) application.FirstName = null;

        application.MiddleName = (application.MiddleName ?? "").Trim();
        if (string.IsNullOrEmpty(application.MiddleName)) application.MiddleName = null;

        application.LastName = (application.LastName ?? "").Trim();
        if (string.IsNullOrEmpty(application.LastName)) application.LastName = null;

        application.Suffix = (application.Suffix ?? "").Trim();
        if (string.IsNullOrEmpty(application.Suffix)) application.Suffix = null;

        application.Last4SSN = (application.Last4SSN ?? "").Trim();
        if (string.IsNullOrEmpty(application.Last4SSN)) application.Last4SSN = null;

        application.DateOfBirth = application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? null : application.DateOfBirth.Value;

        application.IdTypeCd = (application.IdTypeCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.IdTypeCd)) application.IdTypeCd = null;

        application.IdTypeValue = (application.IdTypeValue ?? "").Trim();
        if (string.IsNullOrEmpty(application.IdTypeValue)) application.IdTypeValue = null;

        application.IdIssueDate = application.IdIssueDate.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue ? null : application.IdIssueDate.Value;

        application.GenderCd = (application.GenderCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.GenderCd)) application.GenderCd = null;

        application.RaceCd = (application.RaceCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.RaceCd)) application.RaceCd = null;

        application.EthnicityCd = (application.EthnicityCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.EthnicityCd)) application.EthnicityCd = null;

        application.Pronouns = (application.Pronouns ?? "").Trim();
        if (string.IsNullOrEmpty(application.Pronouns)) application.Pronouns = null;

        application.CountyLivingIn = (application.CountyLivingIn ?? "").Trim();
        if (string.IsNullOrEmpty(application.CountyLivingIn)) application.CountyLivingIn = null;

        application.CountyWorkingIn = (application.CountyWorkingIn ?? "").Trim();
        if (string.IsNullOrEmpty(application.CountyWorkingIn)) application.CountyWorkingIn = null;

        application.PhoneNumberTypeCd = (application.PhoneNumberTypeCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhoneNumberTypeCd)) application.PhoneNumberTypeCd = null;

        application.PhoneNumber = (application.PhoneNumber ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhoneNumber)) application.PhoneNumber = null;

        application.PhoneNumberExtn = (application.PhoneNumberExtn ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhoneNumberExtn)) application.PhoneNumberExtn = null;

        application.AltPhoneNumberTypeCd = (application.AltPhoneNumberTypeCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.AltPhoneNumberTypeCd)) application.AltPhoneNumberTypeCd = null;

        application.AltPhoneNumber = (application.AltPhoneNumber ?? "").Trim();
        if (string.IsNullOrEmpty(application.AltPhoneNumber)) application.AltPhoneNumber = null;

        application.AltPhoneNumberExtn = (application.AltPhoneNumberExtn ?? "").Trim();
        if (string.IsNullOrEmpty(application.AltPhoneNumberExtn)) application.AltPhoneNumberExtn = null;

        application.RealEstateValueAmt = (application.RealEstateValueAmt < 0) ? 0 : application.RealEstateValueAmt;

        application.AssetValueAmt = (application.AssetValueAmt < 0) ? 0 : application.AssetValueAmt;

        application.IncomeValueAmt = (application.IncomeValueAmt < 0) ? 0 : application.IncomeValueAmt;

        application.PhysicalStreetLine1 = (application.PhysicalStreetLine1 ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalStreetLine1)) application.PhysicalStreetLine1 = null;

        application.PhysicalStreetLine2 = (application.PhysicalStreetLine2 ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalStreetLine2)) application.PhysicalStreetLine2 = null;

        application.PhysicalStreetLine3 = (application.PhysicalStreetLine3 ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalStreetLine3)) application.PhysicalStreetLine3 = null;

        application.PhysicalCity = (application.PhysicalCity ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalCity)) application.PhysicalCity = null;

        application.PhysicalStateCd = (application.PhysicalStateCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalStateCd)) application.PhysicalStateCd = null;

        application.PhysicalZipCode = (application.PhysicalZipCode ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalZipCode)) application.PhysicalZipCode = null;

        application.PhysicalCounty = (application.PhysicalCounty ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalCounty)) application.PhysicalCounty = null;

        application.MailingStreetLine1 = (application.MailingStreetLine1 ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingStreetLine1)) application.MailingStreetLine1 = null;

        application.MailingStreetLine2 = (application.MailingStreetLine2 ?? "").Trim();
        if (string.IsNullOrEmpty(application.PhysicalStreetLine2)) application.PhysicalStreetLine2 = null;

        application.MailingStreetLine3 = (application.MailingStreetLine3 ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingStreetLine3)) application.MailingStreetLine3 = null;

        application.MailingCity = (application.PhysicalCity ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingCity)) application.MailingCity = null;

        application.MailingStateCd = (application.PhysicalStateCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingStateCd)) application.MailingStateCd = null;

        application.MailingZipCode = (application.PhysicalZipCode ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingZipCode)) application.MailingZipCode = null;

        application.MailingCounty = (application.PhysicalCounty ?? "").Trim();
        if (string.IsNullOrEmpty(application.MailingCounty)) application.MailingCounty = null;

        application.VoucherCds = (application.VoucherCds ?? "").Trim();
        if (string.IsNullOrEmpty(application.VoucherCds)) application.VoucherCds = null;

        application.VoucherOther = (application.VoucherOther ?? "").Trim();
        if (string.IsNullOrEmpty(application.VoucherOther)) application.VoucherOther = null;

        application.VoucherAdminName = (application.VoucherAdminName ?? "").Trim();
        if (string.IsNullOrEmpty(application.VoucherAdminName)) application.VoucherAdminName = null;

        // application.CoRelationTypeCd = (application.CoRelationTypeCd ?? "").Trim();
        // application.CoRelationTypeOther = (application.CoRelationTypeOther ?? "").Trim();
        // application.CoTitle = (application.CoTitle ?? "").Trim();
        // application.CoFirstName = (application.CoFirstName ?? "").Trim();
        // application.CoMiddleName = (application.CoMiddleName ?? "").Trim();
        // application.CoLastName = (application.CoLastName ?? "").Trim();
        // application.CoSuffix = (application.CoSuffix ?? "").Trim();
        // application.CoLast4SSN = (application.CoLast4SSN ?? "").Trim();
        // application.CoDateOfBirth = (application.CoDateOfBirth ?? "").Trim();
        // application.CoIdTypeCd = (application.CoIdTypeCd ?? "").Trim();
        // application.CoIdTypeValue = (application.CoIdTypeValue ?? "").Trim();
        // application.CoIdIssueDate = (application.CoIdIssueDate ?? "").Trim();
        // application.CoGenderCd = (application.CoGenderCd ?? "").Trim();
        // application.CoRaceCd = (application.CoRaceCd ?? "").Trim();
        // application.CoEthnicityCd = (application.CoEthnicityCd ?? "").Trim();
        // application.CoPronouns = (application.CoPronouns ?? "").Trim();
        // application.CoCountyLivingIn = (application.CoCountyLivingIn ?? "").Trim();
        // application.CoCountyWorkingIn = (application.CoCountyWorkingIn ?? "").Trim();
        // application.CoPhoneNumberTypeCd = (application.CoPhoneNumberTypeCd ?? "").Trim();
        // application.CoPhoneNumber = (application.CoPhoneNumber ?? "").Trim();
        // application.CoPhoneNumberExtn = (application.CoPhoneNumberExtn ?? "").Trim();
        // application.CoAltPhoneNumberTypeCd = (application.CoAltPhoneNumberTypeCd ?? "").Trim();
        // application.CoAltPhoneNumber = (application.CoAltPhoneNumber ?? "").Trim();
        // application.CoAltPhoneNumberExtn = (application.CoAltPhoneNumberExtn ?? "").Trim();
        // application.CoRealEstateValueAmt = (application.CoRealEstateValueAmt < 0) ? 0 : application.CoRealEstateValueAmt;
        // application.CoAssetValueAmt = (application.CoAssetValueAmt < 0) ? 0 : application.CoAssetValueAmt;
        // application.CoIncomeValueAmt = (application.CoIncomeValueAmt < 0) ? 0 : application.CoIncomeValueAmt;

        application.LeadTypeCd = (application.LeadTypeCd ?? "").Trim();
        if (string.IsNullOrEmpty(application.LeadTypeCd)) application.LeadTypeCd = null;

        application.LeadTypeOther = (application.LeadTypeOther ?? "").Trim();
        if (string.IsNullOrEmpty(application.LeadTypeOther)) application.LeadTypeOther = null;
    }

    public async Task<string> ValidateApplication(string requestId, string correlationId, string username, HousingApplication application)
    {
        if (application == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        Sanitize(requestId, correlationId, username, application);

        var listing = await _listingsService.GetPublishedListing(requestId, correlationId, username, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Listing is required");
            return "H101";
        }

        return "";
    }

    public async Task<string> ValidateApplicantInfo(string requestId, string correlationId, HousingApplication application)
    {
        if (application == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        if (string.IsNullOrEmpty(application.FirstName))
        {
            _logger.LogError($"Unable to validate primary applicant information - First Name is required");
            return "H102";
        }

        if (string.IsNullOrEmpty(application.LastName))
        {
            _logger.LogError($"Unable to validate primary applicant information - Last Name is required");
            return "H103";
        }

        if (application.DateOfBirth.GetValueOrDefault(DateTime.MinValue) < new DateTime(1900, 1, 1))
        {
            _logger.LogError($"Unable to validate primary applicant information - Date of Birth is required, must be on or after 01/01/1900");
            return "H104";
        }

        if (string.IsNullOrEmpty(application.Last4SSN) || application.Last4SSN.Length != 4 || !int.TryParse(application.Last4SSN, out int last4SSN))
        {
            _logger.LogError($"Unable to validate primary applicant information - Last 4 of SSN/ITIN is required");
            return "H105";
        }

        if (!string.IsNullOrEmpty(application.IdTypeCd))
        {
            var idTypes = await _metadataService.GetIdTypes();
            if (!idTypes.ContainsKey(application.IdTypeCd))
            {
                _logger.LogError($"Unable to validate primary applicant information - ID Type is invalid");
                return "H106";
            }

            if (string.IsNullOrEmpty(application.IdTypeValue))
            {
                _logger.LogError($"Unable to validate primary applicant information - ID Value is invalid");
                return "H107";
            }

            if (application.IdIssueDate.GetValueOrDefault(DateTime.MinValue).Date > DateTime.MinValue.Date
                    && application.IdIssueDate.GetValueOrDefault(DateTime.MinValue).Date < application.DateOfBirth.Value.Date)
            {
                _logger.LogError($"Unable to validate primary applicant information - ID Issue Date must be on or after 01/01/1900");
                return "H108";
            }
        }

        var genderTypes = await _metadataService.GetGenderTypes();
        if (!genderTypes.ContainsKey(application.GenderCd))
        {
            _logger.LogError($"Unable to validate primary applicant information - Gender Type is invalid");
            return "H109";
        }

        var raceTypes = await _metadataService.GetRaceTypes();
        if (!raceTypes.ContainsKey(application.RaceCd))
        {
            _logger.LogError($"Unable to validate primary applicant information - Race Type is invalid");
            return "H110";
        }

        var ethnicityTypes = await _metadataService.GetEthnicityTypes();
        if (!ethnicityTypes.ContainsKey(application.EthnicityCd))
        {
            _logger.LogError($"Unable to validate primary applicant information - Ethnicity Type is invalid");
            return "H111";
        }

        if (string.IsNullOrEmpty(application.EmailAddress) && string.IsNullOrEmpty(application.PhoneNumber))
        {
            _logger.LogError($"Unable to validate primary applicant information - Either email address or phone number is required");
            return "H112";
        }

        if (!string.IsNullOrEmpty(application.EmailAddress) && !_emailService.IsValidEmailAddress(requestId, correlationId, application.EmailAddress))
        {
            _logger.LogError($"Unable to validate primary applicant information - Invalid Email Address");
            return "H113";
        }

        if (!string.IsNullOrEmpty(application.AltEmailAddress) && !_emailService.IsValidEmailAddress(requestId, correlationId, application.AltEmailAddress))
        {
            _logger.LogError($"Unable to validate primary applicant information - Invalid Alternate Email Address");
            return "H114";
        }

        var phoneNumberTypes = await _metadataService.GetPhoneNumberTypes();
        if (!string.IsNullOrEmpty(application.PhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(requestId, correlationId, application.PhoneNumber))
            {
                _logger.LogError($"Unable to validate primary applicant information - Invalid Phone Number");
                return "H115";
            }

            if (!phoneNumberTypes.ContainsKey(application.PhoneNumberTypeCd))
            {
                _logger.LogError($"Unable to validate primary applicant information - Phone Number Type is invalid");
                return "H116";
            }
        }
        if (!string.IsNullOrEmpty(application.AltPhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(requestId, correlationId, application.AltPhoneNumber))
            {
                _logger.LogError($"Unable to validate primary applicant information - Invalid Alternate Phone Number");
                return "H117";
            }

            if (!phoneNumberTypes.ContainsKey(application.AltPhoneNumberTypeCd))
            {
                _logger.LogError($"Unable to validate primary applicant information - Alternate Phone Number Type is invalid");
                return "H118";
            }
        }

        return "";
    }

    public async Task<string> ValidateHouseholdInfo(string requestId, string correlationId, HousingApplication application)
    {
        var existingApplication = await _applicationRepository.GetOne(new HousingApplication() { ApplicationId = application.ApplicationId });
        if (existingApplication == null)
        {
            _logger.LogError($"Unable to find housing application - {application.ApplicationId}");
            return "H001";
        }

        if (application.AddressInd)
        {
            if (string.IsNullOrEmpty(application.PhysicalStreetLine1))
            {
                _logger.LogError($"Unable to validate household information - Address Line 1 is required");
                return "H211";
            }
            if (string.IsNullOrEmpty(application.PhysicalCity))
            {
                _logger.LogError($"Unable to validate household information - City is required");
                return "H212";
            }
            if (string.IsNullOrEmpty(application.PhysicalStateCd))
            {
                _logger.LogError($"Unable to validate household information - State is required");
                return "H213";
            }
            if (string.IsNullOrEmpty(application.PhysicalZipCode))
            {
                _logger.LogError($"Unable to validate household information - Zip Code is required");
                return "H214";
            }
            if (string.IsNullOrEmpty(application.PhysicalCounty))
            {
                _logger.LogError($"Unable to validate household information - County is required");
                return "H215";
            }

            if (application.DifferentMailingAddressInd)
            {
                if (string.IsNullOrEmpty(application.MailingStreetLine1))
                {
                    _logger.LogError($"Unable to validate household information - Mailing Address Line 1 is required");
                    return "H216";
                }
                if (string.IsNullOrEmpty(application.MailingCity))
                {
                    _logger.LogError($"Unable to validate household information - Mailing City is required");
                    return "H217";
                }
                if (string.IsNullOrEmpty(application.MailingStateCd))
                {
                    _logger.LogError($"Unable to validate household information - Mailing State is required");
                    return "H218";
                }
                if (string.IsNullOrEmpty(application.MailingZipCode))
                {
                    _logger.LogError($"Unable to validate household information - Mailing Zip Code is required");
                    return "H219";
                }
                if (string.IsNullOrEmpty(application.PhysicalCounty))
                {
                    _logger.LogError($"Unable to validate household information - Mailing County is required");
                    return "H220";
                }
            }
        }

        if (application.VoucherInd)
        {
            if (string.IsNullOrEmpty(application.VoucherCds))
            {
                _logger.LogError($"Unable to validate household information - One or more voucher types is required");
                return "H221";
            }

            var voucherTypes = await _metadataService.GetVoucherTypes();
            var voucherTypeCds = application.VoucherCds.Split(",", StringSplitOptions.RemoveEmptyEntries);
            var errorCode = "";
            foreach (var voucherTypeCd in voucherTypeCds)
            {
                if (!voucherTypes.ContainsKey(voucherTypeCd))
                {
                    _logger.LogError($"Unable to validate household information - One or more voucher types is invalid");
                    errorCode = "H222";
                    break;
                }
                else if (voucherTypeCd == "OTHER")
                {
                    if (string.IsNullOrEmpty(application.VoucherOther))
                    {
                        _logger.LogError($"Unable to validate household information - Other voucher type is required");
                        errorCode = "H223";
                        break;
                    }
                }
            }
            if (!string.IsNullOrEmpty(errorCode)) return errorCode;

            if (string.IsNullOrEmpty(application.VoucherAdminName))
            {
                _logger.LogError($"Unable to validate household information - Voucher administrator name is required");
                return "H224";
            }
        }

        return "";
    }

    public async Task<string> ValidateHouseholdMemberInfo(string requestId, string correlationId, HouseholdMember member)
    {
        if (member == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate application - Invalid Input");
            return "H000";
        }

        if (string.IsNullOrEmpty(member.RelationTypeCd))
        {
            _logger.LogError($"Unable to validate co-applicant information - Relation Type is required");
            return "H311";
        }

        if (string.IsNullOrEmpty(member.FirstName))
        {
            _logger.LogError($"Unable to validate co-applicant information - First Name is required");
            return "H312";
        }

        if (string.IsNullOrEmpty(member.LastName))
        {
            _logger.LogError($"Unable to validate co-applicant information - Last Name is required");
            return "H313";
        }

        if (member.DateOfBirth.GetValueOrDefault(DateTime.MinValue) < new DateTime(1900, 1, 1))
        {
            _logger.LogError($"Unable to validate co-applicant information - Date of Birth is required, must be on or after 01/01/1900");
            return "H314";
        }

        if (string.IsNullOrEmpty(member.Last4SSN) || member.Last4SSN.Length != 4 || !int.TryParse(member.Last4SSN, out int last4SSN))
        {
            _logger.LogError($"Unable to validate co-applicant information - Last 4 of SSN/ITIN is required");
            return "H315";
        }

        if (!string.IsNullOrEmpty(member.IdTypeCd))
        {
            var idTypes = await _metadataService.GetIdTypes();
            if (!idTypes.ContainsKey(member.IdTypeCd))
            {
                _logger.LogError($"Unable to validate co-applicant information - ID Type is invalid");
                return "H316";
            }

            if (string.IsNullOrEmpty(member.IdTypeValue))
            {
                _logger.LogError($"Unable to validate co-applicant information - ID Value is invalid");
                return "H317";
            }

            if (member.IdIssueDate.GetValueOrDefault(DateTime.MinValue).Date > DateTime.MinValue.Date
                    && member.IdIssueDate.GetValueOrDefault(DateTime.MinValue).Date < member.DateOfBirth.Value.Date)
            {
                _logger.LogError($"Unable to validate co-applicant information - ID Issue Date must be on or after 01/01/1900");
                return "H318";
            }
        }

        var genderTypes = await _metadataService.GetGenderTypes();
        if (!genderTypes.ContainsKey(member.GenderCd))
        {
            _logger.LogError($"Unable to validate co-applicant information - Gender Type is invalid");
            return "H319";
        }

        var raceTypes = await _metadataService.GetRaceTypes();
        if (!raceTypes.ContainsKey(member.RaceCd))
        {
            _logger.LogError($"Unable to validate co-applicant information - Race Type is invalid");
            return "H320";
        }

        var ethnicityTypes = await _metadataService.GetEthnicityTypes();
        if (!ethnicityTypes.ContainsKey(member.EthnicityCd))
        {
            _logger.LogError($"Unable to validate co-applicant information - Ethnicity Type is invalid");
            return "H321";
        }

        if (string.IsNullOrEmpty(member.EmailAddress) && string.IsNullOrEmpty(member.PhoneNumber))
        {
            _logger.LogError($"Unable to validate co-applicant information - Either email address or phone number is required");
            return "H322";
        }

        if (!string.IsNullOrEmpty(member.EmailAddress) && !_emailService.IsValidEmailAddress(requestId, correlationId, member.EmailAddress))
        {
            _logger.LogError($"Unable to validate co-applicant information - Invalid Email Address");
            return "H323";
        }

        if (!string.IsNullOrEmpty(member.AltEmailAddress) && !_emailService.IsValidEmailAddress(requestId, correlationId, member.AltEmailAddress))
        {
            _logger.LogError($"Unable to validate co-applicant information - Invalid Alternate Email Address");
            return "H324";
        }

        var phoneNumberTypes = await _metadataService.GetPhoneNumberTypes();
        if (!string.IsNullOrEmpty(member.PhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(requestId, correlationId, member.PhoneNumber))
            {
                _logger.LogError($"Unable to validate co-applicant information - Invalid Phone Number");
                return "H325";
            }

            if (!phoneNumberTypes.ContainsKey(member.PhoneNumberTypeCd))
            {
                _logger.LogError($"Unable to validate co-applicant information - Phone Number Type is invalid");
                return "H326";
            }
        }
        if (!string.IsNullOrEmpty(member.AltPhoneNumber))
        {
            if (!_phoneService.IsValidPhoneNumber(requestId, correlationId, member.AltPhoneNumber))
            {
                _logger.LogError($"Unable to validate co-applicant information - Invalid Alternate Phone Number");
                return "H327";
            }

            if (!phoneNumberTypes.ContainsKey(member.AltPhoneNumberTypeCd))
            {
                _logger.LogError($"Unable to validate co-applicant information - Alternate Phone Number Type is invalid");
                return "H328";
            }
        }

        return "";
    }

    public async Task<DuplicatesViewModel> GetPotentialDuplicates(string requestId, string correlationId, string username, long listingId)
    {
        var model = new DuplicatesViewModel()
        {
            Listings = await GetListings(requestId, correlationId, username),
            ListingId = listingId
        };
        if (listingId > 0)
        {
            var duplicates = await _applicationRepository.GetPotentialDuplicates(listingId);
            model.DuplicatesBySsn = duplicates.GetValueOrDefault("SSN")?.Select(s => s.ToViewModel()) ?? [];
            model.DuplicatesByName = duplicates.GetValueOrDefault("NAME")?.Select(s => s.ToViewModel()) ?? [];
            model.DuplicatesByEmailAddress = duplicates.GetValueOrDefault("EMAIL")?.Select(s => s.ToViewModel()) ?? [];
            model.DuplicatesByPhoneNumber = duplicates.GetValueOrDefault("PHONE")?.Select(s => s.ToViewModel()) ?? [];
            model.DuplicatesByStreetAddress = duplicates.GetValueOrDefault("ADDRESS")?.Select(s => s.ToViewModel()) ?? [];
        }
        else
        {
            model.DuplicatesBySsn = [];
            model.DuplicatesByName = [];
            model.DuplicatesByEmailAddress = [];
            model.DuplicatesByPhoneNumber = [];
            model.DuplicatesByStreetAddress = [];
        }
        return model;
    }

    public async Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByDoBLast4Ssn(string requestId, string correlationId, string username, long listingId, string dateOfBirth, string last4Ssn)
    {
        if (listingId <= 0) return null;
        var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
        if (listing == null) return null;

        dateOfBirth = (dateOfBirth ?? "").Trim();
        if (dateOfBirth.Length == 0) return null;
        if (!DateTime.TryParse(dateOfBirth, out var dob)) return null;

        last4Ssn = (last4Ssn ?? "").Trim();
        if (last4Ssn.Length != 4) return null;

        var duplicates = await _applicationRepository.GetPotentialDuplicatesByDateOfBirthLast4Ssn(listingId, dateOfBirth, last4Ssn);
        var applications = ((duplicates?.Count() ?? 0) > 0) ? duplicates.Select(s => s.ToViewModel()).ToList() : [];
        if (applications.Count > 0)
        {
            foreach (var application in applications)
            {
                application.ApplicationMembers = [];
                var members = await _applicationRepository.GetApplicants(application.ApplicationId);
                if ((members?.Count() ?? 0) == 0)
                {
                    continue;
                }

                var applicants = members.Select(s => s.ToViewModel()).ToList();
                application.ApplicationMembers = applicants;
            }
        }

        return new DuplicateApplicationsViewModel()
        {
            Applications = applications,
            ListingDetails = listing,
            ListingId = listingId,
            DateOfBirth = dateOfBirth,
            Last4SSN = last4Ssn,
            TypeCd = "DOBSSN",
            TypeDescription = "Date of Birth and Last 4 SSN/ITIN",
            TypeValue = $"{dateOfBirth}, {last4Ssn}"
        };
    }

    public async Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByName(string requestId, string correlationId, string username, long listingId, string name)
    {
        if (listingId <= 0) return null;
        var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
        if (listing == null) return null;

        name = (name ?? "").Trim();
        if (name.Length == 0) return null;

        var duplicates = await _applicationRepository.GetPotentialDuplicatesByName(listingId, name);
        var applications = ((duplicates?.Count() ?? 0) > 0) ? duplicates.Select(s => s.ToViewModel()).ToList() : [];
        if (applications.Count > 0)
        {
            foreach (var application in applications)
            {
                application.ApplicationMembers = [];
                var members = await _applicationRepository.GetApplicants(application.ApplicationId);
                if ((members?.Count() ?? 0) == 0)
                {
                    continue;
                }

                var applicants = members.Select(s => s.ToViewModel()).ToList();
                application.ApplicationMembers = applicants;
            }
        }

        return new DuplicateApplicationsViewModel()
        {
            Applications = applications,
            ListingDetails = listing,
            ListingId = listingId,
            Name = name,
            TypeCd = "NAME",
            TypeDescription = "Name",
            TypeValue = name
        };
    }

    public async Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByEmailAddress(string requestId, string correlationId, string username, long listingId, string emailAddress)
    {
        if (listingId <= 0) return null;
        var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
        if (listing == null) return null;

        emailAddress = (emailAddress ?? "").Trim();
        if (emailAddress.Length == 0) return null;

        var duplicates = await _applicationRepository.GetPotentialDuplicatesByEmailAddress(listingId, emailAddress);
        var applications = ((duplicates?.Count() ?? 0) > 0) ? duplicates.Select(s => s.ToViewModel()).ToList() : [];
        if (applications.Count > 0)
        {
            foreach (var application in applications)
            {
                application.ApplicationMembers = [];
                var members = await _applicationRepository.GetApplicants(application.ApplicationId);
                if ((members?.Count() ?? 0) == 0)
                {
                    continue;
                }

                var applicants = members.Select(s => s.ToViewModel()).ToList();
                application.ApplicationMembers = applicants;
            }
        }

        return new DuplicateApplicationsViewModel()
        {
            Applications = applications,
            ListingDetails = listing,
            ListingId = listingId,
            EmailAddress = emailAddress,
            TypeCd = "EMAIL",
            TypeDescription = "Email Address",
            TypeValue = emailAddress
        };
    }

    public async Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByPhoneNumber(string requestId, string correlationId, string username, long listingId, string phoneNumber)
    {
        if (listingId <= 0) return null;
        var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
        if (listing == null) return null;

        phoneNumber = (phoneNumber ?? "").Trim();
        if (phoneNumber.Length == 0) return null;

        var duplicates = await _applicationRepository.GetPotentialDuplicatesByPhoneNumber(listingId, phoneNumber);
        var applications = ((duplicates?.Count() ?? 0) > 0) ? duplicates.Select(s => s.ToViewModel()).ToList() : [];
        if (applications.Count > 0)
        {
            foreach (var application in applications)
            {
                application.ApplicationMembers = [];
                var members = await _applicationRepository.GetApplicants(application.ApplicationId);
                if ((members?.Count() ?? 0) == 0)
                {
                    continue;
                }

                var applicants = members.Select(s => s.ToViewModel()).ToList();
                application.ApplicationMembers = applicants;
            }
        }

        return new DuplicateApplicationsViewModel()
        {
            Applications = applications,
            ListingDetails = listing,
            ListingId = listingId,
            PhoneNumber = phoneNumber,
            TypeCd = "PHONE",
            TypeDescription = "Phone Number",
            TypeValue = phoneNumber
        };
    }

    public async Task<DuplicateApplicationsViewModel> GetPotentialDuplicatesByStreetAddress(string requestId, string correlationId, string username, long listingId, string streetAddress)
    {
        if (listingId <= 0) return null;
        var listing = await _listingsService.GetOne(requestId, correlationId, username, listingId);
        if (listing == null) return null;

        streetAddress = (streetAddress ?? "").Trim();
        if (streetAddress.Length == 0) return null;

        var duplicates = await _applicationRepository.GetPotentialDuplicatesByStreetAddress(listingId, streetAddress);
        var applications = ((duplicates?.Count() ?? 0) > 0) ? duplicates.Select(s => s.ToViewModel()).ToList() : [];
        if (applications.Count > 0)
        {
            foreach (var application in applications)
            {
                application.ApplicationMembers = [];
                var members = await _applicationRepository.GetApplicants(application.ApplicationId);
                if ((members?.Count() ?? 0) == 0)
                {
                    continue;
                }

                var applicants = members.Select(s => s.ToViewModel()).ToList();
                application.ApplicationMembers = applicants;
            }
        }

        return new DuplicateApplicationsViewModel()
        {
            Applications = applications,
            ListingDetails = listing,
            ListingId = listingId,
            StreetAddress = streetAddress,
            TypeCd = "ADDRESS",
            TypeDescription = "Street Address",
            TypeValue = streetAddress
        };
    }

    public async Task<string> UpdateDuplicateStatus(string requestId, string correlationId, string username, string siteUrl, EditableDuplicateApplicationViewModel item)
    {
        if (item == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to mark application duplicate - Invalid Input");
            return "H000";
        }

        item.DuplicateCheckCd = (item.DuplicateCheckCd ?? "").Trim().ToUpper();
        if (!"|P|D|N|".Contains($"|{item.DuplicateCheckCd}|"))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Duplicate type is invalid");
            return "H511";
        }

        item.DuplicateReason = (item.DuplicateReason ?? "").Trim();
        if (item.DuplicateReason.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Reason is required");
            return "H512";
        }

        DateTime? dueDate = null;
        if ("|P|".Contains($"|{item.DuplicateCheckCd}|"))
        {
            if (DateTime.TryParse(item.DuplicateCheckResponseDueDate, out var date))
            {
                if (date != DateTime.MinValue)
                {
                    if (date.Date.Ticks < DateTime.Now.AddDays(3).Date.Ticks)
                    {
                        _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Response due date must at least be 3 days from today");
                        return "H513";
                    }
                    dueDate = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
                    dueDate.Value.AddHours(23);
                    dueDate.Value.AddHours(23);
                }
            }
        }
        else
        {
            dueDate = null;
        }

        var application = await _applicationRepository.GetOne(new HousingApplication() { ApplicationId = item.ApplicationId });
        if (application == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find housing application - {item.ApplicationId}");
            return "H001";
        }

        var listing = await _listingsService.GetOne(requestId, correlationId, username, application.ListingId);
        if (listing == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find listing - {application.ListingId}");
            return "L001";
        }

        application.DuplicateCheckCd = item.DuplicateCheckCd;
        application.DuplicateCheckResponseDueDate = dueDate;
        application.DuplicateReason = item.DuplicateReason;
        application.ModifiedBy = username;

        var updated = await _applicationRepository.UpdateDuplicateStatus(correlationId, application);
        if (!updated)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to update duplicate status - {application.ListingId}");
            return "H504";
        }

        if (item.DuplicateCheckCd == "P")
        {
            if ((application.SubmissionTypeCd ?? "").Trim().Equals("PAPER", StringComparison.CurrentCultureIgnoreCase))
            {
                var emailSent = await _emailService.SendPotentialDuplicatePaperApplicationEmail(requestId, correlationId, siteUrl, username, listing, application);
            }
            else
            {
                var emailSent = await _emailService.SendPotentialDuplicateOnlineApplicationEmail(requestId, correlationId, _publicSiteUrl, username, listing, application);
            }
        }
        else if (item.DuplicateCheckCd == "D")
        {
            if ((application.SubmissionTypeCd ?? "").Trim().Equals("PAPER", StringComparison.CurrentCultureIgnoreCase))
            {
                var emailSent = await _emailService.SendDuplicatePaperApplicationEmail(requestId, correlationId, siteUrl, username, listing, application);
            }
            else
            {
                var emailSent = await _emailService.SendDuplicateOnlineApplicationEmail(requestId, correlationId, _publicSiteUrl, username, listing, application);
            }
        }

        return "";
    }

    public async Task<ApplicationCommentsViewModel> GetComments(string requestId, string correlationId, string username, long applicationId)
    {
        var comments = await _applicationRepository.GetComments(requestId, correlationId, username, applicationId) ?? [];
        return new ApplicationCommentsViewModel()
        {
            ApplicationId = applicationId,
            Comments = comments?.Select(s => s.ToViewModel()) ?? []
        };
    }
    public EditableApplicationCommentViewModel GetOneForAddComment(string requestId, string correlationId, string username, long applicationId)
    {
        return new EditableApplicationCommentViewModel()
        {
            ApplicationId = applicationId,
            CommentId = 0,
            Comments = "",
            InternalOnlyInd = false
        };
    }

    public async Task<string> AddComment(string requestId, string correlationId, string username, EditableApplicationCommentViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add comment - Invalid Input");
            return "H000";
        }

        if (model.ApplicationId == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add comment - Application ID is required");
            return "H611";
        }

        model.Comments = (model.Comments ?? "").Trim();
        if (model.Comments.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add comment - Comment is required");
            return "H612";
        }

        var comment = new ApplicationComment()
        {
            ApplicationId = model.ApplicationId,
            CommentId = 0,
            Comments = model.Comments,
            InternalOnlyInd = model.InternalOnlyInd,
            Active = true,
            CreatedBy = username
        };

        var added = await _applicationRepository.AddComment(requestId, correlationId, comment);
        if (!added)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to add comment - {comment.Comments} - Unknown error");
            return "H603";
        }

        return "";
    }
}