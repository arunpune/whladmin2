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

public interface IAdminUserRepository : ICRUDRepository<User>
{
    Task<bool> SetOtp(string correlationId, string emailAddress, string otp);
    Task<bool> Reactivate(string correlationId, User item);
    Task<User> Authenticate(string correlationId, string userId = null, string emailAddress = null);
    Task<User> AuthenticateOtp(string correlationId, string emailAddress, string otp);
}

[ExcludeFromCodeCoverage]
public class AdminUserRepository : IAdminUserRepository
{
    private readonly ILogger<AdminUserRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public AdminUserRepository(ILogger<AdminUserRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, User item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspAdminUserAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UserId,
                item.EmailAddress,
                item.DisplayName,
                item.OrganizationCd,
                item.RoleCd,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Admin User - {item.UserId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Admin User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Admin User!");
            return false;
        }
    }

    public async Task<User> Authenticate(string correlationId, string userId = null, string emailAddress = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspAdminUserAuthenticate]";
        return await dbConn.QuerySingleOrDefaultAsync<User>(query, new
        {
            userId,
            emailAddress
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<User> AuthenticateOtp(string correlationId, string emailAddress, string otp)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspAdminUserAuthenticateOtp]";
        return await dbConn.QuerySingleOrDefaultAsync<User>(query, new
        {
            emailAddress,
            otp
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Delete(string correlationId, User item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspAdminUserDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UserId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Admin User - {item.UserId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Admin User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Admin User!");
            return false;
        }
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspAdminUserRetrieve]";
        return await dbConn.QueryAsync<User>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<User> GetOne(User item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspAdminUserRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<User>(query, new
        {
            item.UserId,
            item.EmailAddress
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Reactivate(string correlationId, User item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspAdminUserReactivate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UserId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Reactivated Admin User - {item.UserId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to reactivate Admin User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to reactivate Admin User!");
            return false;
        }
    }

    public async Task<bool> SetOtp(string correlationId, string emailAddress, string otp)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspAdminUserSetOtp]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                emailAddress,
                otp,
                OTPExpiry = DateTime.Now.AddMinutes(2)
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Set OTP for Admin User - {emailAddress}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to set OTP for Admin User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to set OTP for Admin User!");
            return false;
        }
    }

    public async Task<bool> Update(string correlationId, User item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspAdminUserUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.UserId,
                item.EmailAddress,
                item.DisplayName,
                item.OrganizationCd,
                item.RoleCd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Admin User - {item.UserId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Admin User - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Admin User!");
            return false;
        }
    }
}
