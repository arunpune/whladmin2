using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class FundingSourcesServiceTests()
{
    private readonly Mock<ILogger<FundingSourcesService>> _logger = new();
    private readonly Mock<IFundingSourceRepository> _fundingSourceRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesService(_logger.Object, null, null));
        Assert.Throws<ArgumentNullException>(() => new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, null));

        // Not Null
        var actual = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var fundingSourceRepositoryEmpty = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var fundingSourceRepositoryNonEmpty = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.FundingSources);

        // Not Empty
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.FundingSources);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var fundingSourceRepositoryEmpty = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var fundingSourceRepositoryNonEmpty = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            }
        ]);

        // Empty
        var service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.Empty(actual);

        // Not Empty
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var fundingSourceRepositoryNull = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNull.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync((FundingSource)null);

        var fundingSourceRepositoryNotNull = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync(new FundingSource()
        {
            FundingSourceId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FundingSourceId);
        Assert.Equal("NAME", actual.Name);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal(0, actual.FundingSourceId);
        Assert.Empty(actual.FundingSourceName);
        Assert.Empty(actual.FundingSourceDescription);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var fundingSourceRepositoryNull = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNull.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync((FundingSource)null);

        var fundingSourceRepositoryNotNull = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync(new FundingSource()
        {
            FundingSourceId = 1, Name = "NAME", Active = true
        });

        // Null
        var service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FundingSourceId);
        Assert.Equal("NAME", actual.FundingSourceName);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        FundingSource fundingSource = null;
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        service.Sanitize(fundingSource);
        Assert.Null(fundingSource);
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
        var fundingSource = new FundingSource() { Name = name, Description = description };
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        service.Sanitize(fundingSource);
        Assert.NotNull(fundingSource);
        Assert.Equal(expectedName, fundingSource.Name);
        Assert.Equal(expectedDescription, fundingSource.Description);
    }

    [Fact]
    public void ValidateTests()
    {
        // Null Test
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), null, null);
        Assert.Equal("FS000", actual);

        // Null Name Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel(), null);
        Assert.Equal("FS101", actual);

        // Empty Name Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { Name = "" }, null);
        Assert.Equal("FS101", actual);

        // Spaces Name Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { Name = "  " }, null);
        Assert.Equal("FS101", actual);

        // Valid Name, Null existing amenities Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { Name = "NAME" }, null);
        Assert.Empty(actual);

        // Valid Name, Empty existing amenities Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { Name = "NAME" }, []);
        Assert.Empty(actual);

        // Duplicate Check FAIL for ADD Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 0, Name = "NAME" },
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("FS002", actual);

        // Duplicate Check SUCCESS for ADD Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 0, Name = "NAME" },
        [
            new()
            {
                FundingSourceId = 1, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Existence Check FAIL for UPDATE Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 1, Name = "NAME" },
        [
            new()
            {
                FundingSourceId = 2, Name = "NAME", Active = true
            }
        ]);
        Assert.Equal("FS001", actual);

        // Existence Check SUCCESS for UPDATE Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 1, Name = "NAME" },
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            }
        ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 1, Name = "NEWNAME" },
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            },
            new()
            {
                FundingSourceId = 2, Name = "NEWNAME", Active = true
            }
        ]);
        Assert.Equal("FS002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        actual = service.Validate(It.IsAny<string>(), It.IsAny<string>(), new FundingSourceViewModel() { FundingSourceId = 1, Name = "NEWNAME" },
        [
            new()
            {
                FundingSourceId = 1, Name = "NAME", Active = true
            },
            new()
            {
                FundingSourceId = 2, Name = "ANOTHERNAME", Active = true
            }
        ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var fundingSourceToAdd = new EditableFundingSourceViewModel()
        {
            Active = true, FundingSourceId = 0, FundingSourceName = "NAME"
        };

        var fundingSourceRepositoryException = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var fundingSourceRepositoryFailure = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        fundingSourceRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(false);

        var fundingSourceRepositorySuccess = new Mock<IFundingSourceRepository>();
        fundingSourceRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        fundingSourceRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(true);

        // Null Test
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("FS000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel());
        Assert.Equal("FS101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel() { FundingSourceName = "" });
        Assert.Equal("FS101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel() { FundingSourceName = "   " });
        Assert.Equal("FS101", actual);

        // Add Exception
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToAdd));

        // Add Failure
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToAdd);
        Assert.Equal("FS003", actual);

        // Add Success
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var fundingSourceToUpdate = new EditableFundingSourceViewModel()
        {
            Active = true, FundingSourceId = 1, FundingSourceName = "NAME"
        };
        var existingFundingSource = new FundingSource()
        {
            Active = true, FundingSourceId = 1, Name = "NAME"
        };

        var fundingSourceRepositoryException = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var fundingSourceRepositoryFailure = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingFundingSource ]);
        fundingSourceRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(false);

        var fundingSourceRepositorySuccess = new Mock<IFundingSourceRepository>();
        fundingSourceRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingFundingSource ]);
        fundingSourceRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(true);

        // Null Test
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("FS000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel());
        Assert.Equal("FS101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel() { FundingSourceName = "" });
        Assert.Equal("FS101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), new EditableFundingSourceViewModel() { FundingSourceName = "   " });
        Assert.Equal("FS101", actual);

        // Update Exception
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToUpdate));

        // Update Failure
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToUpdate);
        Assert.Equal("FS004", actual);

        // Update Success
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), fundingSourceToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new FundingSourcesService(_logger.Object, _fundingSourceRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("FS000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("FS000", actual);

        // Setup
        var fundingSourceRepositoryNull = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryNull.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync((FundingSource)null);

        var fundingSourceRepositoryException = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryException.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ThrowsAsync(new Exception() {});

        var fundingSourceRepositoryFailure = new Mock<IFundingSourceRepository>();
        fundingSourceRepositoryFailure.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync(new FundingSource()
        {
            FundingSourceId = 1, Name = "NAME", Active = true
        });
        fundingSourceRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(false);

        var fundingSourceRepositorySuccess = new Mock<IFundingSourceRepository>();
        fundingSourceRepositorySuccess.Setup(s => s.GetOne(It.IsAny<FundingSource>())).ReturnsAsync(new FundingSource()
        {
            FundingSourceId = 1, Name = "NAME", Active = true
        });
        fundingSourceRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<FundingSource>())).ReturnsAsync(true);

        // Not Found Test
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("FS001", actual);

        // Delete Exception
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("FS005", actual);

        // Delete Success
        service = new FundingSourcesService(_logger.Object, fundingSourceRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}