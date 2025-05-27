using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;

namespace WHLAdmin.Services;

public interface IMetadataService
{
    string GetActionDescription(string actionCd);
    Task<IEnumerable<CodeDescription>> GetAll(int codeId = 0);
    Task<Dictionary<string, string>> GetAccessibilities();
    Task<Dictionary<string, string>> GetAnswerTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetApplicationStatuses(bool forUi = false);
    Task<Dictionary<string, string>> GetBathroomPartOptions(bool forUi = false);
    Task<Dictionary<string, string>> GetCategories(bool forUi = false);
    Task<Dictionary<string, string>> GetDeclarations();
    Task<Dictionary<string, string>> GetDisclosures();
    Task<string> GetEntityTypeDescription(string entityTypeCd);
    Task<Dictionary<string, string>> GetEntityTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetEthnicityTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetFrequencyTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetGenderTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetIdTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetLeadTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetListingAgeTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetListingTypes(bool forUi = false, bool removeBoth = false);
    Task<Dictionary<string, string>> GetOrganizations(bool forUi = false);
    Task<Dictionary<string, string>> GetPhoneNumberTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetQuestionCategories(bool forUi = false);
    Task<Dictionary<string, string>> GetRaceTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetRelationTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetRoles(bool forUi = false);
    Task<Dictionary<string, string>> GetStatuses(bool forUi = false);
    Task<Dictionary<string, string>> GetSubmissionTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetUnitTypes(bool forUi = false);
    Task<Dictionary<string, string>> GetVoucherTypes(bool forUi = false);
}

[ExcludeFromCodeCoverage]
public class MetadataService : IMetadataService
{
    private readonly ILogger<MetadataService> _logger;
    private readonly IMetadataRepository _metadataRepository;
    private readonly IUiHelperService _uiHelperService;
    private readonly IEnumerable<CodeDescription> _actions;

    private IEnumerable<CodeDescription> _metadata;
    private IEnumerable<CodeDescription> _accessbilities;
    private IEnumerable<CodeDescription> _answerTypes;
    private IEnumerable<CodeDescription> _applicationStatuses;
    private IEnumerable<CodeDescription> _bathroomPartOptions;
    private IEnumerable<CodeDescription> _categories;
    private IEnumerable<CodeDescription> _declarations;
    private IEnumerable<CodeDescription> _disclosures;
    private IEnumerable<CodeDescription> _entityTypes;
    private IEnumerable<CodeDescription> _ethnicityTypes;
    private IEnumerable<CodeDescription> _frequencyTypes;
    private IEnumerable<CodeDescription> _genderTypes;
    private IEnumerable<CodeDescription> _idTypes;
    private IEnumerable<CodeDescription> _leadTypes;
    private IEnumerable<CodeDescription> _listingAgeTypes;
    private IEnumerable<CodeDescription> _listingTypes;
    private IEnumerable<CodeDescription> _organizations;
    private IEnumerable<CodeDescription> _phoneNumberTypes;
    private IEnumerable<CodeDescription> _questionCategories;
    private IEnumerable<CodeDescription> _raceTypes;
    private IEnumerable<CodeDescription> _relationTypes;
    private IEnumerable<CodeDescription> _roles;
    private IEnumerable<CodeDescription> _statuses;
    private IEnumerable<CodeDescription> _submissionTypes;
    private IEnumerable<CodeDescription> _unitTypes;
    private IEnumerable<CodeDescription> _voucherTypes;

    public MetadataService(ILogger<MetadataService> logger, IMetadataRepository metadataRepository, IUiHelperService uiHelperService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _metadataRepository = metadataRepository ?? throw new ArgumentNullException(nameof(metadataRepository));
        _uiHelperService = uiHelperService ?? throw new ArgumentNullException(nameof(uiHelperService));

        _actions =
        [
            new("ADD", "Add"),
            new("UPDATE", "Update"),
            new("DELETE", "Delete")
        ];
    }

    public string GetActionDescription(string actionCd)
    {
        return _actions?.FirstOrDefault(f => f.Code == actionCd)?.Description ?? actionCd;
    }

    public async Task<IEnumerable<CodeDescription>> GetAll(int codeId = 0)
    {
        _metadata ??= await _metadataRepository.GetAll();

        if (codeId > 0) return _metadata.Where(w => w.CodeId == codeId);

        return _metadata;
    }

    public async Task<Dictionary<string, string>> GetAccessibilities()
    {
        _accessbilities ??= await GetAll(119);
        return _uiHelperService.ToDictionary(_accessbilities.OrderBy(o => o.Description));
    }

    public async Task<Dictionary<string, string>> GetAnswerTypes(bool forUi = false)
    {
        _answerTypes ??= await GetAll(105);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_answerTypes.OrderBy(o => o.MetadataId), "", "Select Answer Type");
        }
        return _uiHelperService.ToDictionary(_answerTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetApplicationStatuses(bool forUi = false)
    {
        _applicationStatuses ??= await GetAll(127);
        var statuses = _applicationStatuses.Where(w => w.Active);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(statuses.OrderBy(o => o.MetadataId), "", "Select Status");
        }
        return _uiHelperService.ToDictionary(statuses.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetBathroomPartOptions(bool forUi = false)
    {
        _bathroomPartOptions ??= await GetAll(130);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_bathroomPartOptions.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_bathroomPartOptions.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetCategories(bool forUi = false)
    {
        _categories ??= await GetAll(103);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_categories.OrderBy(o => o.MetadataId), "", "Select Category");
        }
        return _uiHelperService.ToDictionary(_categories.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetDeclarations()
    {
        _declarations ??= await GetAll(121);
        return _uiHelperService.ToDictionary(_declarations.OrderBy(o => o.Code));
    }

    public async Task<Dictionary<string, string>> GetDisclosures()
    {
        _disclosures ??= await GetAll(122);
        return _uiHelperService.ToDictionary(_disclosures.OrderBy(o => o.Code));
    }

    public async Task<string> GetEntityTypeDescription(string entityTypeCd)
    {
        _entityTypes ??= await GetAll(101);
        return _entityTypes?.FirstOrDefault(f => f.Code == entityTypeCd)?.Description ?? entityTypeCd;
    }

    public async Task<Dictionary<string, string>> GetEntityTypes(bool forUi = false)
    {
        _entityTypes ??= await GetAll(101);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_entityTypes.OrderBy(o => o.MetadataId), "", "Select Entity Type");
        }
        return _uiHelperService.ToDictionary(_entityTypes.OrderBy(o => o.MetadataId));
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

    public async Task<Dictionary<string, string>> GetFrequencyTypes(bool forUi = false)
    {
        _frequencyTypes ??= await GetAll(104);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_frequencyTypes.OrderBy(o => o.MetadataId), "", "Select Frequency");
        }
        return _uiHelperService.ToDictionary(_frequencyTypes.OrderBy(o => o.MetadataId));
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

    public async Task<Dictionary<string, string>> GetLeadTypes(bool forUi = false)
    {
        _leadTypes ??= await GetAll(120);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_leadTypes.OrderBy(o => o.MetadataId), "", "Select One");
        }
        return _uiHelperService.ToDictionary(_leadTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetListingAgeTypes(bool forUi = false)
    {
        _listingAgeTypes ??= await GetAll(129);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_listingAgeTypes.OrderBy(o => o.MetadataId), "", "Select Listing Age Type");
        }
        return _uiHelperService.ToDictionary(_listingAgeTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetListingTypes(bool forUi = false, bool removeBoth = false)
    {
        _listingTypes ??= await GetAll(107);
        var listingTypes = removeBoth ?  _listingTypes.Where(w => w.Code != "BOTH") : _listingTypes;
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(listingTypes.OrderBy(o => o.MetadataId), "", "Select Listing Type");
        }
        return _uiHelperService.ToDictionary(listingTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetOrganizations(bool forUi = false)
    {
        _organizations ??= await GetAll(118);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_organizations.OrderBy(o => o.MetadataId), "", "Select Organization");
        }
        return _uiHelperService.ToDictionary(_organizations.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetPhoneNumberTypes(bool forUi = false)
    {
        _phoneNumberTypes ??= await GetAll(114);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_phoneNumberTypes.OrderBy(o => o.MetadataId), "", "Select Phone Number Type");
        }
        return _uiHelperService.ToDictionary(_phoneNumberTypes.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetQuestionCategories(bool forUi = false)
    {
        _questionCategories ??= await GetAll(117);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_questionCategories.OrderBy(o => o.MetadataId), "", "Select Category");
        }
        return _uiHelperService.ToDictionary(_questionCategories.OrderBy(o => o.MetadataId));
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

    public async Task<Dictionary<string, string>> GetRoles(bool forUi = false)
    {
        _roles ??= await GetAll(102);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_roles.OrderBy(o => o.MetadataId), "", "Select Role");
        }
        return _uiHelperService.ToDictionary(_roles.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetStatuses(bool forUi = false)
    {
        _statuses ??= await GetAll(106);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_statuses.OrderBy(o => o.MetadataId), "", "Select Status");
        }
        return _uiHelperService.ToDictionary(_statuses.OrderBy(o => o.MetadataId));
    }

    public async Task<Dictionary<string, string>> GetSubmissionTypes(bool forUi = false)
    {
        _submissionTypes ??= await GetAll(128);
        if (forUi)
        {
            return _uiHelperService.ToDropdownList(_submissionTypes.OrderBy(o => o.MetadataId), "", "Select Submission Type");
        }
        return _uiHelperService.ToDictionary(_submissionTypes.OrderBy(o => o.MetadataId));
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