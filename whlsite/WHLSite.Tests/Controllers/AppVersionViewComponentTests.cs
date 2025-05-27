using System;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Controllers;

namespace WHLSite.Tests.Controllers;

public class AppVersionViewComponentTests
{
    private readonly Mock<ILogger<AppVersionViewComponent>> _logger = new();
    private readonly Mock<ISystemRepository> _systemRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AppVersionViewComponent(null, null));
        Assert.Throws<ArgumentNullException>(() => new AppVersionViewComponent(_logger.Object, null));

        // Not Null
        var actual = new AppVersionViewComponent(_logger.Object, _systemRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void InvokeAsyncTests()
    {
        // Exception Test
        var systemRepositoryException = new Mock<ISystemRepository>(); 
        systemRepositoryException.Setup(s => s.GetInfo()).ThrowsAsync(new Exception());
        var view = new AppVersionViewComponent(_logger.Object, systemRepositoryException.Object);
        var actual = await view.InvokeAsync();
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        var result = actual as ViewViewComponentResult;
        Assert.NotNull(result);

        // No System Info Test
        var systemRepositoryNullInfo = new Mock<ISystemRepository>(); 
        systemRepositoryNullInfo.Setup(s => s.GetInfo()).ReturnsAsync((SystemInfo)null);
        view = new AppVersionViewComponent(_logger.Object, systemRepositoryNullInfo.Object);
        actual = await view.InvokeAsync();
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        result = actual as ViewViewComponentResult;
        Assert.NotNull(result);

        // Has System Info Test
        var systemRepository = new Mock<ISystemRepository>(); 
        systemRepositoryNullInfo.Setup(s => s.GetInfo()).ReturnsAsync(new SystemInfo());
        view = new AppVersionViewComponent(_logger.Object, systemRepositoryNullInfo.Object);
        actual = await view.InvokeAsync();
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        result = actual as ViewViewComponentResult;
        Assert.NotNull(result);
    }
}