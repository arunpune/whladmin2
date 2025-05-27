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

public interface IListingRepository
{
    Task<bool> Add(string correlationId, Listing item);
    Task<bool> Delete(string correlationId, Listing item);
    Task<IEnumerable<Listing>> GetAll();
    Task<PagedRecords<Listing>> GetAllPaged(int pageNo = 1, int pageSize = 100);
    Task<Listing> GetOne(Listing item);
    Task<bool> Update(string correlationId, Listing item);

    Task<bool> UpdateAddress(string correlationId, Listing item);
    Task<bool> UpdateDates(string correlationId, Listing item);
    Task<bool> UpdatePetsAllowed(string correlationId, Listing item);
    Task<bool> UpdateMarketingAgent(string requestId, string correlationId, Listing item);
    Task<bool> UpdateStatus(string correlationId, Listing item, string note);
    Task<bool> Publish(string correlationId, Listing item, string note);
    Task<bool> Unpublish(string correlationId, Listing item, string note);

    Task<IEnumerable<Listing>> GetPublishedListings();
    Task<Listing> GetPublishedListing(long listingId);
}

[ExcludeFromCodeCoverage]
public class ListingRepository : IListingRepository
{
    private readonly ILogger<ListingRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingRepository(ILogger<ListingRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingTypeCd,
                item.ResaleInd,
                item.ListingAgeTypeCd,
                item.Name,
                item.WebsiteUrl,
                item.Description,
                item.StreetLine1,
                item.StreetLine2,
                item.StreetLine3,
                item.City,
                item.StateCd,
                item.ZipCode,
                item.County,
                item.Municipality,
                item.MunicipalityUrl,
                item.SchoolDistrict,
                item.SchoolDistrictUrl,
                item.MapUrl,
                item.RentIncludesText,
                item.CompletedOrInitialOccupancyYear,
                item.TermOfAffordability,
                item.EsriX,
                item.EsriY,
                item.StatusCd,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.ListingId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.ListingId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Listing Configuration - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Listing Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Listing Configuration!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Listing Configuration - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Listing Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Listing Configuration!");
            return false;
        }
    }

    public async Task<IEnumerable<Listing>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingRetrieve]";
        return await dbConn.QueryAsync<Listing>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedRecords<Listing>> GetAllPaged(int pageNo = 1, int pageSize = 100)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingRetrievePaged]";
        var results = await dbConn.QueryMultipleAsync(query, new
        {
            PageNo = pageNo,
            PageSize = pageSize
        }, commandType: CommandType.StoredProcedure);
        return new PagedRecords<Listing>()
        {
            Records = results.Read<Listing>(),
            PagingInfo = results.ReadSingleOrDefault<PagingInfo>(),
        };
    }

    public async Task<Listing> GetOne(Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<Listing>(query, new
        {
            ListingID = item.ListingId,
            StatusCD = item.StatusCd
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.ListingTypeCd,
                item.ResaleInd,
                item.ListingAgeTypeCd,
                item.Name,
                item.Description,
                item.WebsiteUrl,
                item.StreetLine1,
                item.StreetLine2,
                item.StreetLine3,
                item.City,
                item.StateCd,
                item.ZipCode,
                item.County,
                item.Municipality,
                item.MunicipalityUrl,
                item.SchoolDistrict,
                item.SchoolDistrictUrl,
                item.MapUrl,
                item.RentIncludesText,
                item.CompletedOrInitialOccupancyYear,
                item.TermOfAffordability,
                item.EsriX,
                item.EsriY,
                item.ModifiedBy,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Configuration - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Configuration!");
            return false;
        }
    }

    public async Task<bool> UpdateAddress(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.StreetLine1,
                item.StreetLine2,
                item.StreetLine3,
                item.City,
                item.StateCd,
                item.ZipCode,
                item.County,
                item.Municipality,
                item.MunicipalityUrl,
                item.SchoolDistrict,
                item.SchoolDistrictUrl,
                item.MapUrl,
                item.EsriX,
                item.EsriY,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Address - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Address - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Address!");
            return false;
        }
    }

    public async Task<bool> UpdateDates(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUpdateDates]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.ListingStartDate,
                item.ListingEndDate,
                item.ApplicationStartDate,
                item.ApplicationEndDate,
                item.LotteryEligible,
                item.LotteryDate,
                item.WaitlistEligible,
                item.WaitlistStartDate,
                item.WaitlistEndDate,
                item.ModifiedBy,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Dates - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Dates - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Dates!");
            return false;
        }
    }

    public async Task<bool> UpdatePetsAllowed(string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUpdatePetsAllowed]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.PetsAllowedInd,
                item.PetsAllowedText,
                item.ModifiedBy,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Pet Policy - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Pet Policy - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Pet Policy!");
            return false;
        }
    }

    public async Task<bool> UpdateMarketingAgent(string requestId, string correlationId, Listing item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUpdateMarketingAgent]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.MarketingAgentInd,
                item.MarketingAgentId,
                item.MarketingAgentApplicationLink,
                item.ModifiedBy,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Marketing Agent - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Marketing Agent - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Marketing Agent!");
            return false;
        }
    }

    public async Task<bool> UpdateStatus(string correlationId, Listing item, string note)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUpdateStatus]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.StatusCd,
                note,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Listing Status - {item.ListingId}, {item.StatusCd}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Listing Status - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Listing Status!");
            return false;
        }
    }

    public async Task<bool> Publish(string correlationId, Listing item, string note)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingPublish]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.StatusCd,
                note,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Published Listing - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to publish Listing - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to publish Listing!");
            return false;
        }
    }

    public async Task<bool> Unpublish(string correlationId, Listing item, string note)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingUnpublish]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.ListingId,
                item.StatusCd,
                note,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Unpublished Listing - {item.ListingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to unpublish Listing - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to unpublish Listing!");
            return false;
        }
    }

    public async Task<IEnumerable<Listing>> GetPublishedListings()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingRetrieve]";
        return await dbConn.QueryAsync<Listing>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<Listing> GetPublishedListing(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingRetrieve]";
        return await dbConn.QueryFirstOrDefaultAsync<Listing>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }
}