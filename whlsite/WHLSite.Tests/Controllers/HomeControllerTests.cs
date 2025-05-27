using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Controllers;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Controllers;

public class HomeControllerTests
{
    private readonly Mock<ILogger<HomeController>> _logger = new();
    private readonly Mock<IFaqService> _faqsService = new();
    private readonly Mock<IQuoteService> _quotesService = new();
    private readonly Mock<IResourceService> _resourcesService = new();
    private readonly Mock<IVideoService> _videosService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new HomeController(null, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_logger.Object, null, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_logger.Object, _faqsService.Object, null, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, null));

        // Not Null
        var actual = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void IndexTests()
    {
        // Exception Test
        var quotesService = new Mock<IQuoteService>();
        quotesService.Setup(s => s.GetQuoteForHomePage(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new HomeController(_logger.Object, _faqsService.Object, quotesService.Object, _resourcesService.Object, _videosService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        var videosService = new Mock<IVideoService>();
        videosService.Setup(s => s.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, videosService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public async void AboutTests()
    {
        // Exception Test
        var videosService = new Mock<IVideoService>();
        videosService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, videosService.Object);
        var actual = await controller.About(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        actual = await controller.About(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public async void FaqsTests()
    {
        // Exception Test
        var faqsService = new Mock<IFaqService>();
        faqsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new HomeController(_logger.Object, faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        var actual = await controller.Faqs(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        actual = await controller.Faqs(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public async void ResourcesTests()
    {
        // Exception Test
        var resourcesService = new Mock<IResourceService>();
        resourcesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, resourcesService.Object, _videosService.Object);
        var actual = await controller.Resources(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        actual = await controller.Resources(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
    }

    [Fact]
    public void ErrorTests()
    {
        // Success Test
        var controller = new HomeController(_logger.Object, _faqsService.Object, _quotesService.Object, _resourcesService.Object, _videosService.Object);
        var actual = controller.Error(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = actual as ViewResult;
        Assert.NotNull(result);
        Assert.IsType<ErrorViewModel>(result.Model);
    }
}