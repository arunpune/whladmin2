using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class AmenitiesServiceTests()
{
    private readonly Mock<ILogger<AmenitiesService>> _logger = new();
    private readonly Mock<IAmenityRepository> _amenityRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new AmenitiesService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmenitiesService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new AmenitiesService(_logger.Object, _amenityRepository.Object, null));

        // Not Null
        var actual = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var amenityRepositoryEmpty = new Mock<IAmenityRepository>();
        amenityRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amenityRepositoryNonEmpty = new Mock<IAmenityRepository>();
        amenityRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Amenities);

        // Not Empty
        service = new AmenitiesService(_logger.Object, amenityRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Amenities);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var amenityRepositoryEmpty = new Mock<IAmenityRepository>();
        amenityRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var amenityRepositoryNonEmpty = new Mock<IAmenityRepository>();
        amenityRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new AmenitiesService(_logger.Object, amenityRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new AmenitiesService(_logger.Object, amenityRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var amenityRepositoryNull = new Mock<IAmenityRepository>();
        amenityRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync((Amenity)null);

        var amenityRepositoryNotNull = new Mock<IAmenityRepository>();
        amenityRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync(new Amenity()
        {
            AmenityId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new AmenitiesService(_logger.Object, amenityRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmenitiesService(_logger.Object, amenityRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AmenityId);
        Assert.Equal("NAME", actual.Name);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(0, actual.AmenityId);
        Assert.Empty(actual.AmenityName);
        Assert.Empty(actual.AmenityDescription);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var amenityRepositoryNull = new Mock<IAmenityRepository>();
        amenityRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync((Amenity)null);

        var amenityRepositoryNotNull = new Mock<IAmenityRepository>();
        amenityRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync(new Amenity()
        {
            AmenityId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new AmenitiesService(_logger.Object, amenityRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new AmenitiesService(_logger.Object, amenityRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AmenityId);
        Assert.Equal("NAME", actual.AmenityName);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        Amenity amenity = null;
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        service.Sanitize(amenity);
        Assert.Null(amenity);
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
        var amenity = new Amenity() { Name = name, Description = description };
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        service.Sanitize(amenity);
        Assert.NotNull(amenity);
        Assert.Equal(expectedName, amenity.Name);
        Assert.Equal(expectedDescription, amenity.Description);
    }

    [Fact]
    public void ValidateTests()
    {
        // Null Test
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), null, null);
        Assert.Equal("A000", actual);

        // Null Name Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel(), null);
        Assert.Equal("A101", actual);

        // Empty Name Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { Name = "" }, null);
        Assert.Equal("A101", actual);

        // Spaces Name Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { Name = "  " }, null);
        Assert.Equal("A101", actual);

        // Valid Name, Null existing amenities Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { Name = "NAME" }, null);
        Assert.Empty(actual);

        // Valid Name, Empty existing amenities Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { Name = "NAME" }, []);
        Assert.Empty(actual);

        // Duplicate Check FAIL for ADD Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 0, Name = "NAME" },
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("A002", actual);

        // Duplicate Check SUCCESS for ADD Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 0, Name = "NAME" },
        [
            new()
            {
                AmenityId = 1, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Existence Check FAIL for UPDATE Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 1, Name = "NAME" },
        [
            new()
            {
                AmenityId = 2, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("A001", actual);

        // Existence Check SUCCESS for UPDATE Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 1, Name = "NAME" },
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 1, Name = "NEWNAME" },
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            },
            new()
            {
                AmenityId = 2, Name = "NEWNAME", Active = true
            }
        ]);
        Assert.Equal("A002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new AmenityViewModel() { AmenityId = 1, Name = "NEWNAME" },
        [
            new()
            {
                AmenityId = 1, Name = "NAME", Active = true
            },
            new()
            {
                AmenityId = 2, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var amenityToAdd = new EditableAmenityViewModel()
        {
            Active = true, AmenityId = 0, AmenityName = "NAME"
        };

        var amenityRepositoryException = new Mock<IAmenityRepository>();
        amenityRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var amenityRepositoryFailure = new Mock<IAmenityRepository>();
        amenityRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        amenityRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(false);

        var amenityRepositorySuccess = new Mock<IAmenityRepository>();
        amenityRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        amenityRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(true);

        // Null Test
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("A000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel());
        Assert.Equal("A101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel() { AmenityName = "" });
        Assert.Equal("A101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel() { AmenityName = "   " });
        Assert.Equal("A101", actual);

        // Add Exception
        service = new AmenitiesService(_logger.Object, amenityRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToAdd));

        // Add Failure
        service = new AmenitiesService(_logger.Object, amenityRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToAdd);
        Assert.Equal("A003", actual);

        // Add Success
        service = new AmenitiesService(_logger.Object, amenityRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var amenityToUpdate = new EditableAmenityViewModel()
        {
            Active = true, AmenityId = 1, AmenityName = "NAME"
        };
        var existingAmenity = new Amenity()
        {
            Active = true, AmenityId = 1, Name = "NAME"
        };

        var amenityRepositoryException = new Mock<IAmenityRepository>();
        amenityRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var amenityRepositoryFailure = new Mock<IAmenityRepository>();
        amenityRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingAmenity ]);
        amenityRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(false);

        var amenityRepositorySuccess = new Mock<IAmenityRepository>();
        amenityRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingAmenity ]);
        amenityRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(true);

        // Null Test
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("A000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel());
        Assert.Equal("A101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel() { AmenityName = "" });
        Assert.Equal("A101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableAmenityViewModel() { AmenityName = "   " });
        Assert.Equal("A101", actual);

        // Update Exception
        service = new AmenitiesService(_logger.Object, amenityRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToUpdate));

        // Update Failure
        service = new AmenitiesService(_logger.Object, amenityRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToUpdate);
        Assert.Equal("A004", actual);

        // Update Success
        service = new AmenitiesService(_logger.Object, amenityRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), amenityToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new AmenitiesService(_logger.Object, _amenityRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("A000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("A000", actual);

        // Setup
        var amenityRepositoryNull = new Mock<IAmenityRepository>();
        amenityRepositoryNull.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync((Amenity)null);

        var amenityRepositoryException = new Mock<IAmenityRepository>();
        amenityRepositoryException.Setup(s => s.GetOne(It.IsAny<Amenity>())).ThrowsAsync(new Exception() {});

        var amenityRepositoryFailure = new Mock<IAmenityRepository>();
        amenityRepositoryFailure.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync(new Amenity()
        {
            AmenityId = 1, Name = "NAME", Active = true
        });
        amenityRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(false);

        var amenityRepositorySuccess = new Mock<IAmenityRepository>();
        amenityRepositorySuccess.Setup(s => s.GetOne(It.IsAny<Amenity>())).ReturnsAsync(new Amenity()
        {
            AmenityId = 1, Name = "NAME", Active = true
        });
        amenityRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<Amenity>())).ReturnsAsync(true);

        // Not Found Test
        service = new AmenitiesService(_logger.Object, amenityRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("A001", actual);

        // Delete Exception
        service = new AmenitiesService(_logger.Object, amenityRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new AmenitiesService(_logger.Object, amenityRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("A005", actual);

        // Delete Success
        service = new AmenitiesService(_logger.Object, amenityRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}