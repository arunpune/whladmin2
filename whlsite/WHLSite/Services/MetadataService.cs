using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;

namespace WHLSite.Services;

public interface IMetadataService
{
    Task<IEnumerable<CodeDescription>> GetAll(int codeId = 0);
    Task<Dictionary<string, string>> GetAccountTypes(bool forUi = false);
    Dictionary<string, string> GetAdaptedForDisabilitySearchOptions();
    Task<Dictionary<string, string>> GetAllowedFileTypes();
    Task<Dictionary<string, string>> GetCitySearchOptions();
    Task<Dictionary<string, string>> GetCounties(bool forUi = false);
    Task<Dictionary<string, string>> GetCountiesByStateCd(string stateCd, bool forUi = false);
    Task<Dictionary<string, string>> GetDocumentTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetEthnicityTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetGenderTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetIdTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetLanguages(bool forUi = false);
    Task<Dictionary<string, string>> GetLeadTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetListingDateTypeSearchOptions(bool forUi = false);
    Task<Dictionary<string, string>> GetListingTypeSearchOptions();
    Task<Dictionary<string, string>> GetListingTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetLoginHelpTypes();
    Task<Dictionary<string, string>> GetPetsAllowedSearchOptions(bool forUi = false);
    Task<Dictionary<string, string>> GetPhoneNumberTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetRaceTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetRelationTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetSeniorLivingSearchOptions(bool forUi = false);
    Task<Dictionary<string, string>> GetStatuses(bool forUi = false);
    Task<Dictionary<string, string>> GetUnitTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetUsStates(bool forUi = false);
    Task<Dictionary<string, string>> GetVoucherTypes(bool forUi = false);
}

public class MetadataService : IMetadataService
{
    private readonly ILogger<MetadataService> _logger;
    private readonly IMetadataRepository _metadataRepository;
    private readonly IUiHelperService _uiHelperService;
    private IEnumerable<CodeDescription> _metadata;
    private IEnumerable<CodeDescription> _accountTypes;
    private Dictionary<string, string> _adaptedForDisabilitySearchOptions;
    private IEnumerable<CodeDescription> _allowedFileTypes;
    private IEnumerable<CodeDescription> _counties;
    private IEnumerable<CodeDescription> _documentTypes;
    private IEnumerable<CodeDescription> _ethnicityTypes;
    private IEnumerable<CodeDescription> _genderTypes;
    private IEnumerable<CodeDescription> _idTypes;
    private IEnumerable<CodeDescription> _languageTypes;
    private IEnumerable<CodeDescription> _leadTypes;
    private IEnumerable<CodeDescription> _listingDateTypeSearchOptions;
    private IEnumerable<CodeDescription> _listingTypes;
    private IEnumerable<CodeDescription> _listingTypeSearchOptions;
    private readonly Dictionary<string, string> _loginHelpTypes;
    private IEnumerable<CodeDescription> _petsAllowedSearchOptions;
    private IEnumerable<CodeDescription> _phoneNumberTypes;
    private IEnumerable<CodeDescription> _raceTypes;
    private IEnumerable<CodeDescription> _relationTypes;
    private IEnumerable<CodeDescription> _seniorLivingSearchOptions;
    private IEnumerable<CodeDescription> _statuses;
    private IEnumerable<CodeDescription> _unitTypes;
    private IEnumerable<CodeDescription> _usStates;
    private IEnumerable<CodeDescription> _voucherTypes;

    public MetadataService(ILogger<MetadataService> logger, IMetadataRepository metadataRepository, IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _metadataRepository = metadataRepository ?? throw new ArgumentNullException(nameof(metadataRepository));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));

        _adaptedForDisabilitySearchOptions = new Dictionary<string, string>
        {
            { "", "All" },
            { "YES", "Yes" },
            { "NO", "No" }
        };

        _loginHelpTypes = new Dictionary<string, string>
        {
            { "PWD", "Reset Password" },
            { "USR", "Retrieve Username" }
        };
    }

    public async Task<IEnumerable<CodeDescription>> GetAll(int codeId = 0)
    {
        _metadata ??= await _metadataRepository.GetAll();

        if (codeId > 0) return _metadata.Where(w => w.CodeId == codeId);

        return _metadata;
    }

    public async Task<Dictionary<string, string>> GetAccountTypes(bool forUi = false)
    {
        _accountTypes ??= await GetAll(124);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_accountTypes.OrderBy(o => o.MetadataId), "", "Select Account Type");
        }
        return _uiHelperService.ToDictionary(_accountTypes.OrderBy(o => o.MetadataId));
    }

    public Dictionary<string, string> GetAdaptedForDisabilitySearchOptions()
    {
        return _adaptedForDisabilitySearchOptions;
    }

    public async Task<Dictionary<string, string>> GetAllowedFileTypes()
    {
        _allowedFileTypes ??= await GetAll(131);
        return _uiHelperService.ToDictionary(_allowedFileTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetCitySearchOptions()
    {
        var rawCities = await _metadataRepository.GetCities();
        var cities = new Dictionary<string, string>
        {
            { "ALL", "All" }
        };
        foreach (var c in rawCities.Order())
        {
            cities.Add(c, c);
        }
        return cities;
    }

    public async Task<Dictionary<string, string>> GetCounties(bool forUi = false)
    {
        _counties ??= await GetAll(138);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_counties.OrderBy(o => o.MetadataId), "", "Select County");
        }
        return _uiHelperService.ToDictionary(_counties.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetCountiesByStateCd(string stateCd, bool forUi = false)
    {
        stateCd = (stateCd ?? "").Trim().ToUpper();
        _counties ??= await GetAll(138);
        var stateCounties = _counties.Where(w => (w.AssociatedCode ?? "").Trim().ToUpper() == stateCd.Trim().ToUpper()).ToList();
        var otherCounty = _counties.FirstOrDefault(f => f.MetadataId == 138999);
        if (otherCounty != null)
        {
            stateCounties.Add(otherCounty);
        }
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(stateCounties.OrderBy(o => o.MetadataId), "", "Select County");
        }
        return _uiHelperService.ToDictionary(stateCounties.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetDocumentTypes(bool forUi = false)
    {
        _documentTypes ??= await GetAll(130);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_documentTypes.OrderBy(o => o.MetadataId), "", "Select Document Type");
        }
        return _uiHelperService.ToDictionary(_documentTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetEthnicityTypes(bool forUi = false)
    {
        _ethnicityTypes ??= await GetAll(112);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_ethnicityTypes.OrderBy(o => o.MetadataId), "", "Select Ethnicity");
        }
        return _uiHelperService.ToDictionary(_ethnicityTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetGenderTypes(bool forUi = false)
    {
        _genderTypes ??= await GetAll(110);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_genderTypes.OrderBy(o => o.MetadataId), "", "Select Gender");
        }
        return _uiHelperService.ToDictionary(_genderTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetIdTypes(bool forUi = false)
    {
        _idTypes ??= await GetAll(125);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_idTypes.OrderBy(o => o.MetadataId), "", "Select ID Type");
        }
        return _uiHelperService.ToDictionary(_idTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetLanguages(bool forUi = false)
    {
        _languageTypes ??= await GetAll(113);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_languageTypes.OrderBy(o => o.MetadataId), "", "Select Language");
        }
        return _uiHelperService.ToDictionary(_languageTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetLeadTypes(bool forUi = false)
    {
        _leadTypes ??= await GetAll(120);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_leadTypes.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_leadTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetListingDateTypeSearchOptions(bool forUi = false)
    {
        _listingDateTypeSearchOptions ??= await GetAll(126);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_listingDateTypeSearchOptions.OrderBy(o => o.MetadataId), "", "Select View Type");
        }
        return _uiHelperService.ToDictionary(_listingDateTypeSearchOptions.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetListingTypeSearchOptions()
    {
        _listingTypeSearchOptions ??= await GetAll(136);
        return _uiHelperService.ToDictionary(_listingTypeSearchOptions.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetListingTypes(bool forUi = false)
    {
        _listingTypes ??= await GetAll(107);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_listingTypes.OrderBy(o => o.MetadataId), "", "Select Listing Type");
        }
        return _uiHelperService.ToDictionary(_listingTypes.OrderBy(o => o.MetadataId));
    }

    public Task<Dictionary<string, string>> GetLoginHelpTypes()
    {
        return Task.FromResult(_loginHelpTypes);
    }

    public async Task<Dictionary<string, string>> GetPetsAllowedSearchOptions(bool forUi = false)
    {
        _petsAllowedSearchOptions ??= await GetAll(134);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_petsAllowedSearchOptions.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_petsAllowedSearchOptions.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetPhoneNumberTypes(bool forUi = false)
    {
        _phoneNumberTypes ??= await GetAll(114);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_phoneNumberTypes.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_phoneNumberTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetRaceTypes(bool forUi = false)
    {
        _raceTypes ??= await GetAll(111);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_raceTypes.OrderBy(o => o.MetadataId), "", "Select Race");
        }
        return _uiHelperService.ToDictionary(_raceTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetRelationTypes(bool forUi = false)
    {
        _relationTypes ??= await GetAll(109);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_relationTypes.OrderBy(o => o.MetadataId), "", "Select Relationship Type");
        }
        return _uiHelperService.ToDictionary(_relationTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetSeniorLivingSearchOptions(bool forUi = false)
    {
        _seniorLivingSearchOptions ??= await GetAll(135);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_seniorLivingSearchOptions.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_seniorLivingSearchOptions.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetStatuses(bool forUi = false)
    {
        _statuses ??= await GetAll(106);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_statuses, "", "Select Status");
        }
        return _uiHelperService.ToDictionary(_statuses);
    }

    public async Task<Dictionary<string, string>> GetUnitTypes(bool forUi = false)
    {
        _unitTypes ??= await GetAll(108);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_unitTypes.OrderBy(o => o.MetadataId), "", "Select Unit Type");
        }
        return _uiHelperService.ToDictionary(_unitTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetUsStates(bool forUi = false)
    {
        _usStates ??= await GetAll(137);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_usStates.OrderBy(o => o.MetadataId), "", "Select State");
        }
        return _uiHelperService.ToDictionary(_usStates.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetVoucherTypes(bool forUi = false)
    {
        _voucherTypes ??= await GetAll(123);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_voucherTypes.OrderBy(o => o.MetadataId), "", "Select Voucher Type");
        }
        return _uiHelperService.ToDictionary(_voucherTypes.OrderBy(o => o.MetadataId));
    }
}