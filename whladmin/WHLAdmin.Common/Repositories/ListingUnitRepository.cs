using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Extensions;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Providers;

namespace WHLAdmin.Common.Repositories;

public interface IListingUnitRepository
{
    Task<bool> Add(string correlationId, ListingUnit item);
    Task<bool> Delete(string correlationId, ListingUnit item);
    Task<IEnumerable<ListingUnit>> GetAll(long listingId);
    Task<ListingUnit> GetOne(int unitId);
    Task<bool> Update(string correlationId, ListingUnit item);
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

    public async Task<bool> Add(string correlationId, ListingUnit item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var households = item.Households.ToDataTable(_logger, correlationId);
            var query = "[dbo].[uspListingUnitAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.UnitTypeCd,
                item.BedroomCnt,
                item.BathroomCnt,
                item.BathroomCntPart,
                item.SquareFootage,
                item.AreaMedianIncomePct,
                item.MonthlyRentAmt,
                item.AssetLimitAmt,
                item.EstimatedPriceAmt,
                item.SubsidyAmt,
                item.MonthlyTaxesAmt,
                item.MonthlyMaintenanceAmt,
                item.MonthlyInsuranceAmt,
                item.UnitsAvailableCnt,
                Households = households.AsTableValuedParameter("udtListingUnitHousehold"),
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.UnitId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.UnitId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Listing Unit - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Listing Unit - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Listing Unit!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, ListingUnit item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUnitDelete]";
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
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Listing Unit - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Listing Unit - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Listing Unit!");
            return false;
        }
    }

    public async Task<IEnumerable<ListingUnit>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingUnitRetrieve]";
        return await dbConn.QueryAsync<ListingUnit>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingUnit> GetOne(int unitId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingUnitRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingUnit>(query, new
        {
            UnitID = unitId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, ListingUnit item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var households = item.Households.ToDataTable(_logger, correlationId);
            var query = "[dbo].[uspListingUnitUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UnitId,
                item.UnitTypeCd,
                item.BedroomCnt,
                item.BathroomCnt,
                item.BathroomCntPart,
                item.SquareFootage,
                item.AreaMedianIncomePct,
                item.MonthlyRentAmt,
                item.AssetLimitAmt,
                item.EstimatedPriceAmt,
                item.SubsidyAmt,
                item.MonthlyTaxesAmt,
                item.MonthlyMaintenanceAmt,
                item.MonthlyInsuranceAmt,
                item.UnitsAvailableCnt,
                Households = households.AsTableValuedParameter("udtListingUnitHousehold"),
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Unit - {item.UnitId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Unit - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Unit!");
            return false;
        }
    }
}