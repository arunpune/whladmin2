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

public interface IAmenitiesService
{
    Task<AmenitiesViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<AmenityViewModel>> GetAll(string requestId, string correlationId);
    Task<AmenityViewModel> GetOne(string requestId, string correlationId, int amenityId);
    EditableAmenityViewModel GetOneForAdd(string requestId, string correlationId);
    Task<EditableAmenityViewModel> GetOneForEdit(string requestId, string correlationId, int amenityId);
    Task<string> Add(string requestId, string correlationId, string username, EditableAmenityViewModel model);
    Task<string> Update(string requestId, string correlationId, string username, EditableAmenityViewModel model);
    Task<string> Delete(string requestId, string correlationId, string username, int amenityId);
    void Sanitize(Amenity amenity);
    string Validate(string requestId, string correlationIdAmenity, Amenity amenity, IEnumerable<Amenity> amenities);
}

public class AmenitiesService : IAmenitiesService
{
    private readonly ILogger<AmenitiesService> _logger;
    private readonly IAmenityRepository _amenityRepository;
    private readonly IUsersService _usersService;

    public AmenitiesService(ILogger<AmenitiesService> logger, IAmenityRepository amenityRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _amenityRepository = amenityRepository ?? throw new ArgumentNullException(nameof(amenityRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<AmenitiesViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var amenities = await _amenityRepository.GetAll();
        var model = new AmenitiesViewModel
        {
            Amenities = amenities.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
        return model;
    }

    public async Task<IEnumerable<AmenityViewModel>> GetAll(string requestId, string correlationId)
    {
        var amenities = await _amenityRepository.GetAll();
        return amenities.Select(s => s.ToViewModel());
    }

    public async Task<AmenityViewModel> GetOne(string requestId, string correlationId, int amenityId)
    {
        var amenity = await _amenityRepository.GetOne(new Amenity() { AmenityId = amenityId });
        return amenity.ToViewModel();
    }

    public EditableAmenityViewModel GetOneForAdd(string requestId, string correlationId)
    {
        return new EditableAmenityViewModel()
        {
            AmenityId = 0,
            AmenityName = "",
            AmenityDescription = "",
            Active = true
        };
    }

    public async Task<EditableAmenityViewModel> GetOneForEdit(string requestId, string correlationId, int amenityId)
    {
        var amenity = await _amenityRepository.GetOne(new Amenity() { AmenityId = amenityId });
        return amenity.ToEditableViewModel();
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableAmenityViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add amenity - Invalid Input");
            return "A000";
        }

        var amenity = new Amenity()
        {
            AmenityId = 0,
            Name = model.AmenityName,
            Description = model.AmenityDescription,
            UsageCount = 0,
            Active = true,
            CreatedBy = username
        };
        Sanitize(amenity);

        var amenities = await _amenityRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, amenity, amenities);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for amenity - {amenity.Name}");
            return validationCode;
        }

        var added = await _amenityRepository.Add(correlationId, amenity);
        if (!added)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to add amenity - {amenity.Name} - Unknown error");
            return "A003";
        }

        model.AmenityId = amenity.AmenityId;
        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableAmenityViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to update amenity - Invalid Input");
            return "A000";
        }

        var amenity = new Amenity()
        {
            AmenityId = model.AmenityId,
            Name = model.AmenityName,
            Description = model.AmenityDescription,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(amenity);

        var amenities = await _amenityRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, amenity, amenities);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for amenity - {amenity.Name}");
            return validationCode;
        }

        var updated = await _amenityRepository.Update(correlationId, amenity);
        if (!updated)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to update amenity - {amenity.Name} - Unknown error");
            return "A004";
        }

        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, int amenityId)
    {
        if (amenityId <= 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to delete amenity - Invalid Input");
            return "A000";
        }

        var existingAmenity = await _amenityRepository.GetOne(new Amenity() { AmenityId = amenityId });
        if (existingAmenity == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find amenity - {amenityId}");
            return "A001";
        }

        existingAmenity.ModifiedBy = username;
        var deleted = await _amenityRepository.Delete(correlationId, existingAmenity);
        if (!deleted)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to delete amenity - {existingAmenity.Name} - Unknown error");
            return "A005";
        }

        return "";
    }

    public void Sanitize(Amenity amenity)
    {
        if (amenity == null) return;

        amenity.Name = (amenity.Name ?? "").Trim();

        amenity.Description = (amenity.Description ?? "").Trim();
        if (string.IsNullOrEmpty(amenity.Description)) amenity.Description = null;
    }

    public string Validate(string requestId, string correlationId, Amenity amenity, IEnumerable<Amenity> amenities)
    {
        if (amenity == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate amenity - Invalid Input");
            return "A000";
        }

        Sanitize(amenity);

        if (amenity.Name.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate amenity - Amenity Name is required");
            return "A101";
        }

        if ((amenities?.Count() ?? 0) > 0)
        {
            if (amenity.AmenityId > 0)
            {
                // Existence check
                var existingAmenity = amenities.FirstOrDefault(f => f.AmenityId == amenity.AmenityId);
                if (existingAmenity == null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find amenity - {amenity.AmenityId}");
                    return "A001";
                }

                // Duplicate check
                var duplicateAmenity = amenities.FirstOrDefault(f => f.AmenityId != amenity.AmenityId && f.Name.Equals(amenity.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateAmenity != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate amenity - Duplicate {amenity.Name}");
                    return "A002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateAmenity = amenities.FirstOrDefault(f => f.Name.Equals(amenity.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateAmenity != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate amenity - Duplicate {amenity.Name}");
                    return "A002";
                }
            }
        }

        return "";
    }
}