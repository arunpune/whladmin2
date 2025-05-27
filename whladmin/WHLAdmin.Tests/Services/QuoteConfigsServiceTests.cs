using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class QuoteConfigsServiceTests()
{
    private readonly Mock<ILogger<QuoteConfigsService>> _logger = new();
    private readonly Mock<IQuoteConfigRepository> _quoteConfigRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, null));

        // Not Null
        var actual = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var quoteConfigRepositoryEmpty = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var quoteConfigRepositoryNonEmpty = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                QuoteId = 1, Text = "TEXT", Active = true
            }
        ]);

        // Empty
        var service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Quotes);

        // Not Empty
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Quotes);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var quoteConfigRepositoryEmpty = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var quoteConfigRepositoryNonEmpty = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                QuoteId = 1, Text = "TEXT", Active = true
            }
        ]);

        // Empty
        var service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var quoteConfigRepositoryNull = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync((QuoteConfig)null);

        var quoteConfigRepositoryNotNull = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync(new QuoteConfig()
        {
            QuoteId = 1, Text = "TEXT", Active = true
        });

        // Null
        var service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.QuoteId);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(0, actual.QuoteId);
        Assert.Empty(actual.Text);
        Assert.Empty(actual.Text);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var quoteConfigRepositoryNull = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync((QuoteConfig)null);

        var quoteConfigRepositoryNotNull = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync(new QuoteConfig()
        {
            QuoteId = 1, Text = "TEXT", Active = true
        });

        // Null
        var service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.QuoteId);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        QuoteConfig quoteConfig = null;
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        service.Sanitize(quoteConfig);
        Assert.Null(quoteConfig);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData(" ", "")]
    [InlineData("TEXT", "TEXT")]
    [InlineData("TEXT ", "TEXT")]
    [InlineData(" TEXT", "TEXT")]
    [InlineData(" TEXT ", "TEXT")]
    public void SanitizeObjectTests(string title, string expectedText)
    {
        var quoteConfig = new QuoteConfig() { Text = title };
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        service.Sanitize(quoteConfig);
        Assert.NotNull(quoteConfig);
        Assert.Equal(expectedText, quoteConfig.Text);
    }

    [Fact]
    public void ValidateNullTest()
    {
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(null, null);
        Assert.Equal("QT000", actual);
    }

    [Theory]
    [InlineData(null, "QT101")]
    [InlineData("", "QT101")]
    [InlineData(" ", "QT101")]
    [InlineData("TEXT", "")]
    public void ValidateBaseTests(string title, string expectedCode)
    {
        var quoteToValidate = new QuoteConfig()
        {
            Text = title
        };

        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(quoteToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public void ValidateExistenceTests()
    {
        // Setup
        var quoteToValidate = new QuoteConfig() { QuoteId = 1, Text = "TEXT" };

        // Existence Check FAIL for UPDATE Test
        var existingQuote = new QuoteConfig() { QuoteId = 2, Text = "TEXT" };
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(quoteToValidate, [ existingQuote ]);
        Assert.Equal("QT001", actual);

        // Existence Check SUCCESS for UPDATE Test
        existingQuote.QuoteId = 1;
        service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        actual = service.Validate(quoteToValidate, [ existingQuote ]);
        Assert.Empty(actual);
    }

    [Fact]
    public void ValidateDuplicateTests()
    {
        // Setup
        var quoteToValidate = new QuoteConfig() { QuoteId = 0, Text = "TEXT" };

        // Duplicate Check FAIL for ADD Test
        var existingQuote = new QuoteConfig() { QuoteId = 1, Text = "TEXT" };
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(quoteToValidate, [ existingQuote ]);
        Assert.Equal("QT002", actual);

        // Duplicate Check SUCCESS for ADD Test
        existingQuote.Text = "ANOTHERTEXT";
        service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        actual = service.Validate(quoteToValidate, [ existingQuote ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        quoteToValidate.QuoteId = 1;
        existingQuote = new QuoteConfig() { QuoteId = 2, Text = "TEXT" };
        service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        actual = service.Validate(quoteToValidate, [ quoteToValidate, existingQuote ]);
        Assert.Equal("QT002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        existingQuote = new QuoteConfig() { QuoteId = 2, Text = "ANOTHERTEXT" };
        service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        actual = service.Validate(quoteToValidate, [ quoteToValidate, existingQuote ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var quoteConfigToAdd = new EditableQuoteConfigViewModel()
        {
            Active = true, QuoteId = 0, Text = "TEXT"
        };

        var quoteConfigRepositoryException = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var quoteConfigRepositoryFailure = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        quoteConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(false);

        var quoteConfigRepositorySuccess = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        quoteConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("QT000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel());
        Assert.Equal("QT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel() { Text = "" });
        Assert.Equal("QT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel() { Text = "   " });
        Assert.Equal("QT101", actual);

        // Add Exception
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToAdd));

        // Add Failure
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToAdd);
        Assert.Equal("QT003", actual);

        // Add Success
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var quoteConfigToUpdate = new EditableQuoteConfigViewModel()
        {
            Active = true, QuoteId = 1, Text = "TEXT"
        };
        var existingQuoteConfig = new QuoteConfig()
        {
            Active = true, QuoteId = 1, Text = "TEXT"
        };

        var quoteConfigRepositoryException = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var quoteConfigRepositoryFailure = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingQuoteConfig ]);
        quoteConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(false);

        var quoteConfigRepositorySuccess = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingQuoteConfig ]);
        quoteConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("QT000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel());
        Assert.Equal("QT101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel() { Text = "" });
        Assert.Equal("QT101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableQuoteConfigViewModel() { Text = "   " });
        Assert.Equal("QT101", actual);

        // Update Exception
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToUpdate));

        // Update Failure
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToUpdate);
        Assert.Equal("QT004", actual);

        // Update Success
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), quoteConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new QuoteConfigsService(_logger.Object, _quoteConfigRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("QT000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("QT000", actual);

        // Setup
        var quoteConfigRepositoryNull = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync((QuoteConfig)null);

        var quoteConfigRepositoryException = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ThrowsAsync(new Exception() {});

        var quoteConfigRepositoryFailure = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync(new QuoteConfig()
        {
            QuoteId = 1, Text = "TEXT", Active = true
        });
        quoteConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(false);

        var quoteConfigRepositorySuccess = new Mock<IQuoteConfigRepository>();
        quoteConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<QuoteConfig>())).ReturnsAsync(new QuoteConfig()
        {
            QuoteId = 1, Text = "TEXT", Active = true
        });
        quoteConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<QuoteConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("QT001", actual);

        // Delete Exception
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("QT005", actual);

        // Delete Success
        service = new QuoteConfigsService(_logger.Object, quoteConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}