using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.Services;

public interface IMasterConfigService
{
    Task<IEnumerable<Config>> GetAll();
    Task<ArcGisSettings> GetArcGisSettings();
    Task<EmfluenceSettings> GetEmfluenceSettings();
}

[ExcludeFromCodeCoverage]
public class MasterConfigService : IMasterConfigService
{
    private readonly ILogger<MasterConfigService> _logger;
    private readonly IMasterConfigRepository _configRepository;

    private IEnumerable<Config> _configs;
    private ArcGisSettings _arcGisSettings;
    private EmfluenceSettings _emfluenceSettings;

    public MasterConfigService(ILogger<MasterConfigService> logger, IMasterConfigRepository configRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configRepository = configRepository ?? throw new ArgumentNullException(nameof(_configRepository));
    }

    public async Task<IEnumerable<Config>> GetAll()
    {
        _configs ??= await _configRepository.GetAll();
        return _configs;
    }

    public async Task<ArcGisSettings> GetArcGisSettings()
    {
        if (_arcGisSettings == null)
        {
            _configs ??= await _configRepository.GetAll();
            var arcGisConfigs = _configs.Where(w => w.Category.Equals("ESRIAPI", StringComparison.CurrentCultureIgnoreCase));
            if ((arcGisConfigs?.Count() ?? 0) > 0)
            {
                _arcGisSettings = new ArcGisSettings()
                {
                    Enabled = (arcGisConfigs.FirstOrDefault(f => f.ConfigKey.Equals("ENABLED", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "").Trim().Equals("YES", StringComparison.CurrentCultureIgnoreCase),
                    ApiKey = arcGisConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIKEY", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    ApiKeyExpiry = arcGisConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIKEYEXPIRY", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    ApiMethod = arcGisConfigs.FirstOrDefault(f => f.ConfigKey.Equals("METHOD", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    ApiUrl = arcGisConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIURL", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                };
            }
        }
        return _arcGisSettings;
    }

    public async Task<EmfluenceSettings> GetEmfluenceSettings()
    {
        if (_emfluenceSettings == null)
        {
            _configs ??= await _configRepository.GetAll();
            var emfluenceConfigs = _configs.Where(w => w.Category.Equals("EMFLUENCE", StringComparison.CurrentCultureIgnoreCase));
            if (emfluenceConfigs?.Count() > 0)
            {
                _emfluenceSettings = new ()
                {
                    Enabled = (emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("ENABLED", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "").Trim().Equals("YES", StringComparison.CurrentCultureIgnoreCase),
                    ApiUrl = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIURL", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    ApiKey = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIKEY", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    ApiKeyExpiry = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("APIKEYEXPIRY", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    Domain = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("DOMAIN", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    RentalGroupId = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("RENTALGROUPID", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    RentalGroupName = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("RENTALGROUPNAME", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    RentalTemplateId = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("RENTALTEMPLAETID", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    RentalTemplateName = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("RENTALTEMPLATENAME", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    SaleGroupId = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("SALEGROUPID", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    SaleGroupName = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("SALEGROUPNAME", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    SaleTemplateId = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("SALETEMPLAETID", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                    SaleTemplateName = emfluenceConfigs.FirstOrDefault(f => f.ConfigKey.Equals("SALETEMPLATENAME", StringComparison.CurrentCultureIgnoreCase))?.ConfigValue ?? "",
                };
            }
        }
        return _emfluenceSettings;
    }
}