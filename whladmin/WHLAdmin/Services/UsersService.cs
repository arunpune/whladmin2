using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Services;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IUsersService
{
    Task<UsersViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<UserViewModel>> GetAll();
    Task<UserViewModel> GetOne(string userId = null, string emailAddress = null);
    Task<bool> SendOtp(string requestId, string correlationId, string userId = null, string emailAddress = null);
    Task<EditableUserViewModel> GetOneForAdd();
    Task<EditableUserViewModel> GetOneForEdit(string userId);
    Task<string> Add(string correlationId, string username, EditableUserViewModel user);
    Task<string> Update(string correlationId, string username, EditableUserViewModel user);
    Task<string> Delete(string correlationId, string username, string userId);
    Task<string> Reactivate(string correlationId, string username, string userId);
    Task<UserViewModel> Authenticate(string correlationId, string userId = null, string emailAddress = null);
    Task<UserViewModel> AuthenticateOtp(string correlationId, string emailAddress, string otp);
    Task<string> GetUserRole(string correlationId, string userId);
    void Sanitize(User user);
    Task<string> Validate(User user, IEnumerable<User> users, bool forUpdate = false);
}

public class UsersService : IUsersService
{
    private readonly ILogger<UsersService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IAdminUserRepository _adminUserRepository;
    private readonly IMetadataService _metadataService;
    private readonly IEmailService _emailService;
    private readonly IKeyService _keyService;

    public UsersService(ILogger<UsersService> logger, IConfiguration configuration, IAdminUserRepository adminUserRepository, IMetadataService metadataService, IEmailService emailService, IKeyService keyService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _adminUserRepository = adminUserRepository ?? throw new ArgumentNullException(nameof(adminUserRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _keyService = keyService ?? throw new ArgumentNullException(nameof(keyService));
    }

    public async Task<UsersViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await GetUserRole(correlationId, userId);
        var users = await _adminUserRepository.GetAll();
        return new UsersViewModel()
        {
            Users = users.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<UserViewModel>> GetAll()
    {
        var users = await _adminUserRepository.GetAll();
        return users.Select(s => s.ToViewModel());
    }

    public async Task<UserViewModel> GetOne(string userId = null, string emailAddress = null)
    {
        var user = await _adminUserRepository.GetOne(new User() { UserId = userId, EmailAddress = emailAddress });
        return user.ToViewModel();
    }

    public async Task<bool> SendOtp(string requestId, string correlationId, string userId = null, string emailAddress = null)
    {
        var user = await _adminUserRepository.GetOne(new User() { UserId = userId, EmailAddress = emailAddress });
        if (user == null) return false;
        if (!user.Active) return false;

        var otp = _keyService.GetOtp();
        if (await _adminUserRepository.SetOtp(correlationId, emailAddress, otp))
        {
            return await _emailService.SendOtpEmail(requestId, correlationId, user.EmailAddress, otp);
        }

        return false;
    }

    public async Task<EditableUserViewModel> GetOneForAdd()
    {
        return new EditableUserViewModel()
        {
            OrganizationCd = "",
            Organizations = await _metadataService.GetOrganizations(true),
            RoleCd = "",
            Roles = await _metadataService.GetRoles(true)
        };
    }

    public async Task<EditableUserViewModel> GetOneForEdit(string userId)
    {
        var user = await _adminUserRepository.GetOne(new User() { UserId = userId });
        var model = user.ToEditableViewModel();
        if (model != null)
        {
            model.Organizations = await _metadataService.GetOrganizations(true);
            model.Roles = await _metadataService.GetRoles(true);
        }
        return model;
    }

    public async Task<string> Add(string correlationId, string username, EditableUserViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add User - Invalid Input");
            return "U000";
        }

        var user = new User()
        {
            DisplayName = model.DisplayName,
            EmailAddress = model.EmailAddress,
            OrganizationCd = model.OrganizationCd,
            RoleCd = model.RoleCd,
            UserId = model.UserId,
            CreatedBy = username
        };
        Sanitize(user);

        var users = await _adminUserRepository.GetAll();

        var validationCode = await Validate(user, users);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for User - {user.UserId}");
            return validationCode;
        }

        var added = await _adminUserRepository.Add(correlationId, user);
        if (!added)
        {
            _logger.LogError($"Failed to add User - {user.UserId} - Unknown error");
            return "U003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableUserViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update User - Invalid Input");
            return "U000";
        }

        var user = new User()
        {
            DisplayName = model.DisplayName,
            EmailAddress = model.EmailAddress,
            OrganizationCd = model.OrganizationCd,
            RoleCd = model.RoleCd,
            UserId = model.UserId,
            ModifiedBy = username
        };
        Sanitize(user);

        var users = await _adminUserRepository.GetAll();

        var validationCode = await Validate(user, users, true);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for User - {user.UserId}");
            return validationCode;
        }

        var updated = await _adminUserRepository.Update(correlationId, user);
        if (!updated)
        {
            _logger.LogError($"Failed to update User - {user.UserId} - Unknown error");
            return "U004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, string userId)
    {
        userId = (userId ?? "").Trim();
        if (userId.Length == 0)
        {
            _logger.LogError($"Unable to delete User - Invalid Input");
            return "U000";
        }

        var existingUser = await _adminUserRepository.GetOne(new User() { UserId = userId });
        if (existingUser == null)
        {
            _logger.LogError($"Unable to find User - {userId}");
            return "U001";
        }

        existingUser.ModifiedBy = username;
        var deleted = await _adminUserRepository.Delete(correlationId, existingUser);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete User - {existingUser.UserId} - Unknown error");
            return "U005";
        }

        return "";
    }

    public async Task<string> Reactivate(string correlationId, string username, string userId)
    {
        userId = (userId ?? "").Trim();
        if (userId.Length == 0)
        {
            _logger.LogError($"Unable to reactivate User - Invalid Input");
            return "U000";
        }

        var existingUser = await _adminUserRepository.GetOne(new User() { UserId = userId });
        if (existingUser == null)
        {
            _logger.LogError($"Unable to find User - {userId}");
            return "U001";
        }

        existingUser.ModifiedBy = username;
        var reactivated = await _adminUserRepository.Reactivate(correlationId, existingUser);
        if (!reactivated)
        {
            _logger.LogError($"Failed to reactivate User - {existingUser.UserId} - Unknown error");
            return "U006";
        }

        return "";
    }

    public async Task<UserViewModel> Authenticate(string correlationId, string userId = null, string emailAddress = null)
    {
        var user = await _adminUserRepository.Authenticate(correlationId, userId, emailAddress);
        if (user == null)
        {
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to authenticate User - {userId ?? emailAddress ?? ""}");
            return null;
        }

        return user.ToViewModel();
    }

    public async Task<UserViewModel> AuthenticateOtp(string correlationId, string emailAddress = null, string otp = null)
    {
        var user = await _adminUserRepository.AuthenticateOtp(correlationId, emailAddress, otp);
        if (user == null)
        {
            _logger.LogError($"CorrelationID: {correlationId}, Message: Failed to authenticate User OTP - {emailAddress} {otp}");
            return null;
        }

        return user.ToViewModel();
    }

    public async Task<string> GetUserRole(string correlationId, string userId)
    {
        if (string.IsNullOrEmpty((userId ?? "").Trim())) return "";

        var adminUser = await _adminUserRepository.GetOne(new User() { UserId = userId });
        return adminUser?.RoleCd ?? "";
    }

    public void Sanitize(User user)
    {
        if (user == null) return;

        user.UserId = (user.UserId ?? "").Trim();

        user.EmailAddress = (user.EmailAddress ?? "").Trim();

        user.DisplayName = (user.DisplayName ?? "").Trim();

        user.OrganizationCd = (user.OrganizationCd ?? "").Trim();

        user.RoleCd = (user.RoleCd ?? "").Trim();
    }

    public async Task<string> Validate(User user, IEnumerable<User> users, bool forUpdate = false)
    {
        if (user == null)
        {
            _logger.LogError($"Unable to validate User - Invalid Input");
            return "U000";
        }

        Sanitize(user);

        if (user.UserId.Length == 0)
        {
            _logger.LogError($"Unable to validate User - User ID is required");
            return "U101";
        }

        if (user.EmailAddress.Length == 0)
        {
            _logger.LogError($"Unable to validate User - Email Address is required");
            return "U102";
        }

        try
        {
            var emailAddress = new MailAddress(user.EmailAddress).Address;
        }
        catch (FormatException formatException)
        {
            _logger.LogError(formatException, $"Unable to validate User - Invalid Email Address");
            return "U102";
        }

        if (user.DisplayName.Length == 0)
        {
            _logger.LogError($"Unable to validate User - Display Name is required");
            return "U103";
        }

        var organizations = await _metadataService.GetOrganizations();
        if (user.OrganizationCd.Length == 0 || !organizations.ContainsKey(user.OrganizationCd))
        {
            _logger.LogError($"Unable to validate User - Invalid Organization");
            return "U104";
        }

        var roles = await _metadataService.GetRoles();
        if (user.RoleCd.Length == 0 || !roles.ContainsKey(user.RoleCd))
        {
            _logger.LogError($"Unable to validate User - Invalid Role");
            return "U105";
        }

        if (forUpdate)
        {
            // Existence check
            var existingUser = users?.FirstOrDefault(f => f.UserId.Equals(user.UserId, StringComparison.CurrentCultureIgnoreCase));
            if (existingUser == null)
            {
                _logger.LogError($"Unable to find User - {user.UserId}");
                return "U001";
            }
        }
        else
        {
            // Duplicate check
            var duplicateUser = users?.FirstOrDefault(f => f.UserId.Equals(user.UserId, StringComparison.CurrentCultureIgnoreCase));
            if (duplicateUser != null)
            {
                _logger.LogError($"Unable to validate User - Duplicate {user.UserId}");
                return "U002";
            }
        }

        return "";
    }
}