using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Controllers;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Controllers;

public class HomeControllerTests
{
    private readonly IConfiguration _configuration;
    private readonly Mock<ILogger<HomeController>> _logger = new();
    private readonly Mock<IUsersService> _usersService = new();

    public HomeControllerTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
                                    {"AuthMode", "BASIC"},
                                    {"OverrideEmailAddress", ""}
                                };
        _configuration = new ConfigurationBuilder()
                            .AddInMemoryCollection(inMemorySettings)
                            .Build();
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new HomeController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_configuration, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_configuration, _logger.Object, null));

        // Not Null
        var actual = new HomeController(_configuration, _logger.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void IndexTests()
    {
        // // Exception Test
        // var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        // var actual = controller.Index(It.IsAny<string>());
        // Assert.IsType<ViewResult>(actual);
        // var result = (ViewResult)actual;
        // Assert.Equal("Error", result.ViewName);
        // Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        var actual = controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public void PrivacyTests()
    {
        // // Exception Test
        // var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        // var actual = controller.Privacy(It.IsAny<string>());
        // Assert.IsType<ViewResult>(actual);
        // var result = (ViewResult)actual;
        // Assert.Equal("Error", result.ViewName);
        // Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        var actual = controller.Privacy(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public void NotAuthorizedTests()
    {
        // // Exception Test
        // var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        // var actual = controller.NotAuthorized(It.IsAny<string>());
        // Assert.IsType<ViewResult>(actual);
        // var result = (ViewResult)actual;
        // Assert.Equal("Error", result.ViewName);
        // Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        var actual = controller.NotAuthorized(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public void ErrorTests()
    {
        // Success Test
        var controller = new HomeController(_configuration, _logger.Object, _usersService.Object);
        var actual = controller.Error(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = actual as ViewResult;
        Assert.NotNull(result);
        Assert.IsType<ErrorViewModel>(result.Model);
    }
}