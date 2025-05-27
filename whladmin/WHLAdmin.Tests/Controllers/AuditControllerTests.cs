using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Controllers;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Controllers;

public class AuditControllerTests
{
    private readonly Mock<ILogger<AuditController>> _logger = new();
    private readonly Mock<IAuditService> _auditService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AuditController(null, null));
        Assert.Throws<ArgumentNullException>(() => new AuditController(_logger.Object, null));

        // Not Null
        var actual = new AuditController(_logger.Object, _auditService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var auditService = new Mock<IAuditService>();
        auditService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new AuditController(_logger.Object, auditService.Object);
        var actual = await controller.Index(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        auditService = new Mock<IAuditService>();
        auditService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new AuditViewerModel());
        controller = new AuditController(_logger.Object, auditService.Object);
        actual = await controller.Index(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.IsType<AuditViewerModel>(result.Model);
    }
}