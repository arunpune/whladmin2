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

public class DocumentTypesControllerTests
{
    private readonly Mock<ILogger<DocumentTypesController>> _logger = new();
    private readonly Mock<IDocumentTypesService> _documentTypesService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesController(_logger.Object, _documentTypesService.Object, null));

        // Not Null
        var actual = new DocumentTypesController(_logger.Object, _documentTypesService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        var result = (ViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new DocumentTypesViewModel());
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>());
        Assert.IsType<ViewResult>(actual);
        result = (ViewResult)actual;
        Assert.IsType<DocumentTypesViewModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Returns(new EditableDocumentTypeViewModel());
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_DocumentTypeEditor", result.ViewName);
        Assert.IsType<EditableDocumentTypeViewModel>(result.Model);
        var model = (EditableDocumentTypeViewModel)result.Model;
        Assert.Equal(0, model.DocumentTypeId);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var documentTypeToAdd = new EditableDocumentTypeViewModel()
        {
            DocumentTypeId = 0,
            DocumentTypeName = "NAME"
        };

        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).Throws(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).ReturnsAsync("CODE");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).ReturnsAsync("");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableDocumentTypeViewModel>(okResult.Value);
    }

    [Fact]
    public async Task EditGetTests()
    {
        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Exception Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync((EditableDocumentTypeViewModel)null);
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<NotFoundResult>(actual);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync(new EditableDocumentTypeViewModel()
        {
            DocumentTypeId = 1, DocumentTypeName = "NAME"
        });
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_DocumentTypeEditor", result.ViewName);
        Assert.IsType<EditableDocumentTypeViewModel>(result.Model);
        var model = (EditableDocumentTypeViewModel)result.Model;
        Assert.Equal(1, model.DocumentTypeId);
        Assert.Equal("NAME", model.DocumentTypeName);
    }

    [Fact]
    public async Task EditPostTests()
    {
        var documentTypeToAdd = new EditableDocumentTypeViewModel()
        {
            DocumentTypeId = 1,
            DocumentTypeName = "NAME"
        };

        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).Throws(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = await controller.Edit(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).ReturnsAsync("CODE");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableDocumentTypeViewModel>())).ReturnsAsync("");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Edit(It.IsAny<string>(), documentTypeToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableDocumentTypeViewModel>(okResult.Value);
    }

    [Fact]
    public async Task DeletePostTests()
    {
        // Exception Test
        var documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ThrowsAsync(new Exception());
        var controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        var actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("CODE");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        documentTypesService = new Mock<IDocumentTypesService>();
        documentTypesService.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>())).ReturnsAsync("");
        controller = new DocumentTypesController(_logger.Object, documentTypesService.Object, _messageService.Object);
        actual = await controller.Delete(It.IsAny<string>(), It.IsAny<int>());
        Assert.IsType<OkResult>(actual);
    }
}