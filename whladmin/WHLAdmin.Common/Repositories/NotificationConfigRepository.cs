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

public interface INotificationConfigRepository : ICRUDRepository<NotificationConfig>
{
    Task<NotificationConfig> GetOneByTitle(NotificationConfig item);
}

[ExcludeFromCodeCoverage]
public class NotificationConfigRepository : INotificationConfigRepository
{
    private readonly ILogger<NotificationConfigRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public NotificationConfigRepository(ILogger<NotificationConfigRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, NotificationConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterNotificationAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.CategoryCd,
                item.Title,
                item.Text,
                item.FrequencyCd,
                item.FrequencyInterval,
                item.NotificationList,
                item.InternalNotificationList,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.NotificationId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.NotificationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Notification Configuration - {item.NotificationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Notification Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Notification Configuration!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, NotificationConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterNotificationDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.NotificationId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Notification Configuration - {item.NotificationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Notification Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Notification Configuration!");
            return false;
        }
    }

    public async Task<IEnumerable<NotificationConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterNotificationRetrieve]";
        return await dbConn.QueryAsync<NotificationConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<NotificationConfig> GetOne(NotificationConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterNotificationRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<NotificationConfig>(query, new
        {
            NotificationID = item.NotificationId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<NotificationConfig> GetOneByTitle(NotificationConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterNotificationRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<NotificationConfig>(query, new
        {
            item.CategoryCd,
            item.Title
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, NotificationConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterNotificationUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.NotificationId,
                item.CategoryCd,
                item.Title,
                item.Text,
                item.FrequencyCd,
                item.FrequencyInterval,
                item.NotificationList,
                item.InternalNotificationList,
                item.Active,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Notification Configuration - {item.NotificationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Notification Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Notification Configuration!");
            return false;
        }
    }
}