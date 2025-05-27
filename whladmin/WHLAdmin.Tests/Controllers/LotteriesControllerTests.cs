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

public class LotteriesControllerTests
{
    private readonly Mock<ILogger<LotteriesController>> _logger = new();
    private readonly Mock<ILotteriesService> _lotteriesService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new LotteriesController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new LotteriesController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new LotteriesController(_logger.Object, _lotteriesService.Object, null));

        // Not Null
        var actual = new LotteriesController(_logger.Object, _lotteriesService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.GetEligibleListings(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.GetEligibleListings(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ListingsViewModel());
        controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ListingsViewModel>(result.Model);
    }

    [Fact]
    public async Task RunPostTests()
    {
        var amenityToAdd = new EditableAmenityViewModel()
        {
            AmenityId = 0,
            AmenityName = "NAME"
        };

        // Exception Test
        var lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.RunLottery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool>())).Throws(new Exception());
        var controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        var actual = await controller.RunLottery(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.RunLottery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool>())).ReturnsAsync("LT");
        controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        actual = await controller.RunLottery(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.RunLottery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<long>(), It.IsAny<bool>())).ReturnsAsync("12345");
        controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        actual = await controller.RunLottery(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<bool>());
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<ListingViewModel>(okResult.Value);
        var listingModel = okResult.Value as ListingViewModel;
        Assert.Equal(12345, listingModel.LotteryId);
    }

    [Fact]
    public async Task ResultsTests()
    {
        // Exception Test
        var lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.GetResults(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        var actual = await controller.Results(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.GetResults(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new LotteryResultsViewModel());
        controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        actual = await controller.Results(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<LotteryResultsViewModel>(result.Model);

        // Success Test
        lotteriesService = new Mock<ILotteriesService>();
        lotteriesService.Setup(s => s.GetResults(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new LotteryResultsViewModel());
        controller = new LotteriesController(_logger.Object, lotteriesService.Object, _messageService.Object);
        actual = await controller.Results(It.IsAny<string>(), -1, It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<LotteryResultsViewModel>(result.Model);
    }
}