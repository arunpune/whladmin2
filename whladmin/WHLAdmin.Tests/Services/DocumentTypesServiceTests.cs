using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class DocumentTypesServiceTests()
{
    private readonly Mock<ILogger<DocumentTypesService>> _logger = new();
    private readonly Mock<IDocumentTypeRepository> _documentTypeRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, null));

        // Not Null
        var actual = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var documentTypeRepositoryEmpty = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var documentTypeRepositoryNonEmpty = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.DocumentTypes);

        // Not Empty
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.DocumentTypes);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var documentTypeRepositoryEmpty = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var documentTypeRepositoryNonEmpty = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new DocumentTypesService(_logger.Object, documentTypeRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var documentTypeRepositoryNull = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNull.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync((DocumentType)null);

        var documentTypeRepositoryNotNull = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync(new DocumentType()
        {
            DocumentTypeId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.DocumentTypeId);
        Assert.Equal("NAME", actual.Name);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(0, actual.DocumentTypeId);
        Assert.Empty(actual.DocumentTypeName);
        Assert.Empty(actual.DocumentTypeDescription);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var documentTypeRepositoryNull = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNull.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync((DocumentType)null);

        var documentTypeRepositoryNotNull = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync(new DocumentType()
        {
            DocumentTypeId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.DocumentTypeId);
        Assert.Equal("NAME", actual.DocumentTypeName);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        DocumentType documentType = null;
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        service.Sanitize(documentType);
        Assert.Null(documentType);
    }

    [Theory]
    [InlineData(null, null, "", null)]
    [InlineData("", null, "", null)]
    [InlineData(" ", null, "", null)]
    [InlineData("NAME", null, "NAME", null)]
    [InlineData("NAME ", null, "NAME", null)]
    [InlineData(" NAME", null, "NAME", null)]
    [InlineData(" NAME ", null, "NAME", null)]
    [InlineData("NAME", "", "NAME", null)]
    [InlineData("NAME", " ", "NAME", null)]
    [InlineData("NAME", "DESC", "NAME", "DESC")]
    [InlineData("NAME", "DESC ", "NAME", "DESC")]
    [InlineData("NAME", " DESC", "NAME", "DESC")]
    [InlineData("NAME", " DESC ", "NAME", "DESC")]
    public void SanitizeObjectTests(string name, string description, string expectedName, string expectedDescription)
    {
        var documentType = new DocumentType() { Name = name, Description = description };
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        service.Sanitize(documentType);
        Assert.NotNull(documentType);
        Assert.Equal(expectedName, documentType.Name);
        Assert.Equal(expectedDescription, documentType.Description);
    }

    [Fact]
    public void ValidateTests()
    {
        // Null Test
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), null, null);
        Assert.Equal("DT000", actual);

        // Null Name Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel(), null);
        Assert.Equal("DT101", actual);

        // Empty Name Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { Name = "" }, null);
        Assert.Equal("DT101", actual);

        // Spaces Name Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { Name = "  " }, null);
        Assert.Equal("DT101", actual);

        // Valid Name, Null existing amenities Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { Name = "NAME" }, null);
        Assert.Empty(actual);

        // Valid Name, Empty existing amenities Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { Name = "NAME" }, []);
        Assert.Empty(actual);

        // Duplicate Check FAIL for ADD Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 0, Name = "NAME" },
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("DT002", actual);

        // Duplicate Check SUCCESS for ADD Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 0, Name = "NAME" },
        [
            new()
            {
                DocumentTypeId = 1, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Existence Check FAIL for UPDATE Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 1, Name = "NAME" },
        [
            new()
            {
                DocumentTypeId = 2, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("DT001", actual);

        // Existence Check SUCCESS for UPDATE Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 1, Name = "NAME" },
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 1, Name = "NEWNAME" },
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            },
            new()
            {
                DocumentTypeId = 2, Name = "NEWNAME", Active = true
            }
        ]);
        Assert.Equal("DT002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new DocumentTypeViewModel() { DocumentTypeId = 1, Name = "NEWNAME" },
        [
            new()
            {
                DocumentTypeId = 1, Name = "NAME", Active = true
            },
            new()
            {
                DocumentTypeId = 2, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var documentTypeToAdd = new EditableDocumentTypeViewModel()
        {
            Active = true, DocumentTypeId = 0, DocumentTypeName = "NAME"
        };

        var documentTypeRepositoryException = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var documentTypeRepositoryFailure = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        documentTypeRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(false);

        var documentTypeRepositorySuccess = new Mock<IDocumentTypeRepository>();
        documentTypeRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        documentTypeRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(true);

        // Null Test
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("DT000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel());
        Assert.Equal("DT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel() { DocumentTypeName = "" });
        Assert.Equal("DT101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel() { DocumentTypeName = "   " });
        Assert.Equal("DT101", actual);

        // Add Exception
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToAdd));

        // Add Failure
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToAdd);
        Assert.Equal("DT003", actual);

        // Add Success
        service = new DocumentTypesService(_logger.Object, documentTypeRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var documentTypeToUpdate = new EditableDocumentTypeViewModel()
        {
            Active = true, DocumentTypeId = 1, DocumentTypeName = "NAME"
        };
        var existingDocumentType = new DocumentType()
        {
            Active = true, DocumentTypeId = 1, Name = "NAME"
        };

        var documentTypeRepositoryException = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var documentTypeRepositoryFailure = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingDocumentType ]);
        documentTypeRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(false);

        var documentTypeRepositorySuccess = new Mock<IDocumentTypeRepository>();
        documentTypeRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingDocumentType ]);
        documentTypeRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(true);

        // Null Test
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("DT000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel());
        Assert.Equal("DT101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel() { DocumentTypeName = "" });
        Assert.Equal("DT101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableDocumentTypeViewModel() { DocumentTypeName = "   " });
        Assert.Equal("DT101", actual);

        // Update Exception
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToUpdate));

        // Update Failure
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToUpdate);
        Assert.Equal("DT004", actual);

        // Update Success
        service = new DocumentTypesService(_logger.Object, documentTypeRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), documentTypeToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new DocumentTypesService(_logger.Object, _documentTypeRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("DT000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("DT000", actual);

        // Setup
        var documentTypeRepositoryNull = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryNull.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync((DocumentType)null);

        var documentTypeRepositoryException = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryException.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ThrowsAsync(new Exception() {});

        var documentTypeRepositoryFailure = new Mock<IDocumentTypeRepository>();
        documentTypeRepositoryFailure.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync(new DocumentType()
        {
            DocumentTypeId = 1, Name = "NAME", Active = true
        });
        documentTypeRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(false);

        var documentTypeRepositorySuccess = new Mock<IDocumentTypeRepository>();
        documentTypeRepositorySuccess.Setup(s => s.GetOne(It.IsAny<DocumentType>())).ReturnsAsync(new DocumentType()
        {
            DocumentTypeId = 1, Name = "NAME", Active = true
        });
        documentTypeRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<DocumentType>())).ReturnsAsync(true);

        // Not Found Test
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("DT001", actual);

        // Delete Exception
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new DocumentTypesService(_logger.Object, documentTypeRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("DT005", actual);

        // Delete Success
        service = new DocumentTypesService(_logger.Object, documentTypeRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}