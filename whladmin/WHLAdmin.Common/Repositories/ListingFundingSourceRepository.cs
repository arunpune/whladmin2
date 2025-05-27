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

public interface IListingFundingSourceRepository
{
    Task<IEnumerable<FundingSource>> GetAll(long listingId);
    Task<IEnumerable<FundingSource>> GetAllForEdit(long listingId);
    Task<bool> Save(string correlationId, string username, long listingId, string fundingSourceIds);
}

[ExcludeFromCodeCoverage]
public class ListingFundingSourceRepository : IListingFundingSourceRepository
{
    private readonly ILogger<ListingFundingSourceRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingFundingSourceRepository(ILogger<ListingFundingSourceRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<FundingSource>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingFundingSourceRetrieve]";
        return await dbConn.QueryAsync<FundingSource>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<FundingSource>> GetAllForEdit(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingFundingSourceRetrieveForEdit]";
        return await dbConn.QueryAsync<FundingSource>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Save(string correlationId, string modifiedBy, long listingId, string fundingSourceIds)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingFundingSourceSave]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                listingId,
                fundingSourceIds,
                modifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var saved = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (saved > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Saved Listing Funding Sources - {listingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to save Listing Funding Sources - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to save Listing Funding Sources!");
            return false;
        }
    }
}