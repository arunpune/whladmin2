using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class VideoConfigsServiceTests()
{
    private readonly Mock<ILogger<VideoConfigsService>> _logger = new();
    private readonly Mock<IVideoConfigRepository> _videoConfigRepository = new();
    private readonly Mock<IUsersService> _usersService = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new VideoConfigsService(null, null, null));
        Assert.Throws<ArgumentNullException>(() => new VideoConfigsService(_logger.Object, null, null));

        // Not Null
        var actual = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task GetDataTests()
    {
        // Setup
        var videoConfigRepositoryEmpty = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var videoConfigRepositoryNonEmpty = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                VideoId = 1, Title = "TITLE", Url = "URL", Active = true
            }
        ]);

        // Empty
        var service = new VideoConfigsService(_logger.Object, videoConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Videos);

        // Not Empty
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Videos);
    }

    [Fact]
    public async Task GetAllTests()
    {
        // Setup
        var videoConfigRepositoryEmpty = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryEmpty.Setup(s => s.GetAll()).ReturnsAsync([]);

        var videoConfigRepositoryNonEmpty = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNonEmpty.Setup(s => s.GetAll()).ReturnsAsync(
        [
            new()
            {
                VideoId = 1, Title = "TITLE", Url = "URL", Active = true
            }
        ]);

        // Empty
        var service = new VideoConfigsService(_logger.Object, videoConfigRepositoryEmpty.Object, _usersService.Object);
        var actual = await service.GetAll();
        Assert.Empty(actual);

        // Not Empty
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNonEmpty.Object, _usersService.Object);
        actual = await service.GetAll();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task GetOneTests()
    {
        // Setup
        var videoConfigRepositoryNull = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync((VideoConfig)null);

        var videoConfigRepositoryNotNull = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync(new VideoConfig()
        {
            VideoId = 1, Title = "TITLE", Url = "URL", Active = true
        });

        // Null
        var service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOne(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOne(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.VideoId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
    }

    [Fact]
    public void GetOneForAddTests()
    {
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = service.GetOneForAdd();
        Assert.NotNull(actual);
        Assert.Equal(0, actual.VideoId);
        Assert.Empty(actual.Text);
        Assert.Empty(actual.Title);
        Assert.True(actual.Active);
    }

    [Fact]
    public async Task GetOneForEditTests()
    {
        // Setup
        var videoConfigRepositoryNull = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync((VideoConfig)null);

        var videoConfigRepositoryNotNull = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNotNull.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync(new VideoConfig()
        {
            VideoId = 1, Title = "TITLE", Url = "URL", Active = true
        });

        // Null
        var service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNull.Object, _usersService.Object);
        var actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.Null(actual);

        // Not Empty
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNotNull.Object, _usersService.Object);
        actual = await service.GetOneForEdit(It.IsAny<int>());
        Assert.NotNull(actual);
        Assert.Equal(1, actual.VideoId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
    }

    [Fact]
    public void SanitizeNullTest()
    {
        VideoConfig videoConfig = null;
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        service.Sanitize(videoConfig);
        Assert.Null(videoConfig);
    }

    [Theory]
    [InlineData(null, null, "", "")]
    [InlineData("", null, "", "")]
    [InlineData(" ", null, "", "")]
    [InlineData("TITLE", null, "TITLE", "")]
    [InlineData("TITLE ", null, "TITLE", "")]
    [InlineData(" TITLE", null, "TITLE", "")]
    [InlineData(" TITLE ", null, "TITLE", "")]
    [InlineData("TITLE", "", "TITLE", "")]
    [InlineData("TITLE", " ", "TITLE", "")]
    [InlineData("TITLE", "URL", "TITLE", "URL")]
    [InlineData("TITLE", "URL ", "TITLE", "URL")]
    [InlineData("TITLE", " URL", "TITLE", "URL")]
    [InlineData("TITLE", " URL ", "TITLE", "URL")]
    public void SanitizeObjectTests(string title, string url, string expectedTitle, string expectedUrl)
    {
        var videoConfig = new VideoConfig() { Title = title, Url = url };
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        service.Sanitize(videoConfig);
        Assert.NotNull(videoConfig);
        Assert.Equal(expectedTitle, videoConfig.Title);
        Assert.Equal(expectedUrl, videoConfig.Url);
    }

    [Fact]
    public void ValidateNullTest()
    {
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(null, null);
        Assert.Equal("V000", actual);
    }

    [Theory]
    [InlineData(null, null, "V101")]
    [InlineData("", null, "V101")]
    [InlineData(" ", null, "V101")]
    [InlineData("TITLE", null, "V102")]
    [InlineData("TITLE", "", "V102")]
    [InlineData("TITLE", " ", "V102")]
    [InlineData("TITLE", "URL", "")]
    public void ValidateBaseTests(string title, string url, string expectedCode)
    {
        var videoToValidate = new VideoConfig()
        {
            Title = title, Url = url
        };

        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(videoToValidate, null);
        Assert.Equal(expectedCode, actual);
    }

    [Fact]
    public void ValidateExistenceTests()
    {
        // Setup
        var videoToValidate = new VideoConfig() { VideoId = 1, Title = "TITLE", Url = "URL" };

        // Existence Check FAIL for UPDATE Test
        var existingVideo = new VideoConfig() { VideoId = 2, Title = "TITLE", Url = "URL" };
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(videoToValidate, [ existingVideo ]);
        Assert.Equal("V001", actual);

        // Existence Check SUCCESS for UPDATE Test
        existingVideo.VideoId = 1;
        service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        actual = service.Validate(videoToValidate, [ existingVideo ]);
        Assert.Empty(actual);
    }

    [Fact]
    public void ValidateDuplicateTests()
    {
        // Setup
        var videoToValidate = new VideoConfig() { VideoId = 0, Title = "TITLE", Url = "URL" };

        // Duplicate Check FAIL for ADD Test
        var existingVideo = new VideoConfig() { VideoId = 1, Title = "TITLE", Url = "URL" };
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = service.Validate(videoToValidate, [ existingVideo ]);
        Assert.Equal("V002", actual);

        // Duplicate Check SUCCESS for ADD Test
        existingVideo.Title = "ANOTHERTITLE";
        service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        actual = service.Validate(videoToValidate, [ existingVideo ]);
        Assert.Empty(actual);

        // Duplicate Check FAIL for UPDATE Test
        videoToValidate.VideoId = 1;
        existingVideo = new VideoConfig() { VideoId = 2, Title = "TITLE", Url = "URL" };
        service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        actual = service.Validate(videoToValidate, [ videoToValidate, existingVideo ]);
        Assert.Equal("V002", actual);

        // Duplicate Check SUCCESS for UPDATE Test
        existingVideo = new VideoConfig() { VideoId = 2, Title = "ANOTHERTITLE", Url = "URL" };
        service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        actual = service.Validate(videoToValidate, [ videoToValidate, existingVideo ]);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task AddTests()
    {
        // Setup
        var videoConfigToAdd = new EditableVideoConfigViewModel()
        {
            Active = true, VideoId = 0, Title = "TITLE", Url = "URL"
        };

        var videoConfigRepositoryException = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var videoConfigRepositoryFailure = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([]);
        videoConfigRepositoryFailure.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(false);

        var videoConfigRepositorySuccess = new Mock<IVideoConfigRepository>();
        videoConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([]);
        videoConfigRepositorySuccess.Setup(s => s.Add(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("V000", actual);

        // Validation FAIL Tests
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel());
        Assert.Equal("V101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel() { Title = "" });
        Assert.Equal("V101", actual);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel() { Title = "   " });
        Assert.Equal("V101", actual);

        // Add Exception
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Add(It.IsAny<string>(), It.IsAny<string>(), videoConfigToAdd));

        // Add Failure
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), videoConfigToAdd);
        Assert.Equal("V003", actual);

        // Add Success
        service = new VideoConfigsService(_logger.Object, videoConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Add(It.IsAny<string>(), It.IsAny<string>(), videoConfigToAdd);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task UpdateTests()
    {
        // Setup
        var videoConfigToUpdate = new EditableVideoConfigViewModel()
        {
            Active = true, VideoId = 1, Title = "TITLE", Url = "URL"
        };
        var existingVideoConfig = new VideoConfig()
        {
            Active = true, VideoId = 1, Title = "TITLE", Url = "URL"
        };

        var videoConfigRepositoryException = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryException.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});

        var videoConfigRepositoryFailure = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryFailure.Setup(s => s.GetAll()).ReturnsAsync([ existingVideoConfig ]);
        videoConfigRepositoryFailure.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(false);

        var videoConfigRepositorySuccess = new Mock<IVideoConfigRepository>();
        videoConfigRepositorySuccess.Setup(s => s.GetAll()).ReturnsAsync([ existingVideoConfig ]);
        videoConfigRepositorySuccess.Setup(s => s.Update(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(true);

        // Null Test
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.Equal("V000", actual);

        // Validation FAIL Tests
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel());
        Assert.Equal("V101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel() { Title = "" });
        Assert.Equal("V101", actual);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), new EditableVideoConfigViewModel() { Title = "   " });
        Assert.Equal("V101", actual);

        // Update Exception
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Update(It.IsAny<string>(), It.IsAny<string>(), videoConfigToUpdate));

        // Update Failure
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), videoConfigToUpdate);
        Assert.Equal("V004", actual);

        // Update Success
        service = new VideoConfigsService(_logger.Object, videoConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Update(It.IsAny<string>(), It.IsAny<string>(), videoConfigToUpdate);
        Assert.Empty(actual);
    }

    [Fact]
    public async Task DeleteTests()
    {
        // Invalid Input Tests
        var service = new VideoConfigsService(_logger.Object, _videoConfigRepository.Object, _usersService.Object);
        var actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), -1);
        Assert.Equal("V000", actual);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 0);
        Assert.Equal("V000", actual);

        // Setup
        var videoConfigRepositoryNull = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryNull.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync((VideoConfig)null);

        var videoConfigRepositoryException = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryException.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ThrowsAsync(new Exception() {});

        var videoConfigRepositoryFailure = new Mock<IVideoConfigRepository>();
        videoConfigRepositoryFailure.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync(new VideoConfig()
        {
            VideoId = 1, Title = "TITLE", Url = "URL", Active = true
        });
        videoConfigRepositoryFailure.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(false);

        var videoConfigRepositorySuccess = new Mock<IVideoConfigRepository>();
        videoConfigRepositorySuccess.Setup(s => s.GetOne(It.IsAny<VideoConfig>())).ReturnsAsync(new VideoConfig()
        {
            VideoId = 1, Title = "TITLE", Url = "URL", Active = true
        });
        videoConfigRepositorySuccess.Setup(s => s.Delete(It.IsAny<string>(), It.IsAny<VideoConfig>())).ReturnsAsync(true);

        // Not Found Test
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryNull.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("V001", actual);

        // Delete Exception
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryException.Object, _usersService.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1));

        // Delete Failure
        service = new VideoConfigsService(_logger.Object, videoConfigRepositoryFailure.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Equal("V005", actual);

        // Delete Success
        service = new VideoConfigsService(_logger.Object, videoConfigRepositorySuccess.Object, _usersService.Object);
        actual = await service.Delete(It.IsAny<string>(), It.IsAny<string>(), 1);
        Assert.Empty(actual);
    }
}