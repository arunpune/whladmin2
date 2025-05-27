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

public class VideoConfigsControllerTests
{
    private readonly Mock<ILogger<VideoConfigsController>> _logger = new();
    private readonly Mock<IVideoConfigsService> _videoConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new VideoConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new VideoConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new VideoConfigsController(_logger.Object, _videoConfigsService.Object, null));

        // Not Null
        var actual = new VideoConfigsController(_logger.Object, _videoConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new VideoConfigsViewModel());
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<VideoConfigsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetOneForAdd()).Returns(new EditableVideoConfigViewModel());
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_VideoConfigEditor", result.ViewName);
        Assert.IsType<EditableVideoConfigViewModel>(result.Model);
        var model = (EditableVideoConfigViewModel)result.Model;
        Assert.Equal(0, model.VideoId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var VideoToAdd = new EditableVideoConfigViewModel()
        {
            VideoId = 0,
            Title = "TITLE",
            Url = "URL"
        };

        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).Throws(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).ReturnsAsync("CODE");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).ReturnsAsync("");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), VideoToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableVideoConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableVideoConfigViewModel)null);
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableVideoConfigViewModel()
        {
            VideoId = 1, Title = "TITLE"
        });
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_VideoConfigEditor", result.ViewName);
        Assert.IsType<EditableVideoConfigViewModel>(result.Model);
        var model = (EditableVideoConfigViewModel)result.Model;
        Assert.Equal(1, model.VideoId);
        Assert.Equal("TITLE", model.Title);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableVideoConfigViewModel()
        {
            VideoId = 1,
            Title = "TITLE",
            Url = "URL"
        };

        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).Throws(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).ReturnsAsync("CODE");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableVideoConfigViewModel>())).ReturnsAsync("");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableVideoConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        videoConfigsService = new Mock<IVideoConfigsService>();
        videoConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new VideoConfigsController(_logger.Object, videoConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}