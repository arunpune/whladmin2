using System;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Services;
using WHLSite.Controllers;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Controllers;

public class ApplicationsControllerTests
{
    private readonly Mock<ILogger<ApplicationsController>> _logger = new();
    private readonly Mock<IHousingApplicationService> _applicationsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ApplicationsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ApplicationsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ApplicationsController(_logger.Object, _applicationsService.Object, null));

        // Not Null
        var actual = new ApplicationsController(_logger.Object, _applicationsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void DashboardTests()
    {
        // Unauthenticated User Test
        var controller = new ApplicationsController(_logger.Object, _applicationsService.Object, _messageService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.Dashboard(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var applicationsService = new Mock<IHousingApplicationService>();
        applicationsService.Setup(s => s.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ApplicationsController(_logger.Object, applicationsService.Object, _messageService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.Dashboard(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        applicationsService.Setup(s => s.GetForDashboard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new DashboardViewModel());
        controller = new ApplicationsController(_logger.Object, applicationsService.Object, _messageService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.Dashboard(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DashboardViewModel>(result.Model);
    }
}