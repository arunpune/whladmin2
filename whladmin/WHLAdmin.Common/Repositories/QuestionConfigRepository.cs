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

public interface IQuestionConfigRepository : ICRUDRepository<QuestionConfig>
{
}

[ExcludeFromCodeCoverage]
public class QuestionConfigRepository : IQuestionConfigRepository
{
    private readonly ILogger<QuestionConfigRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public QuestionConfigRepository(ILogger<QuestionConfigRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, QuestionConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterQuestionAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.CategoryCd,
                item.Title,
                item.AnswerTypeCd,
                item.MinLength,
                item.MaxLength,
                item.OptionsList,
                item.HelpText,
                item.CreatedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.QuestionId = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.QuestionId > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Question Configuration - {item.QuestionId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Question Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Question Configuration!");
            return false;
        }
    }

    public async Task<bool> Delete(string correlationId, QuestionConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterQuestionDelete]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.QuestionId,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Deleted Question Configuration - {item.QuestionId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to delete Question Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to delete Question Configuration!");
            return false;
        }
    }

    public async Task<IEnumerable<QuestionConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterQuestionRetrieve]";
        return await dbConn.QueryAsync<QuestionConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<QuestionConfig> GetOne(QuestionConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspMasterQuestionRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<QuestionConfig>(query, new
        {
            QuestionID = item.QuestionId,
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Update(string correlationId, QuestionConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspMasterQuestionUpdate]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                item.QuestionId,
                item.CategoryCd,
                item.Title,
                item.AnswerTypeCd,
                item.MinLength,
                item.MaxLength,
                item.OptionsList,
                item.HelpText,
                item.Active,
                item.ModifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var result = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (result > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Updated Question Configuration - {item.QuestionId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to update Question Configuration - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to update Question Configuration!");
            return false;
        }
    }
}