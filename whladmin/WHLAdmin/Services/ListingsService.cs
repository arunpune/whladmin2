using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IListingsService
{
    string GetMessage(string code);
    Task<ListingsViewModel> GetData(string requestId, string correlationId, string username, int pageNo = 1, int pageSize = 100);
    Task<IEnumerable<ListingViewModel>> GetAll();
    Task<ListingViewModel> GetOne(string requestId, string correlationId, string username, long listingId, bool withDetails = false);
    Task<string> Add(string correlationId, string username, ListingViewModel listing);
    Task<string> Update(string correlationId, string username, ListingViewModel listing);
    Task<string> Delete(string correlationId, string username, long id);

    Task<string> UpdateAddress(string correlationId, string username, ListingViewModel listing);
    Task<string> UpdateDates(string correlationId, string username, ListingViewModel listing);
    Task<string> UpdatePetsAllowed(string correlationId, string username, ListingViewModel listing);
    Task<string> UpdateMarketingAgent(string requestId, string correlationId, string username, ListingViewModel listing);

    Task<IEnumerable<ListingImageViewModel>> GetImages(long listingId);
    Task<ListingImageViewModel> GetImage(int imageId);
    Task<string> AddImage(string correlationId, string username, ListingImageViewModel image);
    Task<string> UpdateImage(string correlationId, string username, ListingImageViewModel image);
    Task<string> DeleteImage(string correlationId, string username, int id);

    Task<IEnumerable<ListingDocumentViewModel>> GetDocuments(long listingId);
    Task<ListingDocumentViewModel> GetDocument(int documentId);
    Task<string> AddDocument(string correlationId, string username, ListingDocumentViewModel document);
    Task<string> UpdateDocument(string correlationId, string username, ListingDocumentViewModel document);
    Task<string> DeleteDocument(string correlationId, string username, int id);

    Task<IEnumerable<ListingUnitViewModel>> GetUnits(long listingId);
    Task<ListingUnitViewModel> GetUnit(int unitId);
    Task<string> AddUnit(string correlationId, string username, ListingUnitViewModel unit);
    Task<string> UpdateUnit(string correlationId, string username, ListingUnitViewModel unit);
    Task<string> DeleteUnit(string correlationId, string username, int id);

    Task<IEnumerable<ListingUnitHouseholdViewModel>> GetHouseholds(long listingId);

    Task<IEnumerable<AmenityViewModel>> GetAmenities(long listingId);
    Task<IEnumerable<AmenityViewModel>> GetAmenitiesForEdit(long listingId);
    Task<string> SaveAmenities(string correlationId, string username, long listingId, string amenityIds);

    Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypes(long listingId);
    Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypesForEdit(long listingId);
    Task<string> SaveDocumentTypes(string correlationId, string username, long listingId, string documentTypeIds);

    Task<IEnumerable<FundingSourceViewModel>> GetFundingSources(long listingId);
    Task<IEnumerable<FundingSourceViewModel>> GetFundingSourcesForEdit(long listingId);
    Task<string> SaveFundingSources(string correlationId, string username, long listingId, string fundingSourceIds);

    Task<Dictionary<string, string>> GetListingAccessibilities(long listingId);
    Task<string> SaveAccessibilities(string correlationId, string username, long listingId, string accessibilityCds);

    Task<IEnumerable<Declaration>> GetListingDeclarations(long listingId);
    Task<Declaration> GetListingDeclaration(int id);
    Task<string> AddDeclaration(string correlationId, string username, ListingDeclarationViewModel declaration);
    Task<string> UpdateDeclaration(string correlationId, string username, ListingDeclarationViewModel declaration);
    Task<string> DeleteDeclaration(string correlationId, string username, int id);

    Task<IEnumerable<Disclosure>> GetListingDisclosures(long listingId);
    Task<Disclosure> GetListingDisclosure(int id);
    Task<string> AddDisclosure(string correlationId, string username, ListingDisclosureViewModel disclosure);
    Task<string> UpdateDisclosure(string correlationId, string username, ListingDisclosureViewModel disclosure);
    Task<string> DeleteDisclosure(string correlationId, string username, int id);

    Task<string> Submit(string requestId, string correlationId, string siteUrl, string username, long listingId);
    Task<string> Publish(string requestId, string correlationId, string siteUrl, string username, long listingId);
    Task<string> Revise(string requestId, string correlationId, string siteUrl, string username, EditableListingActionViewModel model);
    Task<string> Unpublish(string requestId, string correlationId, string siteUrl, string username, EditableListingActionViewModel model);

    Task<PrintableFormViewModel> GetPrintableForm(long listingId);

    Task<IEnumerable<ListingViewModel>> GetPublishedListings(string requestId, string correlationId, string username);
    Task<ListingViewModel> GetPublishedListing(string requestId, string correlationId, string username, long listingId, bool withDetails = false);

    Task<AffordabilityAnalysisViewModel> GetForAffordabilityAnalysis(string requestId, string correlationId, long listingId);
    Task<ListingUnitViewModel> GetAffordabilityAnalysis(string requestId, string correlationId, int unitId, decimal rate, int term);
}

public class ListingsService : IListingsService
{
    private readonly ILogger<ListingsService> _logger;
    private readonly IListingRepository _listingRepository;
    private readonly IListingImageRepository _listingImageRepository;
    private readonly IListingUnitRepository _listingUnitRepository;
    private readonly IListingUnitHouseholdRepository _listingUnitHouseholdRepository;
    private readonly IListingAmenityRepository _listingAmenityRepository;
    private readonly IListingAccessibilityRepository _listingAccessibilityRepository;
    private readonly IListingDeclarationRepository _listingDeclarationRepository;
    private readonly IListingDisclosureRepository _listingDisclosureRepository;
    private readonly IListingDocumentRepository _listingDocumentRepository;
    private readonly IListingDocumentTypeRepository _listingDocumentTypeRepository;
    private readonly IListingFundingSourceRepository _listingFundingSourceRepository;
    private readonly IAmortizationsService _amortizationsService;
    private readonly IEmailService _emailService;
    private readonly IMarketingAgentsService _marketingAgentsService;
    private readonly IMasterConfigService _masterConfigService;
    private readonly IMetadataService _metadataService;
    private readonly IUiHelperService _uiHelperService;
    private readonly IUsersService _usersService;
    private readonly Dictionary<string, string> _messages;

    public ListingsService(ILogger<ListingsService> logger,
                            IListingRepository listingRepository,
                            IListingImageRepository listingImageRepository,
                            IListingUnitRepository listingUnitRepository,
                            IListingUnitHouseholdRepository listingUnitHouseholdRepository,
                            IListingAmenityRepository listingAmenityRepository,
                            IListingAccessibilityRepository listingAccessibilityRepository,
                            IListingDeclarationRepository listingDeclarationRepository,
                            IListingDisclosureRepository listingDisclosureRepository,
                            IListingDocumentRepository listingDocumentRepository,
                            IListingDocumentTypeRepository listingDocumentTypeRepository,
                            IListingFundingSourceRepository listingFundingSourceRepository,
                            IAmortizationsService amortizationsService,
                            IEmailService emailService,
                            IMarketingAgentsService marketingAgentsService,
                            IMasterConfigService masterConfigService,
                            IMetadataService metadataService,
                            IUiHelperService uiHelperService,
                            IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listingRepository = listingRepository ?? throw new ArgumentNullException(nameof(listingRepository));
        _listingImageRepository = listingImageRepository ?? throw new ArgumentNullException(nameof(listingImageRepository));
        _listingUnitRepository = listingUnitRepository ?? throw new ArgumentNullException(nameof(listingUnitRepository));
        _listingUnitHouseholdRepository = listingUnitHouseholdRepository ?? throw new ArgumentNullException(nameof(listingUnitHouseholdRepository));
        _listingAmenityRepository = listingAmenityRepository ?? throw new ArgumentNullException(nameof(listingAmenityRepository));
        _listingAccessibilityRepository = listingAccessibilityRepository ?? throw new ArgumentNullException(nameof(listingAccessibilityRepository));
        _listingDeclarationRepository = listingDeclarationRepository ?? throw new ArgumentNullException(nameof(listingDeclarationRepository));
        _listingDisclosureRepository = listingDisclosureRepository ?? throw new ArgumentNullException(nameof(listingDisclosureRepository));
        _listingDocumentRepository = listingDocumentRepository ?? throw new ArgumentNullException(nameof(listingDocumentRepository));
        _listingDocumentTypeRepository = listingDocumentTypeRepository ?? throw new ArgumentNullException(nameof(listingDocumentTypeRepository));
        _listingFundingSourceRepository = listingFundingSourceRepository ?? throw new ArgumentNullException(nameof(listingFundingSourceRepository));
        _amortizationsService = amortizationsService ?? throw new ArgumentNullException(nameof(amortizationsService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _marketingAgentsService = marketingAgentsService ?? throw new ArgumentNullException(nameof(marketingAgentsService));
        _masterConfigService = masterConfigService ?? throw new ArgumentNullException(nameof(masterConfigService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));

        _messages = new Dictionary<string, string>()
        {
            { "L001", "Listing not found" },
            { "L002", "Duplicate listing" },
            { "L003", "Failed to add listing" },
            { "L004", "Failed to update listing" },
            { "L005", "Failed to delete listing" },
            { "L006", "Failed to update address for listing" },
            { "L007", "Failed to update dates for listing" },
            { "L008", "Failed to update pets allowed for listing" },
            { "L009", "Failed to update documents required for listing" },
            { "L010", "Failed to update marketing agent for listing" },
            { "L101", "Listing Type is required" },
            { "L102", "Senior Living Opportunities is required" },
            { "L103", "Name is required" },
            { "L104", "Street Line 1 is required" },
            { "L105", "City is required" },
            { "L106", "State is required" },
            { "L107", "Zip Code is required" },
            { "L108", "County is required" },
            { "L109", "X Coordinate is required" },
            { "L110", "Y Coordinate is required" },
            { "L111", "Listing End Date must be on or after Listing Start Date" },
            { "L112", "Application Start Date must be on or after Listing End Date" },
            { "L113", "Application End Date must be on or after Application Start Date" },
            { "L114", "Lottery Date must be on or after Lottery End Date" },
            { "L115", "Waitlist Start Date must be on or after Application End Start Date" },
            { "L116", "Waitlist End Date must be on or after Waitlist Start Date" },
            { "L117", "Max. Income must be greater than Min. Income" },
            { "L118", "Max. Household Size must be greater than Min. Household Size" },
            { "L119", "Marketing Agent is required" },
            { "L120", "Marketing Agent Application Link is required" },

            { "L201", "Listing image not found" },
            { "L202", "Duplicate image" },
            { "L203", "Failed to add listing image" },
            { "L204", "Failed to update listing image" },
            { "L205", "Failed to delete listing image" },
            { "L211", "Title is required" },
            { "L212", "Image is required" },
            { "L213", "Image Type is required" },

            { "L251", "Listing document not found" },
            { "L252", "Duplicate document" },
            { "L253", "Failed to add listing document" },
            { "L254", "Failed to update listing document" },
            { "L255", "Failed to delete listing document" },
            { "L261", "Title is required" },
            { "L262", "File Name is required" },
            { "L263", "Document is required" },
            { "L264", "Document Type is required" },

            { "L301", "Listing unit not found" },
            { "L302", "Duplicate unit" },
            { "L303", "Failed to add listing unit" },
            { "L304", "Failed to update listing unit" },
            { "L305", "Failed to delete listing unit" },
            { "L311", "Unit Type is required" },
            { "L312", "Area Median Income Percentage is required" },
            { "L313", "Monthly Rental Amount is required" },
            { "L314", "Asset Limit Amount is required" },
            { "L315", "Estimated Price Amount is required" },
            { "L316", "Units Available is required" },
            { "L317", "Max. Household Income must be greater than Min. Houseshold Income" },
            { "L318", "Subsidy Amount must be between 0 and Estimated Price" },
            { "L319", "Monthly Taxes Amount be between 0 and Estimated Price" },
            { "L320", "Monthly Maintenance Amount be between 0 and Estimated Price" },
            { "L321", "Monthly Insurance Amount be between 0 and Estimated Price" },

            { "L403", "Failed to save listing amenities" },

            { "L503", "Failed to save listing accessibilities" },

            { "L601", "Listing declaration not found" },
            { "L602", "Duplicate listing declaration" },
            { "L603", "Failed to add listing declaration" },
            { "L604", "Failed to update listing declaration" },
            { "L605", "Failed to delete listing declaration" },
            { "L611", "Declaration is required" },

            { "L701", "Listing disclosure not found" },
            { "L702", "Duplicate listing disclosure" },
            { "L703", "Failed to add listing disclosure" },
            { "L704", "Failed to update listing disclosure" },
            { "L705", "Failed to delete listing disclosure" },
            { "L711", "Disclosure is required" },

            { "L901", "Failed to submit listing for review" },
            { "L902", "Failed to publish listing" },
            { "L903", "Failed to send listing back for revisions" },
            { "L904", "Failed to send published listing back for revisions" },
            { "L911", "Unable to submit listing for review - Listing Type is required" },
            { "L912", "Unable to submit listing for review - Name is required" },
            { "L913", "Unable to submit listing for review - Street Line 1 is required" },
            { "L914", "Unable to submit listing for review - City is required" },
            { "L915", "Unable to submit listing for review - State is required" },
            { "L916", "Unable to submit listing for review - Zip Code is required" },
            { "L917", "Unable to submit listing for review - County is required" },
            { "L918", "Unable to submit listing for review - Listing Start Date is required" },
        };
    }

    public string GetMessage(string code)
    {
        if (_messages.ContainsKey(code))
        {
            return _messages[code];
        }
        return code;
    }

    public async Task<ListingsViewModel> GetData(string requestId, string correlationId, string username, int pageNo = 1, int pageSize = 100)
    {
        var userRole = await _usersService.GetUserRole(correlationId, username);
        var pagedListings = await _listingRepository.GetAllPaged(pageNo, pageSize);
        var model = new ListingsViewModel
        {
            Listings = pagedListings.Records.Select(s => s.ToViewModel()),
            PageNo = pagedListings.PagingInfo.PageNo,
            PageSize = pagedListings.PagingInfo.PageSize,
            TotalPages = pagedListings.PagingInfo.TotalPages,
            TotalRecords = pagedListings.PagingInfo.TotalRecords,
            PageSizes = [25, 50, 100, 200],
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|"),

            // ArcGIS
            ArcGisSettings = await _masterConfigService.GetArcGisSettings()
        };
        return model;
    }

    public async Task<IEnumerable<ListingViewModel>> GetAll()
    {
        var listings = await _listingRepository.GetAll();
        return listings.Select(s => s.ToViewModel());
    }

    public async Task<ListingViewModel> GetOne(string requestId, string correlationId, string username, long listingId, bool withDetails = false)
    {
        var userRole = await _usersService.GetUserRole(correlationId, username);
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });

        if (listing == null) return null;
        var model = listing.ToViewModel();
        model.CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|") && "|DRAFT|REVISE|".Contains($"|{listing.StatusCd}|");
        model.CanPublish = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|") && "|REVIEW|".Contains($"|{listing.StatusCd}|");
        model.CanUnpublish = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|") && "|PUBLISHED|".Contains($"|{listing.StatusCd}|");

        if (withDetails)
        {
            model.Images = await GetImages(listingId);
            model.Documents = await GetDocuments(listingId);
            model.Units = await GetUnits(listingId);
            model.Households = await GetHouseholds(listingId);
            model.Amenities = await GetAmenities(listingId);
            model.Accessibilities = await GetListingAccessibilities(listingId);
            model.Declarations = await GetListingDeclarations(listingId);
            model.Disclosures = await GetListingDisclosures(listingId);
            model.DocumentTypes = await GetDocumentTypes(listingId);
            model.FundingSources = await GetFundingSources(listingId);
        }

        // TODO :: Determine logic for paper application submissions
        model.CanSubmitPaperApplication = true;

        // ArcGIS
        model.ArcGisSettings = await _masterConfigService.GetArcGisSettings();

        return model;
    }

    public async Task<string> Add(string correlationId, string username, ListingViewModel listing)
    {
        listing = listing ?? throw new ArgumentNullException(nameof(listing));

        listing.ListingTypeCd = listing.ListingTypeCd?.Trim() ?? string.Empty;
        var listingTypes = await _metadataService.GetListingTypes();
        if (string.IsNullOrEmpty(listing.ListingTypeCd) || !listingTypes.ContainsKey(listing.ListingTypeCd))
        {
            _logger.LogError($"Unable to add listing - Invalid Listing Type");
            return "L101";
        }

        if (listing.IsRental) listing.ResaleInd = false;

        listing.ListingAgeTypeCd = listing.ListingAgeTypeCd?.Trim() ?? string.Empty;
        var listingAgeTypes = await _metadataService.GetListingAgeTypes();
        if (string.IsNullOrEmpty(listing.ListingAgeTypeCd) || !listingAgeTypes.ContainsKey(listing.ListingAgeTypeCd))
        {
            _logger.LogError($"Unable to add listing - Invalid Senior Living Opportunities");
            return "L102";
        }

        listing.Name = listing.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Name))
        {
            _logger.LogError($"Unable to add listing - Invalid Name");
            return "L103";
        }

        listing.Description = listing.Description?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Description)) listing.Description = null;

        listing.WebsiteUrl = listing.WebsiteUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.WebsiteUrl)) listing.WebsiteUrl = null;

        listing.StreetLine1 = listing.StreetLine1?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine1))
        {
            _logger.LogError($"Unable to add listing - Invalid Street Line 1");
            return "L104";
        }

        listing.StreetLine2 = listing.StreetLine2?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine2)) listing.StreetLine2 = null;

        listing.StreetLine3 = listing.StreetLine3?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine3)) listing.StreetLine3 = null;

        listing.City = listing.City?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.City))
        {
            _logger.LogError($"Unable to add listing - Invalid City");
            return "L105";
        }

        listing.StateCd = listing.StateCd?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StateCd))
        {
            _logger.LogError($"Unable to add listing - Invalid State");
            return "L106";
        }

        listing.ZipCode = listing.ZipCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.ZipCode) || (listing.ZipCode.Length != 5 && listing.ZipCode.Length != 9))
        {
            _logger.LogError($"Unable to add listing - Invalid Zip Code");
            return "L107";
        }

        listing.County = listing.County?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.County))
        {
            _logger.LogError($"Unable to add listing - Invalid County");
            return "L108";
        }

        listing.EsriX = listing.EsriX?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.EsriX))
        {
            _logger.LogError($"Unable to add listing - Invalid X Coordinate for Address");
            return "L109";
        }

        listing.EsriY = listing.EsriY?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.EsriY))
        {
            _logger.LogError($"Unable to add listing - Invalid Y Coordinate for Address");
            return "L110";
        }

        listing.Municipality = listing.Municipality?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Municipality)) listing.Municipality = null;

        listing.MunicipalityUrl = listing.MunicipalityUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.MunicipalityUrl)) listing.MunicipalityUrl = null;

        listing.SchoolDistrict = listing.SchoolDistrict?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrict)) listing.SchoolDistrict = null;

        listing.SchoolDistrictUrl = listing.SchoolDistrictUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrictUrl)) listing.SchoolDistrictUrl = null;

        var listings = await _listingRepository.GetAll();

        var duplicateListing = listings.FirstOrDefault(f => f.Name == listing.Name && f.City == listing.City && f.StateCd == listing.StateCd && f.ZipCode == listing.ZipCode);
        if (duplicateListing != null)
        {
            _logger.LogError($"Unable to add duplicate listing - {listing.Name}");
            return "L002";
        }

        listing.RentIncludesText = listing.RentIncludesText?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.RentIncludesText)) listing.RentIncludesText = null;

        listing.CompletedOrInitialOccupancyYear = listing.CompletedOrInitialOccupancyYear?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.CompletedOrInitialOccupancyYear)) listing.CompletedOrInitialOccupancyYear = null;

        listing.TermOfAffordability = listing.TermOfAffordability?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.TermOfAffordability)) listing.TermOfAffordability = null;

        listing.StatusCd = "DRAFT";
        listing.CreatedBy = username;
        var added = await _listingRepository.Add(correlationId, listing);
        if (!added)
        {
            _logger.LogError($"Failed to add listing - Unknown error");
            return "L003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, ListingViewModel listing)
    {
        listing = listing ?? throw new ArgumentNullException(nameof(listing));

        listing.ListingTypeCd = listing.ListingTypeCd?.Trim() ?? string.Empty;
        var listingTypes = await _metadataService.GetListingTypes();
        if (string.IsNullOrEmpty(listing.ListingTypeCd) || !listingTypes.ContainsKey(listing.ListingTypeCd))
        {
            _logger.LogError($"Unable to update listing - Invalid Listing Type");
            return "L101";
        }

        if (listing.IsRental) listing.ResaleInd = false;

        listing.ListingAgeTypeCd = listing.ListingAgeTypeCd?.Trim() ?? string.Empty;
        var listingAgeTypes = await _metadataService.GetListingAgeTypes();
        if (string.IsNullOrEmpty(listing.ListingAgeTypeCd) || !listingAgeTypes.ContainsKey(listing.ListingAgeTypeCd))
        {
            _logger.LogError($"Unable to add listing - Invalid Senior Living Opportunities");
            return "L102";
        }

        listing.Name = listing.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Name))
        {
            _logger.LogError($"Unable to update listing - Invalid Name");
            return "L103";
        }

        listing.Description = listing.Description?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Description)) listing.Description = null;

        listing.WebsiteUrl = listing.WebsiteUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.WebsiteUrl)) listing.WebsiteUrl = null;

        listing.StreetLine1 = listing.StreetLine1?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine1))
        {
            _logger.LogError($"Unable to update listing - Invalid Street Line 1");
            return "L104";
        }

        listing.StreetLine2 = listing.StreetLine2?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine2)) listing.StreetLine2 = null;

        listing.StreetLine3 = listing.StreetLine3?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine3)) listing.StreetLine3 = null;

        listing.City = listing.City?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.City))
        {
            _logger.LogError($"Unable to update listing - Invalid City");
            return "L105";
        }

        listing.StateCd = listing.StateCd?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StateCd))
        {
            _logger.LogError($"Unable to update listing - Invalid State");
            return "L106";
        }

        listing.ZipCode = listing.ZipCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.ZipCode) || (listing.ZipCode.Length != 5 && listing.ZipCode.Length != 9))
        {
            _logger.LogError($"Unable to update listing - Invalid Zip Code");
            return "L107";
        }

        listing.County = listing.County?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.County))
        {
            _logger.LogError($"Unable to update listing - Invalid County");
            return "L108";
        }

        listing.Municipality = listing.Municipality?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Municipality)) listing.Municipality = null;

        listing.MunicipalityUrl = listing.MunicipalityUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.MunicipalityUrl)) listing.MunicipalityUrl = null;

        listing.SchoolDistrict = listing.SchoolDistrict?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrict)) listing.SchoolDistrict = null;

        listing.SchoolDistrictUrl = listing.SchoolDistrictUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrictUrl)) listing.SchoolDistrictUrl = null;

        var listings = await _listingRepository.GetAll();

        var existingListing = listings.FirstOrDefault(f => f.ListingId == listing.ListingId);
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listing.ListingId}");
            return "L001";
        }

        var duplicateListing = listings.FirstOrDefault(f => f.ListingId != listing.ListingId && f.Name == listing.Name && f.City == listing.City && f.StateCd == listing.StateCd && f.ZipCode == listing.ZipCode);
        if (duplicateListing != null)
        {
            _logger.LogError($"Unable to update duplicate listing - {listing.Name}");
            return "L002";
        }

        listing.RentIncludesText = listing.RentIncludesText?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.RentIncludesText)) listing.RentIncludesText = null;

        listing.CompletedOrInitialOccupancyYear = listing.CompletedOrInitialOccupancyYear?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.CompletedOrInitialOccupancyYear)) listing.CompletedOrInitialOccupancyYear = null;

        listing.TermOfAffordability = listing.TermOfAffordability?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.TermOfAffordability)) listing.TermOfAffordability = null;

        listing.StatusCd = existingListing.StatusCd;
        listing.ModifiedBy = username;
        var updated = await _listingRepository.Update(correlationId, listing);
        if (!updated)
        {
            _logger.LogError($"Failed to update listing - Unknown error");
            return "L004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, long id)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = id });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {id}");
            return "L001";
        }

        existingListing.ModifiedBy = username;
        var deleted = await _listingRepository.Delete(correlationId, existingListing);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete listing - Unknown error");
            return "L005";
        }

        return "";
    }

    public async Task<string> UpdateAddress(string correlationId, string username, ListingViewModel listing)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listing.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listing.ListingId}");
            return "L001";
        }

        listing.StreetLine1 = listing.StreetLine1?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine1))
        {
            _logger.LogError($"Unable to update listing - Invalid Street Line 1");
            return "L104";
        }

        listing.StreetLine2 = listing.StreetLine2?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine2)) listing.StreetLine2 = null;

        listing.StreetLine3 = listing.StreetLine3?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine3)) listing.StreetLine3 = null;

        listing.City = listing.City?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.City))
        {
            _logger.LogError($"Unable to update listing - Invalid City");
            return "L105";
        }

        listing.StateCd = listing.StateCd?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StateCd))
        {
            _logger.LogError($"Unable to update listing - Invalid State");
            return "L106";
        }

        listing.ZipCode = listing.ZipCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.ZipCode) || (listing.ZipCode.Length != 5 && listing.ZipCode.Length != 9))
        {
            _logger.LogError($"Unable to update listing - Invalid Zip Code");
            return "L107";
        }

        listing.County = listing.County?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.County))
        {
            _logger.LogError($"Unable to update listing - Invalid County");
            return "L108";
        }

        listing.Municipality = listing.Municipality?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Municipality)) listing.Municipality = null;

        listing.MunicipalityUrl = listing.MunicipalityUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.MunicipalityUrl)) listing.MunicipalityUrl = null;

        listing.SchoolDistrict = listing.SchoolDistrict?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrict)) listing.SchoolDistrict = null;

        listing.SchoolDistrictUrl = listing.SchoolDistrictUrl?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.SchoolDistrictUrl)) listing.SchoolDistrictUrl = null;

        listing.ModifiedBy = username;
        var saved = await _listingRepository.UpdateAddress(correlationId, listing);
        if (!saved)
        {
            _logger.LogError($"Failed to update address for listing - Unknown error");
            return "L006";
        }

        return "";
    }

    public async Task<string> UpdateDates(string correlationId, string username, ListingViewModel listing)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listing.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listing.ListingId}");
            return "L001";
        }

        var minDate = new DateTime(1990, 1, 1);

        var dateValue = listing.ListingStartDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ListingStartDate = null;
        }

        dateValue = listing.ListingEndDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ListingEndDate = null;
        }

        if (listing.ListingStartDate.HasValue && listing.ListingEndDate.HasValue && listing.ListingEndDate.Value < listing.ListingStartDate.Value)
        {
            _logger.LogError($"Unable to update listing - Invalid Listing End Date");
            return "L111";
        }

        dateValue = listing.ApplicationStartDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ApplicationStartDate = null;
        }

        dateValue = listing.ApplicationEndDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ApplicationEndDate = null;
        }

        if (listing.ListingStartDate.HasValue && listing.ApplicationStartDate.HasValue && listing.ApplicationStartDate.Value < listing.ListingStartDate.Value)
        {
            _logger.LogError($"Unable to update listing - Invalid Application Start Date");
            return "L112";
        }

        if (listing.ApplicationStartDate.HasValue && listing.ApplicationEndDate.HasValue && listing.ApplicationEndDate.Value < listing.ApplicationStartDate.Value)
        {
            _logger.LogError($"Unable to update listing - Invalid Application End Date");
            return "L113";
        }

        dateValue = listing.LotteryDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.LotteryDate = null;
        }

        if (listing.ApplicationEndDate.HasValue && listing.LotteryDate.HasValue && listing.LotteryDate.Value < listing.ApplicationEndDate.Value)
        {
            _logger.LogError($"Unable to update listing - Invalid Lottery Date");
            return "L114";
        }

        if (listing.WaitlistEligible)
        {
            dateValue = listing.WaitlistStartDate.GetValueOrDefault(DateTime.MinValue);
            if (dateValue < minDate)
            {
                listing.WaitlistStartDate = null;
            }

            dateValue = listing.WaitlistEndDate.GetValueOrDefault(DateTime.MinValue);
            if (dateValue < minDate)
            {
                listing.WaitlistEndDate = null;
            }

            if (listing.ApplicationEndDate.HasValue && listing.WaitlistStartDate.HasValue && listing.WaitlistStartDate < listing.ApplicationEndDate)
            {
                _logger.LogError($"Unable to update listing - Invalid Waitlist Start Date");
                return "L115";
            }

            if (listing.WaitlistStartDate.HasValue && listing.WaitlistEndDate.HasValue && listing.WaitlistEndDate.Value < listing.WaitlistStartDate.Value)
            {
                _logger.LogError($"Unable to update listing - Invalid Waitlist End Date");
                return "L116";
            }
        }
        else
        {
            listing.WaitlistStartDate = null;
            listing.WaitlistEndDate = null;
        }

        listing.ModifiedBy = username;
        var saved = await _listingRepository.UpdateDates(correlationId, listing);
        if (!saved)
        {
            _logger.LogError($"Failed to update dates for listing - Unknown error");
            return "L007";
        }

        return "";
    }

    public async Task<string> UpdatePetsAllowed(string correlationId, string username, ListingViewModel listing)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listing.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listing.ListingId}");
            return "L001";
        }

        listing.ModifiedBy = username;
        var saved = await _listingRepository.UpdatePetsAllowed(correlationId, listing);
        if (!saved)
        {
            _logger.LogError($"Failed to update pets allowed for listing - Unknown error");
            return "L008";
        }

        return "";
    }

    public async Task<string> UpdateMarketingAgent(string requestId, string correlationId, string username, ListingViewModel listing)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listing.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listing.ListingId}");
            return "L001";
        }

        if (listing.MarketingAgentInd)
        {
            var agents = await _marketingAgentsService.GetAll(requestId, correlationId);
            if (listing.MarketingAgentId <= 0 || agents.FirstOrDefault(f => f.AgentId == listing.MarketingAgentId) == null)
            {
                _logger.LogError($"Unable to update listing - Marketing Agent is required");
                return "L119";
            }

            if (string.IsNullOrEmpty(listing.MarketingAgentApplicationLink))
            {
                _logger.LogError($"Unable to update listing - Marketing Agent Application Link is required");
                return "L120";
            }
        }
        else
        {
            listing.MarketingAgentId = 0;
            listing.MarketingAgentApplicationLink = null;
        }

        listing.ModifiedBy = username;
        var saved = await _listingRepository.UpdateMarketingAgent(requestId, correlationId, listing);
        if (!saved)
        {
            _logger.LogError($"Failed to update marketing agent for listing - Unknown error");
            return "L010";
        }

        return "";
    }

    public async Task<IEnumerable<ListingImageViewModel>> GetImages(long listingId)
    {
        var images = await _listingImageRepository.GetAll(listingId);
        return images.Select(s => new ListingImageViewModel()
        {
            Active = s.Active,
            Contents = s.Contents,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            DisplayOnListingsPageInd = s.DisplayOnListingsPageInd,
            ImageId = s.ImageId,
            IsPrimary = s.IsPrimary,
            ListingId = s.ListingId,
            MimeType = s.MimeType,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            ThumbnailContents = s.ThumbnailContents,
            Title = s.Title,
        });
    }

    public async Task<ListingImageViewModel> GetImage(int imageId)
    {
        var image = await _listingImageRepository.GetOne(imageId);
        return new ListingImageViewModel()
        {
            Active = image.Active,
            Contents = image.Contents,
            CreatedBy = image.CreatedBy,
            CreatedDate = image.CreatedDate,
            DisplayOnListingsPageInd = image.DisplayOnListingsPageInd,
            ImageId = image.ImageId,
            IsPrimary = image.IsPrimary,
            ListingId = image.ListingId,
            MimeType = image.MimeType,
            ModifiedBy = image.ModifiedBy,
            ModifiedDate = image.ModifiedDate,
            ThumbnailContents = image.ThumbnailContents,
            Title = image.Title,
        };
    }

    public async Task<string> AddImage(string correlationId, string username, ListingImageViewModel image)
    {
        image = image ?? throw new ArgumentNullException(nameof(image));

        image.Title = image.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(image.Title))
        {
            _logger.LogError($"Unable to add listing image - Invalid Title");
            return "L211";
        }

        image.ThumbnailContents = image.ThumbnailContents?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(image.ThumbnailContents)) image.ThumbnailContents = null;

        image.Contents = image.Contents?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(image.Contents))
        {
            _logger.LogError($"Unable to add listing image - Invalid Image");
            return "L212";
        }

        image.MimeType = image.MimeType?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(image.MimeType))
        {
            _logger.LogError($"Unable to add listing image - Invalid Image Type");
            return "L213";
        }

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = image.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing image - {image.ListingId}");
            return "L001";
        }

        image.CreatedBy = username;
        var added = await _listingImageRepository.Add(correlationId, image);
        if (!added)
        {
            _logger.LogError($"Failed to add image for listing - Unknown error");
            return "L203";
        }

        return "";
    }

    public async Task<string> UpdateImage(string correlationId, string username, ListingImageViewModel image)
    {
        image = image ?? throw new ArgumentNullException(nameof(image));

        image.Title = image.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(image.Title))
        {
            _logger.LogError($"Unable to update listing image - Invalid Title");
            return "L211";
        }

        var existingImage = await _listingImageRepository.GetOne(image.ImageId);
        if (existingImage == null)
        {
            _logger.LogError($"Unable to find listing image - {image.ImageId}");
            return "L201";
        }

        image.ModifiedBy = username;
        var updated = await _listingImageRepository.Update(correlationId, image);
        if (!updated)
        {
            _logger.LogError($"Failed to update image for listing - Unknown error");
            return "L204";
        }

        return "";
    }

    public async Task<string> DeleteImage(string correlationId, string username, int id)
    {
        var existingImage = await _listingImageRepository.GetOne(id);
        if (existingImage == null)
        {
            _logger.LogError($"Unable to find image - {id}");
            return "L201";
        }

        existingImage.ModifiedBy = username;
        var deleted = await _listingImageRepository.Delete(correlationId, existingImage);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete image for listing - Unknown error");
            return "L205";
        }

        return "";
    }

    public async Task<IEnumerable<ListingDocumentViewModel>> GetDocuments(long listingId)
    {
        var documents = await _listingDocumentRepository.GetAll(listingId);
        return documents.Select(s => new ListingDocumentViewModel()
        {
            Active = s.Active,
            Contents = s.Contents,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            DisplayOnListingsPageInd = s.DisplayOnListingsPageInd,
            DocumentId = s.DocumentId,
            FileName = s.FileName,
            ListingId = s.ListingId,
            MimeType = s.MimeType,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            Title = s.Title,
        });
    }

    public async Task<ListingDocumentViewModel> GetDocument(int documentId)
    {
        var document = await _listingDocumentRepository.GetOne(documentId);
        return new ListingDocumentViewModel()
        {
            Active = document.Active,
            Contents = document.Contents,
            CreatedBy = document.CreatedBy,
            CreatedDate = document.CreatedDate,
            DisplayOnListingsPageInd = document.DisplayOnListingsPageInd,
            DocumentId = document.DocumentId,
            FileName = document.FileName,
            ListingId = document.ListingId,
            MimeType = document.MimeType,
            ModifiedBy = document.ModifiedBy,
            ModifiedDate = document.ModifiedDate,
            Title = document.Title,
        };
    }

    public async Task<string> AddDocument(string correlationId, string username, ListingDocumentViewModel document)
    {
        document = document ?? throw new ArgumentNullException(nameof(document));

        document.Title = document.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(document.Title))
        {
            _logger.LogError($"Unable to add listing document - Invalid Title");
            return "L261";
        }

        document.FileName = document.FileName?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(document.FileName))
        {
            _logger.LogError($"Unable to add listing document - Invalid File Name");
            return "L262";
        }

        document.Contents = document.Contents?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(document.Contents))
        {
            _logger.LogError($"Unable to add listing document - Invalid Document");
            return "L263";
        }

        document.MimeType = document.MimeType?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(document.MimeType))
        {
            _logger.LogError($"Unable to add listing document - Invalid Document Type");
            return "L264";
        }

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = document.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing document - {document.ListingId}");
            return "L001";
        }

        document.CreatedBy = username;
        var added = await _listingDocumentRepository.Add(correlationId, document);
        if (!added)
        {
            _logger.LogError($"Failed to add document for listing - Unknown error");
            return "L253";
        }

        return "";
    }

    public async Task<string> UpdateDocument(string correlationId, string username, ListingDocumentViewModel document)
    {
        document = document ?? throw new ArgumentNullException(nameof(document));

        document.Title = document.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(document.Title))
        {
            _logger.LogError($"Unable to update listing document - Invalid Title");
            return "L261";
        }

        var existingDocument = await _listingDocumentRepository.GetOne(document.DocumentId);
        if (existingDocument == null)
        {
            _logger.LogError($"Unable to find listing document - {document.DocumentId}");
            return "L251";
        }

        document.ModifiedBy = username;
        var updated = await _listingDocumentRepository.Update(correlationId, document);
        if (!updated)
        {
            _logger.LogError($"Failed to update document for listing - Unknown error");
            return "L254";
        }

        return "";
    }

    public async Task<string> DeleteDocument(string correlationId, string username, int id)
    {
        var existingDocument = await _listingDocumentRepository.GetOne(id);
        if (existingDocument == null)
        {
            _logger.LogError($"Unable to find document - {id}");
            return "L251";
        }

        existingDocument.ModifiedBy = username;
        var deleted = await _listingDocumentRepository.Delete(correlationId, existingDocument);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete document for listing - Unknown error");
            return "L255";
        }

        return "";
    }

    public async Task<IEnumerable<ListingUnitViewModel>> GetUnits(long listingId)
    {
        var units = await _listingUnitRepository.GetAll(listingId);
        return units.Select(s => new ListingUnitViewModel()
        {
            Active = s.Active,
            AreaMedianIncomePct = s.AreaMedianIncomePct,
            AssetLimitAmt = s.AssetLimitAmt,
            BathroomCnt = s.BathroomCnt,
            BathroomCntPart = s.BathroomCntPart,
            BedroomCnt = s.BedroomCnt,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            EstimatedPriceAmt = s.EstimatedPriceAmt,
            ListingId = s.ListingId,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            MonthlyInsuranceAmt = s.MonthlyInsuranceAmt,
            MonthlyMaintenanceAmt = s.MonthlyMaintenanceAmt,
            MonthlyRentAmt = s.MonthlyRentAmt,
            MonthlyTaxesAmt = s.MonthlyTaxesAmt,
            SquareFootage = s.SquareFootage,
            SubsidyAmt = s.SubsidyAmt,
            UnitId = s.UnitId,
            UnitsAvailableCnt = s.UnitsAvailableCnt,
            UnitTypeCd = s.UnitTypeCd,
            UnitTypeDescription = s.UnitTypeDescription
        });
    }

    public async Task<ListingUnitViewModel> GetUnit(int unitId)
    {
        var unit = await _listingUnitRepository.GetOne(unitId);
        return new ListingUnitViewModel()
        {
            Active = unit.Active,
            AreaMedianIncomePct = unit.AreaMedianIncomePct,
            AssetLimitAmt = unit.AssetLimitAmt,
            BathroomCnt = unit.BathroomCnt,
            BathroomCntPart = unit.BathroomCntPart,
            BedroomCnt = unit.BedroomCnt,
            CreatedBy = unit.CreatedBy,
            CreatedDate = unit.CreatedDate,
            EstimatedPriceAmt = unit.EstimatedPriceAmt,
            ListingId = unit.ListingId,
            ModifiedBy = unit.ModifiedBy,
            ModifiedDate = unit.ModifiedDate,
            MonthlyInsuranceAmt = unit.MonthlyInsuranceAmt,
            MonthlyMaintenanceAmt = unit.MonthlyMaintenanceAmt,
            MonthlyRentAmt = unit.MonthlyRentAmt,
            MonthlyTaxesAmt = unit.MonthlyTaxesAmt,
            SquareFootage = unit.SquareFootage,
            SubsidyAmt = unit.SubsidyAmt,
            UnitId = unit.UnitId,
            UnitsAvailableCnt = unit.UnitsAvailableCnt,
            UnitTypeCd = unit.UnitTypeCd,
            UnitTypeDescription = unit.UnitTypeDescription
        };
    }

    public async Task<string> AddUnit(string correlationId, string username, ListingUnitViewModel unit)
    {
        unit = unit ?? throw new ArgumentNullException(nameof(unit));

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = unit.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {unit.ListingId}");
            return "L001";
        }

        unit.UnitTypeCd = unit.UnitTypeCd?.Trim() ?? string.Empty;
        var unitTypes = await _metadataService.GetUnitTypes();
        if (string.IsNullOrEmpty(unit.UnitTypeCd) || !unitTypes.ContainsKey(unit.UnitTypeCd))
        {
            _logger.LogError($"Unable to add listing unit - Invalid Unit Type");
            return "L311";
        }

        unit.BedroomCnt = unit.BedroomCnt < 0 ? 0 : unit.BedroomCnt;
        unit.BathroomCnt = unit.BathroomCnt < 0 ? 0 : unit.BathroomCnt;
        unit.BathroomCntPart = unit.BathroomCntPart < 0 ? 0 : unit.BathroomCntPart;
        unit.SquareFootage = unit.SquareFootage < 0 ? 0 : unit.SquareFootage;

        unit.AreaMedianIncomePct = unit.AreaMedianIncomePct < 0 ? 0 : unit.AreaMedianIncomePct;
        if (unit.AreaMedianIncomePct <= 0)
        {
            _logger.LogError($"Unable to add listing unit - Invalid Area Median Income %");
            return "L312";
        }

        if (existingListing.IsRental)
        {
            if (unit.MonthlyRentAmt <= 0)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Rental ($)");
                return "L313";
            }
            unit.AssetLimitAmt = 0L;
            unit.EstimatedPriceAmt = 0L;
            unit.MonthlyInsuranceAmt = 0L;
            unit.MonthlyMaintenanceAmt = 0L;
            unit.MonthlyTaxesAmt = 0L;
        }
        else
        {
            unit.MonthlyRentAmt = 0L;
            // if (unit.AssetLimitAmt <= 0)
            // {
            //     _logger.LogError($"Unable to add listing unit - Invalid Asset Limit ($)");
            //     return "L314";
            // }

            unit.EstimatedPriceAmt = unit.EstimatedPriceAmt < 0L ? 0L : unit.EstimatedPriceAmt;
            if (unit.EstimatedPriceAmt <= 0)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Estimated Price ($)");
                return "L315";
            }

            unit.SubsidyAmt = unit.SubsidyAmt < 0L ? 0L : unit.SubsidyAmt;
            if (unit.SubsidyAmt < 0 || unit.SubsidyAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Subsidy Amount ($)");
                return "L318";
            }

            unit.MonthlyTaxesAmt = unit.MonthlyTaxesAmt < 0L ? 0L : unit.MonthlyTaxesAmt;
            if (unit.MonthlyTaxesAmt < 0 || unit.MonthlyTaxesAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Taxes Amount ($)");
                return "L319";
            }

            unit.MonthlyMaintenanceAmt = unit.MonthlyMaintenanceAmt < 0L ? 0L : unit.MonthlyMaintenanceAmt;
            if (unit.MonthlyMaintenanceAmt < 0 || unit.MonthlyMaintenanceAmt > unit.MonthlyMaintenanceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Maintenance Amount ($)");
                return "L320";
            }

            unit.MonthlyInsuranceAmt = unit.MonthlyInsuranceAmt < 0L ? 0L : unit.MonthlyInsuranceAmt;
            if (unit.MonthlyInsuranceAmt < 0 || unit.MonthlyInsuranceAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Insurance Amount ($)");
                return "L321";
            }
        }

        unit.UnitsAvailableCnt = unit.UnitsAvailableCnt < 0 ? 0 : unit.UnitsAvailableCnt;

        if ((unit.UnitHouseholds?.Count() ?? 0) == 0)
        {
            _logger.LogError($"Unable to add listing unit - Invalid Household Requirements");
            return "L316";
        }

        unit.Households = [];
        foreach (var unitHousehold in unit.UnitHouseholds)
        {
            unit.Households.Add(new ListingUnitHousehold()
            {
                HouseholdId = 0,
                HouseholdSize = unitHousehold.HouseholdSize,
                MaxHouseholdIncomeAmt = unitHousehold.MaxHouseholdIncomeAmt,
                MinHouseholdIncomeAmt = unitHousehold.MinHouseholdIncomeAmt
            });
        }

        unit.CreatedBy = username;
        var added = await _listingUnitRepository.Add(correlationId, unit);
        if (!added)
        {
            _logger.LogError($"Failed to add unit for listing - Unknown error");
            return "L303";
        }

        return "";
    }

    public async Task<string> UpdateUnit(string correlationId, string username, ListingUnitViewModel unit)
    {
        var units = await _listingUnitRepository.GetAll(unit.ListingId);

        var existingUnit = units.FirstOrDefault(f => f.UnitId == unit.UnitId);
        if (existingUnit == null)
        {
            _logger.LogError($"Unable to find unit - {unit.UnitId}");
            return "L301";
        }

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = unit.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {unit.ListingId}");
            return "L001";
        }

        unit.UnitTypeCd = unit.UnitTypeCd?.Trim() ?? string.Empty;
        var unitTypes = await _metadataService.GetUnitTypes();
        if (string.IsNullOrEmpty(unit.UnitTypeCd) || !unitTypes.ContainsKey(unit.UnitTypeCd))
        {
            _logger.LogError($"Unable to update listing unit - Invalid Unit Type");
            return "L311";
        }

        unit.BedroomCnt = unit.BedroomCnt < 0 ? 0 : unit.BedroomCnt;
        unit.BathroomCnt = unit.BathroomCnt < 0 ? 0 : unit.BathroomCnt;
        unit.BathroomCntPart = unit.BathroomCntPart < 0 ? 0 : unit.BathroomCntPart;
        unit.SquareFootage = unit.SquareFootage < 0 ? 0 : unit.SquareFootage;

        unit.AreaMedianIncomePct = unit.AreaMedianIncomePct < 0 ? 0 : unit.AreaMedianIncomePct;
        if (unit.AreaMedianIncomePct <= 0)
        {
            _logger.LogError($"Unable to update listing unit - Invalid Area Median Income %");
            return "L312";
        }

        if (existingListing.IsRental)
        {
            if (unit.MonthlyRentAmt <= 0)
            {
                _logger.LogError($"Unable to update listing unit - Invalid Monthly Rental ($)");
                return "L313";
            }
            unit.AssetLimitAmt = 0L;
            unit.EstimatedPriceAmt = 0L;
            unit.MonthlyInsuranceAmt = 0L;
            unit.MonthlyMaintenanceAmt = 0L;
            unit.MonthlyTaxesAmt = 0L;
        }
        else
        {
            unit.MonthlyRentAmt = 0L;
            // if (unit.AssetLimitAmt <= 0)
            // {
            //     _logger.LogError($"Unable to update listing unit - Invalid Asset Limit ($)");
            //     return "L314";
            // }

            unit.EstimatedPriceAmt = unit.EstimatedPriceAmt < 0L ? 0L : unit.EstimatedPriceAmt;
            if (unit.EstimatedPriceAmt <= 0)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Estimated Price ($)");
                return "L315";
            }

            unit.SubsidyAmt = unit.SubsidyAmt < 0L ? 0L : unit.SubsidyAmt;
            if (unit.SubsidyAmt < 0 || unit.SubsidyAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Subsidy Amount ($)");
                return "L318";
            }

            unit.MonthlyTaxesAmt = unit.MonthlyTaxesAmt < 0L ? 0L : unit.MonthlyTaxesAmt;
            if (unit.MonthlyTaxesAmt < 0 || unit.MonthlyTaxesAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Taxes Amount ($)");
                return "L319";
            }

            unit.MonthlyMaintenanceAmt = unit.MonthlyMaintenanceAmt < 0L ? 0L : unit.MonthlyMaintenanceAmt;
            if (unit.MonthlyMaintenanceAmt < 0 || unit.MonthlyMaintenanceAmt > unit.MonthlyMaintenanceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Maintenance Amount ($)");
                return "L320";
            }

            unit.MonthlyInsuranceAmt = unit.MonthlyInsuranceAmt < 0L ? 0L : unit.MonthlyInsuranceAmt;
            if (unit.MonthlyInsuranceAmt < 0 || unit.MonthlyInsuranceAmt > unit.EstimatedPriceAmt)
            {
                _logger.LogError($"Unable to add listing unit - Invalid Monthly Insurance Amount ($)");
                return "L321";
            }
        }

        unit.UnitsAvailableCnt = unit.UnitsAvailableCnt < 0 ? 0 : unit.UnitsAvailableCnt;

        if ((unit.UnitHouseholds?.Count() ?? 0) == 0)
        {
            _logger.LogError($"Unable to update listing unit - Invalid Household Requirements");
            return "L316";
        }

        unit.Households = [];
        foreach (var unitHousehold in unit.UnitHouseholds)
        {
            unit.Households.Add(new ListingUnitHousehold()
            {
                HouseholdId = 0,
                HouseholdSize = unitHousehold.HouseholdSize,
                MaxHouseholdIncomeAmt = unitHousehold.MaxHouseholdIncomeAmt,
                MinHouseholdIncomeAmt = unitHousehold.MinHouseholdIncomeAmt
            });
        }

        unit.ModifiedBy = username;
        var updated = await _listingUnitRepository.Update(correlationId, unit);
        if (!updated)
        {
            _logger.LogError($"Failed to update unit for listing - Unknown error");
            return "L304";
        }

        return "";
    }

    public async Task<string> DeleteUnit(string correlationId, string username, int id)
    {
        var existingUnit = await _listingUnitRepository.GetOne(id);
        if (existingUnit == null)
        {
            _logger.LogError($"Unable to find unit - {id}");
            return "L301";
        }

        existingUnit.ModifiedBy = username;
        var deleted = await _listingUnitRepository.Delete(correlationId, existingUnit);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete unit for listing - Unknown error");
            return "L305";
        }

        return "";
    }

    public async Task<IEnumerable<ListingUnitHouseholdViewModel>> GetHouseholds(long listingId)
    {
        var units = await _listingUnitHouseholdRepository.GetAll(listingId);
        return units.Select(s => new ListingUnitHouseholdViewModel()
        {
            Active = s.Active,
            HouseholdId = s.HouseholdId,
            HouseholdSize = s.HouseholdSize,
            MaxHouseholdIncomeAmt = s.MaxHouseholdIncomeAmt,
            MinHouseholdIncomeAmt = s.MinHouseholdIncomeAmt,
            UnitId = s.UnitId,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate
        });
    }

    public async Task<IEnumerable<AmenityViewModel>> GetAmenities(long listingId)
    {
        var amenities = await _listingAmenityRepository.GetAll(listingId);
        return amenities.Select(s => new AmenityViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            AmenityId = s.AmenityId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            UsageCount = s.UsageCount
        });
    }

    public async Task<IEnumerable<AmenityViewModel>> GetAmenitiesForEdit(long listingId)
    {
        var amenities = await _listingAmenityRepository.GetAllForEdit(listingId);
        return amenities.Select(s => new AmenityViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            AmenityId = s.AmenityId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            Selected = s.Selected,
            UsageCount = s.UsageCount
        });
    }

    public async Task<string> SaveAmenities(string correlationId, string username, long listingId, string amenityIds)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        var saved = await _listingAmenityRepository.Save(correlationId, username, listingId, amenityIds);
        if (!saved)
        {
            _logger.LogError($"Failed to update amenities for listing - Unknown error");
            return "L403";
        }

        return "";
    }

    public async Task<IEnumerable<FundingSourceViewModel>> GetFundingSources(long listingId)
    {
        var fundingSources = await _listingFundingSourceRepository.GetAll(listingId);
        return fundingSources.Select(s => new FundingSourceViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            FundingSourceId = s.FundingSourceId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            UsageCount = s.UsageCount
        });
    }

    public async Task<IEnumerable<FundingSourceViewModel>> GetFundingSourcesForEdit(long listingId)
    {
        var fundingSources = await _listingFundingSourceRepository.GetAllForEdit(listingId);
        return fundingSources.Select(s => new FundingSourceViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            FundingSourceId = s.FundingSourceId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            Selected = s.Selected,
            UsageCount = s.UsageCount
        });
    }

    public async Task<string> SaveFundingSources(string correlationId, string username, long listingId, string fundingSourceIds)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        var saved = await _listingFundingSourceRepository.Save(correlationId, username, listingId, fundingSourceIds);
        if (!saved)
        {
            _logger.LogError($"Failed to update funding sources for listing - Unknown error");
            return "L403";
        }

        return "";
    }

    public async Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypes(long listingId)
    {
        var documentTypes = await _listingDocumentTypeRepository.GetAll(listingId);
        return documentTypes.Select(s => new DocumentTypeViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            DocumentTypeId = s.DocumentTypeId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            UsageCount = s.UsageCount
        });
    }

    public async Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypesForEdit(long listingId)
    {
        var documentTypes = await _listingDocumentTypeRepository.GetAllForEdit(listingId);
        return documentTypes.Select(s => new DocumentTypeViewModel()
        {
            Active = s.Active,
            Description = s.Description,
            DocumentTypeId = s.DocumentTypeId,
            Name = s.Name,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            Selected = s.Selected,
            UsageCount = s.UsageCount
        });
    }

    public async Task<string> SaveDocumentTypes(string correlationId, string username, long listingId, string documentTypeIds)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        var saved = await _listingDocumentTypeRepository.Save(correlationId, username, listingId, documentTypeIds);
        if (!saved)
        {
            _logger.LogError($"Failed to update document types for listing - Unknown error");
            return "L403";
        }

        return "";
    }

    public async Task<Dictionary<string, string>> GetListingAccessibilities(long listingId)
    {
        var accessibilities = await _listingAccessibilityRepository.GetAll(listingId);
        return _uiHelperService.ToDictionary(accessibilities);
    }

    public async Task<string> SaveAccessibilities(string correlationId, string username, long listingId, string accessibilityCds)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        var saved = await _listingAccessibilityRepository.Save(correlationId, username, listingId, accessibilityCds);
        if (!saved)
        {
            _logger.LogError($"Failed to update accessibilities for listing - Unknown error");
            return "L503";
        }

        return "";
    }

    public async Task<IEnumerable<Declaration>> GetListingDeclarations(long listingId)
    {
        return await _listingDeclarationRepository.GetAll(listingId);
    }

    public async Task<Declaration> GetListingDeclaration(int declarationId)
    {
        return await _listingDeclarationRepository.GetOne(new Declaration() { DeclarationId = declarationId });
    }

    public async Task<string> AddDeclaration(string correlationId, string username, ListingDeclarationViewModel declaration)
    {
        declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));

        declaration.Text = declaration.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(declaration.Text))
        {
            _logger.LogError($"Unable to add listing declaration - Invalid Text");
            return "L611";
        }

        declaration.SortOrder = declaration.SortOrder < 0 ? 0 : declaration.SortOrder;

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = declaration.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing declaration - {declaration.ListingId}");
            return "L001";
        }

        var declarations = await _listingDeclarationRepository.GetAll(declaration.ListingId);
        var duplicateDeclaration = declarations.FirstOrDefault(f => f.Text.Equals(declaration.Text, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateDeclaration != null)
        {
            _logger.LogError($"Unable to add duplicate listing declaration - {declaration.Text}");
            return "L602";
        }

        declaration.CreatedBy = username;
        var added = await _listingDeclarationRepository.Add(correlationId, declaration);
        if (!added)
        {
            _logger.LogError($"Failed to add declaration for listing - Unknown error");
            return "L603";
        }

        return "";
    }

    public async Task<string> UpdateDeclaration(string correlationId, string username, ListingDeclarationViewModel declaration)
    {
        declaration = declaration ?? throw new ArgumentNullException(nameof(declaration));

        declaration.Text = declaration.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(declaration.Text))
        {
            _logger.LogError($"Unable to update listing declaration - Invalid Text");
            return "L611";
        }

        declaration.SortOrder = declaration.SortOrder < 0 ? 0 : declaration.SortOrder;

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = declaration.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing declaration - {declaration.ListingId}");
            return "L001";
        }

        var declarations = await _listingDeclarationRepository.GetAll(declaration.ListingId);
        var duplicateDeclaration = declarations.FirstOrDefault(f => f.DeclarationId != declaration.DeclarationId && f.Text.Equals(declaration.Text, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateDeclaration != null)
        {
            _logger.LogError($"Unable to update duplicate listing declaration - {declaration.Text}");
            return "L602";
        }

        declaration.ModifiedBy = username;
        var updated = await _listingDeclarationRepository.Update(correlationId, declaration);
        if (!updated)
        {
            _logger.LogError($"Failed to update declaration for listing - Unknown error");
            return "L604";
        }

        return "";
    }

    public async Task<string> DeleteDeclaration(string correlationId, string username, int id)
    {
        var existingDeclaration = await _listingDeclarationRepository.GetOne(new Declaration() { DeclarationId = id });
        if (existingDeclaration == null)
        {
            _logger.LogError($"Unable to find declaration - {id}");
            return "L601";
        }

        existingDeclaration.ModifiedBy = username;
        var deleted = await _listingDeclarationRepository.Delete(correlationId, existingDeclaration);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete declaration for listing - Unknown error");
            return "L605";
        }

        return "";
    }

    public async Task<IEnumerable<Disclosure>> GetListingDisclosures(long listingId)
    {
        return await _listingDisclosureRepository.GetAll(listingId);
    }

    public async Task<Disclosure> GetListingDisclosure(int disclosureId)
    {
        return await _listingDisclosureRepository.GetOne(new Disclosure() { DisclosureId = disclosureId });
    }

    public async Task<string> AddDisclosure(string correlationId, string username, ListingDisclosureViewModel disclosure)
    {
        disclosure = disclosure ?? throw new ArgumentNullException(nameof(disclosure));

        disclosure.Text = disclosure.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(disclosure.Text))
        {
            _logger.LogError($"Unable to add listing disclosure - Invalid Text");
            return "L711";
        }

        disclosure.SortOrder = disclosure.SortOrder < 0 ? 0 : disclosure.SortOrder;

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = disclosure.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing disclosure - {disclosure.ListingId}");
            return "L001";
        }

        var disclosures = await _listingDisclosureRepository.GetAll(disclosure.ListingId);
        var duplicateDisclosure = disclosures.FirstOrDefault(f => f.Text.Equals(disclosure.Text, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateDisclosure != null)
        {
            _logger.LogError($"Unable to add duplicate listing disclosure - {disclosure.Text}");
            return "L702";
        }

        disclosure.CreatedBy = username;
        var added = await _listingDisclosureRepository.Add(correlationId, disclosure);
        if (!added)
        {
            _logger.LogError($"Failed to add disclosure for listing - Unknown error");
            return "L703";
        }

        return "";
    }

    public async Task<string> UpdateDisclosure(string correlationId, string username, ListingDisclosureViewModel disclosure)
    {
        disclosure = disclosure ?? throw new ArgumentNullException(nameof(disclosure));

        disclosure.Text = disclosure.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(disclosure.Text))
        {
            _logger.LogError($"Unable to update listing disclosure - Invalid Text");
            return "L711";
        }

        disclosure.SortOrder = disclosure.SortOrder < 0 ? 0 : disclosure.SortOrder;

        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = disclosure.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing disclosure - {disclosure.ListingId}");
            return "L001";
        }

        var disclosures = await _listingDisclosureRepository.GetAll(disclosure.ListingId);
        var duplicateDisclosure = disclosures.FirstOrDefault(f => f.DisclosureId != disclosure.DisclosureId && f.Text.Equals(disclosure.Text, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateDisclosure != null)
        {
            _logger.LogError($"Unable to update duplicate listing disclosure - {disclosure.Text}");
            return "L702";
        }

        disclosure.ModifiedBy = username;
        var updated = await _listingDisclosureRepository.Update(correlationId, disclosure);
        if (!updated)
        {
            _logger.LogError($"Failed to update disclosure for listing - Unknown error");
            return "L704";
        }

        return "";
    }

    public async Task<string> DeleteDisclosure(string correlationId, string username, int id)
    {
        var existingDisclosure = await _listingDisclosureRepository.GetOne(new Disclosure() { DisclosureId = id });
        if (existingDisclosure == null)
        {
            _logger.LogError($"Unable to find disclosure - {id}");
            return "L701";
        }

        existingDisclosure.ModifiedBy = username;
        var deleted = await _listingDisclosureRepository.Delete(correlationId, existingDisclosure);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete disclosure for listing - Unknown error");
            return "L705";
        }

        return "";
    }

    public async Task<string> Submit(string requestId, string correlationId, string siteUrl, string username, long listingId)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        existingListing.ListingTypeCd = existingListing.ListingTypeCd?.Trim() ?? string.Empty;
        var listingTypes = await _metadataService.GetListingTypes();
        if (string.IsNullOrEmpty(existingListing.ListingTypeCd) || !listingTypes.ContainsKey(existingListing.ListingTypeCd))
        {
            _logger.LogError($"Unable to submit listing - Listing Type is required");
            return "L911";
        }

        if (existingListing.IsRental) existingListing.ResaleInd = false;

        existingListing.Name = existingListing.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.Name))
        {
            _logger.LogError($"Unable to submit listing - Name is required");
            return "L912";
        }

        existingListing.StreetLine1 = existingListing.StreetLine1?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.StreetLine1))
        {
            _logger.LogError($"Unable to submit listing - Street Line 1 is required");
            return "L913";
        }

        existingListing.City = existingListing.City?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.City))
        {
            _logger.LogError($"Unable to submit listing - City is required");
            return "L914";
        }

        existingListing.StateCd = existingListing.StateCd?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.StateCd))
        {
            _logger.LogError($"Unable to submit listing - State is required");
            return "L915";
        }

        existingListing.ZipCode = existingListing.ZipCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.ZipCode) || (existingListing.ZipCode.Length != 5 && existingListing.ZipCode.Length != 9))
        {
            _logger.LogError($"Unable to submit listing - Zip Code is required");
            return "L916";
        }

        existingListing.County = existingListing.County?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(existingListing.County))
        {
            _logger.LogError($"Unable to submit listing - County is required");
            return "L917";
        }

        var minDate = new DateTime(1990, 1, 1);

        var dateValue = existingListing.ListingStartDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            _logger.LogError($"Unable to submit listing - Listing Start Date is required");
            return "L918";
        }

        existingListing.ModifiedBy = username;
        existingListing.StatusCd = "REVIEW";
        var submitted = await _listingRepository.UpdateStatus(correlationId, existingListing, "Submitted listing for review.");
        if (!submitted)
        {
            _logger.LogError($"Failed to submit listing for review - Unknown error");
            return "L901";
        }

        var emailSent = await _emailService.SendListingPendingReviewEmail(correlationId, correlationId, siteUrl, username, existingListing);

        return "";
    }

    public async Task<string> Publish(string requestId, string correlationId, string siteUrl, string username, long listingId)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {listingId}");
            return "L001";
        }

        existingListing.ModifiedBy = username;
        existingListing.StatusCd = "PUBLISHED";
        var published = await _listingRepository.Publish(correlationId, existingListing, "Published listing.");
        if (!published)
        {
            _logger.LogError($"Failed to publish listing - Unknown error");
            return "L902";
        }

        var emailSent = await _emailService.SendListingPublishedEmail(correlationId, correlationId, siteUrl, username, existingListing);

        return "";
    }

    public async Task<string> Revise(string requestId, string correlationId, string siteUrl, string username, EditableListingActionViewModel model)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = model.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {model.ListingId}");
            return "L001";
        }

        model.Reason = (model.Reason ?? "").Trim();
        if (model.Reason.Length == 0) model.Reason = "No reason provided.";
        var note = $"Send listing back for revisions. {model.Reason}".Trim();
        if (!note.EndsWith(".")) note += ".";

        existingListing.ModifiedBy = username;
        existingListing.StatusCd = "REVISE";
        var published = await _listingRepository.UpdateStatus(correlationId, existingListing, note);
        if (!published)
        {
            _logger.LogError($"Failed to send listing back for revisions - Unknown error");
            return "L903";
        }

        var emailSent = await _emailService.SendListingRequiresRevisionsEmail(correlationId, correlationId, siteUrl, username, existingListing, model.Reason);

        return "";
    }

    public async Task<string> Unpublish(string requestId, string correlationId, string siteUrl, string username, EditableListingActionViewModel model)
    {
        var existingListing = await _listingRepository.GetOne(new Listing() { ListingId = model.ListingId });
        if (existingListing == null)
        {
            _logger.LogError($"Unable to find listing - {model.ListingId}");
            return "L001";
        }

        model.Reason = (model.Reason ?? "").Trim();
        if (model.Reason.Length == 0) model.Reason = "No reason provided.";
        var note = $"Unpublished listing. {model.Reason}".Trim();
        if (!note.EndsWith(".")) note += ".";

        existingListing.ModifiedBy = username;
        existingListing.StatusCd = "REVISE";
        var published = await _listingRepository.Unpublish(correlationId, existingListing, "Unpublished listing.");
        if (!published)
        {
            _logger.LogError($"Failed to send published listing back for revisions - Unknown error");
            return "L904";
        }

        var emailSent = await _emailService.SendListingUnpublishedEmail(correlationId, correlationId, siteUrl, username, existingListing, model.Reason);

        return "";
    }

    public async Task<PrintableFormViewModel> GetPrintableForm(long listingId)
    {
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        var model = new PrintableFormViewModel
        {
            Active = listing.Active,
            ApplicationStartDate = listing.ApplicationStartDate,
            ApplicationEndDate = listing.ApplicationEndDate,
            City = listing.City,
            County = listing.County,
            CompletedOrInitialOccupancyYear = listing.CompletedOrInitialOccupancyYear,
            CreatedBy = listing.CreatedBy,
            CreatedDate = listing.CreatedDate,
            Description = listing.Description,
            EsriX = listing.EsriX,
            EsriY = listing.EsriY,
            ListingEndDate = listing.ListingEndDate,
            ListingId = listing.ListingId,
            ListingStartDate = listing.ListingStartDate,
            ListingTypeCd = listing.ListingTypeCd,
            ListingTypeDescription = listing.ResaleInd ? "Resale" : listing.ListingTypeDescription,
            LotteryDate = listing.LotteryDate,
            MapUrl = listing.MapUrl,
            MaxHouseholdSize = listing.MaxHouseholdSize,
            MaxHouseholdIncomeAmt = listing.MaxHouseholdIncomeAmt,
            MinHouseholdSize = listing.MinHouseholdSize,
            MinHouseholdIncomeAmt = listing.MinHouseholdIncomeAmt,
            ModifiedBy = listing.ModifiedBy,
            ModifiedDate = listing.ModifiedDate,
            Municipality = listing.Municipality,
            MunicipalityUrl = listing.MunicipalityUrl,
            Name = listing.Name,
            PetsAllowedInd = listing.PetsAllowedInd,
            PetsAllowedText = listing.PetsAllowedText,
            PublishedVersionNo = listing.PublishedVersionNo,
            RentIncludesText = listing.RentIncludesText,
            ResaleInd = listing.ResaleInd,
            SchoolDistrict = listing.SchoolDistrict,
            SchoolDistrictUrl = listing.SchoolDistrictUrl,
            StateCd = listing.StateCd,
            StatusCd = listing.StatusCd,
            StatusDescription = listing.StatusDescription,
            StreetLine1 = listing.StreetLine1,
            StreetLine2 = listing.StreetLine2,
            StreetLine3 = listing.StreetLine3,
            TermOfAffordability = listing.TermOfAffordability,
            VersionNo = listing.VersionNo,
            WaitlistEligible = listing.WaitlistEligible,
            WaitlistEndDate = listing.WaitlistEndDate,
            WaitlistStartDate = listing.WaitlistStartDate,
            WebsiteUrl = listing.WebsiteUrl,
            ZipCode = listing.ZipCode,

            UnitTypes = await _metadataService.GetUnitTypes(),
            VoucherTypes = await _metadataService.GetVoucherTypes(),
            RelationTypes = await _metadataService.GetRelationTypes(),
            RaceTypes = await _metadataService.GetRaceTypes(),
            EthnicityTypes = await _metadataService.GetEthnicityTypes(),
            LeadTypes = await _metadataService.GetLeadTypes()
        };
        return model;
    }

    public async Task<string> AddOld(string correlationId, string username, ListingViewModel listing)
    {
        listing = listing ?? throw new ArgumentNullException(nameof(listing));

        listing.ListingTypeCd = listing.ListingTypeCd?.Trim() ?? string.Empty;
        var listingTypes = await _metadataService.GetListingTypes();
        if (string.IsNullOrEmpty(listing.ListingTypeCd) || !listingTypes.ContainsKey(listing.ListingTypeCd))
        {
            _logger.LogError($"Unable to add listing - Invalid Listing Type");
            return "L101";
        }

        listing.Name = listing.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Name))
        {
            _logger.LogError($"Unable to add listing - Invalid Name");
            return "L102";
        }

        listing.Description = listing.Description?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.Description)) listing.Description = null;

        listing.StreetLine1 = listing.StreetLine1?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine1))
        {
            _logger.LogError($"Unable to add listing - Invalid Street Line 1");
            return "L103";
        }

        listing.StreetLine2 = listing.StreetLine2?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine2)) listing.StreetLine2 = null;

        listing.StreetLine3 = listing.StreetLine3?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StreetLine3)) listing.StreetLine3 = null;

        listing.City = listing.City?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.City))
        {
            _logger.LogError($"Unable to add listing - Invalid City");
            return "L104";
        }

        listing.StateCd = listing.StateCd?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.StateCd))
        {
            _logger.LogError($"Unable to add listing - Invalid State");
            return "L105";
        }

        listing.ZipCode = listing.ZipCode?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.ZipCode) || (listing.ZipCode.Length != 5 && listing.ZipCode.Length != 9))
        {
            _logger.LogError($"Unable to add listing - Invalid Zip Code");
            return "L106";
        }

        listing.County = listing.County?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(listing.County))
        {
            _logger.LogError($"Unable to add listing - Invalid County");
            return "L107";
        }

        var minDate = new DateTime(1990, 1, 1);

        var dateValue = listing.ListingStartDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ListingStartDate = null;
        }

        dateValue = listing.ListingEndDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ListingEndDate = null;
        }

        if (listing.ListingStartDate.HasValue && listing.ListingEndDate.HasValue && listing.ListingEndDate.Value < listing.ListingStartDate.Value)
        {
            _logger.LogError($"Unable to add listing - Invalid Listing End Date");
            return "L108";
        }

        dateValue = listing.ApplicationStartDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ApplicationStartDate = null;
        }

        dateValue = listing.ApplicationEndDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.ApplicationEndDate = null;
        }

        if (listing.ListingStartDate.HasValue && listing.ApplicationStartDate.HasValue && listing.ApplicationStartDate.Value < listing.ListingStartDate.Value)
        {
            _logger.LogError($"Unable to add listing - Invalid Application Start Date");
            return "L110";
        }

        if (listing.ApplicationStartDate.HasValue && listing.ApplicationEndDate.HasValue && listing.ApplicationEndDate.Value < listing.ApplicationStartDate.Value)
        {
            _logger.LogError($"Unable to add listing - Invalid Application End Date");
            return "L110";
        }

        dateValue = listing.LotteryDate.GetValueOrDefault(DateTime.MinValue);
        if (dateValue < minDate)
        {
            listing.LotteryDate = null;
        }

        if (listing.ApplicationEndDate.HasValue && listing.LotteryDate.HasValue && listing.LotteryDate.Value < listing.ApplicationEndDate.Value)
        {
            _logger.LogError($"Unable to add listing - Invalid Lottery Date");
            return "L111";
        }

        if (listing.WaitlistEligible)
        {
            dateValue = listing.WaitlistStartDate.GetValueOrDefault(DateTime.MinValue);
            if (dateValue < minDate)
            {
                listing.WaitlistStartDate = null;
            }

            dateValue = listing.WaitlistEndDate.GetValueOrDefault(DateTime.MinValue);
            if (dateValue < minDate)
            {
                listing.WaitlistEndDate = null;
            }

            if (listing.ApplicationEndDate.HasValue && listing.WaitlistStartDate.HasValue && listing.WaitlistStartDate < listing.ApplicationEndDate)
            {
                _logger.LogError($"Unable to add listing - Invalid Waitlist Start Date");
                return "L112";
            }

            if (listing.WaitlistStartDate.HasValue && listing.WaitlistEndDate.HasValue && listing.WaitlistEndDate.Value < listing.WaitlistStartDate.Value)
            {
                _logger.LogError($"Unable to add listing - Invalid Waitlist End Date");
                return "L113";
            }
        }
        else
        {
            listing.WaitlistStartDate = null;
            listing.WaitlistEndDate = null;
        }

        if (listing.MinHouseholdIncomeAmt < 0L) listing.MinHouseholdIncomeAmt = 0L;
        if (listing.MaxHouseholdIncomeAmt < 0L) listing.MaxHouseholdIncomeAmt = 0L;
        if (listing.MinHouseholdIncomeAmt > listing.MaxHouseholdIncomeAmt)
        {
            _logger.LogError($"Unable to add listing - Invalid Min/Max. Household Income Amount");
            return "L117";
        }

        if (listing.MinHouseholdSize < 0) listing.MinHouseholdSize = 0;
        if (listing.MaxHouseholdSize < 0) listing.MaxHouseholdSize = 0;
        if (listing.MinHouseholdSize > listing.MaxHouseholdSize)
        {
            _logger.LogError($"Unable to add listing - Invalid Min/Max. Household Size");
            return "L118";
        }

        var listings = await _listingRepository.GetAll();

        var duplicateListing = listings.FirstOrDefault(f => f.Name == listing.Name && f.City == listing.City && f.StateCd == listing.StateCd && f.ZipCode == listing.ZipCode);
        if (duplicateListing != null)
        {
            _logger.LogError($"Unable to add duplicate listing - {listing.Name}");
            return "L002";
        }

        listing.StatusCd = "DRAFT";
        listing.CreatedBy = username;
        var added = await _listingRepository.Add(correlationId, listing);
        if (!added)
        {
            return "L003";
        }

        return "";
    }

    public async Task<IEnumerable<ListingViewModel>> GetPublishedListings(string requestId, string correlationId, string username)
    {
        var listings = await _listingRepository.GetPublishedListings();
        return listings.Select(s => s.ToViewModel());
    }

    public async Task<ListingViewModel> GetPublishedListing(string requestId, string correlationId, string username, long listingId, bool withDetails = false)
    {
        var userRole = await _usersService.GetUserRole(correlationId, username);
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });

        if (listing == null) return null;
        var model = listing.ToViewModel();
        model.CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|") && "|DRAFT|REVISE|".Contains($"|{listing.StatusCd}|");
        model.CanPublish = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|") && "|REVIEW|".Contains($"|{listing.StatusCd}|");
        model.CanUnpublish = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|") && "|PUBLISHED|".Contains($"|{listing.StatusCd}|");

        if (withDetails)
        {
            model.Images = await GetImages(listingId);
            model.Documents = await GetDocuments(listingId);
            model.Units = await GetUnits(listingId);
            model.Households = await GetHouseholds(listingId);
            model.Amenities = await GetAmenities(listingId);
            model.Accessibilities = await GetListingAccessibilities(listingId);
            model.Declarations = await GetListingDeclarations(listingId);
            model.Disclosures = await GetListingDisclosures(listingId);
            model.DocumentTypes = await GetDocumentTypes(listingId);
            model.FundingSources = await GetFundingSources(listingId);
        }

        // TODO :: Determine logic for paper application submissions
        model.CanSubmitPaperApplication = true;

        return model;
    }

    public async Task<AffordabilityAnalysisViewModel> GetForAffordabilityAnalysis(string requestId, string correlationId, long listingId)
    {
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (listing == null) return null;

        var units = await GetUnits(listingId);
        if ((units?.Count() ?? 0) == 0) return null;
        units = units.OrderBy(o => o.BedroomCnt);

        var unitSelector = new Dictionary<int, string>
        {
            { 0, "Select One" }
        };
        foreach (var unit in units)
        {
            unitSelector.Add(unit.UnitId, $"{unit.UnitTypeDescription} - ${unit.EstimatedPriceAmt}");
        }

        var maxRate = 7m;
        var rates = new Dictionary<decimal, string>();
        var amortizations = await _amortizationsService.GetAll(requestId, correlationId);
        if ((amortizations?.Count() ?? 0) > 0)
        {
            foreach (var amortization in amortizations)
            {
                rates.Add(amortization.Rate, $"{amortization.Rate} %");
                maxRate = Math.Max(amortization.Rate, maxRate);
            }
        }
        else
        {
            rates.Add(7m, "7 %");
        }

        var terms = new Dictionary<int, string>
        {
            { 10, "10 Years" },
            { 15, "15 Years" },
            { 20, "20 Years" },
            { 25, "25 Years" },
            { 30, "30 Years" },
            { 40, "40 Years" }
        };

        return new AffordabilityAnalysisViewModel()
        {
            ListingId = listingId,
            MortgageRatePct = maxRate,
            MortgageRates = rates,
            MortgageTerm = 30,
            MortgageTerms = terms,
            UnitId = 0,
            Units = unitSelector
        };
    }

    public async Task<ListingUnitViewModel> GetAffordabilityAnalysis(string requestId, string correlationId, int unitId, decimal rate, int term)
    {
        var unit = await GetUnit(unitId);
        if (unit == null) return null;

        var amortization = await _amortizationsService.GetOne(requestId, correlationId, rate);
        if (amortization == null) return null;

        unit.NetSalePriceAmt = unit.EstimatedPriceAmt - unit.SubsidyAmt;
        unit.DownPaymentAmt = (long)Math.Truncate(unit.NetSalePriceAmt * 0.05);
        unit.MortgageAmt = unit.NetSalePriceAmt - unit.DownPaymentAmt;

        unit.MortgageRatePct = rate;
        unit.MortgageTerm = term;
        unit.LoanRatePct = term switch
        {
            10 => amortization.Rate10Year,
            15 => amortization.Rate15Year,
            20 => amortization.Rate20Year,
            25 => amortization.Rate25Year,
            30 => amortization.Rate30Year,
            40 => amortization.Rate40Year,
            _ => 6.65302m,
        };

        unit.MonthlyPrincipalInterestAmt = (long)Math.Ceiling(unit.MortgageAmt / 1000m * unit.LoanRatePct);
        unit.MonthlyPmiAmt = (long)Math.Ceiling(unit.MortgageAmt * 0.0078m / 12);
        unit.MonthlyPaymentAmt = unit.MonthlyPrincipalInterestAmt + unit.MonthlyTaxesAmt + unit.MonthlyMaintenanceAmt + unit.MonthlyInsuranceAmt + unit.MonthlyPmiAmt;

        return unit;
    }
}