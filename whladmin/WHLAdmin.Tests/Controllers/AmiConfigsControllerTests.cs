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

public class AmiConfigsControllerTests
{
    private readonly Mock<ILogger<AmiConfigsController>> _logger = new();
    private readonly Mock<IAmiConfigsService> _amiConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmiConfigsController(_logger.Object, _amiConfigsService.Object, null));

        // Not Null
        var actual = new AmiConfigsController(_logger.Object, _amiConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AmiConfigsViewModel());
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<AmiConfigsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetOneForAdd()).Returns(new EditableAmiConfigViewModel() { EffectiveDate = "2025-03-01", EffectiveYear = 2025 });
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_AMIConfigEditor", result.ViewName);
        Assert.IsType<EditableAmiConfigViewModel>(result.Model);
        var model = (EditableAmiConfigViewModel)result.Model;
        Assert.Equal("2025-03-01", model.EffectiveDate);
        Assert.Equal(2025, model.EffectiveYear);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amiConfigToAdd = new EditableAmiConfigViewModel()
        {
            EffectiveDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
            EffectiveYear = DateTime.Now.Year,
            IncomeAmt = 10000,
            HhPctAmts = []
        };

        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).Throws(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amiConfigToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).ReturnsAsync("CODE");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amiConfigToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).ReturnsAsync("");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amiConfigToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableAmiConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableAmiConfigViewModel)null);
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableAmiConfigViewModel()
        {
            EffectiveDate = "2025-03-01",
            EffectiveYear = 2025,
            IncomeAmt = 10000,
            HhPctAmts = []
        });
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_AMIConfigEditor", result.ViewName);
        Assert.IsType<EditableAmiConfigViewModel>(result.Model);
        var model = (EditableAmiConfigViewModel)result.Model;
        Assert.Equal("2025-03-01", model.EffectiveDate);
        Assert.Equal(2025, model.EffectiveYear);
        Assert.Equal(10000, model.IncomeAmt);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amiConfigToUpdate = new EditableAmiConfigViewModel()
        {
            EffectiveDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
            EffectiveYear = DateTime.Now.Year,
            IncomeAmt = 10000,
            HhPctAmts = [
                new () { Pct = 70 },
                new () { Pct = 80 },
                new () { Pct = 90 },
                new () { Pct = 100 },
                new () { Pct = 108 },
                new () { Pct = 116 },
                new () { Pct = 124 },
                new () { Pct = 132 },
                new () { Pct = 140 },
                new () { Pct = 148 }
            ]
        };

        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).Throws(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amiConfigToUpdate);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).ReturnsAsync("CODE");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amiConfigToUpdate);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableAmiConfigViewModel>())).ReturnsAsync("");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amiConfigToUpdate);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableAmiConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        amiConfigsService = new Mock<IAmiConfigsService>();
        amiConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new AmiConfigsController(_logger.Object, amiConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}