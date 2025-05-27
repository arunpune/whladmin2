using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Providers;

namespace WHLAdmin.Common.Repositories;

public interface IMetadataRepository : IReadOnlyRepository<CodeDescription>
{
}

[ExcludeFromCodeCoverage]
public class MetadataRepository : IMetadataRepository
{
    private readonly ILogger<MetadataRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public MetadataRepository(ILogger<MetadataRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<CodeDescription>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMetadataRetrieve]";
        return await dbConn.QueryAsync<CodeDescription>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<CodeDescription> GetOne(CodeDescription item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMetadataRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<CodeDescription>(query, new
        {
            MetadataID = item.MetadataId
        }, commandType: CommandType.StoredProcedure);
    }
}