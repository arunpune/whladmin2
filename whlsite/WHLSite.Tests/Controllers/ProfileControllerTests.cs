using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Services;
using WHLSite.Controllers;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Controllers;

public class ProfileControllerTests
{
    private readonly Mock<ILogger<ProfileController>> _logger = new();
    private readonly Mock<IProfileService> _profileService = new();
    private readonly Mock<IMessageService> _messageService = new();
    private readonly Mock<IMetadataService> _metadataService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ProfileController(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileController(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileController(_logger.Object, _messageService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, null));

        // Not Null
        var actual = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void IndexTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((ProfileViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ProfileViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(false)
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ProfileViewModel>(result.Model);
    }

    [Fact]
    public async void EditProfileGetTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditProfile(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetForProfileInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditProfile(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetForProfileInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableProfileViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditProfile(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetForProfileInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableProfileViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditProfile(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableProfileViewModel>(result.Model);
    }

    [Fact]
    public async void EditProfilePostTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditProfile(It.IsAny<string>(), It.IsAny<EditableProfileViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditProfile(It.IsAny<string>(), It.IsAny<EditableProfileViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ReturnsAsync("CODE");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditProfile(It.IsAny<string>(), new EditableProfileViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableProfileViewModel>(result.Model);
        var model = (EditableProfileViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        profileService.Setup(s => s.SaveProfileInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ReturnsAsync("");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditProfile(It.IsAny<string>(), new EditableProfileViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Profile", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditContactInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditContactInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetForContactInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditContactInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetForContactInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableProfileViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditContactInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetForContactInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableProfileViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditContactInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableProfileViewModel>(result.Model);
    }

    [Fact]
    public async void EditContactInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditContactInfo(It.IsAny<string>(), It.IsAny<EditableProfileViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditContactInfo(It.IsAny<string>(), It.IsAny<EditableProfileViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ReturnsAsync("CODE");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditContactInfo(It.IsAny<string>(), new EditableProfileViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableProfileViewModel>(result.Model);
        var model = (EditableProfileViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        profileService.Setup(s => s.SaveContactInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableProfileViewModel>())).ReturnsAsync("");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditContactInfo(It.IsAny<string>(), new EditableProfileViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Profile", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditPreferencesGetTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditPreferences(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetForPreferencesInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditPreferences(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetForPreferencesInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditablePreferencesViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditPreferences(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetForPreferencesInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditablePreferencesViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditPreferences(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditablePreferencesViewModel>(result.Model);
    }

    [Fact]
    public async void EditPreferencesPostTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditPreferences(It.IsAny<string>(), It.IsAny<EditablePreferencesViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditablePreferencesViewModel>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditPreferences(It.IsAny<string>(), It.IsAny<EditablePreferencesViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditablePreferencesViewModel>())).ReturnsAsync("CODE");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditPreferences(It.IsAny<string>(), new EditablePreferencesViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditablePreferencesViewModel>(result.Model);
        var model = (EditablePreferencesViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        profileService.Setup(s => s.SavePreferencesInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditablePreferencesViewModel>())).ReturnsAsync("");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditPreferences(It.IsAny<string>(), new EditablePreferencesViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Profile", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditNetWorthGetTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditNetWorth(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetForNetWorthInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditNetWorth(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetForNetWorthInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableNetWorthViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditNetWorth(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetForNetWorthInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableNetWorthViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditNetWorth(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableNetWorthViewModel>(result.Model);
    }

    [Fact]
    public async void EditNetWorthPostTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditNetWorth(It.IsAny<string>(), It.IsAny<EditableNetWorthViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNetWorthViewModel>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditNetWorth(It.IsAny<string>(), It.IsAny<EditableNetWorthViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNetWorthViewModel>())).ReturnsAsync("CODE");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditNetWorth(It.IsAny<string>(), new EditableNetWorthViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableNetWorthViewModel>(result.Model);
        var model = (EditableNetWorthViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        profileService.Setup(s => s.SaveNetWorthInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNetWorthViewModel>())).ReturnsAsync("");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditNetWorth(It.IsAny<string>(), new EditableNetWorthViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Profile", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void NotificationsTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.Notifications(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.Notifications(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        profileService.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((UserNotificationsViewModel)null);
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.Notifications(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        profileService.Setup(s => s.GetNotifications(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UserNotificationsViewModel());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(false)
        };
        actual = await controller.Notifications(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<UserNotificationsViewModel>(result.Model);
    }

    [Fact]
    public async void UpdateNotificationPostTests()
    {
        // Unauthenticated User Test
        var controller = new ProfileController(_logger.Object, _messageService.Object, _profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.UpdateNotification(It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>());
        Assert.IsType<UnauthorizedResult>(actual);

        // Exception Test
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ThrowsAsync(new Exception());
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Bad Request Test
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ReturnsAsync("N101");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), new EditableUserNotificationViewModel());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);
        var errorModel = (ErrorViewModel)badRequestResult.Value;
        Assert.Equal("N101", errorModel.Code);

        // Bad Request Test
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ReturnsAsync("N102");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), new EditableUserNotificationViewModel());
        Assert.IsType<BadRequestObjectResult>(actual);
        badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);
        errorModel = (ErrorViewModel)badRequestResult.Value;
        Assert.Equal("N102", errorModel.Code);

        // Bad Request Test
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ReturnsAsync("N103");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), new EditableUserNotificationViewModel());
        Assert.IsType<BadRequestObjectResult>(actual);
        badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);
        errorModel = (ErrorViewModel)badRequestResult.Value;
        Assert.Equal("N103", errorModel.Code);

        // Failure Test
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ReturnsAsync("CODE");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), new EditableUserNotificationViewModel());
        Assert.IsType<ObjectResult>(actual);
        var objectResult = (ObjectResult)actual;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.IsType<ErrorViewModel>(objectResult.Value);
        errorModel = (ErrorViewModel)objectResult.Value;
        Assert.Equal("CODE", errorModel.Code);

        // Success Test
        profileService.Setup(s => s.UpdateNotification(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserNotificationViewModel>())).ReturnsAsync("");
        controller = new ProfileController(_logger.Object, _messageService.Object, profileService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.UpdateNotification(It.IsAny<string>(), new EditableUserNotificationViewModel());
        Assert.IsType<OkResult>(actual);
    }
}