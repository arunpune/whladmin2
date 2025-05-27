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

public interface IListingUnitRepository
{
    Task<IEnumerable<ListingUnit>> GetAll(int listingId);
    Task<ListingUnit> GetOne(long unitId);
}

[ExcludeFromCodeCoverage]
public class ListingUnitRepository : IListingUnitRepository
{
    private readonly ILogger<ListingUnitRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingUnitRepository(ILogger<ListingUnitRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ListingUnit>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingUnitRetrieve]";
        return await dbConn.QueryAsync<ListingUnit>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingUnit> GetOne(long unitId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingUnitRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingUnit>(query, new
        {
            UnitID = unitId,
        }, commandType: CommandType.StoredProcedure);
    }
}