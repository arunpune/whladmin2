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

public interface IHousingApplicationRepository
{
    Task<bool> Add(string correlationId, HousingApplication item);
    Task<bool> Delete(string correlationId, HousingApplication item);
    Task<HousingApplication> GetOne(HousingApplication item);
    Task<bool> Update(string correlationId, HousingApplication item);

    Task<IEnumerable<HousingApplication>> GetAll(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL");
    Task<PagedRecords<HousingApplication>> GetAllPaged(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL", int pageNo = 1, int pageSize = 100);
    Task<DataTable> GetDownload(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL");
    Task<IEnumerable<HousingApplication>> GetAllByLast4SSN(long listingId, string last4SSN);
    Task<bool> UpdateHouseholdInfo(string correlationId, HousingApplication item);

    Task<IEnumerable<HousingApplicant>> GetApplicants(long applicationId);
    Task<IEnumerable<HousingApplicantAsset>> GetApplicantAccounts(long applicationId);

    Task<Dictionary<string, IEnumerable<DuplicateApplication>>> GetPotentialDuplicates(long listingId);
    Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByDateOfBirthLast4Ssn(long listingId, string dateOfBirth, string last4SSN);
    Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByName(long listingId, string name);
    Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByEmailAddress(long listingId, string emailAddress);
    Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByPhoneNumber(long listingId, string phoneNumber);
    Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByStreetAddress(long listingId, string streetAddress);
    Task<bool> UpdateDuplicateStatus(string correlationId, HousingApplication item);
    Task<IEnumerable<ApplicationComment>> GetComments(string requestId, string correlationId, string username, long applicationId);
    Task<bool> AddComment(string requestId, string correlationId, ApplicationComment comment);
}

[ExcludeFromCodeCoverage]
public class HousingApplicationRepository : IHousingApplicationRepository
{
    private readonly ILogger<HousingApplicationRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public HousingApplicationRepository(ILogger<HousingApplicationRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<HousingApplication>> GetAll(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL")
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieve]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            SubmissionTypeCD = submissionTypeCd,
            StatusCD = statusCd
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedRecords<HousingApplication>> GetAllPaged(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL", int pageNo = 1, int pageSize = 1000)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrievePaged]";
        var results = await dbConn.QueryMultipleAsync(query, new
        {
            ListingID = listingId,
            SubmissionTypeCD = submissionTypeCd,
            StatusCD = statusCd,
            PageNo = pageNo,
            PageSize = pageSize
        }, commandType: CommandType.StoredProcedure);
        return new PagedRecords<HousingApplication>()
        {
            Records = results.Read<HousingApplication>(),
            PagingInfo = results.ReadSingleOrDefault<PagingInfo>(),
        };
    }

    public async Task<DataTable> GetDownload(long listingId, string submissionTypeCd = "ALL", string statusCd = "ALL")
    {
        using var dbConn = _dbProvider.GetConnection();

        dbConn.Open();

        var query = "[dbo].[uspHousingApplicationDownload]";
        var dataReader = await dbConn.ExecuteReaderAsync(query, new
        {
            ListingID = listingId,
            SubmissionTypeCD = submissionTypeCd,
            StatusCD = statusCd
        }, commandType: CommandType.StoredProcedure);

        var dataTable = new DataTable();
        if (!dataReader.IsClosed)
        {
            dataTable.Load(dataReader);
        }
        return dataTable;
    }

    public async Task<IEnumerable<HousingApplication>> GetAllByLast4SSN(long listingId, string last4SSN)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveByLast4SSN]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            Last4SSN = last4SSN
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<HousingApplication> GetOne(HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<HousingApplication>(query, new
        {
            ApplicationID = item.ApplicationId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplicant>> GetApplicants(long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationApplicantRetrieve]";
        return await dbConn.QueryAsync<HousingApplicant>(query, new
        {
            applicationId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplicantAsset>> GetApplicantAccounts(long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationApplicantAccountRetrieve]";
        return await dbConn.QueryAsync<HousingApplicantAsset>(query, new
        {
            applicationId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Add(string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.Username,
                item.Title,
                item.FirstName,
                item.MiddleName,
                item.LastName,
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
                item.SubmissionTypeCd,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ApplicationId = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ApplicationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Lottery Application - {item.ApplicationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Lottery Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Lottery Application!");
            return false;

        }
    }

    public async Task<bool> Update(string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Title,
                item.FirstName,
                item.MiddleName,
                item.LastName,
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
                item.SubmissionTypeCd,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ApplicationId = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ApplicationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Lottery Application - {item.ApplicationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Lottery Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Lottery Application!");
            return false;

        }
    }

    public async Task<bool> UpdateHouseholdInfo(string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationUpdateHouseholdInfo]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.ListingId,
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
                item.VoucherInd,
                item.VoucherCds,
                item.VoucherOther,
                item.VoucherAdminName,
                item.Username,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ApplicationId = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ApplicationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Lottery Application - {item.ApplicationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Lottery Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Lottery Application!");
            return false;

        }
    }

    public async Task<bool> Delete(string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.Username,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ApplicationId = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ApplicationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Lottery Application - {item.ApplicationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Lottery Application - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Lottery Application!");
            return false;

        }
    }

    public async Task<Dictionary<string, IEnumerable<DuplicateApplication>>> GetPotentialDuplicates(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicates]";
        var results = await dbConn.QueryMultipleAsync(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
        var duplicates = new Dictionary<string, IEnumerable<DuplicateApplication>>
            {
                { "SSN", results.Read<DuplicateApplication>() },
                { "NAME", results.Read<DuplicateApplication>() },
                { "EMAIL", results.Read<DuplicateApplication>() },
                { "PHONE", results.Read<DuplicateApplication>() },
                { "ADDRESS", results.Read<DuplicateApplication>() },
            };
        return duplicates;
    }

    public async Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByDateOfBirthLast4Ssn(long listingId, string dateOfBirth, string last4Ssn)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicatesByDoBLast4SSN]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            DateOfBirth = dateOfBirth,
            Last4SSN = last4Ssn
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByName(long listingId, string name)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicatesByName]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            Name = name
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByEmailAddress(long listingId, string emailAddress)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicatesByEmailAddress]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            EmailAddress = emailAddress
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByPhoneNumber(long listingId, string phoneNumber)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicatesByPhoneNumber]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            PhoneNumber = phoneNumber
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<HousingApplication>> GetPotentialDuplicatesByStreetAddress(long listingId, string streetAddress)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationRetrieveDuplicatesByStreetAddress]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            ListingID = listingId,
            StreetAddress = streetAddress
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateDuplicateStatus(string correlationId, HousingApplication item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationUpdateDuplicateStatus]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ApplicationId,
                item.DuplicateCheckCd,
                item.DuplicateReason,
                item.DuplicateCheckResponseDueDate,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ApplicationId = await dbConn.ExecuteScalarAsync<long>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ApplicationId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Lottery Application duplicate status - {item.ApplicationId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to mark Lottery Application duplicate status - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to mark Lottery Application duplicate status!");
            return false;

        }
    }

    public async Task<IEnumerable<ApplicationComment>> GetComments(string requestId, string correlationId, string username, long applicationId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspHousingApplicationCommentRetrieve]";
        return await dbConn.QueryAsync<ApplicationComment>(query, new
        {
            applicationId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> AddComment(string requestId, string correlationId, ApplicationComment comment)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspHousingApplicationCommentAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                comment.ApplicationId,
                comment.Comments,
                comment.InternalOnlyInd,
                comment.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added comment for {comment.ApplicationId} - {comment.Comments}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add comment for {comment.ApplicationId} - {comment.Comments} - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add comment for {comment.ApplicationId} - {comment.Comments}!");
            return false;

        }
    }
}