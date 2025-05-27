using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IResourceService
{
    Task<ResourcesViewModel> GetData(string requestId, string correlationId);
}

public class ResourceService : IResourceService
{
    private readonly ILogger<ResourceService> _logger;
    private readonly IResourceRepository _resourceRepository;

    public ResourceService(ILogger<ResourceService> logger, IResourceRepository resourceRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resourceRepository = resourceRepository ?? throw new ArgumentNullException(nameof(resourceRepository));
    }

    public async Task<ResourcesViewModel> GetData(string requestId, string correlationId)
    {
        var resources = await _resourceRepository.GetAll();
        var model = new ResourcesViewModel()
        {
            Resources = resources.Select(s => s.ToViewModel())
        };
        return model;
    }
}