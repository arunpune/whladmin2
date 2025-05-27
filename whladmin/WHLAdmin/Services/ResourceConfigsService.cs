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

public interface IResourceConfigsService
{
    Task<ResourceConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<ResourceConfigViewModel>> GetAll();
    Task<ResourceConfigViewModel> GetOne(int id);
    EditableResourceConfigViewModel GetOneForAdd();
    Task<EditableResourceConfigViewModel> GetOneForEdit(int id);
    Task<string> Add(string correlationId, string username, EditableResourceConfigViewModel resource);
    Task<string> Update(string correlationId, string username, EditableResourceConfigViewModel resource);
    Task<string> Delete(string correlationId, string username, int id);
}

public class ResourceConfigsService : IResourceConfigsService
{
    private readonly ILogger<ResourceConfigsService> _logger;
    private readonly IResourceConfigRepository _resourceConfigRepository;
    private readonly IUsersService _usersService;

    public ResourceConfigsService(ILogger<ResourceConfigsService> logger, IResourceConfigRepository resourceConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _resourceConfigRepository = resourceConfigRepository ?? throw new ArgumentNullException(nameof(resourceConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<ResourceConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var resources = await _resourceConfigRepository.GetAll();
        return new ResourceConfigsViewModel()
        {
            Resources = resources.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<ResourceConfigViewModel>> GetAll()
    {
        var resources = await _resourceConfigRepository.GetAll();
        return resources.Select(s => s.ToViewModel());
    }

    public async Task<ResourceConfigViewModel> GetOne(int id)
    {
        var resource = await _resourceConfigRepository.GetOne(new ResourceConfig() { ResourceId = id });
        return resource.ToViewModel();
    }

    public EditableResourceConfigViewModel GetOneForAdd()
    {
        return new EditableResourceConfigViewModel()
        {
            ResourceId = 0,
            Title = "",
            Text = "",
            Url = "",
            DisplayOrder = 0,
            Active = true
        };
    }

    public async Task<EditableResourceConfigViewModel> GetOneForEdit(int id)
    {
        var resource = await _resourceConfigRepository.GetOne(new ResourceConfig() { ResourceId = id });
        return resource.ToEditableViewModel();
    }

    public async Task<string> Add(string correlationId, string username, EditableResourceConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Resource - Invalid Input");
            return "R000";
        }

        var resource = new ResourceConfig()
        {
            ResourceId = 0,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            DisplayOrder = model.DisplayOrder,
            Active = true,
            CreatedBy = username
        };
        Sanitize(resource);

        var resources = await _resourceConfigRepository.GetAll();

        var validationCode = Validate(resource, resources);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Resource - {resource.Title}");
            return validationCode;
        }

        var added = await _resourceConfigRepository.Add(correlationId, resource);
        if (!added)
        {
            _logger.LogError($"Failed to add Resource - {resource.Title} - Unknown error");
            return "R003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableResourceConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update Resource - Invalid Input");
            return "R000";
        }

        var resource = new ResourceConfig()
        {
            ResourceId = model.ResourceId,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            DisplayOrder = model.DisplayOrder,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(resource);

        var resources = await _resourceConfigRepository.GetAll();

        var validationCode = Validate(resource, resources);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Resource - {resource.Title}");
            return validationCode;
        }

        var updated = await _resourceConfigRepository.Update(correlationId, resource);
        if (!updated)
        {
            _logger.LogError($"Failed to update Resource - {resource.Title} - Unknown error");
            return "R004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete Resource - Invalid Input");
            return "R000";
        }

        var existingResource = await _resourceConfigRepository.GetOne(new ResourceConfig() { ResourceId = id });
        if (existingResource == null)
        {
            _logger.LogError($"Unable to find Resource - {id}");
            return "R001";
        }

        existingResource.ModifiedBy = username;
        var deleted = await _resourceConfigRepository.Delete(correlationId, existingResource);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete Resource - {existingResource.Title} - Unknown error");
            return "R005";
        }

        return "";
    }

    public void Sanitize(ResourceConfig resource)
    {
        if (resource == null) return;

        resource.Title = (resource.Title ?? "").Trim();

        resource.Text = (resource.Text ?? "").Trim();
        if (string.IsNullOrEmpty(resource.Text)) resource.Text = null;

        resource.Url = (resource.Url ?? "").Trim();

        resource.DisplayOrder = resource.DisplayOrder < 0 ? 0 : resource.DisplayOrder;
    }

    public string Validate(ResourceConfig resource, IEnumerable<ResourceConfig> resources)
    {
        if (resource == null)
        {
            _logger.LogError($"Unable to validate Resource - Invalid Input");
            return "R000";
        }

        Sanitize(resource);

        if (resource.Title.Length == 0)
        {
            _logger.LogError($"Unable to validate Resource - Title is required");
            return "R101";
        }

        if (resource.Url.Length == 0)
        {
            _logger.LogError($"Unable to validate Resource - Url is required");
            return "R102";
        }

        if ((resources?.Count() ?? 0) > 0)
        {
            if (resource.ResourceId > 0)
            {
                // Existence check
                var existingResource = resources.FirstOrDefault(f => f.ResourceId == resource.ResourceId);
                if (existingResource == null)
                {
                    _logger.LogError($"Unable to find Resource - {resource.ResourceId}");
                    return "R001";
                }

                // Duplicate check
                var duplicateResource = resources.FirstOrDefault(f => f.ResourceId != resource.ResourceId && f.Title.Equals(resource.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateResource != null)
                {
                    _logger.LogError($"Unable to validate Resource - Duplicate {resource.Title}");
                    return "R002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateResource = resources.FirstOrDefault(f => f.Title.Equals(resource.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateResource != null)
                {
                    _logger.LogError($"Unable to validate Resource - Duplicate {resource.Title}");
                    return "R002";
                }
            }
        }

        return "";
    }
}