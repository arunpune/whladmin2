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

public interface IReportRepository
{
    Task<IEnumerable<ApplicationDemographicRecord>> GetApplicationDemographicsReport(int fromDate, int toDate = 0, long listingId = 0);
    Task<IEnumerable<RegistrationSummaryByStateRecord>> GetRegistrationsSummaryByStateReport(int fromDate, int toDate = 0);
    Task<IEnumerable<RegistrationSummaryByCountyRecord>> GetRegistrationsSummaryByCountyReport(int fromDate, int toDate = 0, string stateCd = null);
    Task<IEnumerable<CategoryCountPercentage>> GetMarketEvaluationsReport(long listingId, int fromDate, int toDate = 0);
}

[ExcludeFromCodeCoverage]
public class ReportRepository : IReportRepository
{
    private readonly ILogger<ReportRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ReportRepository(ILogger<ReportRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ApplicationDemographicRecord>> GetApplicationDemographicsReport(int fromDate, int toDate = 0, long listingId = 0)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspReportRetrieveAFHMByDemographic]";
        return await dbConn.QueryAsync<ApplicationDemographicRecord>(query, new
        {
            fromDate,
            toDate,
            listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RegistrationSummaryByStateRecord>> GetRegistrationsSummaryByStateReport(int fromDate, int toDate = 0)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspReportRetrieveCISByState]";
        return await dbConn.QueryAsync<RegistrationSummaryByStateRecord>(query, new
        {
            fromDate,
            toDate
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<RegistrationSummaryByCountyRecord>> GetRegistrationsSummaryByCountyReport(int fromDate, int toDate = 0, string stateCd = null)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspReportRetrieveCISByCounty]";
        return await dbConn.QueryAsync<RegistrationSummaryByCountyRecord>(query, new
        {
            fromDate,
            toDate,
            stateCd
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<CategoryCountPercentage>> GetMarketEvaluationsReport(long listingId, int fromDate, int toDate = 0)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspReportRetrieveMarketEvaluationByListing]";
        return await dbConn.QueryAsync<CategoryCountPercentage>(query, new
        {
            listingId,
            fromDate,
            toDate
        }, commandType: CommandType.StoredProcedure);
    }
}