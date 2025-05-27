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

public interface IListingFundingSourceRepository
{
    Task<IEnumerable<FundingSource>> GetAll(int listingId);
}

[ExcludeFromCodeCoverage]
public class ListingFundingSourceRepository : IListingFundingSourceRepository
{
    private readonly ILogger<ListingFundingSourceRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingFundingSourceRepository(ILogger<ListingFundingSourceRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<FundingSource>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingFundingSourceRetrieve]";
        return await dbConn.QueryAsync<FundingSource>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }
}