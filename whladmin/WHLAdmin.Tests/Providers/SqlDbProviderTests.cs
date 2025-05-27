using System;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Providers;

namespace WHLAdmin.Tests.Providers;

public class SqlDbProviderTests
{
    private readonly Mock<ILogger<SqlDbProvider>> _logger = new();
    private readonly IConfiguration _configuration;

    public SqlDbProviderTests()
    {
        _configuration = new ConfigurationBuilder()
                                .SetBasePath(AppContext.BaseDirectory)
                                .AddJsonFile("unittestsettings.json")
                                .Build();
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new SqlDbProvider(null, null));
        Assert.Throws<ArgumentNullException>(() => new SqlDbProvider(_logger.Object, null));

        // ApplicationException
        var configuration = new ConfigurationBuilder().Build();
        Assert.Throws<ApplicationException>(() => new SqlDbProvider(_logger.Object, configuration));

        // Not Null
        var actual = new SqlDbProvider(_logger.Object, _configuration);
        Assert.NotNull(actual);
    }

    [Fact]
    public void GetConnectionTests()
    {
        var provider = new SqlDbProvider(_logger.Object, _configuration);
        var actual = provider.GetConnection();
        Assert.NotNull(actual);
        Assert.IsType<SqlConnection>(actual);
    }
}