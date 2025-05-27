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

public class ResourceConfigsControllerTests
{
    private readonly Mock<ILogger<ResourceConfigsController>> _logger = new();
    private readonly Mock<IResourceConfigsService> _resourceConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ResourceConfigsController(_logger.Object, _resourceConfigsService.Object, null));

        // Not Null
        var actual = new ResourceConfigsController(_logger.Object, _resourceConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ResourceConfigsViewModel());
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ResourceConfigsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetOneForAdd()).Returns(new EditableResourceConfigViewModel());
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_ResourceConfigEditor", result.ViewName);
        Assert.IsType<EditableResourceConfigViewModel>(result.Model);
        var model = (EditableResourceConfigViewModel)result.Model;
        Assert.Equal(0, model.ResourceId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var ResourceToAdd = new EditableResourceConfigViewModel()
        {
            ResourceId = 0,
            Title = "TITLE",
            Url = "URL"
        };

        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).Throws(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), ResourceToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).ReturnsAsync("CODE");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), ResourceToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).ReturnsAsync("");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), ResourceToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableResourceConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableResourceConfigViewModel)null);
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableResourceConfigViewModel()
        {
            ResourceId = 1, Title = "TITLE"
        });
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_ResourceConfigEditor", result.ViewName);
        Assert.IsType<EditableResourceConfigViewModel>(result.Model);
        var model = (EditableResourceConfigViewModel)result.Model;
        Assert.Equal(1, model.ResourceId);
        Assert.Equal("TITLE", model.Title);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableResourceConfigViewModel()
        {
            ResourceId = 1,
            Title = "TITLE",
            Url = "URL"
        };

        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).Throws(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).ReturnsAsync("CODE");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableResourceConfigViewModel>())).ReturnsAsync("");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableResourceConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        resourceConfigsService = new Mock<IResourceConfigsService>();
        resourceConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new ResourceConfigsController(_logger.Object, resourceConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}