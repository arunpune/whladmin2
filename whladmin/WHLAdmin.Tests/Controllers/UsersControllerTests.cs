using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Services;
using WHLAdmin.Controllers;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<ILogger<UsersController>> _logger = new();
    private readonly Mock<IUsersService> _usersService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new UsersController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new UsersController(_logger.Object, _usersService.Object, null));

        // Not Null
        var actual = new UsersController(_logger.Object, _usersService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new UsersViewModel());
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<UsersViewModel>(result.Model);
    }

    [Fact]
    public async Task AddGetTests()
    {
        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetOneForAdd()).ReturnsAsync(new EditableUserViewModel());
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_UserEditor", result.ViewName);
        Assert.IsType<EditableUserViewModel>(result.Model);
        var model = (EditableUserViewModel)result.Model;
        Assert.Null(model.EmailAddress);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var VideoToAdd = new EditableUserViewModel()
        {
            EmailAddress = "EMAIL@UNIT.TST",
            DisplayName = "NAME",
            RoleCd = "ROLECD",
            OrganizationCd = "ORGCD"
        };

        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).Throws(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).ReturnsAsync("CODE");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).ReturnsAsync("");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableUserViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetOneForEdit(It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetOneForEdit(It.IsAny<string>())).ReturnsAsync((EditableUserViewModel)null);
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.GetOneForEdit(It.IsAny<string>())).ReturnsAsync(new EditableUserViewModel()
        {
            EmailAddress = "EMAIL@UNIT.TST", DisplayName = "NAME", RoleCd = "ROLECD", OrganizationCd = "ORGCD"
        });
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_UserEditor", result.ViewName);
        Assert.IsType<EditableUserViewModel>(result.Model);
        var model = (EditableUserViewModel)result.Model;
        Assert.Equal("EMAIL@UNIT.TST", model.EmailAddress);
        Assert.Equal("NAME", model.DisplayName);
        Assert.Equal("ROLECD", model.RoleCd);
        Assert.Equal("ORGCD", model.OrganizationCd);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableUserViewModel()
        {
            EmailAddress = "EMAIL@UNIT.TST",
            DisplayName = "NAME",
            RoleCd = "ROLECD",
            OrganizationCd = "ORGCD"
        };

        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).Throws(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).ReturnsAsync("CODE");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableUserViewModel>())).ReturnsAsync("");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableUserViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("CODE");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<OkResult>(actual);
    }

    [Fact]
    public async Task ReactivatePostTests()
    {
        // Exception Test
        var usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Reactivate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        var actual = await controller.Reactivate(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Reactivate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("CODE");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Reactivate(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        usersService = new Mock<IUsersService>();
        usersService.Setup(s => s.Reactivate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("");
        controller = new UsersController(_logger.Object, usersService.Object, _messageService.Object);
        actual = await controller.Reactivate(It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<OkResult>(actual);
    }
}