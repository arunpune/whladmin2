using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Settings;

namespace WHLSite.Services;

public interface IMasterConfigService
{
    Task<IEnumerable<Config>> GetAll();
    Task<ArcGisSettings> GetArcGisSettings();
}

[ExcludeFromCodeCoverage]
public class MasterConfigService : IMasterConfigService
{
    private readonly ILogger<MasterConfigService> _logger;
    private readonly IMasterConfigRepository _configRepository;

    private IEnumerable<Config> _configs;
    private ArcGisSettings _arcGisSettings;

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
}