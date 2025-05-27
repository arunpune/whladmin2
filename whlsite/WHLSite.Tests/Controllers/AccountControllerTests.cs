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

public class AccountControllerTests
{
    private readonly Mock<ILogger<AccountController>> _logger = new();
    private readonly Mock<IAccountService> _accountService = new();
    private readonly Mock<IMessageService> _messageService = new();
    private readonly Mock<IProfileService> _profileService = new();

    public AccountControllerTests()
    {
    }

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AccountController(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountController(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountController(_logger.Object, _accountService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AccountController(_logger.Object, _accountService.Object, _messageService.Object, null));

        // Not Null
        var actual = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void RegisterGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.Register(It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForRegistration(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        accountService.Setup(s => s.GetForRegistration(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RegistrationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationViewModel>(result.Model);
    }

    [Fact]
    public async void RegisterPostTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.Register(It.IsAny<string>(), It.IsAny<RegistrationViewModel>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Null Input
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForRegistration(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new RegistrationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), It.IsAny<RegistrationViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;

        // Exception Test
        var metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetPhoneNumberTypes(It.IsAny<bool>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), new RegistrationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        metadataService = new Mock<IMetadataService>();
        metadataService.Setup(s => s.GetLeadTypes(It.IsAny<bool>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), new RegistrationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RegistrationViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), new RegistrationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failed to Register Test
        accountService.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RegistrationViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), new RegistrationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationViewModel>(result.Model);
        var model = result.Model as RegistrationViewModel;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RegistrationViewModel>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Register(It.IsAny<string>(), new RegistrationViewModel() { Username = "USERNAME" });
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Registered", result.ViewName);
        Assert.IsType<AccountViewModel>(result.Model);
        var accountModel = result.Model as AccountViewModel;
        Assert.Equal("USERNAME", accountModel.Username);
    }

    [Fact]
    public void ResendActivationGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = controller.ResendActivation(It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForResendActivationLink(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = controller.ResendActivation(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        accountService.Setup(s => s.GetForResendActivationLink(It.IsAny<string>(), It.IsAny<string>())).Returns(new ResendActivationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = controller.ResendActivation(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ResendActivationViewModel>(result.Model);
    }

    [Fact]
    public async void ResendActivationPostTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.ResendActivation(It.IsAny<string>(), It.IsAny<ResendActivationViewModel>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Null Input
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForResendActivationLink(It.IsAny<string>(), It.IsAny<string>())).Returns(new ResendActivationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResendActivation(It.IsAny<string>(), It.IsAny<ResendActivationViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;

        // Exception Test
        accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResendActivation(It.IsAny<string>(), new ResendActivationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failed to Request Activation Link Test
        accountService.Setup(s => s.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResendActivation(It.IsAny<string>(), new ResendActivationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("ActivationFailed", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);
        var model = result.Model as ErrorViewModel;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.ResendActivationLink(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResendActivation(It.IsAny<string>(), new ResendActivationViewModel() { Username = "USERNAME" });
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("ActivationRequested", result.ViewName);
        Assert.IsType<AccountViewModel>(result.Model);
        var accountModel = result.Model as AccountViewModel;
        Assert.Equal("USERNAME", accountModel.Username);
    }

    [Fact]
    public async void ActivateGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.Activate(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Activate(It.IsAny<string>(), "KEY");
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failed to Request Activation Link Test
        accountService.Setup(s => s.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Activate(It.IsAny<string>(), "KEY");
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("ActivationFailed", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);
        var model = result.Model as ErrorViewModel;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.Activate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.Activate(It.IsAny<string>(), "KEY" );
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Activated", result.ViewName);
    }

    [Fact]
    public void LogInGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = controller.LogIn(It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = controller.LogIn(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        accountService.Setup(s => s.GetForLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new LogInViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = controller.LogIn(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<LogInViewModel>(result.Model);
    }

    [Fact]
    public async void LogInPostTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.LogIn(It.IsAny<string>(), It.IsAny<LogInViewModel>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Null Input
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new LogInViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.LogIn(It.IsAny<string>(), It.IsAny<LogInViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;

        // Exception Test
        accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LogInViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.LogIn(It.IsAny<string>(), new LogInViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failed to Login Link Test
        accountService.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LogInViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.LogIn(It.IsAny<string>(), new LogInViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<LogInViewModel>(result.Model);
        var model = result.Model as LogInViewModel;
        Assert.Equal("CODE", model.Code);

        // Incomplete Profile Test
        accountService.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LogInViewModel>())).ReturnsAsync("");
        var profileService = new Mock<IProfileService>();
        profileService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ProfileViewModel());
        profileService.Setup(s => s.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProfileViewModel>())).Returns(true);
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.LogIn(It.IsAny<string>(), new LogInViewModel() { Username = "USERNAME" });
        Assert.IsType<RedirectToActionResult>(actual);
        redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Complete Profile, With Redirect Url Test
        accountService.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>(), new LogInViewModel())).ReturnsAsync("");
        profileService.Setup(s => s.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProfileViewModel>())).Returns(false);
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.LogIn(It.IsAny<string>(), new LogInViewModel() { Username = "USERNAME", ReturnUrl = "/ABC" });
        Assert.IsType<RedirectResult>(actual);
        var redirectOtherResult = (RedirectResult)actual;
        Assert.Equal("/ABC", redirectOtherResult.Url);

        // Complete Profile, No Redirect Url Test
        accountService.Setup(s => s.Login(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<LogInViewModel>())).ReturnsAsync("");
        profileService.Setup(s => s.IsIncompleteProfile(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ProfileViewModel>())).Returns(false);
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.LogIn(It.IsAny<string>(), new LogInViewModel() { Username = "USERNAME" });
        Assert.IsType<RedirectToActionResult>(actual);
        redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Listings", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async void LogOutGetTests()
    {
        // Exception Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        var actual = await controller.LogOut(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Authenticated User Test
        controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.LogOut(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Unauthenticated User Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForLogin(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new LogInViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.LogOut(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);
    }

    [Fact]
    public async void ChangePasswordGetTests()
    {
        // Unauthenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.ChangePassword(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Authenticated User, Not Found Test
        accountService.Setup(s => s.GetForChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((ChangePasswordViewModel)null);
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("LogOut", redirectActionResult.ActionName);

        // Authenticated User, Not Found Test
        accountService.Setup(s => s.GetForChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ChangePasswordViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ChangePasswordViewModel>(result.Model);
    }

    [Fact]
    public async void ChangePasswordPostTests()
    {
        // Unauthenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.ChangePassword(It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>(), new ChangePasswordViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Change Password Failure Test
        accountService.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>(), new ChangePasswordViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ChangePasswordViewModel>(result.Model);
        var model = (ChangePasswordViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Change Password Success Test
        accountService.Setup(s => s.ChangePassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.ChangePassword(It.IsAny<string>(), new ChangePasswordViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("PasswordChangeComplete", result.ViewName);
    }

    [Fact]
    public void ResetPasswordRequestGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = controller.ResetPasswordRequest(It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForPasswordResetLink(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = controller.ResetPasswordRequest(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        accountService.Setup(s => s.GetForPasswordResetLink(It.IsAny<string>(), It.IsAny<string>())).Returns(new ResendActivationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = controller.ResetPasswordRequest(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ResendActivationViewModel>(result.Model);
    }

    [Fact]
    public async void ResetPasswordRequestPostTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.ResetPasswordRequest(It.IsAny<string>(), It.IsAny<ResendActivationViewModel>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Null Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForPasswordResetLink(It.IsAny<string>(), It.IsAny<string>())).Returns(new ResendActivationViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPasswordRequest(It.IsAny<string>(), It.IsAny<ResendActivationViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.IsType<ResendActivationViewModel>(result.Model);

        // Exception Test
        accountService.Setup(s => s.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPasswordRequest(It.IsAny<string>(), new ResendActivationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        accountService.Setup(s => s.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPasswordRequest(It.IsAny<string>(), new ResendActivationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ResendActivationViewModel>(result.Model);
        var model = (ResendActivationViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.RequestPasswordReset(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ResendActivationViewModel>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPasswordRequest(It.IsAny<string>(), new ResendActivationViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("ResetPasswordRequested", result.ViewName);
    }

    [Fact]
    public async void ResetPasswordGetTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        accountService.Setup(s => s.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ChangePasswordViewModel() { Code = "CODE" });
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("ResetPasswordRequestFailed", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);
        var model = (ErrorViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.GetForPasswordResetRequest(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ChangePasswordViewModel());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        model = (ChangePasswordViewModel)result.Model;
    }

    [Fact]
    public async void ResetPasswordPostTests()
    {
        // Authenticated User Test
        var controller = new AccountController(_logger.Object, _accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        var actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectResult = (RedirectToActionResult)actual;
        Assert.Equal("Profile", redirectResult.ControllerName);
        Assert.Equal("Index", redirectResult.ActionName);

        // Exception Test
        var accountService = new Mock<IAccountService>();
        accountService.Setup(s => s.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ThrowsAsync(new Exception());
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        accountService.Setup(s => s.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ReturnsAsync("CODE");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), new ChangePasswordViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ChangePasswordViewModel>(result.Model);
        var model = (ChangePasswordViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        accountService.Setup(s => s.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ChangePasswordViewModel>())).ReturnsAsync("");
        controller = new AccountController(_logger.Object, accountService.Object, _messageService.Object, _profileService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext(),
        };
        actual = await controller.ResetPassword(It.IsAny<string>(), new ChangePasswordViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("PasswordResetComplete", result.ViewName);
    }
}