using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;

namespace WHLAdmin.Tests.Services;

public class AuditServiceTests()
{
    private readonly Mock<ILogger<AuditService>> _logger = new();
    private readonly Mock<IAuditRepository> _auditRepository = new();
    private readonly Mock<IMetadataService> _metadataService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AuditService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AuditService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AuditService(_logger.Object, _auditRepository.Object, null));

        // Not Null
        var actual = new AuditService(_logger.Object, _auditRepository.Object, _metadataService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var auditRepositoryEmpty = new Mock<IAuditRepository>();
        auditRepositoryEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);

        var auditRepositoryNonEmpty = new Mock<IAuditRepository>();
        auditRepositoryNonEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
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
        var service = new AuditService(_logger.Object, auditRepositoryEmpty.Object, _metadataService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Entries);

        // Not Empty
        service = new AuditService(_logger.Object, auditRepositoryNonEmpty.Object, _metadataService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Entries);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var auditRepositoryEmpty = new Mock<IAuditRepository>();
        auditRepositoryEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync([]);

        var auditRepositoryNonEmpty = new Mock<IAuditRepository>();
        auditRepositoryNonEmpty.Setup(s => s.GetAll(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(
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
        var service = new AuditService(_logger.Object, auditRepositoryEmpty.Object, _metadataService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new AuditService(_logger.Object, auditRepositoryNonEmpty.Object, _metadataService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }
}