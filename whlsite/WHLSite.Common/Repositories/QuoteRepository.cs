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

public interface IQuoteRepository : IReadOnlyRepository<QuoteConfig>
{
}

[ExcludeFromCodeCoverage]
public class QuoteRepository : IQuoteRepository
{
    private readonly ILogger<QuoteRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public QuoteRepository(ILogger<QuoteRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<QuoteConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteQuoteRetrieve]";
        return await dbConn.QueryAsync<QuoteConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<QuoteConfig> GetOne(QuoteConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteQuoteRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<QuoteConfig>(query, new
        {
            QuoteID = item.QuoteId,
        }, commandType: CommandType.StoredProcedure);
    }
}