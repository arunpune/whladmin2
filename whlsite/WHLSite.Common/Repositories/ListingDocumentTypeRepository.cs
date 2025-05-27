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

public interface IListingDocumentTypeRepository
{
    Task<IEnumerable<DocumentType>> GetAll(int listingId);
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

    public async Task<IEnumerable<DocumentType>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingDocumentTypeRetrieve]";
        return await dbConn.QueryAsync<DocumentType>(query, new
        {
            ListingID = listingId
        }, commandType: CommandType.StoredProcedure);
    }
}