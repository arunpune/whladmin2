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

public class AmenitiesControllerTests
{
    private readonly Mock<ILogger<AmenitiesController>> _logger = new();
    private readonly Mock<IAmenitiesService> _amenitiesService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AmenitiesController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmenitiesController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmenitiesController(_logger.Object, _amenitiesService.Object, null));

        // Not Null
        var actual = new AmenitiesController(_logger.Object, _amenitiesService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AmenitiesViewModel());
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<AmenitiesViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Returns(new EditableAmenityViewModel());
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_AmenityEditor", result.ViewName);
        Assert.IsType<EditableAmenityViewModel>(result.Model);
        var model = (EditableAmenityViewModel)result.Model;
        Assert.Equal(0, model.AmenityId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amenityToAdd = new EditableAmenityViewModel()
        {
            AmenityId = 0,
            AmenityName = "NAME"
        };

        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).Throws(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).ReturnsAsync("CODE");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).ReturnsAsync("");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableAmenityViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((EditableAmenityViewModel)null);
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new EditableAmenityViewModel()
        {
            AmenityId = 1, AmenityName = "NAME"
        });
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_AmenityEditor", result.ViewName);
        Assert.IsType<EditableAmenityViewModel>(result.Model);
        var model = (EditableAmenityViewModel)result.Model;
        Assert.Equal(1, model.AmenityId);
        Assert.Equal("NAME", model.AmenityName);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableAmenityViewModel()
        {
            AmenityId = 1,
            AmenityName = "NAME"
        };

        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).Throws(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).ReturnsAsync("CODE");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmenityViewModel>())).ReturnsAsync("");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableAmenityViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amenitiesService = new Mock<IAmenitiesService>();
        amenitiesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new AmenitiesController(_logger.Object, amenitiesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}