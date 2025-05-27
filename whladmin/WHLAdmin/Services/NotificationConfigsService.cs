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

public interface INotificationConfigsService
{
    Task<NotificationConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<NotificationConfigViewModel>> GetAll();
    Task<NotificationConfigViewModel> GetOne(int id);
    Task<EditableNotificationConfigViewModel> GetOneForAdd();
    Task<EditableNotificationConfigViewModel> GetOneForEdit(int id);
    Task<string> Add(string correlationId, string username, EditableNotificationConfigViewModel model);
    Task<string> Update(string correlationId, string username, EditableNotificationConfigViewModel model);
    Task<string> Delete(string correlationId, string username, int id);
    void Sanitize(NotificationConfig notification);
    Task<string> Validate(NotificationConfig notification, IEnumerable<NotificationConfig> notifications);
}

public class NotificationConfigsService : INotificationConfigsService
{
    private readonly ILogger<NotificationConfigsService> _logger;
    private readonly INotificationConfigRepository _notificationConfigRepository;
    private readonly IMetadataService _metadataService;
    private readonly IUsersService _usersService;

    public NotificationConfigsService(ILogger<NotificationConfigsService> logger, INotificationConfigRepository notificationConfigRepository, IMetadataService metadataService, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationConfigRepository = notificationConfigRepository ?? throw new ArgumentNullException(nameof(notificationConfigRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<NotificationConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var notifications = await _notificationConfigRepository.GetAll();
        return new NotificationConfigsViewModel()
        {
            Notifications = notifications.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<NotificationConfigViewModel>> GetAll()
    {
        var notifications = await _notificationConfigRepository.GetAll();
        return notifications.Select(s => s.ToViewModel());
    }

    public async Task<NotificationConfigViewModel> GetOne(int id)
    {
        var notification = await _notificationConfigRepository.GetOne(new NotificationConfig() { NotificationId = id });
        return notification.ToViewModel();
    }

    public async Task<EditableNotificationConfigViewModel> GetOneForAdd()
    {
        return new EditableNotificationConfigViewModel()
        {
            NotificationId = 0,
            Title = "",
            Text = "",
            CategoryCd = "",
            Categories = await _metadataService.GetCategories(true),
            FrequencyInterval = 0,
            FrequencyCd = "",
            Frequencies = await _metadataService.GetFrequencyTypes(true),
            NotificationList = "",
            Active = true
        };
    }

    public async Task<EditableNotificationConfigViewModel> GetOneForEdit(int id)
    {
        var notification = await _notificationConfigRepository.GetOne(new NotificationConfig() { NotificationId = id });
        var model = notification.ToEditableViewModel();
        if (model != null)
        {
            model.Categories = await _metadataService.GetCategories(true);
            model.Frequencies = await _metadataService.GetFrequencyTypes(true);
        }
        return model;
    }

    public async Task<string> Add(string correlationId, string username, EditableNotificationConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Notification - Invalid Input");
            return "N000";
        }

        var notification = new NotificationConfig()
        {
            NotificationId = 0,
            CategoryCd = model.CategoryCd,
            Title = model.Title,
            Text = model.Text,
            FrequencyCd = model.FrequencyCd,
            FrequencyInterval = model.FrequencyInterval,
            NotificationList = model.NotificationList,
            Active = model.Active,
            CreatedBy = username
        };

        var notifications = await _notificationConfigRepository.GetAll();

        var validationCode = await Validate(notification, notifications);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Notification - {notification.Title}");
            return validationCode;
        }

        var added = await _notificationConfigRepository.Add(correlationId, notification);
        if (!added)
        {
            _logger.LogError($"Failed to add Notification - {notification.Title} - Unknown error");
            return "N003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableNotificationConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update Notification - Invalid Input");
            return "N000";
        }

        var notification = new NotificationConfig()
        {
            NotificationId = model.NotificationId,
            CategoryCd = model.CategoryCd,
            Title = model.Title,
            Text = model.Text,
            FrequencyCd = model.FrequencyCd,
            FrequencyInterval = model.FrequencyInterval,
            NotificationList = model.NotificationList,
            Active = model.Active,
            ModifiedBy = username
        };

        var notifications = await _notificationConfigRepository.GetAll();

        var validationCode = await Validate(notification, notifications);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Notification - {notification.Title}");
            return validationCode;
        }

        var updated = await _notificationConfigRepository.Update(correlationId, notification);
        if (!updated)
        {
            _logger.LogError($"Failed to update Notification - {model.Title} - Unknown error");
            return "N004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete Notification - Invalid Input");
            return "N000";
        }

        var existingNotification = await _notificationConfigRepository.GetOne(new NotificationConfig() { NotificationId = id });
        if (existingNotification == null)
        {
            _logger.LogError($"Unable to find Notification - {id}");
            return "N001";
        }

        existingNotification.ModifiedBy = username;
        var deleted = await _notificationConfigRepository.Delete(correlationId, existingNotification);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete Notification - {existingNotification.Title} - Unknown error");
            return "N005";
        }

        return "";
    }

    public void Sanitize(NotificationConfig notification)
    {
        if (notification == null) return;

        notification.CategoryCd = (notification.CategoryCd ?? "").Trim();
        notification.Title = (notification.Title ?? "").Trim();
        notification.Text = (notification.Text ?? "").Trim();
        notification.FrequencyCd = (notification.FrequencyCd ?? "").Trim();

        if (notification.FrequencyInterval <= 0) notification.FrequencyInterval = 0;

        notification.NotificationList = (notification.NotificationList ?? "").Trim();
        if (notification.NotificationList.Length == 0) notification.NotificationList = null;
    }

    public async Task<string> Validate(NotificationConfig notification, IEnumerable<NotificationConfig> notifications)
    {
        if (notification == null)
        {
            _logger.LogError($"Unable to validate Notification - Invalid Input");
            return "N000";
        }

        Sanitize(notification);

        var categories = await _metadataService.GetCategories();
        if (notification.CategoryCd.Length == 0 || !categories.ContainsKey(notification.CategoryCd))
        {
            _logger.LogError($"Unable to validate Notification - Invalid Category");
            return "N101";
        }

        if (notification.Title.Length == 0)
        {
            _logger.LogError($"Unable to validate Notification - Invalid Title");
            return "N102";
        }

        if (notification.Text.Length == 0)
        {
            _logger.LogError($"Unable to validate Notification - Invalid Text");
            return "N103";
        }

        if (notification.FrequencyInterval < 0)
        {
            _logger.LogError($"Unable to validate Notification - Invalid Frequency Interval");
            return "N104";
        }

        var frequencyTypes = await _metadataService.GetFrequencyTypes();
        if (notification.FrequencyCd.Length == 0 || !frequencyTypes.ContainsKey(notification.FrequencyCd))
        {
            _logger.LogError($"Unable to validate Notification - Invalid Frequency Type");
            return "N105";
        }
        if (notification.FrequencyCd.Equals("ON", StringComparison.CurrentCultureIgnoreCase))
        {
            notification.FrequencyInterval = 0;
        }

        if (notification.CategoryCd.Equals("INTERNAL", StringComparison.CurrentCultureIgnoreCase) && string.IsNullOrEmpty(notification.NotificationList))
        {
            _logger.LogError($"Unable to validate Notification - Invalid Notification List");
            return "N106";
        }

        if ((notifications?.Count() ?? 0) > 0)
        {
            if (notification.NotificationId > 0)
            {
                // Existence check
                var existingNotification = notifications.FirstOrDefault(f => f.NotificationId == notification.NotificationId);
                if (existingNotification == null)
                {
                    _logger.LogError($"Unable to find Notification - {notification.NotificationId}");
                    return "N001";
                }

                // Duplicate check
                var duplicateNotification = notifications.FirstOrDefault(f => f.NotificationId != notification.NotificationId
                                                                                && f.CategoryCd.Equals(notification.CategoryCd, StringComparison.CurrentCultureIgnoreCase)
                                                                                && f.Title.Equals(notification.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateNotification != null)
                {
                    _logger.LogError($"Unable to validate Notification - Duplicate {notification.CategoryCd}, {notification.Title}");
                    return "N002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateNotification = notifications.FirstOrDefault(f => f.CategoryCd.Equals(notification.CategoryCd, StringComparison.CurrentCultureIgnoreCase)
                                                                                && f.Title.Equals(notification.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateNotification != null)
                {
                    _logger.LogError($"Unable to validate Notification - Duplicate {notification.CategoryCd}, {notification.Title}");
                    return "N002";
                }
            }
        }

        return "";
    }
}