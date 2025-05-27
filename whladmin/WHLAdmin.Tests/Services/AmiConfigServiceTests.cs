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

public class AmiConfigsServiceTests()
{
    private readonly Mock<ILogger<AmiConfigsService>> _logger = new();
    private readonly Mock<IAmiConfigRepository> _amiConfigRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, null));

        // Not Null
        var actual = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var amiConfigRepositoryEmpty = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amiConfigRepositoryNonEmpty = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
                Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
                Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
                Active = true
            }
        ]);

        // Empty
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Amis);

        // Not Empty
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Amis);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var amiConfigRepositoryEmpty = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amiConfigRepositoryNonEmpty = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
                Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
                Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
                Active = true
            }
        ]);

        // Empty
        var service = new AmiConfigsService(_logger.Object, amiConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var amiConfigRepositoryNull = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync((AmiConfig)null);

        var amiConfigRepositoryNotNull = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync(new AmiConfig()
        {
            EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
            Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
            Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
            Active = true
        });

        // Null
        var service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(20250301, actual.EffectiveDate);
        Assert.Equal(2025, actual.EffectiveYear);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(DateTime.Now.Date.ToString("yyyy-MM-dd"), actual.EffectiveDate);
        Assert.Equal(DateTime.Now.Year, actual.EffectiveYear);
        Assert.Equal(0, actual.IncomeAmt);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var amiConfigRepositoryNull = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync((AmiConfig)null);

        var amiConfigRepositoryNotNull = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync(new AmiConfig()
        {
            EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
            Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
            Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
            Active = true
        });

        // Null
        var service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal("2025-03-01", actual.EffectiveDate);
        Assert.Equal(2025, actual.EffectiveYear);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        AmiConfig amiConfig = null;
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        service.Sanitize(amiConfig);
        Assert.Null(amiConfig);
    }

    [Theory]
    [InlineData(-1, -1, -1, 0, 0, 0)]
    [InlineData(0, -1, -1, 0, 0, 0)]
    [InlineData(20250301, -1, -1, 20250301, 0, 0)]
    [InlineData(20250301, 0, -1, 20250301, 0, 0)]
    [InlineData(20250301, 2025, -1, 20250301, 2025, 0)]
    [InlineData(20250301, 2025, 0, 20250301, 2025, 0)]
    [InlineData(20250301, 2025, 10000, 20250301, 2025, 10000)]
    public void SanitizeBaseTests(int effectiveDate, int effectiveYear, long incomeAmt
                                    , int expectedEffectiveDate, int expectedEffectiveYear, long expectedIncomeAmt)
    {
        var amiConfig = new AmiConfig() { EffectiveDate = effectiveDate, EffectiveYear = effectiveYear, IncomeAmt = incomeAmt };
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        service.Sanitize(amiConfig);
        Assert.NotNull(amiConfig);
        Assert.Equal(expectedEffectiveDate, amiConfig.EffectiveDate);
        Assert.Equal(expectedEffectiveYear, amiConfig.EffectiveYear);
        Assert.Equal(expectedIncomeAmt, amiConfig.IncomeAmt);
    }

    [Theory]
    [InlineData(-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(0, -1, -1, -1, -1, -1, -1, -1, -1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, -1, -1, -1, -1, -1, -1, -1, -1, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 0, -1, -1, -1, -1, -1, -1, -1, -1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, -1, -1, -1, -1, -1, -1, -1, -1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 0, -1, -1, -1, -1, -1, -1, -1, 1, 2, 0, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, -1, -1, -1, -1, -1, -1, -1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 0, -1, -1, -1, -1, -1, -1, 1, 2, 3, 0, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, -1, -1, -1, -1, -1, -1, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 0, -1, -1, -1, -1, -1, 1, 2, 3, 4, 0, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, -1, -1, -1, -1, -1, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 0, -1, -1, -1, -1, 1, 2, 3, 4, 5, 0, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, -1, -1, -1, -1, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 0, -1, -1, -1, 1, 2, 3, 4, 5, 6, 0, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, -1, -1, -1, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 0, -1, -1, 1, 2, 3, 4, 5, 6, 7, 0, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, -1, -1, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 0, -1, 1, 2, 3, 4, 5, 6, 7, 8, 0, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, -1, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0)]
    [InlineData(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
    public void SanitizeHh5Tests(int hh1, int hh2, int hh3, int hh4, int hh5
                                    , int hh6, int hh7, int hh8, int hh9, int hh10
                                    , int expectedHh1, int expectedHh2, int expectedHh3, int expectedHh4, int expectedHh5
                                    , int expectedHh6, int expectedHh7, int expectedHh8, int expectedHh9, int expectedHh10)
    {
        var amiConfig = new AmiConfig() { Hh1 = hh1, Hh2 = hh2, Hh3 = hh3, Hh4 = hh4, Hh5 = hh5, Hh6 = hh6, Hh7 = hh7, Hh8 = hh8, Hh9 = hh9, Hh10 = hh10 };
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        service.Sanitize(amiConfig);
        Assert.NotNull(amiConfig);
        Assert.Equal(expectedHh1, amiConfig.Hh1);
        Assert.Equal(expectedHh2, amiConfig.Hh2);
        Assert.Equal(expectedHh3, amiConfig.Hh3);
        Assert.Equal(expectedHh4, amiConfig.Hh4);
        Assert.Equal(expectedHh5, amiConfig.Hh5);
        Assert.Equal(expectedHh6, amiConfig.Hh6);
        Assert.Equal(expectedHh7, amiConfig.Hh7);
        Assert.Equal(expectedHh8, amiConfig.Hh8);
        Assert.Equal(expectedHh9, amiConfig.Hh9);
        Assert.Equal(expectedHh10, amiConfig.Hh10);
    }

    [Fact]
    public void ValidateTests()
    {
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);

        // Null Test
        AmiConfig amiConfig = null;
        var actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM000", actual);

        // Invalid Effective Date Tests
        amiConfig = new AmiConfig();
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM101", actual);
        amiConfig.EffectiveDate = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM101", actual);
        amiConfig.EffectiveDate = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM101", actual);
        amiConfig.EffectiveDate = 19991231;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM101", actual);

        // Invalid Effective Year Tests
        amiConfig.EffectiveDate = 20250301;
        amiConfig.EffectiveYear = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM102", actual);
        amiConfig.EffectiveYear = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM102", actual);
        amiConfig.EffectiveYear = 1999;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM102", actual);

        // Invalid Income Amount Tests
        amiConfig.EffectiveYear = 2025;
        amiConfig.IncomeAmt = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM103", actual);
        amiConfig.IncomeAmt = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM103", actual);

        // Invalid Hh1 Tests
        amiConfig.IncomeAmt = 10000;
        amiConfig.Hh1 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM104", actual);
        amiConfig.Hh1 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM104", actual);

        // Invalid Hh2 Tests
        amiConfig.Hh1 = 70;
        amiConfig.Hh2 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM105", actual);
        amiConfig.Hh2 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM105", actual);

        // Invalid Hh3 Tests
        amiConfig.Hh2 = 80;
        amiConfig.Hh3 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM106", actual);
        amiConfig.Hh3 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM106", actual);

        // Invalid Hh4 Tests
        amiConfig.Hh3 = 90;
        amiConfig.Hh4 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM107", actual);
        amiConfig.Hh4 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM107", actual);

        // Invalid Hh5 Tests
        amiConfig.Hh4 = 100;
        amiConfig.Hh5 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM108", actual);
        amiConfig.Hh5 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM108", actual);

        // Invalid Hh6 Tests
        amiConfig.Hh5 = 108;
        amiConfig.Hh6 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM109", actual);
        amiConfig.Hh6 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM109", actual);

        // Invalid Hh7 Tests
        amiConfig.Hh6 = 116;
        amiConfig.Hh7 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM110", actual);
        amiConfig.Hh7 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM110", actual);

        // Invalid Hh8 Tests
        amiConfig.Hh7 = 124;
        amiConfig.Hh8 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM111", actual);
        amiConfig.Hh8 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM111", actual);

        // Invalid Hh9 Tests
        amiConfig.Hh8 = 132;
        amiConfig.Hh9 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM112", actual);
        amiConfig.Hh9 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM112", actual);

        // Invalid Hh10 Tests
        amiConfig.Hh9 = 140;
        amiConfig.Hh10 = -1;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM113", actual);
        amiConfig.Hh10 = 0;
        actual = service.Validate(amiConfig, null, It.IsAny<bool>());
        Assert.Equal("AM113", actual);

        // Duplicate Check FAIL for ADD Test
        amiConfig.Hh10 = 148;
        var amis = new List<AmiConfig>()
        {
            new() { EffectiveDate = 20250301 }
        };
        actual = service.Validate(amiConfig, amis, false);
        Assert.Equal("AM002", actual);

        // Duplicate Check SUCCESS for ADD Test
        amis =
        [
            new AmiConfig() { EffectiveDate = 20250201 }
        ];
        actual = service.Validate(amiConfig, amis, false);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var amiConfigToAdd = new EditableAmiConfigViewModel()
        {
            EffectiveDate = "2025-03-01", EffectiveYear = 2025, IncomeAmt = 10000,
            HhPctAmts =
            [
                new AmiHhPctAmt() { Pct = 70 },
                new AmiHhPctAmt() { Pct = 80 },
                new AmiHhPctAmt() { Pct = 90 },
                new AmiHhPctAmt() { Pct = 100 },
                new AmiHhPctAmt() { Pct = 108 },
                new AmiHhPctAmt() { Pct = 116 },
                new AmiHhPctAmt() { Pct = 124 },
                new AmiHhPctAmt() { Pct = 132 },
                new AmiHhPctAmt() { Pct = 140 },
                new AmiHhPctAmt() { Pct = 148 }
            ],
            Active = true
        };

        var amiConfigRepositoryException = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var amiConfigRepositoryFailure = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        amiConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(false);

        var amiConfigRepositorySuccess = new Mock<IAmiConfigRepository>();
        amiConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        amiConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("AM000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableAmiConfigViewModel());
        Assert.Equal("AM101", actual);

        // Add Exception
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), amiConfigToAdd));

        // Add Failure
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), amiConfigToAdd);
        Assert.Equal("AM003", actual);

        // Add Success
        service = new AmiConfigsService(_logger.Object, amiConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), amiConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var amiConfigToUpdate = new EditableAmiConfigViewModel()
        {
            EffectiveDate = "2025-03-01", EffectiveYear = 2025, IncomeAmt = 10000,
            HhPctAmts =
            [
                new AmiHhPctAmt() { Pct = 70 },
                new AmiHhPctAmt() { Pct = 80 },
                new AmiHhPctAmt() { Pct = 90 },
                new AmiHhPctAmt() { Pct = 100 },
                new AmiHhPctAmt() { Pct = 108 },
                new AmiHhPctAmt() { Pct = 116 },
                new AmiHhPctAmt() { Pct = 124 },
                new AmiHhPctAmt() { Pct = 132 },
                new AmiHhPctAmt() { Pct = 140 },
                new AmiHhPctAmt() { Pct = 148 }
            ],
            Active = true
        };

        var amiConfigRepositoryException = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var amiConfigRepositoryFailure = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        amiConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(false);

        var amiConfigRepositorySuccess = new Mock<IAmiConfigRepository>();
        amiConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        amiConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("AM000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableAmiConfigViewModel());
        Assert.Equal("AM101", actual);

        // Update Exception
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), amiConfigToUpdate));

        // Update Failure
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), amiConfigToUpdate);
        Assert.Equal("AM004", actual);

        // Update Success
        service = new AmiConfigsService(_logger.Object, amiConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), amiConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new AmiConfigsService(_logger.Object, _amiConfigRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("AM000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("AM000", actual);

        // Setup
        var amiConfigRepositoryNull = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync((AmiConfig)null);

        var amiConfigRepositoryException = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ThrowsAsync(new Exception() {});

        var amiConfigRepositoryFailure = new Mock<IAmiConfigRepository>();
        amiConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync(new AmiConfig()
        {
            EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
            Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
            Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
            Active = true
        });
        amiConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(false);

        var amiConfigRepositorySuccess = new Mock<IAmiConfigRepository>();
        amiConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<AmiConfig>())).ReturnsAsync(new AmiConfig()
        {
            EffectiveDate = 20250301, EffectiveYear = 2025, IncomeAmt = 1000,
            Hh1 = 70, Hh2 = 80, Hh3 = 90, Hh4 = 100, Hh5 = 108,
            Hh6 = 116, Hh7 = 124, Hh8 = 132, Hh9 = 140, Hh10 = 148,
            Active = true
        });
        amiConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<AmiConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("AM001", actual);

        // Delete Exception
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new AmiConfigsService(_logger.Object, amiConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("AM005", actual);

        // Delete Success
        service = new AmiConfigsService(_logger.Object, amiConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}