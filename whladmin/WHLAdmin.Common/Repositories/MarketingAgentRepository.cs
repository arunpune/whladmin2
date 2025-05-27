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

public interface IMarketingAgentRepository : ICRUDRepository<MarketingAgent>
{
}

[ExcludeFromCodeCoverage]
public class MarketingAgentRepository : IMarketingAgentRepository
{
    private readonly ILogger<MarketingAgentRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public MarketingAgentRepository(ILogger<MarketingAgentRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, MarketingAgent item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterMarketingAgentAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Name,
                item.ContactName,
                item.PhoneNumber,
                item.EmailAddress,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.AgentId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.AgentId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Marketing Agent - {item.AgentId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Marketing Agent - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Marketing Agent!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, MarketingAgent item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterMarketingAgentDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.AgentId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Marketing Agent - {item.AgentId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Marketing Agent - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Marketing Agent!");
            return false;
        }
    }

    public async Task<IEnumerable<MarketingAgent>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterMarketingAgentRetrieve]";
        return await dbConn.QueryAsync<MarketingAgent>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<MarketingAgent> GetOne(MarketingAgent item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterMarketingAgentRetrieve]";
        return await dbConn.QueryFirstOrDefaultAsync<MarketingAgent>(query, new
        {
            AgentID = item.AgentId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, MarketingAgent item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterMarketingAgentUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.AgentId,
                item.Name,
                item.ContactName,
                item.PhoneNumber,
                item.EmailAddress,
                item.Active,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Marketing Agent - {item.AgentId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Marketing Agent - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Marketing Agent!");
            return false;
        }
    }
}