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

public interface IListingDocumentTypeRepository
{
    Task<IEnumerable<DocumentType>> GetAll(long listingId);
    Task<IEnumerable<DocumentType>> GetAllForEdit(long listingId);
    Task<bool> Save(string correlationId, string username, long listingId, string documentTypeIds);
}

[ExcludeFromCodeCoverage]
public class ListingDocumentTypeRepository : IListingDocumentTypeRepository
{
    private readonly ILogger<ListingDocumentTypeRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingDocumentTypeRepository(ILogger<ListingDocumentTypeRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<DocumentType>> GetAll(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingDocumentTypeRetrieve]";
        return await dbConn.QueryAsync<DocumentType>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<IEnumerable<DocumentType>> GetAllForEdit(long listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspListingDocumentTypeRetrieveForEdit]";
        return await dbConn.QueryAsync<DocumentType>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> Save(string correlationId, string modifiedBy, long listingId, string documentTypeIds)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        try
        {
            var query = "[dbo].[uspListingDocumentTypeSave]";
            var queryParams = new DynamicParameters();
            queryParams.AddDynamicParams(new
            {
                listingId,
                documentTypeIds,
                modifiedBy
            });
            queryParams.Add("ErrorMessage", dbType: DbType.String, direction: ParameterDirection.Output, size: 1000);
            var saved = await dbConn.ExecuteScalarAsync<int>(query, param: queryParams, commandType: CommandType.StoredProcedure);

            if (saved > 0)
            {
                _logger.LogDebug($"CorrelationID: {correlationId}, Message: Saved Listing Document Types - {listingId}");
                return true;
            }

            var errorMessage = queryParams.Get<string>("ErrorMessage");
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to save Listing Document Types - {errorMessage ?? "Database exception"}");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"CorrelationID: {correlationId}, Message: Failed to save Listing Document Types!");
            return false;
        }
    }
}