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

public class FundingSourcesControllerTests
{
    private readonly Mock<ILogger<FundingSourcesController>> _logger = new();
    private readonly Mock<IFundingSourcesService> _fundingSourcesService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesController(_logger.Object, _fundingSourcesService.Object, null));

        // Not Null
        var actual = new FundingSourcesController(_logger.Object, _fundingSourcesService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new FundingSourcesViewModel());
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<FundingSourcesViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Returns(new EditableFundingSourceViewModel());
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_FundingSourceEditor", result.ViewName);
        Assert.IsType<EditableFundingSourceViewModel>(result.Model);
        var model = (EditableFundingSourceViewModel)result.Model;
        Assert.Equal(0, model.FundingSourceId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amenityToAdd = new EditableFundingSourceViewModel()
        {
            FundingSourceId = 0,
            FundingSourceName = "NAME"
        };

        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).Throws(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).ReturnsAsync("CODE");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).ReturnsAsync("");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableFundingSourceViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((EditableFundingSourceViewModel)null);
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new EditableFundingSourceViewModel()
        {
            FundingSourceId = 1, FundingSourceName = "NAME"
        });
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_FundingSourceEditor", result.ViewName);
        Assert.IsType<EditableFundingSourceViewModel>(result.Model);
        var model = (EditableFundingSourceViewModel)result.Model;
        Assert.Equal(1, model.FundingSourceId);
        Assert.Equal("NAME", model.FundingSourceName);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableFundingSourceViewModel()
        {
            FundingSourceId = 1,
            FundingSourceName = "NAME"
        };

        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).Throws(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).ReturnsAsync("CODE");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableFundingSourceViewModel>())).ReturnsAsync("");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableFundingSourceViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        fundingSourcesService = new Mock<IFundingSourcesService>();
        fundingSourcesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new FundingSourcesController(_logger.Object, fundingSourcesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}