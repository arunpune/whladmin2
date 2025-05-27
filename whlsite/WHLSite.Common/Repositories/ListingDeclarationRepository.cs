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

public interface IListingDeclarationRepository
{
    Task<IEnumerable<Declaration>> GetAll(int listingId);
}

[ExcludeFromCodeCoverage]
public class ListingDeclarationRepository : IListingDeclarationRepository
{
    private readonly ILogger<ListingDeclarationRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public ListingDeclarationRepository(ILogger<ListingDeclarationRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<Declaration>> GetAll(int listingId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteListingDeclarationRetrieve]";
        return await dbConn.QueryAsync<Declaration>(query, new
        {
            listingId
        }, commandType: CommandType.StoredProcedure);
    }
}