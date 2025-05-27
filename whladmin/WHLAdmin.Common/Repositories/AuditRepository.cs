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

public interface IAuditRepository
{
    Task<IEnumerable<AuditEntry>> GetAll(string entityTypeCd, string entityId);
}

[ExcludeFromCodeCoverage]
public class AuditRepository : IAuditRepository
{
    private readonly ILogger<AuditRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public AuditRepository(ILogger<AuditRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<AuditEntry>> GetAll(string entityTypeCd, string entityId)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspAuditRetrieve]";
        return await dbConn.QueryAsync<AuditEntry>(query, new
        {
            EntityTypeCD = entityTypeCd,
            EntityID = entityId
        }, commandType: CommandType.StoredProcedure);
    }
}