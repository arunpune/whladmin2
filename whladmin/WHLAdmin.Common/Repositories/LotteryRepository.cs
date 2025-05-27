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

public interface ILotteryRepository : IReadOnlyRepository<Lottery>
{
    Task<IEnumerable<Listing>> GetEligibleListings();
    Task<int> RunLottery(string requestId, string correlationId, string username, long listingId, bool rerun = false);
    Task<IEnumerable<HousingApplication>> GetResults(int lotteryId);
    Task<PagedRecords<HousingApplication>> GetResultsPaged(int lotteryId, int pageNo = 1, int pageSize = 1000);
    Task<DataTable> GetDownload(int lotteryId);
}

[ExcludeFromCodeCoverage]
public class LotteryRepository : ILotteryRepository
{
    private readonly ILogger<LotteryRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public LotteryRepository(ILogger<LotteryRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Lottery>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspLotteryRetrieve]";
        return await dbConn.QueryAsync<Lottery>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<Lottery> GetOne(Lottery item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspLotteryRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<Lottery>(query, new
        {
            LotteryID = item.LotteryId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<Listing>> GetEligibleListings()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspLotteryRetrieveEligibleListings]";
        return await dbConn.QueryAsync<Listing>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<int> RunLottery(string requestId, string correlationId, string username, long listingId, bool rerun = false)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspLotteryGenerator]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                ListingID = listingId,
                RunBy = username,
                ManualInd = true,
                ReRunInd = rerun
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Ran Lottery Generator manually for {listingId} - {result}");
                return result;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to run Lottery Generator manually for {listingId} - {errorMessage ?? "Database exception"}");
            return 0;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to run Lottery Generator manually!");
            return 0;

        }
    }

    public async Task<IEnumerable<HousingApplication>> GetResults(int lotteryId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspLotteryRetrieveResults]";
        return await dbConn.QueryAsync<HousingApplication>(query, new
        {
            lotteryId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<PagedRecords<HousingApplication>> GetResultsPaged(int lotteryId, int pageNo = 1, int pageSize = 1000)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspLotteryRetrieveResultsPaged]";
        var results = await dbConn.QueryMultipleAsync(query, new
        {
            LotteryID = lotteryId,
            PageNo = pageNo,
            PageSize = pageSize
        }, commandType: CommandType.StoredProcedure);
        return new PagedRecords<HousingApplication>()
        {
            Records = results.Read<HousingApplication>(),
            PagingInfo = results.ReadSingleOrDefault<PagingInfo>(),
        };
    }

    public async Task<DataTable> GetDownload(int lotteryId)
    {
        using var dbConn = _dbProvider.GetConnection();

        dbConn.Open();

        var query = "[dbo].[uspLotteryDownloadResults]";
        var dataReader = await dbConn.ExecuteReaderAsync(query, new
        {
            LotteryID = lotteryId
        }, commandType: CommandType.StoredProcedure);

        var dataTable = new DataTable();
        if (!dataReader.IsClosed)
        {
            dataTable.Load(dataReader);
        }
        return dataTable;
    }
}