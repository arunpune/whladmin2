using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class ReportsServiceTests
{
    private readonly Mock<ILogger<ReportsService>> _logger = new();
    private readonly Mock<IReportRepository> _reportRepository = new();
    private readonly Mock<IListingsService> _listingsService = new();
    private readonly Mock<IMetadataService> _metadataService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ReportsService(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ReportsService(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ReportsService(_logger.Object, _reportRepository.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ReportsService(_logger.Object, _reportRepository.Object, _listingsService.Object, null));

        // Not Null
        var actual = new ReportsService(_logger.Object, _reportRepository.Object, _listingsService.Object, _metadataService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void InitializeApplicationDemographicsReportTests()
    {
        var service = new ReportsService(_logger.Object, _reportRepository.Object, _listingsService.Object, _metadataService.Object);
        var actual = service.InitializeApplicationDemographicsReport();
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.FromDate);
        Assert.NotEmpty(actual.ToDate);
        Assert.False(actual.Searched);
    }

    [Theory]
    [InlineData(null, null, -1, "RP101")]
    [InlineData("", null, -1, "RP101")]
    [InlineData("2023-12-31", null, -1, "RP101")]
    [InlineData("2099-12-31", null, -1, "RP101")]
    [InlineData("2024-01-01", null, -1, "")]
    [InlineData("2024-01-01", "", -1, "")]
    [InlineData("2024-01-01", "2023-12-31", -1, "RP102")]
    [InlineData("2024-01-01", "2099-12-31", -1, "RP102")]
    [InlineData("2024-01-01", "2024-01-31", -1, "")]
    [InlineData("2024-01-01", "2024-01-31", 0, "")]
    [InlineData("2024-01-01", "2024-01-31", 1, "")]
    public async Task GetApplicationDemographicsReportTests(string fromDate, string toDate, int recordCount, string expectedCode)
    {
        var model = new DemographicsReportViewModel()
        {
            FromDate = fromDate, ToDate = toDate
        };

        List<ApplicationDemographicRecord> data = null;
        if (recordCount >= 0)
        {
            data = [];
            if (recordCount > 0)
            {
                data.Add(new ApplicationDemographicRecord()
                {
                    ListingId = 1,
                    ListingTypeCd = "TYPECD",
                    Name = "NAME",
                    StreetLine1 = "STREET1",
                    City = "CITY",
                    StateCd = "ST",
                    ZipCode = "10111",
                    County = "COUNTY",
                    GenderCd = "NOANS",
                    RaceCd = "NOANS",
                    EthnicityCd = "NOANS"
                });
            }
        }

        var allMetadata = new Dictionary<string, string>()
        {
            { "NOANS", "Not Answered" }
        };

        var reportRepository = new Mock<IReportRepository>();
        reportRepository.Setup(s => s.GetApplicationDemographicsReport(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(data);

        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetGenderTypes(It.IsAny<bool>())).ReturnsAsync(allMetadata);
        metadataService.Setup(s => s.GetRaceTypes(It.IsAny<bool>())).ReturnsAsync(allMetadata);
        metadataService.Setup(s => s.GetEthnicityTypes(It.IsAny<bool>())).ReturnsAsync(allMetadata);

        var service = new ReportsService(_logger.Object, reportRepository.Object, _listingsService.Object, metadataService.Object);
        var actual = await service.GetApplicationDemographicsReport(model);
        Assert.Equal(expectedCode, actual);

        if (string.IsNullOrEmpty(expectedCode))
        {
            Assert.True(model.Searched);

            if (recordCount > 0)
            {
                Assert.NotEmpty(model.Data);
            }
        }
    }

    [Fact]
    public void InitializeRegistrationsSummaryReportTests()
    {
        var service = new ReportsService(_logger.Object, _reportRepository.Object, _listingsService.Object, _metadataService.Object);
        var actual = service.InitializeRegistrationsSummaryReport();
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.FromDate);
        Assert.NotEmpty(actual.ToDate);
        Assert.False(actual.Searched);
    }

    [Theory]
    [InlineData(null, null, "RP101")]
    [InlineData("", null, "RP101")]
    [InlineData("2023-12-31", null, "RP101")]
    [InlineData("2099-12-31", null, "RP101")]
    [InlineData("2024-01-01", null, "")]
    [InlineData("2024-01-01", "", "")]
    [InlineData("2024-01-01", "2023-12-31", "RP102")]
    [InlineData("2024-01-01", "2099-12-31", "RP102")]
    [InlineData("2024-01-01", "2024-01-31", "")]
    public async Task GetRegistrationsSummaryReportTests(string fromDate, string toDate, string expectedCode)
    {
        var model = new RegistrationsSummaryReportViewModel()
        {
            FromDate = fromDate, ToDate = toDate
        };

        var service = new ReportsService(_logger.Object, _reportRepository.Object, _listingsService.Object, _metadataService.Object);
        var actual = await service.GetRegistrationsSummaryReport(model);
        Assert.Equal(expectedCode, actual);
    }
}