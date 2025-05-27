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

public interface IVideoRepository : IReadOnlyRepository<VideoConfig>
{
}

[ExcludeFromCodeCoverage]
public class VideoRepository : IVideoRepository
{
    private readonly ILogger<VideoRepository> _logger;
    private readonly IDbProvider _dbProvider;

    public VideoRepository(ILogger<VideoRepository> logger, IDbProvider dbProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbProvider = dbProvider ?? throw new ArgumentNullException(nameof(dbProvider));
    }

    public async Task<IEnumerable<VideoConfig>> GetAll()
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteVideoRetrieve]";
        return await dbConn.QueryAsync<VideoConfig>(query, commandType: CommandType.StoredProcedure);
    }

    public async Task<VideoConfig> GetOne(VideoConfig item)
    {
        using var dbConn = _dbProvider.GetConnection();
        dbConn.Open();
        var query = "[dbo].[uspSiteVideoRetrieve]";
        return await dbConn.QuerySingleOrDefaultAsync<VideoConfig>(query, new
        {
            VideoID = item.VideoId,
        }, commandType: CommandType.StoredProcedure);
    }
}