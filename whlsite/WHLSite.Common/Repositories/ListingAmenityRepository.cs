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

public interface IListingAmenityRepository
{
    Task<IEnumerable<Amenity>> GetAll(int listingId);
}

[ExcludeFromCodeCoverage]
public class ListingAmenityRepository : IListingAmenityRepository
{
    private readonly ILogger<ListingAmenityRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingAmenityRepository(ILogger<ListingAmenityRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Amenity>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingAmenityRetrieve]";
        return await dbConn.QueryAsync<Amenity>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }
}