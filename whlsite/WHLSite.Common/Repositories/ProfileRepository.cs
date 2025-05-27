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

public interface IProfileRepository
{
    Task<IEnumerable<UserAccount>> GetAll();
    Task<UserAccount> GetOne(string username);
    Task<bool> UpdateProfile(string requestId, string correlationId, UserAccount item);
    Task<bool> UpdateAddressInfo(string requestId, string correlationId, Household item);
    Task<bool> UpdateContactInfo(string requestId, string correlationId, UserAccount item);
    Task<bool> UpdatePreferences(string requestId, string correlationId, UserAccount item);
    Task<bool> UpdateNetWorth(string requestId, string correlationId, UserAccount item);
    Task<IEnumerable<UserNotification>> GetNotifications(string requestId, string correlationId, string username, string filterTypeCd = null);
    Task<bool> UpdateNotification(string requestId, string correlationId, UserNotification item);
    Task<bool> DeleteNotification(string requestId, string correlationId, UserNotification item);
    Task<IEnumerable<UserDocument>> GetDocuments(string requestId, string correlationId, string username);
    Task<UserDocument> GetDocument(string requestId, string correlationId, string username, long docId);
    Task<byte[]> GetDocumentContents(string requestId, string correlationId, string username, long docId);
    Task<bool> AddDocument(string requestId, string correlationId, UserDocument item);
    Task<bool> DeleteDocument(string requestId, string correlationId, UserDocument item);
}

[ExcludeFromCodeCoverage]
public class ProfileRepository : IProfileRepository
{
    private readonly ILogger<ProfileRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ProfileRepository(ILogger<ProfileRepository> logger, IDbProvider dbProvider)
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

    public async Task<bool> UpdateProfile(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserUpdateAccountProfile]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.Title,
                item.FirstName,
                item.MiddleName,
                item.LastName,
                item.Suffix,
                item.DateOfBirth,
                item.Last4SSN,
                item.IdTypeCd,
                item.IdTypeValue,
                item.IdIssueDate,
                item.GenderCd,
                item.RaceCd,
                item.EthnicityCd,
                item.StudentInd,
                item.DisabilityInd,
                item.VeteranInd,
                item.Pronouns,
                item.EverLivedInWestchesterInd,
                item.CountyLivingIn,
                item.CurrentlyWorkingInWestchesterInd,
                item.CountyWorkingIn,
                item.HouseholdSize,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Profile - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Profile - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Profile!");
            return false;
        }
    }

    public async Task<bool> UpdateAddressInfo(string requestId, string correlationId, Household item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserUpdateAddressInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.AddressInd,
                item.PhysicalStreetLine1,
                item.PhysicalStreetLine2,
                item.PhysicalStreetLine3,
                item.PhysicalCity,
                item.PhysicalStateCd,
                item.PhysicalZipCode,
                item.PhysicalCounty,
                item.DifferentMailingAddressInd,
                item.MailingStreetLine1,
                item.MailingStreetLine2,
                item.MailingStreetLine3,
                item.MailingCity,
                item.MailingStateCd,
                item.MailingZipCode,
                item.MailingCounty,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Address Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Address Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Address Information!");
            return false;
        }
    }

    public async Task<bool> UpdateContactInfo(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserUpdateContactInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.EmailAddress,
                item.AuthRepEmailAddressInd,
                item.AltEmailAddress,
                item.AltEmailAddressKey,
                item.AltEmailAddressKeyExpiry,
                item.AltEmailAddressVerifiedInd,
                item.PhoneNumberTypeCd,
                item.PhoneNumber,
                item.PhoneNumberExtn,
                item.PhoneNumberKey,
                item.PhoneNumberKeyExpiry,
                item.PhoneNumberVerifiedInd,
                item.AltPhoneNumberTypeCd,
                item.AltPhoneNumber,
                item.AltPhoneNumberExtn,
                item.AltPhoneNumberKey,
                item.AltPhoneNumberKeyExpiry,
                item.AltPhoneNumberVerifiedInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Contact Info - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Contact Info - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Contact Info!");
            return false;
        }
    }

    public async Task<bool> UpdatePreferences(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserUpdateAccountPreferences]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.LanguagePreferenceCd,
                item.LanguagePreferenceOther,
                item.ListingPreferenceCd,
                item.SmsNotificationsPreferenceInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Preferences - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Preferences - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Preferences!");
            return false;
        }
    }

    public async Task<bool> UpdateNetWorth(string requestId, string correlationId, UserAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserUpdateAccountNetWorth]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.OwnRealEstateInd,
                item.RealEstateValueAmt,
                item.AssetValueAmt,
                item.IncomeValueAmt,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Net Worth - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Net Worth - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Net Worth!");
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
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Net Worth - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Net Worth - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Net Worth!");
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
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Site User Net Worth - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Net Worth - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Net Worth!");
            return false;
        }
    }

    public async Task<IEnumerable<UserDocument>> GetDocuments(string requestId, string correlationId, string username)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserDocumentRetrieve]";
        return await dbConn.QueryAsync<UserDocument>(query, new
        {
            username
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<UserDocument> GetDocument(string requestId, string correlationId, string username, long docId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserDocumentRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<UserDocument>(query, new
        {
            username,
            docId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<byte[]> GetDocumentContents(string requestId, string correlationId, string username, long docId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteUserDocumentRetrieveContents]";
        return await dbConn.QuerySingleOrDefaultAsync<byte[]>(query, new
        {
            username,
            docId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> AddDocument(string requestId, string correlationId, UserDocument item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserDocumentAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.DocTypeCd,
                item.DocName,
                item.FileName,
                item.MimeType,
                item.DocContents,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Site User Document - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Document - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Site User Document!");
            return false;
        }
    }

    public async Task<bool> DeleteDocument(string requestId, string correlationId, UserDocument item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteUserDocumentDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.DocId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Site User Document - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Document - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Site User Document!");
            return false;
        }
    }
}