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

public interface IResourceRepository : IReadOnlyRepository<ResourceConfig>
{
}

[ExcludeFromCodeCoverage]
public class ResourceRepository : IResourceRepository
{
    private readonly ILogger<ResourceRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ResourceRepository(ILogger<ResourceRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ResourceConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteResourceRetrieve]";
        return await dbConn.QueryAsync<ResourceConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<ResourceConfig> GetOne(ResourceConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteResourceRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ResourceConfig>(query, new
        {
            ResourceID = item.ResourceId,
        }, commandType: CommandType.StoredProcedure);
    }
}