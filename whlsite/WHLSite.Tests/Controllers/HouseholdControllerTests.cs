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

public class HouseholdControllerTests
{
    private readonly Mock<ILogger<HouseholdController>> _logger = new();
    private readonly Mock<IHouseholdService> _householdService = new();
    private readonly Mock<IMessageService> _messageService = new();
    private readonly Mock<IMetadataService> _metadataService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new HouseholdController(null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdController(_logger.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdController(_logger.Object, _householdService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, null));

        // Not Null
        var actual = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void IndexTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((HouseholdViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new HouseholdViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(false)
        };
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<HouseholdViewModel>(result.Model);
    }

    [Fact]
    public async void EditAddressInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditAddressInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetForAddressInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetForAddressInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableAddressInfoViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetForAddressInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableAddressInfoViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableAddressInfoViewModel>(result.Model);
    }

    [Fact]
    public async void EditAddressInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditAddressInfo(It.IsAny<string>(), It.IsAny<EditableAddressInfoViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAddressInfoViewModel>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>(), It.IsAny<EditableAddressInfoViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAddressInfoViewModel>())).ReturnsAsync("CODE");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>(), new EditableAddressInfoViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableAddressInfoViewModel>(result.Model);
        var model = (EditableAddressInfoViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        householdService.Setup(s => s.SaveAddressInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAddressInfoViewModel>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditAddressInfo(It.IsAny<string>(), new EditableAddressInfoViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditVoucherInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditVoucherInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetForVoucherInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetForVoucherInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableVoucherInfoViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetForVoucherInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableVoucherInfoViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableVoucherInfoViewModel>(result.Model);
    }

    [Fact]
    public async void EditVoucherInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditVoucherInfo(It.IsAny<string>(), It.IsAny<EditableVoucherInfoViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVoucherInfoViewModel>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>(), It.IsAny<EditableVoucherInfoViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVoucherInfoViewModel>())).ReturnsAsync("CODE");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>(), new EditableVoucherInfoViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableVoucherInfoViewModel>(result.Model);
        var model = (EditableVoucherInfoViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        householdService.Setup(s => s.SaveVoucherInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVoucherInfoViewModel>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditVoucherInfo(It.IsAny<string>(), new EditableVoucherInfoViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditLiveInAideInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditLiveInAideInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetForLiveInAideInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetForLiveInAideInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((EditableLiveInAideInfoViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetForLiveInAideInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new EditableLiveInAideInfoViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableLiveInAideInfoViewModel>(result.Model);
    }

    [Fact]
    public async void EditLiveInAideInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditLiveInAideInfo(It.IsAny<string>(), It.IsAny<EditableLiveInAideInfoViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableLiveInAideInfoViewModel>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>(), It.IsAny<EditableLiveInAideInfoViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableLiveInAideInfoViewModel>())).ReturnsAsync("CODE");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>(), new EditableLiveInAideInfoViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableLiveInAideInfoViewModel>(result.Model);
        var model = (EditableLiveInAideInfoViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        householdService.Setup(s => s.SaveLiveInAideInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableLiveInAideInfoViewModel>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditLiveInAideInfo(It.IsAny<string>(), new EditableLiveInAideInfoViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditMemberInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync((EditableHouseholdMemberViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetForMemberInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new EditableHouseholdMemberViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableHouseholdMemberViewModel>(result.Model);
    }

    [Fact]
    public async void EditMemberInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditMember(It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>())).ReturnsAsync("CODE");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditMember(It.IsAny<string>(), new EditableHouseholdMemberViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableHouseholdMemberViewModel>(result.Model);
        var model = (EditableHouseholdMemberViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        householdService.Setup(s => s.SaveMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdMemberViewModel>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditMember(It.IsAny<string>(), new EditableHouseholdMemberViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void DeleteMemberInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Not Found Test
        householdService.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H001");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<NotFoundObjectResult>(actual);
        var notFoundResult = (NotFoundObjectResult)actual;
        Assert.IsType<ErrorViewModel>(notFoundResult.Value);
        var errorModel = (ErrorViewModel)notFoundResult.Value;
        Assert.Equal("H001", errorModel.Code);

        // Not Found Test
        householdService.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H401");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<NotFoundObjectResult>(actual);
        notFoundResult = (NotFoundObjectResult)actual;
        Assert.IsType<ErrorViewModel>(notFoundResult.Value);
        errorModel = (ErrorViewModel)notFoundResult.Value;
        Assert.Equal("H401", errorModel.Code);

        // Failure Test
        householdService.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H405");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ObjectResult>(actual);
        var objectResult = (ObjectResult)actual;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.IsType<ErrorViewModel>(objectResult.Value);
        errorModel = (ErrorViewModel)objectResult.Value;
        Assert.Equal("H405", errorModel.Code);

        // Success Test
        householdService.Setup(s => s.DeleteMemberInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.DeleteMember(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void EditAccountInfoGetTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync((EditableHouseholdAccountViewModel)null);
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Success Test
        householdService.Setup(s => s.GetForAccountInfoEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync(new EditableHouseholdAccountViewModel());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableHouseholdAccountViewModel>(result.Model);
    }

    [Fact]
    public async void EditAccountInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.EditAccount(It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        householdService.Setup(s => s.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>())).ReturnsAsync("CODE");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.EditAccount(It.IsAny<string>(), new EditableHouseholdAccountViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<EditableHouseholdAccountViewModel>(result.Model);
        var model = (EditableHouseholdAccountViewModel)result.Model;
        Assert.Equal("CODE", model.Code);

        // Success Test
        householdService.Setup(s => s.SaveAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableHouseholdAccountViewModel>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.EditAccount(It.IsAny<string>(), new EditableHouseholdAccountViewModel());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }

    [Fact]
    public async void DeleteAccountInfoPostTests()
    {
        // Unauthenticated User Test
        var controller = new HouseholdController(_logger.Object, _householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetDefaultControllerContext()
        };
        var actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<LocalRedirectResult>(actual);
        var redirectResult = (LocalRedirectResult)actual;
        Assert.Equal("/", redirectResult.Url);

        // Exception Test
        var householdService = new Mock<IHouseholdService>();
        householdService.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ThrowsAsync(new Exception());
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedControllerContext()
        };
        actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Not Found Test
        householdService.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H001");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<NotFoundObjectResult>(actual);
        var notFoundResult = (NotFoundObjectResult)actual;
        Assert.IsType<ErrorViewModel>(notFoundResult.Value);
        var errorModel = (ErrorViewModel)notFoundResult.Value;
        Assert.Equal("H001", errorModel.Code);

        // Not Found Test
        householdService.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H501");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<NotFoundObjectResult>(actual);
        notFoundResult = (NotFoundObjectResult)actual;
        Assert.IsType<ErrorViewModel>(notFoundResult.Value);
        errorModel = (ErrorViewModel)notFoundResult.Value;
        Assert.Equal("H501", errorModel.Code);

        // Failure Test
        householdService.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("H505");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true)
        };
        actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<ObjectResult>(actual);
        var objectResult = (ObjectResult)actual;
        Assert.Equal(500, objectResult.StatusCode);
        Assert.IsType<ErrorViewModel>(objectResult.Value);
        errorModel = (ErrorViewModel)objectResult.Value;
        Assert.Equal("H505", errorModel.Code);

        // Success Test
        householdService.Setup(s => s.DeleteAccountInfo(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>())).ReturnsAsync("");
        controller = new HouseholdController(_logger.Object, householdService.Object, _messageService.Object, _metadataService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext(true),
            Url = new UrlHelper(new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor()))
        };
        actual = await controller.DeleteAccount(It.IsAny<string>(), It.IsAny<long>());
        Assert.IsType<RedirectToActionResult>(actual);
        var redirectActionResult = (RedirectToActionResult)actual;
        Assert.Equal("Index", redirectActionResult.ActionName);
        Assert.Equal("Household", redirectActionResult.ControllerName);
    }
}