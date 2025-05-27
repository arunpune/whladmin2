using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WHLAdmin.Common.Providers;

public interface IDbProvider
{
    IDbConnection GetConnection();
}

public class SqlDbProvider : IDbProvider
{
    private readonly ILogger<SqlDbProvider> _logger;
    private readonly string _dbConnString;

    public SqlDbProvider(ILogger<SqlDbProvider> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        if (configuration != null)
        {
            _dbConnString = configuration.GetConnectionString("AdminDbConnString") ?? throw new ApplicationException("Unable to initialize data connection");
        }
        else
        {
            throw new ArgumentNullException(nameof(configuration));
        }
    }

    public IDbConnection GetConnection()
    {
        return new SqlConnection(_dbConnString);
    }
}