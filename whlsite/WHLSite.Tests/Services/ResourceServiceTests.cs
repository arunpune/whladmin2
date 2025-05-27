using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;

namespace WHLSite.Tests.Services;

public class ResourceServiceTests
{
    private readonly Mock<ILogger<ResourceService>> _logger = new();
    private readonly Mock<IResourceRepository> _resourceRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ResourceService(null, null));
        Assert.Throws<ArgumentNullException>(() => new ResourceService(_logger.Object, null));

        // Not Null
        var actual = new ResourceService(_logger.Object, _resourceRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetDataTests()
    {
        // Exception
        var resourceRepository = new Mock<IResourceRepository>();
        resourceRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});
        var service = new ResourceService(_logger.Object, resourceRepository.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetData(It.IsAny<string>(), It.IsAny<string>()));

        // Null Resources
        service = new ResourceService(_logger.Object, _resourceRepository.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Resources);

        // No Resources
        var resources = new List<ResourceConfig>();
        resourceRepository.Setup(s => s.GetAll()).ReturnsAsync(resources);
        service = new ResourceService(_logger.Object, resourceRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Resources);

        // With Resources
        resources.Add(new()
        {
            Title = "TITLE",
            Text = "TEXT",
            Url = "URL"
        });
        resourceRepository.Setup(s => s.GetAll()).ReturnsAsync(resources);
        service = new ResourceService(_logger.Object, resourceRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Resources);
        Assert.Contains("TITLE", actual.Resources.Select(s => s.Title));
    }
}