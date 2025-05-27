using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IVideoConfigsService
{
    Task<VideoConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<VideoConfigViewModel>> GetAll();
    Task<VideoConfigViewModel> GetOne(int id);
    EditableVideoConfigViewModel GetOneForAdd();
    Task<EditableVideoConfigViewModel> GetOneForEdit(int id);
    Task<string> Add(string correlationId, string username, EditableVideoConfigViewModel video);
    Task<string> Update(string correlationId, string username, EditableVideoConfigViewModel video);
    Task<string> Delete(string correlationId, string username, int id);
    void Sanitize(VideoConfig video);
    string Validate(VideoConfig video, IEnumerable<VideoConfig> videos);
}

public class VideoConfigsService : IVideoConfigsService
{
    private readonly ILogger<VideoConfigsService> _logger;
    private readonly IVideoConfigRepository _videoConfigRepository;
    private readonly IUsersService _usersService;

    public VideoConfigsService(ILogger<VideoConfigsService> logger, IVideoConfigRepository videoConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _videoConfigRepository = videoConfigRepository ?? throw new ArgumentNullException(nameof(videoConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<VideoConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var videos = await _videoConfigRepository.GetAll();
        return new VideoConfigsViewModel()
        {
            Videos = videos.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<VideoConfigViewModel>> GetAll()
    {
        var videos = await _videoConfigRepository.GetAll();
        return videos.Select(s => s.ToViewModel());
    }

    public async Task<VideoConfigViewModel> GetOne(int id)
    {
        var video = await _videoConfigRepository.GetOne(new VideoConfig() { VideoId = id });
        return video.ToViewModel();
    }

    public EditableVideoConfigViewModel GetOneForAdd()
    {
        return new EditableVideoConfigViewModel()
        {
            VideoId = 0,
            Title = "",
            Text = "",
            Url = "",
            DisplayOrder = 0,
            DisplayOnHomePageInd = false,
            Active = true
        };
    }

    public async Task<EditableVideoConfigViewModel> GetOneForEdit(int id)
    {
        var video = await _videoConfigRepository.GetOne(new VideoConfig() { VideoId = id });
        return video.ToEditableViewModel();
    }

    public async Task<string> Add(string correlationId, string username, EditableVideoConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Video - Invalid Input");
            return "V000";
        }

        var video = new VideoConfig()
        {
            VideoId = 0,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            DisplayOrder = model.DisplayOrder,
            DisplayOnHomePageInd = model.DisplayOnHomePageInd,
            Active = true,
            CreatedBy = username
        };
        Sanitize(video);

        var videos = await _videoConfigRepository.GetAll();

        var validationCode = Validate(video, videos);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Video - {video.Title}");
            return validationCode;
        }

        var added = await _videoConfigRepository.Add(correlationId, video);
        if (!added)
        {
            _logger.LogError($"Failed to add Video - {video.Title} - Unknown error");
            return "V003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableVideoConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update Video - Invalid Input");
            return "V000";
        }

        var video = new VideoConfig()
        {
            VideoId = model.VideoId,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            DisplayOrder = model.DisplayOrder,
            DisplayOnHomePageInd = model.DisplayOnHomePageInd,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(video);

        var videos = await _videoConfigRepository.GetAll();

        var validationCode = Validate(video, videos);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Video - {video.Title}");
            return validationCode;
        }

        var updated = await _videoConfigRepository.Update(correlationId, video);
        if (!updated)
        {
            _logger.LogError($"Failed to update Video - {video.Title} - Unknown error");
            return "V004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete Video - Invalid Input");
            return "V000";
        }

        var existingVideo = await _videoConfigRepository.GetOne(new VideoConfig() { VideoId = id });
        if (existingVideo == null)
        {
            _logger.LogError($"Unable to find video - {id}");
            return "V001";
        }

        existingVideo.ModifiedBy = username;
        var deleted = await _videoConfigRepository.Delete(correlationId, existingVideo);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete Video - {existingVideo.Title} - Unknown error");
            return "V005";
        }

        return "";
    }

    public void Sanitize(VideoConfig video)
    {
        if (video == null) return;

        video.Title = (video.Title ?? "").Trim();

        video.Text = (video.Text ?? "").Trim();
        if (string.IsNullOrEmpty(video.Text)) video.Text = null;

        video.Url = (video.Url ?? "").Trim();

        video.DisplayOrder = video.DisplayOrder < 0 ? 0 : video.DisplayOrder;
    }

    public string Validate(VideoConfig video, IEnumerable<VideoConfig> videos)
    {
        if (video == null)
        {
            _logger.LogError($"Unable to validate Video - Invalid Input");
            return "V000";
        }

        Sanitize(video);

        if (video.Title.Length == 0)
        {
            _logger.LogError($"Unable to validate Video - Title is required");
            return "V101";
        }

        if (video.Url.Length == 0)
        {
            _logger.LogError($"Unable to validate Video - Url is required");
            return "V102";
        }

        if ((videos?.Count() ?? 0) > 0)
        {
            if (video.VideoId > 0)
            {
                // Existence check
                var existingVideo = videos.FirstOrDefault(f => f.VideoId == video.VideoId);
                if (existingVideo == null)
                {
                    _logger.LogError($"Unable to find Video - {video.VideoId}");
                    return "V001";
                }

                // Duplicate check
                var duplicateVideo = videos.FirstOrDefault(f => f.VideoId != video.VideoId && f.Title.Equals(video.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateVideo != null)
                {
                    _logger.LogError($"Unable to validate Video - Duplicate {video.Title}");
                    return "V002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateVideo = videos.FirstOrDefault(f => f.Title.Equals(video.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateVideo != null)
                {
                    _logger.LogError($"Unable to validate Video - Duplicate {video.Title}");
                    return "V002";
                }
            }
        }

        return "";
    }
}