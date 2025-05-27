using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Providers;

namespace WHLAdmin.Common.Repositories;

public interface IUserRepository
{
    Task<bool> AddNotification(string requestId, string correlationId, UserNotification item);
}

[ExcludeFromCodeCoverage]
public class UserRepository : IUserRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public UserRepository(ILogger<UserRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> AddNotification(string requestId, string correlationId, UserNotification item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserNotificationAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.Subject,
                item.Body,
                item.EmailSentInd,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Added Site User Notification - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to add Site User Notification - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to add Site User Notification!");
            return false;

        }
    }
}