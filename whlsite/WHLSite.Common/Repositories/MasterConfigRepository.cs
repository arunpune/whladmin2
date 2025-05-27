using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Providers;

namespace WHLSite.Common.Repositories;

public interface IMasterConfigRepository : IReadOnlyRepository<Config>
{
}

[ExcludeFromCodeCoverage]
public class MasterConfigRepository : IMasterConfigRepository
{
    private readonly ILogger<MasterConfigRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public MasterConfigRepository(ILogger<MasterConfigRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Config>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterConfigRetrieve]";
        return await dbConn.QueryAsync<Config>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<Config> GetOne(Config item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterConfigRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<Config>(query, new
        {
            ConfigID = item.ConfigId,
            Category = item.Category,
            SubCategory = item.SubCategory,
            ConfigKey = item.ConfigKey
        }, commandType: CommandType.StoredProcedure);
    }
}