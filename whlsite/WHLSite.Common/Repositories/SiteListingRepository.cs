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

public interface ISiteListingRepository
{
    Task<IEnumerable<Listing>> GetAll();
    Task<PagedRecords<Listing>> GetAllPaged(string listingTypeCd, int pageNo = 1, int pageSize = 12);
    Task<Listing> GetOne(Listing item);
    Task<IEnumerable<Listing>> Search(ListingSearchParameters searchParameters);
}

[ExcludeFromCodeCoverage]
public class SiteListingRepository : ISiteListingRepository
{
    private readonly ILogger<SiteListingRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public SiteListingRepository(ILogger<SiteListingRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Listing>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingRetrieve]";
        return await dbConn.QueryAsync<Listing>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedRecords<Listing>> GetAllPaged(string listingTypeCd, int pageNo = 1, int pageSize = 12)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingRetrievePaged]";
        var results = await dbConn.QueryMultipleAsync(query, new
        {
            PageNo = pageNo,
            PageSize = pageSize
        }, commandType: CommandType.StoredProcedure);
        return new PagedRecords<Listing>()
        {
            Records = results.Read<Listing>(),
            PagingInfo = results.ReadSingleOrDefault<PagingInfo>()
        };
    }

    public async Task<Listing> GetOne(Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<Listing>(query, new
        {
            ListingID = item.ListingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Listing>> Search(ListingSearchParameters searchParameters)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingSearch]";
        return await dbConn.QueryAsync<Listing>(query, new
        {
            searchParameters.ListingTypeOptionCd,
            City = searchParameters.CityOptionCd,
            searchParameters.SeniorLivingOptionCd,
            searchParameters.AdaptedForDisabilityOptionCd,
            searchParameters.PetsAllowedOptionCd,
            searchParameters.ListingDateFilterOptionCd
        }, commandType: CommandType.StoredProcedure);
    }
}