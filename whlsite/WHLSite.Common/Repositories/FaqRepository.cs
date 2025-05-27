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

public interface IFaqRepository : IReadOnlyRepository<FaqConfig>
{
}

[ExcludeFromCodeCoverage]
public class FaqRepository : IFaqRepository
{
    private readonly ILogger<FaqRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public FaqRepository(ILogger<FaqRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<FaqConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteFAQRetrieve]";
        return await dbConn.QueryAsync<FaqConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<FaqConfig> GetOne(FaqConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteFAQRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<FaqConfig>(query, new
        {
            FAQID = item.FaqId,
        }, commandType: CommandType.StoredProcedure);
    }
}