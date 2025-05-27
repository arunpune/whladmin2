using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class ResourceConfigsServiceTests()
{
    private readonly Mock<ILogger<ResourceConfigsService>> _logger = new();
    private readonly Mock<IResourceConfigRepository> _resourceConfigRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, null));

        // Not Null
        var actual = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var resourceConfigRepositoryEmpty = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var resourceConfigRepositoryNonEmpty = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
            }
        ]);

        // Empty
        var service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Resources);

        // Not Empty
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Resources);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var resourceConfigRepositoryEmpty = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var resourceConfigRepositoryNonEmpty = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
            }
        ]);

        // Empty
        var service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var resourceConfigRepositoryNull = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync((ResourceConfig)null);

        var resourceConfigRepositoryNotNull = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync(new ResourceConfig()
        {
            ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
        });

        // Null
        var service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.ResourceId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(0, actual.ResourceId);
        Assert.Empty(actual.Text);
        Assert.Empty(actual.Title);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var resourceConfigRepositoryNull = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync((ResourceConfig)null);

        var resourceConfigRepositoryNotNull = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync(new ResourceConfig()
        {
            ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
        });

        // Null
        var service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.ResourceId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        ResourceConfig resourceConfig = null;
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        service.Sanitize(resourceConfig);
        Assert.Null(resourceConfig);
    }

    [Theory]
    [InlineData(null, null, "", "")]
    [InlineData("", null, "", "")]
    [InlineData(" ", null, "", "")]
    [InlineData("TITLE", null, "TITLE", "")]
    [InlineData("TITLE ", null, "TITLE", "")]
    [InlineData(" TITLE", null, "TITLE", "")]
    [InlineData(" TITLE ", null, "TITLE", "")]
    [InlineData("TITLE", "", "TITLE", "")]
    [InlineData("TITLE", " ", "TITLE", "")]
    [InlineData("TITLE", "URL", "TITLE", "URL")]
    [InlineData("TITLE", "URL ", "TITLE", "URL")]
    [InlineData("TITLE", " URL", "TITLE", "URL")]
    [InlineData("TITLE", " URL ", "TITLE", "URL")]
    public void SanitizeObjectTests(string title, string url, string expectedTitle, string expectedUrl)
    {
        var resourceConfig = new ResourceConfig() { Title = title, Url = url };
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        service.Sanitize(resourceConfig);
        Assert.NotNull(resourceConfig);
        Assert.Equal(expectedTitle, resourceConfig.Title);
        Assert.Equal(expectedUrl, resourceConfig.Url);
    }

    [Fact]
    public void ValidateNullTest()
    {
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(null, null);
        Assert.Equal("R000", actual);
    }

    [Theory]
    [InlineData(null, null, "R101")]
    [InlineData("", null, "R101")]
    [InlineData(" ", null, "R101")]
    [InlineData("TITLE", null, "R102")]
    [InlineData("TITLE", "", "R102")]
    [InlineData("TITLE", " ", "R102")]
    [InlineData("TITLE", "URL", "")]
    public void ValidateBaseTests(string title, string url, string expectedCode)
    {
        var resourceToValidate = new ResourceConfig()
        {
            Title = title, Url = url
        };

        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(resourceToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public void ValidateExistenceTests()
    {
        // Setup
        var resourceToValidate = new ResourceConfig() { ResourceId = 1, Title = "TITLE", Url = "URL" };

        // Existence Check FAIL for UPDATE Test
        var existingResource = new ResourceConfig() { ResourceId = 2, Title = "TITLE", Url = "URL" };
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(resourceToValidate, [ existingResource ]);
        Assert.Equal("R001", actual);

        // Existence Check SUCCESS for UPDATE Test
        existingResource.ResourceId = 1;
        service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        actual = service.Validate(resourceToValidate, [ existingResource ]);
        Assert.Empty(actual);
    }

    [Fact]
    public void ValidateDuplicateTests()
    {
        // Setup
        var resourceToValidate = new ResourceConfig() { ResourceId = 0, Title = "TITLE", Url = "URL" };

        // Duplicate Check FAIL for ADD Test
        var existingResource = new ResourceConfig() { ResourceId = 1, Title = "TITLE", Url = "URL" };
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(resourceToValidate, [ existingResource ]);
        Assert.Equal("R002", actual);

        // Duplicate Check SUCCESS for ADD Test
        existingResource.Title = "ANOTHERTITLE";
        service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        actual = service.Validate(resourceToValidate, [ existingResource ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        resourceToValidate.ResourceId = 1;
        existingResource = new ResourceConfig() { ResourceId = 2, Title = "TITLE", Url = "URL" };
        service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        actual = service.Validate(resourceToValidate, [ resourceToValidate, existingResource ]);
        Assert.Equal("R002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        existingResource = new ResourceConfig() { ResourceId = 2, Title = "ANOTHERTITLE", Url = "URL" };
        service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        actual = service.Validate(resourceToValidate, [ resourceToValidate, existingResource ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var resourceConfigToAdd = new EditableResourceConfigViewModel()
        {
            Active = true, ResourceId = 0, Title = "TITLE", Url = "URL"
        };

        var resourceConfigRepositoryException = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var resourceConfigRepositoryFailure = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        resourceConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(false);

        var resourceConfigRepositorySuccess = new Mock<IResourceConfigRepository>();
        resourceConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        resourceConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("R000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel());
        Assert.Equal("R101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel() { Title = "" });
        Assert.Equal("R101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel() { Title = "   " });
        Assert.Equal("R101", actual);

        // Add Exception
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToAdd));

        // Add Failure
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToAdd);
        Assert.Equal("R003", actual);

        // Add Success
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var resourceConfigToUpdate = new EditableResourceConfigViewModel()
        {
            Active = true, ResourceId = 1, Title = "TITLE", Url = "URL"
        };
        var existingResourceConfig = new ResourceConfig()
        {
            Active = true, ResourceId = 1, Title = "TITLE", Url = "URL"
        };

        var resourceConfigRepositoryException = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var resourceConfigRepositoryFailure = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingResourceConfig ]);
        resourceConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(false);

        var resourceConfigRepositorySuccess = new Mock<IResourceConfigRepository>();
        resourceConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingResourceConfig ]);
        resourceConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("R000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel());
        Assert.Equal("R101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel() { Title = "" });
        Assert.Equal("R101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableResourceConfigViewModel() { Title = "   " });
        Assert.Equal("R101", actual);

        // Update Exception
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToUpdate));

        // Update Failure
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToUpdate);
        Assert.Equal("R004", actual);

        // Update Success
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), resourceConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new ResourceConfigsService(_logger.Object, _resourceConfigRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("R000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("R000", actual);

        // Setup
        var resourceConfigRepositoryNull = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync((ResourceConfig)null);

        var resourceConfigRepositoryException = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ThrowsAsync(new Exception() {});

        var resourceConfigRepositoryFailure = new Mock<IResourceConfigRepository>();
        resourceConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync(new ResourceConfig()
        {
            ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
        });
        resourceConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(false);

        var resourceConfigRepositorySuccess = new Mock<IResourceConfigRepository>();
        resourceConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<ResourceConfig>())).ReturnsAsync(new ResourceConfig()
        {
            ResourceId = 1, Title = "TITLE", Url = "URL", Active = true
        });
        resourceConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<ResourceConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("R001", actual);

        // Delete Exception
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("R005", actual);

        // Delete Success
        service = new ResourceConfigsService(_logger.Object, resourceConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}