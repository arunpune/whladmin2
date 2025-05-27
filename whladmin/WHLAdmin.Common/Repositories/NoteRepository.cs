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

public interface INoteRepository
{
    Task<bool> Add(string correlationId, Note item);
    Task<IEnumerable<Note>> GetAll(string entityTypeCd, string entityId);
}

[ExcludeFromCodeCoverage]
public class NoteRepository : INoteRepository
{
    private readonly ILogger<NoteRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public NoteRepository(ILogger<NoteRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<bool> Add(string correlationId, Note item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspNoteAdd]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                EntityTypeCD = item.EntityTypeCd,
                EntityID = item.EntityId,
                Note = item.Note,
                Username = item.Username,
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            item.Id = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (item.Id > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Added Note - {item.Id}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to add Note - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to add Note!");
            return false;

        }
    }


    public async Task<IEnumerable<Note>> GetAll(string entityTypeCd, string entityId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspNoteRetrieve]";
        return await dbConn.QueryAsync<Note>(query, new
        {
            EntityTypeCD = entityTypeCd,
            EntityID = entityId
        }, commandType: CommandType.StoredProcedure);
    }
}