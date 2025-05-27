using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IListingService
{
    Task<ListingsViewModel> GetData(string requestId, string correlationId);
    Task<ListingsViewModel> GetDataPaged(string requestId, string correlationId, int pageNo = 1, int pageSize = 12);
    Task<ListingViewModel> GetOne(string requestId, string correlationId, int listingId, string username = null);
    Task<ListingsViewModel> Search(string requestId, string correlationId, ListingSearchViewModel searchModel);
    Task<IEnumerable<ListingDocumentViewModel>> GetDocuments(string requestId, string correlationId, int listingId);
    Task<ListingDocumentViewModel> GetDocument(string requestId, string correlationId, long documentId);
    Task<IEnumerable<ListingImageViewModel>> GetImages(string requestId, string correlationId, int listingId);
    Task<IEnumerable<ListingUnitViewModel>> GetUnits(string requestId, string correlationId, int listingId);
    Task<ListingUnitViewModel> GetUnit(string requestId, string correlationId, long unitId);
    Task<IEnumerable<ListingUnitHouseholdViewModel>> GetHouseholds(string requestId, string correlationId, int listingId);
    Task<IEnumerable<AmenityViewModel>> GetAmenities(string requestId, string correlationId, int listingId);
    Task<Dictionary<string, string>> GetAccessibilities(string requestId, string correlationId, int listingId);
    Task<IEnumerable<Declaration>> GetDeclarations(string requestId, string correlationId, int listingId);
    Task<IEnumerable<Disclosure>> GetDisclosures(string requestId, string correlationId, int listingId);
    Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypes(string requestId, string correlationId, int listingId);
    Task<IEnumerable<FundingSourceViewModel>> GetFundingSources(string requestId, string correlationId, int listingId);
    Task<PrintableFormViewModel> GetPrintableForm(string requestId, string correlationId, int listingId);

    Task<AffordabilityAnalysisViewModel> GetForAffordabilityAnalysis(string requestId, string correlationId, int listingId);
    Task<ListingUnitViewModel> GetAffordabilityAnalysis(string requestId, string correlationId, int unitId, decimal rate, int term);
}

public class ListingService : IListingService
{
    private readonly ILogger<ListingService> _logger;
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
    private readonly IHousingApplicationRepository _applicationRepository;
    private readonly IUiHelperService _uiHelperService;
    private readonly IMetadataService _metadataService;
    private readonly IAmortizationsService _amortizationsService;

    public ListingService(ILogger<ListingService> logger, IListingRepository listingRepository
                            , IListingImageRepository listingImageRepository
                            , IListingUnitRepository listingUnitRepository
                            , IListingUnitHouseholdRepository listingUnitHouseholdRepository
                            , IListingAmenityRepository listingAmenityRepository
                            , IListingAccessibilityRepository listingAccessibilityRepository
                            , IListingDeclarationRepository listingDeclarationRepository
                            , IListingDisclosureRepository listingDisclosureRepository
                            , IListingDocumentRepository listingDocumentRepository
                            , IListingDocumentTypeRepository listingDocumentTypeRepository
                            , IListingFundingSourceRepository listingFundingSourceRepository
                            , IHousingApplicationRepository applicationRepository
                            , IUiHelperService uiHelperService, IMetadataService metadataService
                            , IAmortizationsService amortizationsService)
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
        _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _amortizationsService = amortizationsService ?? throw new ArgumentNullException(nameof(amortizationsService));
    }

    public async Task<ListingsViewModel> GetData(string requestId, string correlationId)
    {
        var listings = await _listingRepository.GetAll() ?? [];
        var viewModels = listings.Select(s => s.ToViewModel());
        var model = new ListingsViewModel()
        {
            Listings = viewModels,
            RentalListings = viewModels.Where(w => w.IsRental),
            SaleListings = viewModels.Where(w => !w.IsRental)
        };
        await AssignMetadata(model, true);
        return model;
    }

    public async Task<ListingsViewModel> GetDataPaged(string requestId, string correlationId, int pageNo = 1, int pageSize = 12)
    {
        var rentalListings = await _listingRepository.GetAllPaged("RENTAL", pageNo, pageSize);
        var saleListings = await _listingRepository.GetAllPaged("SALE", pageNo, pageSize);
        var model = new ListingsViewModel()
        {
            RentalListings = rentalListings.Records.Select(s => s.ToViewModel()),
            SaleListings = saleListings.Records.Select(s => s.ToViewModel()),
            PageNo = rentalListings.PagingInfo.PageNo,
            PageSize = rentalListings.PagingInfo.PageSize,
            TotalPages = rentalListings.PagingInfo.TotalPages,
            TotalRecords = rentalListings.PagingInfo.TotalRecords,
            PageSizes = [12, 24, 36, 48, 60]
        };
        await AssignMetadata(model, true);
        return model;
    }

    async Task AssignMetadata(ListingsViewModel model, bool initSearch = false)
    {
        model ??= new ListingsViewModel();
        if (initSearch)
        {
            model.AdaptedForDisabilityOptionCd = "ALL";
            model.CityOptionCd = "ALL";
            model.ListingDateFilterOptionCd = "ALL";
            model.ListingTypeOptionCd = "BOTH";
            model.PetsAllowedOptionCd = "ALL";
            model.SeniorLivingOptionCd = "NO";
            model.Searched = false;
        }

        model.AdaptedForDisabilityOptions = _metadataService.GetAdaptedForDisabilitySearchOptions() ?? [];
        model.CityOptions = await _metadataService.GetCitySearchOptions() ?? [];
        model.ListingDateFilterOptions = await _metadataService.GetListingDateTypeSearchOptions() ?? [];
        model.ListingTypeOptions = await _metadataService.GetListingTypeSearchOptions() ?? [];
        model.PetsAllowedOptions = await _metadataService.GetPetsAllowedSearchOptions() ?? [];
        model.SeniorLivingOptions = await _metadataService.GetSeniorLivingSearchOptions() ?? [];

        model.CurrentFilters = [];
        model.CurrentFilters.Add("Listing Type", model.ListingTypeOptions.FirstOrDefault(f => f.Key.Equals(model.ListingTypeOptionCd)).Value ?? "Rentals & Sales");
        model.CurrentFilters.Add("City", (model.CityOptionCd ?? "ALL").Trim() == "ALL" ? "All Cities" : model.CityOptionCd);
        model.CurrentFilters.Add("Age Restricted Living", model.SeniorLivingOptions.FirstOrDefault(f => f.Key.Equals(model.SeniorLivingOptionCd)).Value ?? "No");
        //model.CurrentFilters.Add("Adapted for Disabilities", SetYesNoDisplayValue(model.AdaptedForDisabilityOptionCd, "Adapted for Disabilities", "Not Adapted for Disabilities", "All including Adapted for Disabilities"));
        model.CurrentFilters.Add("Pets Allowed", model.PetsAllowedOptions.FirstOrDefault(f => f.Key.Equals(model.PetsAllowedOptionCd)).Value ?? "Yes");
        model.CurrentFilters.Add("Listing Date Filter", model.ListingDateFilterOptions.FirstOrDefault(f => f.Key.Equals(model.ListingDateFilterOptionCd)).Value ?? "Current Opportunities");
    }

    string SetYesNoDisplayValue(string optionCd, string yesValue, string noValue, string defaultValue)
    {
        optionCd = (optionCd ?? "").Trim().ToUpper();
        var displayValue = defaultValue;
        switch (optionCd)
        {
            case "YES": displayValue = yesValue; break;
            case "NO": displayValue = noValue; break;
        }
        return displayValue;
    }

    public async Task<ListingViewModel> GetOne(string requestId, string correlationId, int listingId, string username = null)
    {
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        var model = listing.ToViewModel();

        if (model != null)
        {
            model.AddressText = _uiHelperService.ToAddressTextSingleLine(model);
            model.Images = await GetImages(requestId, correlationId, listingId);
            model.Documents = await GetDocuments(requestId, correlationId, listingId);
            model.Units = await GetUnits(requestId, correlationId, listingId);
            model.Households = await GetHouseholds(requestId, correlationId, listingId);
            model.Amenities = await GetAmenities(requestId, correlationId, listingId);
            model.Accessibilities = await GetAccessibilities(requestId, correlationId, listingId);
            model.Declarations = await GetDeclarations(requestId, correlationId, listingId);
            model.Disclosures = await GetDisclosures(requestId, correlationId, listingId);
            model.DocumentTypes = await GetDocumentTypes(requestId, correlationId, listingId);
            model.FundingSources = await GetFundingSources(requestId, correlationId, listingId);
        }

        username = (username ?? "").Trim();
        if (!string.IsNullOrEmpty(username))
        {
            var applications = await _applicationRepository.GetAllByListing(requestId, correlationId, username, listingId);
            var eligibleApplications = applications?.Where(w => !(w.StatusCd ?? "").Trim().Equals("WITHDRAWN", StringComparison.CurrentCultureIgnoreCase));
            if (eligibleApplications?.Count() == 1)
            {
                model.Application = eligibleApplications.First().ToViewModel();
            }
        }

        return model;
    }

    public async Task<ListingsViewModel> Search(string requestId, string correlationId, ListingSearchViewModel searchModel)
    {
        searchModel.Searched = false;
        searchModel.AdaptedForDisabilityOptionCd = (searchModel.AdaptedForDisabilityOptionCd ?? "ALL").Trim();
        searchModel.CityOptionCd = (searchModel.CityOptionCd ?? "ALL").Trim();
        searchModel.ListingDateFilterOptionCd = (searchModel.ListingDateFilterOptionCd ?? "ALL").Trim();
        searchModel.ListingTypeOptionCd = (searchModel.ListingTypeOptionCd ?? "BOTH").Trim();
        searchModel.PetsAllowedOptionCd = (searchModel.PetsAllowedOptionCd ?? "ALL").Trim();
        searchModel.SeniorLivingOptionCd = (searchModel.SeniorLivingOptionCd ?? "ALL").Trim();

        var listings = await _listingRepository.Search(searchModel) ?? [];
        var viewModels = listings.Select(s => s.ToViewModel());

        var model = new ListingsViewModel()
        {
            Listings = viewModels,
            RentalListings = viewModels.Where(w => w.IsRental),
            SaleListings = viewModels.Where(w => !w.IsRental),
            Searched = true,

            AdaptedForDisabilityOptionCd = searchModel.AdaptedForDisabilityOptionCd,
            CityOptionCd = searchModel.CityOptionCd,
            ListingDateFilterOptionCd = searchModel.ListingDateFilterOptionCd,
            ListingTypeOptionCd = searchModel.ListingTypeOptionCd,
            PetsAllowedOptionCd = searchModel.PetsAllowedOptionCd,
            SeniorLivingOptionCd = searchModel.SeniorLivingOptionCd
        };
        await AssignMetadata(model);
        return model;
    }

    public async Task<IEnumerable<ListingDocumentViewModel>> GetDocuments(string requestId, string correlationId, int listingId)
    {
        var documents = await _listingDocumentRepository.GetAll(listingId) ?? [];
        return documents.Select(s => s.ToViewModel());
    }

    public async Task<ListingDocumentViewModel> GetDocument(string requestId, string correlationId, long documentId)
    {
        var document = await _listingDocumentRepository.GetOne(documentId);
        return document.ToViewModel();
    }

    public async Task<IEnumerable<ListingImageViewModel>> GetImages(string requestId, string correlationId, int listingId)
    {
        var images = await _listingImageRepository.GetAll(listingId) ?? [];
        return images.Select(s => s.ToViewModel());
    }

    public async Task<IEnumerable<ListingUnitViewModel>> GetUnits(string requestId, string correlationId, int listingId)
    {
        var units = await _listingUnitRepository.GetAll(listingId) ?? [];
        return units.Select(s => s.ToViewModel());
    }

    public async Task<ListingUnitViewModel> GetUnit(string requestId, string correlationId, long unitId)
    {
        var unit = await _listingUnitRepository.GetOne(unitId);
        return unit.ToViewModel();
    }

    public async Task<IEnumerable<ListingUnitHouseholdViewModel>> GetHouseholds(string requestId, string correlationId, int listingId)
    {
        var units = await _listingUnitHouseholdRepository.GetAll(listingId) ?? [];
        return units.Select(s => s.ToViewModel());
    }

    public async Task<IEnumerable<AmenityViewModel>> GetAmenities(string requestId, string correlationId, int listingId)
    {
        var amenities = await _listingAmenityRepository.GetAll(listingId) ?? [];
        return amenities.Select(s => s.ToViewModel());
    }

    public async Task<Dictionary<string, string>> GetAccessibilities(string requestId, string correlationId, int listingId)
    {
        var accessibilities = await _listingAccessibilityRepository.GetAll(listingId) ?? [];
        return _uiHelperService.ToDictionary(accessibilities) ?? [];
    }

    public async Task<IEnumerable<DocumentTypeViewModel>> GetDocumentTypes(string requestId, string correlationId, int listingId)
    {
        var documentTypes = await _listingDocumentTypeRepository.GetAll(listingId) ?? [];
        return documentTypes.Select(s => s.ToViewModel());
    }

    public async Task<IEnumerable<Declaration>> GetDeclarations(string requestId, string correlationId, int listingId)
    {
        return await _listingDeclarationRepository.GetAll(listingId) ?? [];
    }

    public async Task<IEnumerable<Disclosure>> GetDisclosures(string requestId, string correlationId, int listingId)
    {
        return await _listingDisclosureRepository.GetAll(listingId) ?? [];
    }

    public async Task<IEnumerable<FundingSourceViewModel>> GetFundingSources(string requestId, string correlationId, int listingId)
    {
        var fundingSources = await _listingFundingSourceRepository.GetAll(listingId) ?? [];
        return fundingSources.Select(s => s.ToViewModel());
    }

    public async Task<PrintableFormViewModel> GetPrintableForm(string requestId, string correlationId, int listingId)
    {
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        var model = listing.ToPrintableViewModel();

        if (model != null)
        {
            model.UnitTypes = await _metadataService.GetUnitTypes() ?? [];
            model.VoucherTypes = await _metadataService.GetVoucherTypes() ?? [];
            model.RelationTypes = await _metadataService.GetRelationTypes() ?? [];
            model.RaceTypes = await _metadataService.GetRaceTypes() ?? [];
            model.EthnicityTypes = await _metadataService.GetEthnicityTypes() ?? [];
            model.LeadTypes = await _metadataService.GetLeadTypes() ?? [];

            model.Images = await GetImages(requestId, correlationId, listingId) ?? [];
            if ((model.Images?.Count() ?? 0) > 0)
            {
                var image = model.Images.FirstOrDefault(f => f.IsPrimary) ?? model.Images.First();
                model.ImageContents = (image.ThumbnailContents?.Length ?? 0) > 0 ? image.ThumbnailContents : image.Contents;
            }
            else
            {
                model.ImageContents = "/images/listings-default-image.png";
            }

            model.Accessibilities = await GetAccessibilities(requestId, correlationId, listingId) ?? [];
            model.Declarations = await GetDeclarations(requestId, correlationId, listingId) ?? [];
            model.Disclosures = await GetDisclosures(requestId, correlationId, listingId) ?? [];
            model.DocumentTypes = await GetDocumentTypes(requestId, correlationId, listingId) ?? [];
            model.FundingSources = await GetFundingSources(requestId, correlationId, listingId) ?? [];

            model.PetDisclosure = model.Disclosures?.FirstOrDefault(f => f.Code.Equals("DISCPETYES", StringComparison.CurrentCultureIgnoreCase)
                                                                            || f.Code.Equals("DISCPETNO", StringComparison.CurrentCultureIgnoreCase))?.Text;
            model.Disclosures = model.Disclosures?.Where(w => !w.Code.Equals("DISCPETYES", StringComparison.CurrentCultureIgnoreCase)
                                                                && !w.Code.Equals("DISCPETNO", StringComparison.CurrentCultureIgnoreCase));

            model.AddressText = _uiHelperService.ToAddressTextSingleLine(model);
            model.ApplicationEndDateText = _uiHelperService.ToDateTimeDisplayText(model.ApplicationEndDate, "MMMM d, yyyy", "t");
            model.LotteryDateText = _uiHelperService.ToDateTimeDisplayText(model.LotteryDate, "MMMM d, yyyy", "t");
        }
        return model;
    }

    public async Task<AffordabilityAnalysisViewModel> GetForAffordabilityAnalysis(string requestId, string correlationId, int listingId)
    {
        var listing = await _listingRepository.GetOne(new Listing() { ListingId = listingId });
        if (listing == null) return null;

        var units = await GetUnits(requestId, correlationId, listingId);
        if ((units?.Count() ?? 0) == 0) return null;
        units = units.OrderBy(o => o.BedroomCnt);

        var unitSelector = new Dictionary<long, string>
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
        var unit = await GetUnit(requestId, correlationId, unitId);
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