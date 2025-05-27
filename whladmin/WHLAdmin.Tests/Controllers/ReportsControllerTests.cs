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

public class ReportsControllerTests
{
    private readonly Mock<ILogger<ReportsController>> _logger = new();
    private readonly Mock<IReportsService> _reportsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new ReportsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new ReportsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new ReportsController(_logger.Object, _reportsService.Object, null));

        // Not Null
        var actual = new ReportsController(_logger.Object, _reportsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void DemographicsReportGetTests()
    {
        // Exception Test
        var reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Throws(new Exception());
        var controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        var actual = controller.DemographicsReport(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Returns(new DemographicsReportViewModel());
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = controller.DemographicsReport(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DemographicsReportViewModel>(result.Model);
    }

    [Fact]
    public async Task DemographicsReportPostTests()
    {
        // Exception Test
        var reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Throws(new Exception());
        var controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        var actual = await controller.DemographicsReport(It.IsAny<string>(), null);
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Invalid Input Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Returns(new DemographicsReportViewModel());
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.DemographicsReport(It.IsAny<string>(), null);
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DemographicsReportViewModel>(result.Model);

        // Failure Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Returns(new DemographicsReportViewModel());
        reportsService.Setup(s => s.GetApplicationDemographicsReport(It.IsAny<DemographicsReportViewModel>())).ReturnsAsync("CODE");
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.DemographicsReport(It.IsAny<string>(), new DemographicsReportViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DemographicsReportViewModel>(result.Model);
        var model = result.Model as DemographicsReportViewModel;
        Assert.Equal("CODE", model.Code);

        // Success Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeApplicationDemographicsReport()).Returns(new DemographicsReportViewModel());
        reportsService.Setup(s => s.GetApplicationDemographicsReport(It.IsAny<DemographicsReportViewModel>())).ReturnsAsync("");
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.DemographicsReport(It.IsAny<string>(), new DemographicsReportViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DemographicsReportViewModel>(result.Model);
    }

    [Fact]
    public void RegistrationsReportGetTests()
    {
        // Exception Test
        var reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Throws(new Exception());
        var controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        var actual = controller.RegistrationsReport(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Returns(new RegistrationsSummaryReportViewModel());
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = controller.RegistrationsReport(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationsSummaryReportViewModel>(result.Model);
    }

    [Fact]
    public async Task RegistrationsReportPostTests()
    {
        // Exception Test
        var reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Throws(new Exception());
        var controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        var actual = await controller.RegistrationsReport(It.IsAny<string>(), null);
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Invalid Input Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Returns(new RegistrationsSummaryReportViewModel());
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.RegistrationsReport(It.IsAny<string>(), null);
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationsSummaryReportViewModel>(result.Model);

        // Failure Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Returns(new RegistrationsSummaryReportViewModel());
        reportsService.Setup(s => s.GetRegistrationsSummaryReport(It.IsAny<RegistrationsSummaryReportViewModel>())).ReturnsAsync("CODE");
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.RegistrationsReport(It.IsAny<string>(), new RegistrationsSummaryReportViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationsSummaryReportViewModel>(result.Model);
        var model = result.Model as RegistrationsSummaryReportViewModel;
        Assert.Equal("CODE", model.Code);

        // Success Test
        reportsService = new Mock<IReportsService>();
        reportsService.Setup(s => s.InitializeRegistrationsSummaryReport()).Returns(new RegistrationsSummaryReportViewModel());
        reportsService.Setup(s => s.GetRegistrationsSummaryReport(It.IsAny<RegistrationsSummaryReportViewModel>())).ReturnsAsync("");
        controller = new ReportsController(_logger.Object, reportsService.Object, _messageService.Object);
        actual = await controller.RegistrationsReport(It.IsAny<string>(), new RegistrationsSummaryReportViewModel());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<RegistrationsSummaryReportViewModel>(result.Model);
    }
}