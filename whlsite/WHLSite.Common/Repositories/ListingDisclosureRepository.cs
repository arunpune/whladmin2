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

public interface IListingDisclosureRepository
{
    Task<IEnumerable<Disclosure>> GetAll(int listingId);
}

[ExcludeFromCodeCoverage]
public class ListingDisclosureRepository : IListingDisclosureRepository
{
    private readonly ILogger<ListingDisclosureRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingDisclosureRepository(ILogger<ListingDisclosureRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Disclosure>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingDisclosureRetrieve]";
        return await dbConn.QueryAsync<Disclosure>(query, new
        {
            listingId,
        }, commandType: CommandType.StoredProcedure);
    }
}