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

public interface IListingUnitHouseholdRepository
{
    Task<bool> Add(string correlationId, ListingUnitHousehold item);
    Task<bool> Delete(string correlationId, ListingUnitHousehold item);
    Task<IEnumerable<ListingUnitHousehold>> GetAll(long listingId);
    Task<ListingUnitHousehold> GetOne(int unitId);
    Task<bool> Update(string correlationId, ListingUnitHousehold item);
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

    public async Task<bool> Add(string correlationId, ListingUnitHousehold item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUnitHouseholdAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UnitId,
                item.HouseholdSize,
                item.MinHouseholdIncomeAmt,
                item.MaxHouseholdIncomeAmt,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.UnitId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.UnitId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Listing Unit Household - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Listing Unit Household - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Listing Unit Household!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, ListingUnitHousehold item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUnitHouseholdDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UnitId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Listing Unit Household - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Listing Unit Household - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Listing Unit Household!");
            return false;
        }
    }

    public async Task<IEnumerable<ListingUnitHousehold>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingUnitHouseholdRetrieve]";
        return await dbConn.QueryAsync<ListingUnitHousehold>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingUnitHousehold> GetOne(int unitId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingUnitHouseholdRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingUnitHousehold>(query, new
        {
            UnitID = unitId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, ListingUnitHousehold item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUnitHouseholdUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UnitId,
                item.HouseholdSize,
                item.MinHouseholdIncomeAmt,
                item.MaxHouseholdIncomeAmt,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Unit Household - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Unit Household - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Unit Household!");
            return false;
        }
    }
}