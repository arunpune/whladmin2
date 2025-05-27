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

public interface IListingAmenityRepository
{
    Task<IEnumerable<Amenity>> GetAll(long listingId);
    Task<IEnumerable<Amenity>> GetAllForEdit(long listingId);
    Task<bool> Save(string correlationId, string username, long listingId, string amenityIds);
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

    public async Task<IEnumerable<Amenity>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingAmenityRetrieve]";
        return await dbConn.QueryAsync<Amenity>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Amenity>> GetAllForEdit(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingAmenityRetrieveForEdit]";
        return await dbConn.QueryAsync<Amenity>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Save(string correlationId, string modifiedBy, long listingId, string amenityIds)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingAmenitySave]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                listingId,
                amenityIds,
                modifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var saved = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (saved > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Saved Listing Amenities - {listingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to save Listing Amenities - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to save Listing Amenities!");
            return false;
        }
    }
}