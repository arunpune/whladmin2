using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Providers;

namespace WHLAdmin.Common.Repositories;

public interface ISystemRepository
{
    Task<SystemInfo> GetInfo();
}

[ExcludeFromCodeCoverage]
public class SystemRepository : ISystemRepository
{
    private readonly ILogger<SystemRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public SystemRepository(ILogger<SystemRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<SystemInfo> GetInfo()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSystemInfoRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<SystemInfo>(query, commandType: CommandType.StoredProcedure);
    }
}