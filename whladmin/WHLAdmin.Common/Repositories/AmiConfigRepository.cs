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

public interface IAmiConfigRepository : ICRUDRepository<AmiConfig>
{
}

[ExcludeFromCodeCoverage]
public class AmiConfigRepository : IAmiConfigRepository
{
    private readonly ILogger<AmiConfigRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public AmiConfigRepository(ILogger<AmiConfigRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, AmiConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterAMIAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.EffectiveDate,
                item.EffectiveYear,
                item.IncomeAmt,
                item.Hh1,
                item.Hh2,
                item.Hh3,
                item.Hh4,
                item.Hh5,
                item.Hh6,
                item.Hh7,
                item.Hh8,
                item.Hh9,
                item.Hh10,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added AMI Configuration - {item.EffectiveDate}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add AMI Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add AMI Configuration!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, AmiConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterAMIDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.EffectiveDate,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted AMI Configuration - {item.EffectiveDate}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete AMI Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete AMI Configuration!");
            return false;
        }
    }

    public async Task<IEnumerable<AmiConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterAMIRetrieve]";
        return await dbConn.QueryAsync<AmiConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<AmiConfig> GetOne(AmiConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterAMIRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<AmiConfig>(query, new
        {
            item.EffectiveDate,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, AmiConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterAMIUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.EffectiveDate,
                item.EffectiveYear,
                item.IncomeAmt,
                item.Hh1,
                item.Hh2,
                item.Hh3,
                item.Hh4,
                item.Hh5,
                item.Hh6,
                item.Hh7,
                item.Hh8,
                item.Hh9,
                item.Hh10,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated AMI Configuration - {item.EffectiveDate}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update AMI Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update AMI Configuration!");
            return false;
        }
    }
}