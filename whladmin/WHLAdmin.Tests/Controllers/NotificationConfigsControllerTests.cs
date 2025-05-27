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

public class NotificationConfigsControllerTests
{
    private readonly Mock<ILogger<NotificationConfigsController>> _logger = new();
    private readonly Mock<INotificationConfigsService> _notificationConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotificationConfigsController(_logger.Object, _notificationConfigsService.Object, null));

        // Not Null
        var actual = new NotificationConfigsController(_logger.Object, _notificationConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new NotificationConfigsViewModel());
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<NotificationConfigsViewModel>(result.Model);
    }

    [Fact]
    public async Task AddGetTests()
    {
        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetOneForAdd()).ReturnsAsync(new EditableNotificationConfigViewModel());
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_NotificationConfigEditor", result.ViewName);
        Assert.IsType<EditableNotificationConfigViewModel>(result.Model);
        var model = (EditableNotificationConfigViewModel)result.Model;
        Assert.Equal(0, model.NotificationId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amenityToAdd = new EditableNotificationConfigViewModel()
        {
            NotificationId = 0,
            Title = "TITLE"
        };

        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).Throws(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).ReturnsAsync("CODE");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).ReturnsAsync("");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableNotificationConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableNotificationConfigViewModel)null);
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableNotificationConfigViewModel()
        {
            NotificationId = 1, Title = "TITLE"
        });
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_NotificationConfigEditor", result.ViewName);
        Assert.IsType<EditableNotificationConfigViewModel>(result.Model);
        var model = (EditableNotificationConfigViewModel)result.Model;
        Assert.Equal(1, model.NotificationId);
        Assert.Equal("TITLE", model.Title);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableNotificationConfigViewModel()
        {
            NotificationId = 1,
            Title = "TITLE"
        };

        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).Throws(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).ReturnsAsync("CODE");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNotificationConfigViewModel>())).ReturnsAsync("");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableNotificationConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        notificationConfigsService = new Mock<INotificationConfigsService>();
        notificationConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new NotificationConfigsController(_logger.Object, notificationConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}