using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Services;

namespace WHLSite.Tests.Services;

public class VideoServiceTests
{
    private readonly Mock<ILogger<VideoService>> _logger = new();
    private readonly Mock<IVideoRepository> _videoRepository = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new VideoService(null, null));
        Assert.Throws<ArgumentNullException>(() => new VideoService(_logger.Object, null));

        // Not Null
        var actual = new VideoService(_logger.Object, _videoRepository.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public async void GetDataTests()
    {
        // Exception
        var videoRepository = new Mock<IVideoRepository>();
        videoRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});
        var service = new VideoService(_logger.Object, videoRepository.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetData(It.IsAny<string>(), It.IsAny<string>()));

        // Null Videos
        service = new VideoService(_logger.Object, _videoRepository.Object);
        var actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Videos);

        // No Videos
        var videos = new List<VideoConfig>();
        videoRepository.Setup(s => s.GetAll()).ReturnsAsync(videos);
        service = new VideoService(_logger.Object, videoRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Videos);

        // With Videos
        videos.Add(new()
        {
            Title = "TITLE",
            Text = "TEXT",
            Url = "URL"
        });
        videoRepository.Setup(s => s.GetAll()).ReturnsAsync(videos);
        service = new VideoService(_logger.Object, videoRepository.Object);
        actual = await service.GetData(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Videos);
        Assert.Contains("TITLE", actual.Videos.Select(s => s.Title));
    }

    [Fact]
    public async void GetVideoForHomePageTests()
    {
        // Exception
        var videoRepository = new Mock<IVideoRepository>();
        videoRepository.Setup(s => s.GetAll()).ThrowsAsync(new Exception() {});
        var service = new VideoService(_logger.Object, videoRepository.Object);
        await Assert.ThrowsAnyAsync<Exception>(() => service.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>()));

        // Null Videos
        service = new VideoService(_logger.Object, _videoRepository.Object);
        var actual = await service.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // No Videos
        var videos = new List<VideoConfig>();
        videoRepository.Setup(s => s.GetAll()).ReturnsAsync(videos);
        service = new VideoService(_logger.Object, videoRepository.Object);
        actual = await service.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // With Videos, No Home Page Indidcator
        videos.Add(new()
        {
            Title = "TITLE",
            Text = "TEXT",
            Url = "URL"
        });
        videoRepository.Setup(s => s.GetAll()).ReturnsAsync(videos);
        service = new VideoService(_logger.Object, videoRepository.Object);
        actual = await service.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.Null(actual);

        // With Videos, With Home Page Indidcator
        videos.Add(new()
        {
            Title = "FORHOMEPAGE",
            Text = "TEXT",
            Url = "URL",
            DisplayOnHomePageInd = true
        });
        videoRepository.Setup(s => s.GetAll()).ReturnsAsync(videos);
        service = new VideoService(_logger.Object, videoRepository.Object);
        actual = await service.GetVideoForHomePage(It.IsAny<string>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Equal("FORHOMEPAGE", actual.Title);
    }
}