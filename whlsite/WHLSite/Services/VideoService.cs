using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IVideoService
{
    Task<VideosViewModel> GetData(string requestId, string correlationId);
    Task<VideoViewModel> GetVideoForHomePage(string requestId, string correlationId);
}

public class VideoService : IVideoService
{
    private readonly ILogger<VideoService> _logger;
    private readonly IVideoRepository _videoRepository;

    public VideoService(ILogger<VideoService> logger, IVideoRepository videoRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _videoRepository = videoRepository ?? throw new ArgumentNullException(nameof(videoRepository));
    }

    public async Task<VideosViewModel> GetData(string requestId, string correlationId)
    {
        var videos = await _videoRepository.GetAll();
        var model = new VideosViewModel()
        {
            Videos = videos.Select(s => s.ToViewModel())
        };
        return model;
    }

    public async Task<VideoViewModel> GetVideoForHomePage(string requestId, string correlationId)
    {
        var videos = await _videoRepository.GetAll();
        var video = videos.FirstOrDefault(f => f.DisplayOnHomePageInd);
        return video.ToViewModel();
    }
}