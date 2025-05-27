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

public interface IFundingSourceRepository : ICRUDRepository<FundingSource>
{
}

[ExcludeFromCodeCoverage]
public class FundingSourceRepository : IFundingSourceRepository
{
    private readonly ILogger<FundingSourceRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public FundingSourceRepository(ILogger<FundingSourceRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, FundingSource item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFundingSourceAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Name,
                item.Description,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.FundingSourceId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.FundingSourceId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Funding Source - {item.FundingSourceId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Funding Source - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Funding Source!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, FundingSource item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFundingSourceDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.FundingSourceId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Funding Source - {item.FundingSourceId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Funding Source - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Funding Source!");
            return false;
        }
    }

    public async Task<IEnumerable<FundingSource>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterFundingSourceRetrieve]";
        return await dbConn.QueryAsync<FundingSource>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<FundingSource> GetOne(FundingSource item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterFundingSourceRetrieve]";
        return await dbConn.QueryFirstOrDefaultAsync<FundingSource>(query, new
        {
            FundingSourceID = item.FundingSourceId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, FundingSource item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFundingSourceUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.FundingSourceId,
                item.Name,
                item.Description,
                item.Active,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Funding Source - {item.FundingSourceId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Funding Source - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Funding Source!");
            return false;
        }
    }
}