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

public interface ISiteHousingApplicationRepository
{
    Task<bool> Add(string requestId, string correlationId, HousingApplication item);
    Task<bool> Delete(string requestId, string correlationId, HousingApplication item);
    Task<IEnumerable<HousingApplication>> GetAll(string requestId, string correlationId, string username);
    Task<IEnumerable<HousingApplication>> GetAllByListing(string requestId, string correlationId, string username, int listingId);
    Task<HousingApplication> GetOne(string requestId, string correlationId, string username, long applicationId);
    Task<HousingApplication> GetOneByListing(string requestId, string correlationId, string username, int listingId);
    Task<bool> Update(string requestId, string correlationId, HousingApplication item);
    Task<bool> UpdateHouseholdInfo(string requestId, string correlationId, HousingApplication item);
    Task<bool> UpdateMembers(string requestId, string correlationId, HousingApplication item);
    Task<bool> UpdateAccounts(string requestId, string correlationId, HousingApplication item);
    Task<bool> Submit(string requestId, string correlationId, HousingApplication item);
    Task<long> SubmitEx(string requestId, string correlationId, HousingApplication item);
    Task<bool> Withdraw(string requestId, string correlationId, HousingApplication item);

    Task<IEnumerable<ApplicationDocument>> GetDocuments(string requestId, string correlationId, string username, long applicationId);
    Task<ApplicationDocument> GetDocument(string requestId, string correlationId, string username, long applicationId, long docId);
    Task<byte[]> GetDocumentContents(string requestId, string correlationId, string username, long applicationId, long docId);
    Task<bool> AddDocument(string requestId, string correlationId, ApplicationDocument item);
    Task<bool> DeleteDocument(string requestId, string correlationId, ApplicationDocument item);

    Task<IEnumerable<ApplicationComment>> GetComments(string requestId, string correlationId, string username, long applicationId);
    Task<bool> AddComment(string requestId, string correlationId, ApplicationComment item);
}

[ExcludeFromCodeCoverage]
public class SiteHousingApplicationRepository : ISiteHousingApplicationRepository
{
    private readonly ILogger<ProfileRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public SiteHousingApplicationRepository(ILogger<ProfileRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
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
                item.PhoneNumberTypeCd,
                item.PhoneNumber,
                item.PhoneNumberExtn,
                item.AltPhoneNumberTypeCd,
                item.AltPhoneNumber,
                item.AltPhoneNumberExtn,
                item.EmailAddress,
                item.AltEmailAddress,
                item.OwnRealEstateInd,
                item.RealEstateValueAmt,
                item.AssetValueAmt,
                item.IncomeValueAmt,
                item.LeadTypeCd,
                item.LeadTypeOther,
                item.UpdateProfileInd,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                item.ApplicationId = result;
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Housing Application - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Housing Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Housing Application!");
            return false;
        }
    }

    public async Task<bool> Delete(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Housing Application - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Housing Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Housing Application!");
            return false;
        }
    }

    public async Task<IEnumerable<HousingApplication>> GetAll(string requestId, string correlationId, string username)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHousingApplicationRetrieve]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            Username = username
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplication>> GetAllByListing(string requestId, string correlationId, string username, int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHousingApplicationRetrieve]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            Username = username,
            ListingId = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<HousingApplication> GetOne(string requestId, string correlationId, string username, long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHousingApplicationRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<HousingApplication>(query, new
        {
            Username = username,
            ApplicationID = applicationId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<HousingApplication> GetOneByListing(string requestId, string correlationId, string username, int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHousingApplicationRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<HousingApplication>(query, new
        {
            Username = username,
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
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
                item.PhoneNumberTypeCd,
                item.PhoneNumber,
                item.PhoneNumberExtn,
                item.AltPhoneNumberTypeCd,
                item.AltPhoneNumber,
                item.AltPhoneNumberExtn,
                item.EmailAddress,
                item.AltEmailAddress,
                item.OwnRealEstateInd,
                item.RealEstateValueAmt,
                item.AssetValueAmt,
                item.IncomeValueAmt,
                item.UpdateProfileInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Housing Application - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Housing Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Housing Application!");
            return false;
        }
    }

    public async Task<bool> UpdateHouseholdInfo(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationUpdateHouseholdInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
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
                item.VoucherInd,
                item.VoucherCds,
                item.VoucherOther,
                item.VoucherAdminName,
                item.LiveInAideInd,
                item.UnitTypeCds,
                item.UpdateProfileInd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Housing Application Household Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Household Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Household Information!");
            return false;
        }
    }

    public async Task<bool> UpdateMembers(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationUpdateMembers]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Username,
                item.CoApplicantMemberId,
                item.MemberIds,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Housing Application Members Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Members Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Members Information!");
            return false;
        }
    }

    public async Task<bool> UpdateAccounts(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationUpdateAccounts]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Username,
                item.AccountIds,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Housing Application Accounts Information - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Accounts Information - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Accounts Information!");
            return false;
        }
    }

    public async Task<bool> Submit(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationSubmit]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Username,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Submitted Housing Application - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to submit Housing Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to submit Housing Application!");
            return false;
        }
    }

    public async Task<long> SubmitEx(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationSubmitEx]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.ListingId,
                item.Username,
                item.UnitTypeCds,
                item.MemberIds,
                item.CoApplicantMemberId,
                item.AccountIds,
                item.AccessibilityCds,
                item.LeadTypeCd,
                item.LeadTypeOther,
                item.StatusCd,
                item.ModifiedBy,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Submitted Housing Application - {item.Username}");
                return result;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to submit Housing Application - {errorMessage ?? "Database exception"}");
            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to submit Housing Application!");
            return -1;
        }
    }

    public async Task<bool> Withdraw(string requestId, string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationWithdraw]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Username,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Withdrew Housing Application - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to withdraw Housing Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to withdraw Housing Application!");
            return false;
        }
    }

    public async Task<IEnumerable<ApplicationDocument>> GetDocuments(string requestId, string correlationId, string username, long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationDocumentRetrieve]";
        return await dbConn.QueryAsync<ApplicationDocument>(query, new
        {
            username,
            applicationId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ApplicationDocument> GetDocument(string requestId, string correlationId, string username, long applicationId, long docId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationDocumentRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ApplicationDocument>(query, new
        {
            username,
            applicationId,
            docId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<byte[]> GetDocumentContents(string requestId, string correlationId, string username, long applicationId, long docId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationDocumentRetrieveContents]";
        return await dbConn.QuerySingleOrDefaultAsync<byte[]>(query, new
        {
            username,
            applicationId,
            docId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> AddDocument(string requestId, string correlationId, ApplicationDocument item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationDocumentAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ApplicationId,
                item.DocTypeId,
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
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Housing Application Document - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Document - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Document!");
            return false;
        }
    }

    public async Task<bool> DeleteDocument(string requestId, string correlationId, ApplicationDocument item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationDocumentDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ApplicationId,
                item.DocId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Deleted Housing Application Document - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Housing Application Document - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to delete Housing Application Document!");
            return false;
        }
    }

    public async Task<IEnumerable<ApplicationComment>> GetComments(string requestId, string correlationId, string username, long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteHousingApplicationCommentRetrieve]";
        return await dbConn.QueryAsync<ApplicationComment>(query, new
        {
            username,
            applicationId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> AddComment(string requestId, string correlationId, ApplicationComment item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspSiteHousingApplicationCommentAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.Username,
                item.ApplicationId,
                item.Comments,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Updated Housing Application Comment - {item.Username}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Comment - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestID: {requestId}, CorrelationID: {correlationId}, Message: Failed to update Housing Application Comment!");
            return false;
        }
    }
}