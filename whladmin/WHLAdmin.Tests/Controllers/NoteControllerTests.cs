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

public class NoteControllerTests
{
    private readonly Mock<ILogger<NoteController>> _logger = new();
    private readonly Mock<INotesService> _notesService = new();
    private readonly Mock<IMessageService> _messageService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new NoteController(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new NoteController(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new NoteController(_logger.Object, _notesService.Object, null));

        // Not Null
        var actual = new NoteController(_logger.Object, _notesService.Object, _messageService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task IndexTests()
    {
        // Exception Test
        var notesService = new Mock<INotesService>();
        notesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());
        var controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        var actual = await controller.Index(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        notesService = new Mock<INotesService>();
        notesService.Setup(s => s.GetData(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new NoteViewerModel());
        controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        actual = await controller.Index(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.IsType<NoteViewerModel>(result.Model);
    }

    [Fact]
    public void AddGetTests()
    {
        // Exception Test
        var notesService = new Mock<INotesService>();
        notesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
        var controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        var actual = controller.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        var result = (PartialViewResult)actual;
        Assert.Equal("Error", result.ViewName);
        Assert.IsType<ErrorViewModel>(result.Model);

        // Success Test
        notesService = new Mock<INotesService>();
        notesService.Setup(s => s.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>())).Returns(new EditableNoteViewModel());
        controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        actual = controller.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.IsType<PartialViewResult>(actual);
        result = (PartialViewResult)actual;
        Assert.Equal("_NoteEditor", result.ViewName);
        Assert.IsType<EditableNoteViewModel>(result.Model);
        var model = (EditableNoteViewModel)result.Model;
        Assert.Equal(0, model.Id);
    }

    [Fact]
    public async Task AddPostTests()
    {
        var amenityToAdd = new EditableNoteViewModel()
        {
            Id = 0,
            Note = "NOTE"
        };

        // Exception Test
        var notesService = new Mock<INotesService>();
        notesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNoteViewModel>())).Throws(new Exception());
        var controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        var actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<ObjectResult>(actual);
        var exceptionResult = (ObjectResult)actual;
        Assert.Equal(500, exceptionResult.StatusCode);
        Assert.IsType<ErrorViewModel>(exceptionResult.Value);

        // Failure Test
        notesService = new Mock<INotesService>();
        notesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNoteViewModel>())).ReturnsAsync("CODE");
        controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<BadRequestObjectResult>(actual);
        var badRequestResult = (BadRequestObjectResult)actual;
        Assert.Equal(400, badRequestResult.StatusCode);
        Assert.IsType<ErrorViewModel>(badRequestResult.Value);

        // Success Test
        notesService = new Mock<INotesService>();
        notesService.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<EditableNoteViewModel>())).ReturnsAsync("");
        controller = new NoteController(_logger.Object, notesService.Object, _messageService.Object);
        actual = await controller.Add(It.IsAny<string>(), amenityToAdd);
        Assert.IsType<OkObjectResult>(actual);
        var okResult = (OkObjectResult)actual;
        Assert.Equal(200, okResult.StatusCode);
        Assert.IsType<EditableNoteViewModel>(okResult.Value);
    }
}