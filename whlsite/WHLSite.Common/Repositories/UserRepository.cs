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

public interface IUserRepository
{
    Task<IEnumerable<UserAccount>> GetAll();
    Task<UserAccount> GetOne(string username);
    Task<UserAccount> GetOneByEmailAddress(string emailAddress);

    Task<UserAccount> GetOneByKey(string requestId, string correlationId, string activationKey = null, string passwordResetKey = null);
    Task<bool> DeactivateKey(string requestId, string correlationId, string activationKey = null, string passwordResetKey = null);
    Task<bool> ExchangeKey(string requestId, string correlationId, string newKey, string activationKey = null, string passwordResetKey = null);

    Task<bool> Register(string requestId, string correlationId, UserAccount item);

    Task<UserAccount> Authenticate(string requestId, string correlationId, UserAccount userAccount);
    Task<bool> ChangePassword(string requestId, string correlationId, UserAccount item);
    Task<bool> InitiatePasswordResetRequest(string requestId, string correlationId, UserAccount item);
    Task<bool> ProcessPasswordResetRequest(string requestId, string correlationId, UserAccount item);

    Task<bool> ReinitiateActivation(string requestId, string correlationId, UserAccount item);
    Task<bool> Activate(string requestId, string correlationId, UserAccount item);
    Task<bool> ActivateAltEmailAddress(string requestId, string correlationId, UserAccount item);
    Task<bool> ActivatePhoneNumber(string requestId, string correlationId, UserAccount item);

    Task<IEnumerable<UserNotification>> GetNotifications(string requestId, string correlationId, string username, string filterTypeCd = null);
    Task<bool> AddNotification(string requestId, string correlationId, UserNotification item);
    Task<bool> DeleteNotification(string requestId, string correlationId, UserNotification item);
    Task<bool> UpdateNotification(string requestId, string correlationId, UserNotification item);
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

    public async Task<IEnumerable<UserAccount>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserRetrieve]";
        return await dbConn.QueryAsync<UserAccount>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<UserAccount> GetOne(string username)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<UserAccount>(query, new
        {
            username
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<UserAccount> GetOneByEmailAddress(string emailAddress)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserRetrieveByEmailAddress]";
        return await dbConn.QuerySingleOrDefaultAsync<UserAccount>(query, new
        {
            emailAddress
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<UserAccount> GetOneByKey(string requestId, string correlationId, string activationKey = null, string passwordResetKey = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserRetrieveByKey]";
        return await dbConn.QuerySingleOrDefaultAsync<UserAccount>(query, new
        {
            activationKey,
            passwordResetKey
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> DeactivateKey(string requestId, string correlationId, string activationKey = null, string passwordResetKey = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserDeactivateKey]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                activationKey,
                passwordResetKey
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deactivated Site User Key - {activationKey ?? passwordResetKey}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to deactivate Site User Key - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to deactivate Site User Key!");
            return false;

        }
    }

    public async Task<bool> ExchangeKey(string requestId, string correlationId, string newKey, string activationKey = null, string passwordResetKey = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserExchangeKey]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                newKey,
                activationKey,
                passwordResetKey
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Exchanged Site User Key - {activationKey ?? passwordResetKey}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to exchange Site User Key - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to exchange Site User Key!");
            return false;

        }
    }

    public async Task<bool> Register(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserRegister]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.PasswordHash,
                item.ActivationKey,
                item.ActivationKeyExpiry,
                item.EmailAddress,
                item.AuthRepEmailAddressInd,
                item.PhoneNumber,
                item.PhoneNumberExtn,
                item.PhoneNumberTypeCd,
                item.LeadTypeCd,
                item.LeadTypeOther,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Added Site User - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to add Site User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to add Site User!");
            return false;

        }
    }

    public async Task<UserAccount> Authenticate(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserAuthenticate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.PasswordHash
            });
            // queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var account = await dbConn.QuerySingleOrDefaultAsync<UserAccount>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (account != null)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Authenticated Site User - {item.Username}");
                return account;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to authenticate Site User - {errorMessage ?? "Database exception"}");
            return null;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to authenticate Site User!");
            return null;

        }
    }

    public async Task<bool> ChangePassword(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserChangePassword]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.PasswordHash,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Changed Site User Password - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to change Site User Password - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to change Site User Password!");
            return false;

        }
    }

    public async Task<bool> InitiatePasswordResetRequest(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserInitiatePasswordReset]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                PasswordResetKey = item.ActivationKey,
                PasswordResetKeyExpiry = item.ActivationKeyExpiry,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Initiated Site User Password Reset Request - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to initiate Site User Password Reset Request - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to initiate Site User Password Reset Request!");
            return false;

        }
    }

    public async Task<bool> ProcessPasswordResetRequest(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserProcessPasswordReset]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ActivationKey,
                item.ActivationKeyExpiry,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Password Reset Request - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Password Reset Request - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Password Reset Request!");
            return false;

        }
    }

    public async Task<bool> ReinitiateActivation(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserReinitiateActivation]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ActivationKey,
                item.ActivationKeyExpiry,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Reinitiated Site User activation - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to reinitiate Site User activation - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to reinitiate Site User activation!");
            return false;

        }
    }

    public async Task<bool> Activate(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserActivate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ActivationKey,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Activated Site User - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User!");
            return false;

        }
    }

    public async Task<bool> ActivateAltEmailAddress(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserActivateAltEmailAddress]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.AltEmailAddressKey,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Activated Site User Alternate Email Address - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User Alternate Email Address - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User Alternate Email Address!");
            return false;

        }
    }

    public async Task<bool> ActivatePhoneNumber(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserActivatePhoneNumber]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.PhoneNumberKey,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Activated Site User Phone Number - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User Phone Number - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to activate Site User Phone Number!");
            return false;

        }
    }

    public async Task<IEnumerable<UserNotification>> GetNotifications(string requestId, string correlationId, string username, string filterTypeCd = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserNotificationRetrieve]";
        return await dbConn.QueryAsync<UserNotification>(query, new
        {
            username,
            filterTypeCd
        }, commandType: CommandType.StoredProcedure);
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

    public async Task<bool> DeleteNotification(string requestId, string correlationId, UserNotification item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserNotificationDelete]";
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
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Site User Notification - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Notification - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Notification!");
            return false;

        }
    }

    public async Task<bool> UpdateNotification(string requestId, string correlationId, UserNotification item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserNotificationUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.NotificationId,
                item.ReadInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Notification - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Notification - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Notification!");
            return false;

        }
    }
}