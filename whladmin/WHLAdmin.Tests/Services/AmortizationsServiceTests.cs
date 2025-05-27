using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class AmortizationsServiceTests()
{
    private readonly Mock<ILogger<AmortizationsService>> _logger = new();
    private readonly Mock<IAmortizationRepository> _amortizationRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AmortizationsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmortizationsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmortizationsService(_logger.Object, _amortizationRepository.Object, null));

        // Not Null
        var actual = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var amortizationRepositoryEmpty = new Mock<IAmortizationRepository>();
        amortizationRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amortizationRepositoryNonEmpty = new Mock<IAmortizationRepository>();
        amortizationRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                Rate = 10m, RateInterestOnly = 0.5m, Rate10Year = 1m, Rate15Year = 1.5m,
                Rate20Year = 2m, Rate25Year = 2.5m, Rate30Year = 3m, Rate40Year = 4m,
                Active = true
            }
        ]);

        // Empty
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Amortizations);

        // Not Empty
        service = new AmortizationsService(_logger.Object, amortizationRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Amortizations);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var amortizationRepositoryEmpty = new Mock<IAmortizationRepository>();
        amortizationRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amortizationRepositoryNonEmpty = new Mock<IAmortizationRepository>();
        amortizationRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                Rate = 10m, RateInterestOnly = 0.5m, Rate10Year = 1m, Rate15Year = 1.5m,
                Rate20Year = 2m, Rate25Year = 2.5m, Rate30Year = 3m, Rate40Year = 4m,
                Active = true
            }
        ]);

        // Empty
        var service = new AmortizationsService(_logger.Object, amortizationRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new AmortizationsService(_logger.Object, amortizationRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var amortizationRepositoryNull = new Mock<IAmortizationRepository>();
        amortizationRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync((Amortization)null);

        var amortizationRepositoryNotNull = new Mock<IAmortizationRepository>();
        amortizationRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync(new Amortization()
        {
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m,
            Active = true
        });

        // Null
        var service = new AmortizationsService(_logger.Object, amortizationRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmortizationsService(_logger.Object, amortizationRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(10m, actual.Rate);
        Assert.Equal(0.5m, actual.RateInterestOnly);
        Assert.Equal(1m, actual.Rate10Year);
        Assert.Equal(1.5m, actual.Rate15Year);
        Assert.Equal(2m, actual.Rate20Year);
        Assert.Equal(2.5m, actual.Rate25Year);
        Assert.Equal(3m, actual.Rate30Year);
        Assert.Equal(4m, actual.Rate40Year);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(0m, actual.Rate);
        Assert.Equal(0m, actual.RateInterestOnly);
        Assert.Equal(0m, actual.Rate10Year);
        Assert.Equal(0m, actual.Rate15Year);
        Assert.Equal(0m, actual.Rate20Year);
        Assert.Equal(0m, actual.Rate25Year);
        Assert.Equal(0m, actual.Rate30Year);
        Assert.Equal(0m, actual.Rate40Year);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var amortizationRepositoryNull = new Mock<IAmortizationRepository>();
        amortizationRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync((Amortization)null);

        var amortizationRepositoryNotNull = new Mock<IAmortizationRepository>();
        amortizationRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync(new Amortization()
        {
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m,
            Active = true
        });

        // Null
        var service = new AmortizationsService(_logger.Object, amortizationRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmortizationsService(_logger.Object, amortizationRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(10m, actual.Rate);
        Assert.Equal(0.5m, actual.RateInterestOnly);
        Assert.Equal(1m, actual.Rate10Year);
        Assert.Equal(1.5m, actual.Rate15Year);
        Assert.Equal(2m, actual.Rate20Year);
        Assert.Equal(2.5m, actual.Rate25Year);
        Assert.Equal(3m, actual.Rate30Year);
        Assert.Equal(4m, actual.Rate40Year);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        Amortization amortization = null;
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        service.Sanitize(It.IsAny<string>(), It.IsAny<string>(), amortization);
        Assert.Null(amortization);
    }

    [Theory]
    [InlineData("RATE", -1, 0)]
    [InlineData("RATE", 0, 0)]
    [InlineData("INTONLY", -1, 0)]
    [InlineData("INTONLY", 0, 0)]
    [InlineData("10YEAR", -1, 0)]
    [InlineData("10YEAR", 0, 0)]
    [InlineData("15YEAR", -1, 0)]
    [InlineData("15YEAR", 0, 0)]
    [InlineData("20YEAR", -1, 0)]
    [InlineData("20YEAR", 0, 0)]
    [InlineData("25YEAR", -1, 0)]
    [InlineData("25YEAR", 0, 0)]
    [InlineData("30YEAR", -1, 0)]
    [InlineData("30YEAR", 0, 0)]
    [InlineData("40YEAR", -1, 0)]
    [InlineData("40YEAR", 0, 0)]
    public void SanitizeObjectTests(string rateType, decimal rate, decimal expectedRate)
    {
        var amortization = new Amortization()
        {
            Rate = rate, RateInterestOnly = rate, Rate10Year = rate, Rate15Year = rate,
            Rate20Year = rate, Rate25Year = rate, Rate30Year = rate, Rate40Year = rate,
            Active = true
        };
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        service.Sanitize(It.IsAny<string>(), It.IsAny<string>(), amortization);
        Assert.NotNull(amortization);
        switch (rateType)
        {
            case "RATE": Assert.Equal(amortization.Rate, expectedRate); break;
            case "INTONLY": Assert.Equal(amortization.RateInterestOnly, expectedRate); break;
            case "10YEAR": Assert.Equal(amortization.Rate10Year, expectedRate); break;
            case "15YEAR": Assert.Equal(amortization.Rate15Year, expectedRate); break;
            case "20YEAR": Assert.Equal(amortization.Rate20Year, expectedRate); break;
            case "25YEAR": Assert.Equal(amortization.Rate25Year, expectedRate); break;
            case "30YEAR": Assert.Equal(amortization.Rate30Year, expectedRate); break;
            case "40YEAR": Assert.Equal(amortization.Rate40Year, expectedRate); break;
        }
    }

    [Fact]
    public void ValidateTests()
    {
        // Null Test
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), null, null);
        Assert.Equal("AT000", actual);

        var amortization = new Amortization();

        // Invalid Rate Tests
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT101", actual);
        amortization.Rate = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT101", actual);
        amortization.Rate = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT101", actual);

        // Invalid Interest Only Rate Tests
        amortization.Rate = 10m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT102", actual);
        amortization.RateInterestOnly = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT102", actual);
        amortization.RateInterestOnly = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT102", actual);

        // Invalid 10 Year Rate Tests
        amortization.RateInterestOnly = 0.5m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT103", actual);
        amortization.Rate10Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT103", actual);
        amortization.Rate10Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT103", actual);

        // Invalid 15 Year Rate Tests
        amortization.Rate10Year = 1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT104", actual);
        amortization.Rate15Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT104", actual);
        amortization.Rate15Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT104", actual);

        // Invalid 20 Year Rate Tests
        amortization.Rate15Year = 1.5m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT105", actual);
        amortization.Rate20Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT105", actual);
        amortization.Rate20Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT105", actual);

        // Invalid 25 Year Rate Tests
        amortization.Rate20Year = 2m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT106", actual);
        amortization.Rate25Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT106", actual);
        amortization.Rate25Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT106", actual);

        // Invalid 30 Year Rate Tests
        amortization.Rate25Year = 2.5m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT107", actual);
        amortization.Rate30Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT107", actual);
        amortization.Rate30Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT107", actual);

        // Invalid 40 Year Rate Tests
        amortization.Rate30Year = 3m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT108", actual);
        amortization.Rate40Year = -1m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT108", actual);
        amortization.Rate40Year = 100.01m;
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), amortization, null);
        Assert.Equal("AT108", actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var amortizationToAdd = new EditableAmortizationViewModel()
        {
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m,
            Active = true
        };

        var amortizationRepositoryException = new Mock<IAmortizationRepository>();
        amortizationRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() { });

        var amortizationRepositoryFailure = new Mock<IAmortizationRepository>();
        amortizationRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        amortizationRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(false);

        var amortizationRepositorySuccess = new Mock<IAmortizationRepository>();
        amortizationRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        amortizationRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(true);

        // Null Test
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("AT000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmortizationViewModel());
        Assert.Equal("AT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmortizationViewModel() { Rate = 0 });
        Assert.Equal("AT101", actual);

        // Add Exception
        service = new AmortizationsService(_logger.Object, amortizationRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToAdd));

        // Add Failure
        service = new AmortizationsService(_logger.Object, amortizationRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToAdd);
        Assert.Equal("AT003", actual);

        // Add Success
        service = new AmortizationsService(_logger.Object, amortizationRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var amortizationToUpdate = new EditableAmortizationViewModel()
        {
            Active = true,
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m
        };
        var existingAmortization = new Amortization()
        {
            Active = true,
            Rate = 10m,
            RateInterestOnly = 0.75m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m
        };

        var amortizationRepositoryException = new Mock<IAmortizationRepository>();
        amortizationRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() { });

        var amortizationRepositoryFailure = new Mock<IAmortizationRepository>();
        amortizationRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([existingAmortization]);
        amortizationRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(false);

        var amortizationRepositorySuccess = new Mock<IAmortizationRepository>();
        amortizationRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([existingAmortization]);
        amortizationRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(true);

        // Null Test
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("AT000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmortizationViewModel());
        Assert.Equal("AT101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmortizationViewModel() { Rate = 0 });
        Assert.Equal("AT101", actual);

        // Update Exception
        service = new AmortizationsService(_logger.Object, amortizationRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToUpdate));

        // Update Failure
        service = new AmortizationsService(_logger.Object, amortizationRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToUpdate);
        Assert.Equal("AT004", actual);

        // Update Success
        service = new AmortizationsService(_logger.Object, amortizationRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amortizationToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new AmortizationsService(_logger.Object, _amortizationRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("AT000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("AT000", actual);

        // Setup
        var amortizationRepositoryNull = new Mock<IAmortizationRepository>();
        amortizationRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync((Amortization)null);

        var amortizationRepositoryException = new Mock<IAmortizationRepository>();
        amortizationRepositoryException.Setup(s => s.GetOne(It.IsAny<Amortization>())).ThrowsAsync(new Exception() { });

        var amortizationRepositoryFailure = new Mock<IAmortizationRepository>();
        amortizationRepositoryFailure.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync(new Amortization()
        {
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m,
            Active = true
        });
        amortizationRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(false);

        var amortizationRepositorySuccess = new Mock<IAmortizationRepository>();
        amortizationRepositorySuccess.Setup(s => s.GetOne(It.IsAny<Amortization>())).ReturnsAsync(new Amortization()
        {
            Rate = 10m,
            RateInterestOnly = 0.5m,
            Rate10Year = 1m,
            Rate15Year = 1.5m,
            Rate20Year = 2m,
            Rate25Year = 2.5m,
            Rate30Year = 3m,
            Rate40Year = 4m,
            Active = true
        });
        amortizationRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Amortization>())).ReturnsAsync(true);

        // Not Found Test
        service = new AmortizationsService(_logger.Object, amortizationRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 10m);
        Assert.Equal("AT001", actual);

        // Delete Exception
        service = new AmortizationsService(_logger.Object, amortizationRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 10m));

        // Delete Failure
        service = new AmortizationsService(_logger.Object, amortizationRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 10m);
        Assert.Equal("AT005", actual);

        // Delete Success
        service = new AmortizationsService(_logger.Object, amortizationRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 10m);
        Assert.Empty(actual);
    }
}