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

public interface IListingDocumentRepository
{
    Task<IEnumerable<ListingDocument>> GetAll(int listingId);
    Task<ListingDocument> GetOne(long documentId);
}

[ExcludeFromCodeCoverage]
public class ListingDocumentRepository : IListingDocumentRepository
{
    private readonly ILogger<ListingDocumentRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingDocumentRepository(ILogger<ListingDocumentRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<ListingDocument>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingDocumentRetrieve]";
        return await dbConn.QueryAsync<ListingDocument>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }

    public async Task<ListingDocument> GetOne(long documentId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingDocumentRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<ListingDocument>(query, new
        {
            DocumentID = documentId,
        }, commandType: CommandType.StoredProcedure);
    }
}