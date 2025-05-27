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

public interface IHouseholdRepository
{
    Task<Household> GetOne(string username);
    Task<IEnumerable<HouseholdMember>> GetMembers(long householdId);
    Task<HouseholdMember> GetMember(long householdId, long memberId);
    Task<bool> SaveMemberProfile(string requestId, string correlationId, HouseholdMember item);
    Task<bool> DeleteMemberInfo(string requestId, string correlationId, HouseholdMember item);
    Task<IEnumerable<HouseholdAccount>> GetAccounts(long householdId);
    Task<HouseholdAccount> GetAccount(long householdId, long accountId);
    Task<bool> SaveAccountInfo(string requestId, string correlationId, HouseholdAccount item);
    Task<bool> DeleteAccountInfo(string requestId, string correlationId, HouseholdAccount item);
    Task<IEnumerable<HouseholdMemberAsset>> GetMemberAssets(long memberId);
    Task<IEnumerable<HouseholdMemberIncome>> GetMemberIncomes(long memberId);
    Task<bool> UpdateAddressInfo(string requestId, string correlationId, Household item);
    Task<bool> UpdateVoucherInfo(string requestId, string correlationId, Household item);
    Task<bool> UpdateLiveInAideInfo(string requestId, string correlationId, Household item);
}

[ExcludeFromCodeCoverage]
public class HouseholdRepository : IHouseholdRepository
{
    private readonly ILogger<HouseholdRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public HouseholdRepository(ILogger<HouseholdRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<Household> GetOne(string username)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<Household>(query, new
        {
            username
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HouseholdMember>> GetMembers(long householdId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveMembers]";
        return await dbConn.QueryAsync<HouseholdMember>(query, new
        {
            householdId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<HouseholdMember> GetMember(long householdId, long memberId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveMembers]";
        return await dbConn.QuerySingleOrDefaultAsync<HouseholdMember>(query, new
        {
            householdId,
            memberId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> SaveMemberProfile(string requestId, string correlationId, HouseholdMember item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdSaveMemberProfile]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.HouseholdId,
                item.MemberId,
                item.RelationTypeCd,
                item.RelationTypeOther,
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
                item.PhoneNumber,
                item.PhoneNumberExtn,
                item.PhoneNumberTypeCd,
                item.AltPhoneNumber,
                item.AltPhoneNumberExtn,
                item.AltPhoneNumberTypeCd,
                item.EmailAddress,
                item.AltEmailAddress,
                item.OwnRealEstateInd,
                item.RealEstateValueAmt,
                item.AssetValueAmt,
                item.IncomeValueAmt,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                item.MemberId = result;
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Saved Household Member Profile - {item.MemberId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to save Household Member Profile - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to save Household Member Profile!");
            return false;

        }
    }

    public async Task<bool> DeleteMemberInfo(string requestId, string correlationId, HouseholdMember item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdDeleteMemberInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.HouseholdId,
                item.MemberId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Household Member Information - {item.MemberId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Household Member Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Household Member Information!");
            return false;

        }
    }

    public async Task<IEnumerable<HouseholdAccount>> GetAccounts(long householdId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveAccounts]";
        return await dbConn.QueryAsync<HouseholdAccount>(query, new
        {
            householdId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<HouseholdAccount> GetAccount(long householdId, long accountId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveAccounts]";
        return await dbConn.QuerySingleOrDefaultAsync<HouseholdAccount>(query, new
        {
            householdId,
            accountId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> SaveAccountInfo(string requestId, string correlationId, HouseholdAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdSaveAccountInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.HouseholdId,
                item.AccountId,
                item.AccountNumber,
                item.AccountTypeCd,
                item.AccountTypeOther,
                item.AccountValueAmt,
                item.PrimaryHolderMemberId,
                item.InstitutionName,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Saved Household Account Information - {item.AccountId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to save Household Account Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to save Household Account Information!");
            return false;

        }
    }

    public async Task<bool> DeleteAccountInfo(string requestId, string correlationId, HouseholdAccount item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdDeleteAccountInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.HouseholdId,
                item.AccountId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Household Account Information - {item.AccountId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Household Account Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Household Account Information!");
            return false;

        }
    }

    public async Task<IEnumerable<HouseholdMemberAsset>> GetMemberAssets(long memberId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveMemberAssets]";
        return await dbConn.QueryAsync<HouseholdMemberAsset>(query, new
        {
            memberId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HouseholdMemberIncome>> GetMemberIncomes(long memberId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHouseholdRetrieveMemberIncomes]";
        return await dbConn.QueryAsync<HouseholdMemberIncome>(query, new
        {
            memberId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateAddressInfo(string requestId, string correlationId, Household item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdUpdateAddressInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.HouseholdId,
                item.Username,
                item.AddressInd,
                item.PhysicalStreetLine1,
                item.PhysicalStreetLine2,
                item.PhysicalStreetLine3,
                item.PhysicalCity,
                item.PhysicalStateCd,
                item.PhysicalZipCode,
                item.DifferentMailingAddressInd,
                item.MailingStreetLine1,
                item.MailingStreetLine2,
                item.MailingStreetLine3,
                item.MailingCity,
                item.MailingStateCd,
                item.MailingZipCode,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Household Address Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Address Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Address Information!");
            return false;

        }
    }

    public async Task<bool> UpdateVoucherInfo(string requestId, string correlationId, Household item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdUpdateVoucherInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.HouseholdId,
                item.Username,
                item.VoucherInd,
                item.VoucherCds,
                item.VoucherOther,
                item.VoucherAdminName,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Household Voucher Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Voucher Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Voucher Information!");
            return false;

        }
    }

    public async Task<bool> UpdateLiveInAideInfo(string requestId, string correlationId, Household item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHouseholdUpdateLiveInAideInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.HouseholdId,
                item.Username,
                item.LiveInAideInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Household Live-in Aide Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Live-in Aide Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Household Live-in Aide Information!");
            return false;

        }
    }
}