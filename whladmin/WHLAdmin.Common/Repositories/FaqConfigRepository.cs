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

public interface IFaqConfigRepository : ICRUDRepository<FaqConfig>
{
}

[ExcludeFromCodeCoverage]
public class FaqConfigRepository : IFaqConfigRepository
{
    private readonly ILogger<FaqConfigRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public FaqConfigRepository(ILogger<FaqConfigRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, FaqConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFAQAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.CategoryName,
                item.Title,
                item.Text,
                item.Url,
                item.Url1,
                item.Url2,
                item.Url3,
                item.Url4,
                item.Url5,
                item.Url6,
                item.Url7,
                item.Url8,
                item.Url9,
                item.DisplayOrder,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.FaqId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.FaqId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added FAQ Configuration - {item.FaqId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add FAQ Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add FAQ Configuration!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, FaqConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFAQDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.FaqId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted FAQ Configuration - {item.FaqId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete FAQ Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete FAQ Configuration!");
            return false;
        }
    }

    public async Task<IEnumerable<FaqConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterFAQRetrieve]";
        return await dbConn.QueryAsync<FaqConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<FaqConfig> GetOne(FaqConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterFAQRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<FaqConfig>(query, new
        {
            FAQID = item.FaqId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, FaqConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterFAQUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.FaqId,
                item.CategoryName,
                item.Title,
                item.Text,
                item.Url,
                item.Url1,
                item.Url2,
                item.Url3,
                item.Url4,
                item.Url5,
                item.Url6,
                item.Url7,
                item.Url8,
                item.Url9,
                item.DisplayOrder,
                item.Active,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated FAQ Configuration - {item.FaqId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update FAQ Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update FAQ Configuration!");
            return false;
        }
    }
}