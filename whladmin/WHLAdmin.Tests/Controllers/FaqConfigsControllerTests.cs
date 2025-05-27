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

public class FaqConfigsControllerTests
{
    private readonly Mock<ILogger<FaqConfigsController>> _logger = new();
    private readonly Mock<IFaqConfigsService> _faqConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new FaqConfigsController(_logger.Object, _faqConfigsService.Object, null));

        // Not Null
        var actual = new FaqConfigsController(_logger.Object, _faqConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FaqConfigsViewModel());
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<FaqConfigsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetOneForAdd()).Returns(new EditableFaqConfigViewModel());
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_FAQConfigEditor", result.ViewName);
        Assert.IsType<EditableFaqConfigViewModel>(result.Model);
        var model = (EditableFaqConfigViewModel)result.Model;
        Assert.Equal(0, model.FaqId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var faqToAdd = new EditableFaqConfigViewModel()
        {
            FaqId = 0,
            CategoryName = "CATEGORY",
            Title = "TITLE",
            Text = "TEXT"
        };

        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).Throws(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), faqToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).ReturnsAsync("CODE");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), faqToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).ReturnsAsync("");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), faqToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableFaqConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableFaqConfigViewModel)null);
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableFaqConfigViewModel()
        {
            FaqId = 1, Title = "TITLE"
        });
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_FAQConfigEditor", result.ViewName);
        Assert.IsType<EditableFaqConfigViewModel>(result.Model);
        var model = (EditableFaqConfigViewModel)result.Model;
        Assert.Equal(1, model.FaqId);
        Assert.Equal("TITLE", model.Title);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableFaqConfigViewModel()
        {
            FaqId = 1,
            CategoryName = "CATEGORY",
            Title = "TITLE",
            Text = "TEXT"
        };

        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).Throws(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).ReturnsAsync("CODE");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFaqConfigViewModel>())).ReturnsAsync("");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableFaqConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        faqConfigsService = new Mock<IFaqConfigsService>();
        faqConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new FaqConfigsController(_logger.Object, faqConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}