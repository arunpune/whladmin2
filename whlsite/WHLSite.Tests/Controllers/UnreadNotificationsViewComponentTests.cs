using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Controllers;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Controllers;

public class UnreadNotificationsViewComponentTests
{
    private readonly Mock<ILogger<UnreadNotificationsViewComponent>> _logger = new();
    private readonly Mock<IUserRepository> _userRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new UnreadNotificationsViewComponent(null, null));
        Assert.Throws<ArgumentNullException>(() => new UnreadNotificationsViewComponent(_logger.Object, null));

        // Not Null
        var actual = new UnreadNotificationsViewComponent(_logger.Object, _userRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void InvokeAsyncTests()
    {
        // Unauthenticated User Test
        List<UserNotification> list = null;
        var userRepositoryNullInfo = new Mock<IUserRepository>();
        userRepositoryNullInfo.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(list);
        var view = new UnreadNotificationsViewComponent(_logger.Object, userRepositoryNullInfo.Object);
        var actual = await view.InvokeAsync(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        var result = actual as ViewViewComponentResult;
        Assert.NotNull(result);

        // Exception Test
        var userRepositoryException = new Mock<IUserRepository>();
        userRepositoryException.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        view = new UnreadNotificationsViewComponent(_logger.Object, userRepositoryException.Object)
        {
            ViewComponentContext = TestHelper.GetAuthenticatedClaimsViewComponentContext()
        };
        actual = await view.InvokeAsync(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        result = actual as ViewViewComponentResult;
        Assert.NotNull(result);

        // No Unread Notifications Test
        list = null;
        userRepositoryNullInfo = new Mock<IUserRepository>();
        userRepositoryNullInfo.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(list);
        view = new UnreadNotificationsViewComponent(_logger.Object, userRepositoryNullInfo.Object)
        {
            ViewComponentContext = TestHelper.GetAuthenticatedClaimsViewComponentContext()
        };
        actual = await view.InvokeAsync(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        result = actual as ViewViewComponentResult;
        Assert.NotNull(result);

        // Has Unread notifications Test
        list = [];
        var userRepository = new Mock<IUserRepository>();
        userRepositoryNullInfo.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(list);
        view = new UnreadNotificationsViewComponent(_logger.Object, userRepositoryNullInfo.Object)
        {
            ViewComponentContext = TestHelper.GetAuthenticatedClaimsViewComponentContext()
        };
        actual = await view.InvokeAsync(It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.IsType<ViewViewComponentResult>(actual);
        result = actual as ViewViewComponentResult;
        Assert.NotNull(result);
    }
}