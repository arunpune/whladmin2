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

public class MarketingAgentsControllerTests
{
    private readonly Mock<ILogger<MarketingAgentsController>> _logger = new();
    private readonly Mock<IMarketingAgentsService> _agentsService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new MarketingAgentsController(_logger.Object, _agentsService.Object, null));

        // Not Null
        var actual = new MarketingAgentsController(_logger.Object, _agentsService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new MarketingAgentsViewModel());
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<MarketingAgentsViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Returns(new EditableMarketingAgentViewModel());
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_MarketingAgentEditor", result.ViewName);
        Assert.IsType<EditableMarketingAgentViewModel>(result.Model);
        var model = (EditableMarketingAgentViewModel)result.Model;
        Assert.Equal(0, model.AgentId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amenityToAdd = new EditableMarketingAgentViewModel()
        {
            AgentId = 0,
            AgentName = "NAME"
        };

        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).Throws(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).ReturnsAsync("CODE");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).ReturnsAsync("");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableMarketingAgentViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((EditableMarketingAgentViewModel)null);
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new EditableMarketingAgentViewModel()
        {
            AgentId = 1, AgentName = "NAME"
        });
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_MarketingAgentEditor", result.ViewName);
        Assert.IsType<EditableMarketingAgentViewModel>(result.Model);
        var model = (EditableMarketingAgentViewModel)result.Model;
        Assert.Equal(1, model.AgentId);
        Assert.Equal("NAME", model.AgentName);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var amenityToAdd = new EditableMarketingAgentViewModel()
        {
            AgentId = 1,
            AgentName = "NAME"
        };

        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).Throws(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).ReturnsAsync("CODE");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableMarketingAgentViewModel>())).ReturnsAsync("");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableMarketingAgentViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        agentsService = new Mock<IMarketingAgentsService>();
        agentsService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new MarketingAgentsController(_logger.Object, agentsService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}