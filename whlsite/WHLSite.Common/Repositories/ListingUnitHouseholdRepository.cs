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

public interface IListingUnitHouseholdRepository
{
    Task<IEnumerable<ListingUnitHousehold>> GetAll(int listingId);
    Task<ListingUnitHousehold> GetOne(long unitId);
}

[ExcludeFromCodeCoverage]
public class ListingUnitHouseholdRepository : IListingUnitHouseholdRepository
{
    private readonly ILogger<ListingUnitHouseholdRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingUnitHouseholdRepository(ILogger<ListingUnitHouseholdRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ListingUnitHousehold>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingUnitHouseholdRetrieve]";
        return await dbConn.QueryAsync<ListingUnitHousehold>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingUnitHousehold> GetOne(long unitId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingUnitHouseholdRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingUnitHousehold>(query, new
        {
            UnitID = unitId,
        }, commandType: CommandType.StoredProcedure);
    }
}