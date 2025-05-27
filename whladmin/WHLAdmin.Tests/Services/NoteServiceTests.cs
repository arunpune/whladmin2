using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class NotesServiceTests()
{
    private readonly Mock<ILogger<NotesService>> _logger = new();
    private readonly Mock<INoteRepository> _noteRepository = new();
    private readonly Mock<IMetadataService> _metadataService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new NotesService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotesService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new NotesService(_logger.Object, _noteRepository.Object, null));

        // Not Null
        var actual = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var noteRepositoryEmpty = new Mock<INoteRepository>();
        noteRepositoryEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);

        var noteRepositoryNonEmpty = new Mock<INoteRepository>();
        noteRepositoryNonEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
        [
            new()
            {
                ActionCd = "ADD",
                ActionDescription = "Add",
                EntityDescription = "Amenity",
                EntityId = "1",
                EntityName = "NAME",
                EntityTypeCd = "AMENITY",
                Id = 1,
                Note = "New note added.",
                Timestamp = DateTime.Now,
                Username = "USERNAME"
            }
        ]);

        // Empty
        var service = new NotesService(_logger.Object, noteRepositoryEmpty.Object, _metadataService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Notes);

        // Not Empty
        service = new NotesService(_logger.Object, noteRepositoryNonEmpty.Object, _metadataService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Notes);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var noteRepositoryEmpty = new Mock<INoteRepository>();
        noteRepositoryEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);

        var noteRepositoryNonEmpty = new Mock<INoteRepository>();
        noteRepositoryNonEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
        [
            new()
            {
                ActionCd = "ADD",
                ActionDescription = "Add",
                EntityDescription = "Amenity",
                EntityId = "1",
                EntityName = "NAME",
                EntityTypeCd = "AMENITY",
                Id = 1,
                Note = "New amenity added.",
                Timestamp = DateTime.Now,
                Username = "USERNAME"
            }
        ]);

        // Empty
        var service = new NotesService(_logger.Object, noteRepositoryEmpty.Object, _metadataService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new NotesService(_logger.Object, noteRepositoryNonEmpty.Object, _metadataService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        var actual = service.GetOneForAdd("TYPE", "ID");
        Assert.NotNull(actual);
        Assert.Equal(0, actual.Id);
        Assert.Empty(actual.Note);
        Assert.Equal("TYPE", actual.EntityTypeCd);
        Assert.Equal("ID", actual.EntityId);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        Note note = null;
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        service.Sanitize(note);
        Assert.Null(note);
    }

    [Theory]
    [InlineData(null, null, null, "", "", "")]
    [InlineData("", null, null, "", "", "")]
    [InlineData(" ", null, null, "", "", "")]
    [InlineData("TYPE", null, null, "TYPE", "", "")]
    [InlineData("TYPE ", null, null, "TYPE", "", "")]
    [InlineData(" TYPE", null, null, "TYPE", "", "")]
    [InlineData(" TYPE ", null, null, "TYPE", "", "")]
    [InlineData("TYPE", "", null, "TYPE", "", "")]
    [InlineData("TYPE", " ", null, "TYPE", "", "")]
    [InlineData("TYPE", "ID", null, "TYPE", "ID", "")]
    [InlineData("TYPE", "ID ", null, "TYPE", "ID", "")]
    [InlineData("TYPE", " ID", null, "TYPE", "ID", "")]
    [InlineData("TYPE", " ID ", null, "TYPE", "ID", "")]
    [InlineData("TYPE", "ID", "", "TYPE", "ID", "")]
    [InlineData("TYPE", "ID", " ", "TYPE", "ID", "")]
    [InlineData("TYPE", "ID", "NOTE", "TYPE", "ID", "NOTE")]
    [InlineData("TYPE", "ID", "NOTE ", "TYPE", "ID", "NOTE")]
    [InlineData("TYPE", "ID", " NOTE", "TYPE", "ID", "NOTE")]
    [InlineData("TYPE", "ID", " NOTE ", "TYPE", "ID", "NOTE")]
    public void SanitizeObjectTests(string entityTypeCd, string entityId, string note,
                                        string expectedEntityTypeCd, string expectedEntityId, string expectedNote)
    {
        var noteEntry = new Note() { EntityTypeCd = entityTypeCd, EntityId = entityId, Note = note };
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        service.Sanitize(noteEntry);
        Assert.NotNull(noteEntry);
        Assert.Equal(expectedEntityTypeCd, noteEntry.EntityTypeCd);
        Assert.Equal(expectedEntityId, noteEntry.EntityId);
        Assert.Equal(expectedNote, noteEntry.Note);
    }

    [Fact]
    public void ValidateNullTests()
    {
        // Null Test
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        var actual = service.Validate(null);
        Assert.Equal("NT000", actual);
    }

    [Theory]
    [InlineData(null, null, null, "NT101")]
    [InlineData("", null, null, "NT101")]
    [InlineData(" ", null, null, "NT101")]
    [InlineData("TYPE", null, null, "NT102")]
    [InlineData("TYPE", "", null, "NT102")]
    [InlineData("TYPE", " ", null, "NT102")]
    [InlineData("TYPE", "ID", null, "NT102")]
    [InlineData("TYPE", "1", null, "NT103")]
    [InlineData("TYPE", "1", "", "NT103")]
    [InlineData("TYPE", "1", " ", "NT103")]
    [InlineData("TYPE", "1", "NOTE", "")]
    public void ValidateTests(string entityTypeCd, string entityId, string note, string expectedCode)
    {
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        var actual = service.Validate(new Note() { EntityTypeCd = entityTypeCd, EntityId = entityId, Note = note });
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var noteToAdd = new EditableNoteViewModel()
        {
            EntityTypeCd = "TYPE", EntityId = "1", Note = "NOTE"
        };

        var noteRepositoryException = new Mock<INoteRepository>();
        noteRepositoryException.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Note>())).ThrowsAsync(new Exception() {});

        var amenityRepositoryFailure = new Mock<INoteRepository>();
        amenityRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Note>())).ReturnsAsync(false);

        var amenityRepositorySuccess = new Mock<INoteRepository>();
        amenityRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Note>())).ReturnsAsync(true);

        // Null Test
        var service = new NotesService(_logger.Object, _noteRepository.Object, _metadataService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("NT000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNoteViewModel());
        Assert.Equal("NT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNoteViewModel() { EntityTypeCd = "" });
        Assert.Equal("NT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableNoteViewModel() { EntityTypeCd = "   " });
        Assert.Equal("NT101", actual);

        // Add Exception
        service = new NotesService(_logger.Object, noteRepositoryException.Object, _metadataService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), noteToAdd));

        // Add Failure
        service = new NotesService(_logger.Object, amenityRepositoryFailure.Object, _metadataService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), noteToAdd);
        Assert.Equal("NT003", actual);

        // Add Success
        service = new NotesService(_logger.Object, amenityRepositorySuccess.Object, _metadataService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), noteToAdd);
        Assert.Empty(actual);
    }
}