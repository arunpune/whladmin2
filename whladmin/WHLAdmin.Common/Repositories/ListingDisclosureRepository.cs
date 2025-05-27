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

public interface IListingDisclosureRepository : ICRUDRepository<Disclosure>
{
    Task<IEnumerable<Disclosure>> GetAll(long listingId);
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

    public async Task<bool> Add(string correlationId, Disclosure item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingDisclosureAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.Text,
                item.SortOrder,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.DisclosureId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.DisclosureId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Listing Disclosure - {item.DisclosureId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Listing Disclosure - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Listing Disclosure!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, Disclosure item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingDisclosureDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.DisclosureId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Listing Disclosure - {item.DisclosureId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Listing Disclosure - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Listing Disclosure!");
            return false;
        }
    }

    public async Task<IEnumerable<Disclosure>> GetAll()
    {
        return await GetAll(0);
    }

    public async Task<IEnumerable<Disclosure>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingDisclosureRetrieve]";
        return await dbConn.QueryAsync<Disclosure>(query, new
        {
            listingId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<Disclosure> GetOne(Disclosure item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingDisclosureRetrieve]";
        return await dbConn.QueryFirstOrDefaultAsync<Disclosure>(query, new
        {
            item.DisclosureId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, Disclosure item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingDisclosureUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.DisclosureId,
                item.Text,
                item.SortOrder,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Disclosure - {item.DisclosureId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Disclosure - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Disclosure!");
            return false;
        }
    }
}