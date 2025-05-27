using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class FaqConfigsServiceTests()
{
    private readonly Mock<ILogger<FaqConfigsService>> _logger = new();
    private readonly Mock<IFaqConfigRepository> _faqConfigRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, null));

        // Not Null
        var actual = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var faqConfigRepositoryEmpty = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var faqConfigRepositoryNonEmpty = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
            }
        ]);

        // Empty
        var service = new FaqConfigsService(_logger.Object, faqConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Faqs);

        // Not Empty
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Faqs);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var faqConfigRepositoryEmpty = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var faqConfigRepositoryNonEmpty = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
            }
        ]);

        // Empty
        var service = new FaqConfigsService(_logger.Object, faqConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var faqConfigRepositoryNull = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync((FaqConfig)null);

        var faqConfigRepositoryNotNull = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync(new FaqConfig()
        {
            FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
        });

        // Null
        var service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FaqId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(0, actual.FaqId);
        Assert.Empty(actual.Text);
        Assert.Empty(actual.Title);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var faqConfigRepositoryNull = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync((FaqConfig)null);

        var faqConfigRepositoryNotNull = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync(new FaqConfig()
        {
            FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
        });

        // Null
        var service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FaqId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        FaqConfig faqConfig = null;
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        service.Sanitize(faqConfig);
        Assert.Null(faqConfig);
    }

    [Theory]
    [InlineData(null, null, null, "General", "", "")]
    [InlineData("", null, null, "General", "", "")]
    [InlineData(" ", null, null, "General", "", "")]
    [InlineData("CATEGORY", null, null, "CATEGORY", "", "")]
    [InlineData("CATEGORY ", null, null, "CATEGORY", "", "")]
    [InlineData(" CATEGORY", null, null, "CATEGORY", "", "")]
    [InlineData(" CATEGORY ", null, null, "CATEGORY", "", "")]
    [InlineData("CATEGORY", "", null, "CATEGORY", "", "")]
    [InlineData("CATEGORY", " ", null, "CATEGORY", "", "")]
    [InlineData("CATEGORY", "TITLE", null, "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", "TITLE ", null, "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", " TITLE", null, "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", " TITLE ", null, "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", "TITLE", "", "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", "TITLE", " ", "CATEGORY", "TITLE", "")]
    [InlineData("CATEGORY", "TITLE", "TEXT", "CATEGORY", "TITLE", "TEXT")]
    [InlineData("CATEGORY", "TITLE", "TEXT ", "CATEGORY", "TITLE", "TEXT")]
    [InlineData("CATEGORY", "TITLE", " TEXT", "CATEGORY", "TITLE", "TEXT")]
    [InlineData("CATEGORY", "TITLE", " TEXT ", "CATEGORY", "TITLE", "TEXT")]
    public void SanitizeObjectTests(string categoryName, string title, string text, string expectedCategoryName, string expectedTitle, string expectedText)
    {
        var faqConfig = new FaqConfig() { CategoryName = categoryName, Title = title, Text = text };
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        service.Sanitize(faqConfig);
        Assert.NotNull(faqConfig);
        Assert.Equal(expectedCategoryName, faqConfig.CategoryName);
        Assert.Equal(expectedTitle, faqConfig.Title);
        Assert.Equal(expectedText, faqConfig.Text);
    }

    [Fact]
    public void ValidateNullTest()
    {
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(null, null);
        Assert.Equal("F000", actual);
    }

    [Theory]
    [InlineData(null, null, null, "F102")]
    [InlineData("", null, null, "F102")]
    [InlineData(" ", null, null, "F102")]
    [InlineData("CATEGORY", null, null, "F102")]
    [InlineData("CATEGORY", "", null, "F102")]
    [InlineData("CATEGORY", " ", null, "F102")]
    [InlineData("CATEGORY", "TITLE", null, "F103")]
    [InlineData("CATEGORY", "TITLE", "", "F103")]
    [InlineData("CATEGORY", "TITLE", " ", "F103")]
    [InlineData("CATEGORY", "TITLE", "TEXT", "")]
    public void ValidateBaseTests(string categoryName, string title, string text, string expectedCode)
    {
        var faqToValidate = new FaqConfig()
        {
            CategoryName = categoryName, Title = title, Text = text
        };

        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(faqToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public void ValidateExistenceTests()
    {
        // Setup
        var faqToValidate = new FaqConfig() { FaqId = 1, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT" };

        // Existence Check FAIL for UPDATE Test
        var existingFaq = new FaqConfig() { FaqId = 2, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT" };
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(faqToValidate, [ existingFaq ]);
        Assert.Equal("F001", actual);

        // Existence Check SUCCESS for UPDATE Test
        existingFaq.FaqId = 1;
        service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        actual = service.Validate(faqToValidate, [ existingFaq ]);
        Assert.Empty(actual);
    }

    [Fact]
    public void ValidateDuplicateTests()
    {
        // Setup
        var faqToValidate = new FaqConfig() { FaqId = 0, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT" };

        // Duplicate Check FAIL for ADD Test
        var existingFaq = new FaqConfig() { FaqId = 1, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT" };
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(faqToValidate, [ existingFaq ]);
        Assert.Equal("F002", actual);

        // Duplicate Check SUCCESS for ADD Test
        existingFaq.Title = "ANOTHERTITLE";
        service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        actual = service.Validate(faqToValidate, [ existingFaq ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        faqToValidate.FaqId = 1;
        existingFaq = new FaqConfig() { FaqId = 2, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT" };
        service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        actual = service.Validate(faqToValidate, [ faqToValidate, existingFaq ]);
        Assert.Equal("F002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        existingFaq = new FaqConfig() { FaqId = 2, CategoryName = "CATEGORY", Title = "ANOTHERTITLE", Text = "TEXT" };
        service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        actual = service.Validate(faqToValidate, [ faqToValidate, existingFaq ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var faqConfigToAdd = new EditableFaqConfigViewModel()
        {
            Active = true, FaqId = 0, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT"
        };

        var faqConfigRepositoryException = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var faqConfigRepositoryFailure = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        faqConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(false);

        var faqConfigRepositorySuccess = new Mock<IFaqConfigRepository>();
        faqConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        faqConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("F000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel());
        Assert.Equal("F102", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel() { Title = "" });
        Assert.Equal("F102", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel() { Title = "   " });
        Assert.Equal("F102", actual);

        // Add Exception
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), faqConfigToAdd));

        // Add Failure
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), faqConfigToAdd);
        Assert.Equal("F003", actual);

        // Add Success
        service = new FaqConfigsService(_logger.Object, faqConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), faqConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var faqConfigToUpdate = new EditableFaqConfigViewModel()
        {
            Active = true, FaqId = 1, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT"
        };
        var existingFaqConfig = new FaqConfig()
        {
            Active = true, FaqId = 1, CategoryName = "CATEGORY", Title = "TITLE", Text = "TEXT"
        };

        var faqConfigRepositoryException = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var faqConfigRepositoryFailure = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingFaqConfig ]);
        faqConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(false);

        var faqConfigRepositorySuccess = new Mock<IFaqConfigRepository>();
        faqConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingFaqConfig ]);
        faqConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("F000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel());
        Assert.Equal("F102", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel() { Title = "" });
        Assert.Equal("F102", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableFaqConfigViewModel() { Title = "   " });
        Assert.Equal("F102", actual);

        // Update Exception
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), faqConfigToUpdate));

        // Update Failure
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), faqConfigToUpdate);
        Assert.Equal("F004", actual);

        // Update Success
        service = new FaqConfigsService(_logger.Object, faqConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), faqConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new FaqConfigsService(_logger.Object, _faqConfigRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("F000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("F000", actual);

        // Setup
        var faqConfigRepositoryNull = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync((FaqConfig)null);

        var faqConfigRepositoryException = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ThrowsAsync(new Exception() {});

        var faqConfigRepositoryFailure = new Mock<IFaqConfigRepository>();
        faqConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync(new FaqConfig()
        {
            FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
        });
        faqConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(false);

        var faqConfigRepositorySuccess = new Mock<IFaqConfigRepository>();
        faqConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<FaqConfig>())).ReturnsAsync(new FaqConfig()
        {
            FaqId = 1, Title = "TITLE", Text = "TEXT", Active = true
        });
        faqConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<FaqConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("F001", actual);

        // Delete Exception
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new FaqConfigsService(_logger.Object, faqConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("F005", actual);

        // Delete Success
        service = new FaqConfigsService(_logger.Object, faqConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}