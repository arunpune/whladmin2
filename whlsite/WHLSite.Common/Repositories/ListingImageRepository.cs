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

public interface IListingImageRepository
{
    Task<IEnumerable<ListingImage>> GetAll(int listingId);
    Task<ListingImage> GetOne(long imageId);
}

[ExcludeFromCodeCoverage]
public class ListingImageRepository : IListingImageRepository
{
    private readonly ILogger<ListingImageRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingImageRepository(ILogger<ListingImageRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ListingImage>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingImageRetrieve]";
        return await dbConn.QueryAsync<ListingImage>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingImage> GetOne(long imageId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingImageRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingImage>(query, new
        {
            ImageID = imageId,
        }, commandType: CommandType.StoredProcedure);
    }
}