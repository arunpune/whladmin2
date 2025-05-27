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

public class QuoteConfigsControllerTests
{
    private readonly Mock<ILogger<QuoteConfigsController>> _logger = new();
    private readonly Mock<IQuoteConfigsService> _quoteConfigsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new QuoteConfigsController(_logger.Object, _quoteConfigsService.Object, null));

        // Not Null
        var actual = new QuoteConfigsController(_logger.Object, _quoteConfigsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new QuoteConfigsViewModel());
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<QuoteConfigsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetOneForAdd()).Throws(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetOneForAdd()).Returns(new EditableQuoteConfigViewModel());
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_QuoteConfigEditor", result.ViewName);
        Assert.IsType<EditableQuoteConfigViewModel>(result.Model);
        var model = (EditableQuoteConfigViewModel)result.Model;
        Assert.Equal(0, model.QuoteId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var QuoteToAdd = new EditableQuoteConfigViewModel()
        {
            QuoteId = 0,
            Text = "TEXT"
        };

        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).Throws(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), QuoteToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).ReturnsAsync("CODE");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), QuoteToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).ReturnsAsync("");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), QuoteToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableQuoteConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync((EditableQuoteConfigViewModel)null);
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.GetOneForEdit(It.IsAny<int>())).ReturnsAsync(new EditableQuoteConfigViewModel()
        {
            QuoteId = 1, Text = "TEXT"
        });
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_QuoteConfigEditor", result.ViewName);
        Assert.IsType<EditableQuoteConfigViewModel>(result.Model);
        var model = (EditableQuoteConfigViewModel)result.Model;
        Assert.Equal(1, model.QuoteId);
        Assert.Equal("TEXT", model.Text);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableQuoteConfigViewModel()
        {
            QuoteId = 1,
            Text = "TEXT"
        };

        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).Throws(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).ReturnsAsync("CODE");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableQuoteConfigViewModel>())).ReturnsAsync("");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableQuoteConfigViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        quoteConfigsService = new Mock<IQuoteConfigsService>();
        quoteConfigsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new QuoteConfigsController(_logger.Object, quoteConfigsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}