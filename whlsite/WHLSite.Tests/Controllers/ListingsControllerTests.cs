using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Controllers;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Controllers;

public class ListingsControllerTests
{
    private readonly Mock<ILogger<ListingsController>> _logger = new();
    private readonly Mock<IListingService> _listingsService = new();
    private readonly Mock<IUiHelperService> _uiHelperService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ListingsController(null, null));
        Assert.Throws<ArgumentNullException>(() => new ListingsController(_logger.Object, null));

        // Not Null
        var actual = new ListingsController(_logger.Object, _listingsService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void IndexGetTests()
    {
        // Exception Test
        var listingsService = new Mock<IListingService>();
        listingsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new ListingsController(_logger.Object, listingsService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        listingsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new ListingsViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ListingsViewModel>(result.Model);
    }

    [Fact]
    public async void IndexPostTests()
    {
        // Exception Test
        var listingsService = new Mock<IListingService>();
        listingsService.Setup(s => s.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ListingSearchViewModel>())).ThrowsAsync(new Exception());
        var controller = new ListingsController(_logger.Object, listingsService.Object);
        var actual = await controller.Index(It.IsAny<string>(), It.IsAny<ListingSearchViewModel>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        listingsService.Setup(s => s.Search(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ListingSearchViewModel>())).ReturnsAsync(new ListingsViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.Index(It.IsAny<string>(), It.IsAny<ListingSearchViewModel>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ListingsViewModel>(result.Model);
    }

    [Fact]
    public async void DetailsTests()
    {
        // Exception Test
        var listingsService = new Mock<IListingService>();
        listingsService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new ListingsController(_logger.Object, listingsService.Object);
        var actual = await controller.Details(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        listingsService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync((ListingViewModel)null);
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.Details(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        listingsService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ListingViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.Details(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ListingViewModel>(result.Model);

        // Success (Print) Test
        listingsService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ListingViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.Details(It.IsAny<string>(), It.IsAny<int>(), "1");
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("PrintableDetails", result.ViewName);
        Assert.IsType<ListingViewModel>(result.Model);

        // Success Test (authenticated user)
        listingsService.Setup(s => s.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>())).ReturnsAsync(new ListingViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object)
        {
            ControllerContext = TestHelper.GetAuthenticatedClaimsControllerContext()
        };
        actual = await controller.Details(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<ListingViewModel>(result.Model);
    }

    [Fact]
    public async void PrintableFormTests()
    {
        // Exception Test
        var listingsService = new Mock<IListingService>();
        listingsService.Setup(s => s.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new ListingsController(_logger.Object, listingsService.Object);
        var actual = await controller.PrintableForm(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Failure Test
        listingsService.Setup(s => s.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((PrintableFormViewModel)null);
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.PrintableForm(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        listingsService.Setup(s => s.GetPrintableForm(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new PrintableFormViewModel());
        controller = new ListingsController(_logger.Object, listingsService.Object);
        actual = await controller.PrintableForm(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<PrintableFormViewModel>(result.Model);
    }
}