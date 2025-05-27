using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Services;
using WHLSite.Common.Settings;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IAccountService
{
    Task AssignMetadata(RegistrationViewModel model);
    void AssignRecaptcha(RecaptchaViewModel model, string action);
    Task<bool> CheckAvailability(string requestId, string correlationId, string username, string emailAddress);
    Task<RegistrationViewModel> GetForRegistration(string requestId, string correlationId);
    Task<string> Register(string requestId, string correlationId, string siteUrl, RegistrationViewModel registration);
    ResendActivationViewModel GetForResendActivationLink(string requestId, string correlationId);
    Task<string> ResendActivationLink(string requestId, string correlationId, string siteUrl, ResendActivationViewModel resendActivation);
    Task<string> Activate(string requestId, string correlationId, string siteUrl, string activationKey);
    LogInViewModel GetForLogin(string requestId, string correlationId, string redirectUrl = null);
    Task<string> Login(string requestId, string correlationId, LogInViewModel signIn);
    Task<ChangePasswordViewModel> GetForChangePassword(string requestId, string correlationId, string username);
    Task<string> ChangePassword(string requestId, string correlationId, string siteUrl, ChangePasswordViewModel changePassword);
    ResendActivationViewModel GetForPasswordResetLink(string requestId, string correlationId);
    Task<string> RequestPasswordReset(string requestId, string correlationId, string siteUrl, ResendActivationViewModel resetPasswordRequest);
    Task<ChangePasswordViewModel> GetForPasswordResetRequest(string requestId, string correlationId, string passwordResetKey);
    Task<string> ResetPassword(string requestId, string correlationId, string siteUrl, ChangePasswordViewModel changePassword);
    Task<LoginHelpViewModel> GetForLoginHelp(string requestId, string correlationId);
}

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMetadataService _metadataService;
    private readonly IKeyService _keyService;
    private readonly IEmailService _emailService;
    private readonly IPhoneService _phoneService;
    private readonly RecaptchaSettings _recaptchaSettings;
    private readonly IRecaptchaService _recaptchaService;

    public AccountService(ILogger<AccountService> logger, IUserRepository userRepository, IMetadataService metadataService, IKeyService keyService, IEmailService emailService, IPhoneService phoneService, IOptions<RecaptchaSettings> recaptchaSettingsOptions, IRecaptchaService recaptchaService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _keyService = keyService ?? throw new ArgumentNullException(nameof(keyService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _phoneService = phoneService ?? throw new ArgumentNullException(nameof(phoneService));
        _recaptchaSettings = recaptchaSettingsOptions?.Value ?? throw new ArgumentNullException(nameof(recaptchaSettingsOptions));
        _recaptchaService = recaptchaService ?? throw new ArgumentNullException(nameof(recaptchaService));
    }

    public async Task AssignMetadata(RegistrationViewModel model)
    {
        model.PhoneNumberTypes = await _metadataService.GetPhoneNumberTypes(true);
        model.LeadTypes = await _metadataService.GetLeadTypes(true);
    }

    public void AssignRecaptcha(RecaptchaViewModel model, string action)
    {
        model.RecaptchaAction = action;
        model.RecaptchaEnabled = _recaptchaSettings.Enabled ? "1" : "";
        model.RecaptchaKey = _recaptchaSettings.Key;
        model.RecaptchaToken = "";
        model.RecaptchaTokenUrl = _recaptchaSettings.TokenUrl;
        model.RecaptchaVersion = _recaptchaSettings.Version;
    }

    public async Task<bool> CheckAvailability(string requestId, string correlationId, string username, string emailAddress)
    {
        username = (username ?? "").Trim();
        if (string.IsNullOrWhiteSpace(username)) username = null;

        emailAddress = (emailAddress ?? "").Trim();
        if (string.IsNullOrWhiteSpace(emailAddress)) emailAddress = null;

        if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(emailAddress)) return false;

        var account = string.IsNullOrEmpty(username) ? await _userRepository.GetOneByEmailAddress(emailAddress) : await _userRepository.GetOne(username);
        return account == null;
    }

    public async Task<RegistrationViewModel> GetForRegistration(string requestId, string correlationId)
    {
        var model = new RegistrationViewModel()
        {
            Username = "",
            EmailAddress = "",
            Password = "",
            ConfirmationPassword = "",
            PhoneNumber = "",
            PhoneNumberExtn = "",
            PhoneNumberTypeCd = "",
            LeadTypeCd = "",
            LeadTypeOther = "",
            ConsentToReceiveEmailNotifications = false,
            AcceptTermsAndConditions = false,
        };
        await AssignMetadata(model);
        AssignRecaptcha(model, "REGISTER");
        return model;
    }

    public async Task<string> Register(string requestId, string correlationId, string siteUrl, RegistrationViewModel registration)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to register";

        if (registration == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        registration.Username = (registration.Username ?? "").Trim().ToLower();
        registration.EmailAddress = (registration.EmailAddress ?? "").Trim().ToLower();
        registration.Password = (registration.Password ?? "").Trim();
        registration.ConfirmationPassword = (registration.ConfirmationPassword ?? "").Trim();
        registration.PhoneNumber = (registration.PhoneNumber ?? "").Trim();
        registration.PhoneNumberExtn = (registration.PhoneNumberExtn ?? "").Trim();
        if (registration.PhoneNumberExtn.Length == 0) registration.PhoneNumberExtn = null;
        registration.PhoneNumberTypeCd = (registration.PhoneNumberTypeCd ?? "").Trim().ToUpper();
        registration.LeadTypeCd = (registration.LeadTypeCd ?? "").Trim().ToUpper();
        registration.LeadTypeOther = (registration.LeadTypeOther ?? "").Trim();
        if (registration.LeadTypeOther.Length == 0) registration.LeadTypeOther = null;
        registration.RecaptchaToken = (registration.RecaptchaToken ?? "").Trim();

        if (!_keyService.IsValidUsername(registration.Username))
        {
            _logger.LogError($"{logPrefix} - Invalid Username");
            return "R1021";
        }
        else
        {
            var duplicateAccount = await _userRepository.GetOne(registration.Username);
            if (duplicateAccount != null)
            {
                _logger.LogError($"{logPrefix} - Account already exists");
                return "R0021";
            }
        }

        if (!_emailService.IsValidEmailAddress(requestId, correlationId, registration.EmailAddress))
        {
            _logger.LogError($"{logPrefix} - Invalid Email Address");
            return "R102";
        }
        // 20250110 :: RM :: Allow duplicates by email address
        // else
        // {
        //     var duplicateAccount = await _userRepository.GetOneByEmailAddress(registration.EmailAddress);
        //     if (duplicateAccount != null)
        //     {
        //         _logger.LogError($"{logPrefix} - Account already exists");
        //         return "R002";
        //     }
        // }

        if (!_keyService.IsValidPassword(registration.Password))
        {
            _logger.LogError($"{logPrefix} - Invalid Password");
            return "R103";
        }

        if (!_keyService.IsValidPassword(registration.ConfirmationPassword))
        {
            _logger.LogError($"{logPrefix} - Invalid Password");
            return "R104";
        }

        if (registration.Password != registration.ConfirmationPassword)
        {
            _logger.LogError($"{logPrefix} - Passwords must match");
            return "R105";
        }

        if (!_phoneService.IsValidPhoneNumber(registration.PhoneNumber))
        {
            _logger.LogError($"{logPrefix} - Invalid Phone Number");
            return "R106";
        }

        var phoneNumberTypes = await _metadataService.GetPhoneNumberTypes();
        if (!phoneNumberTypes.ContainsKey(registration.PhoneNumberTypeCd))
        {
            _logger.LogError($"{logPrefix} - Phone Number Type is invalid");
            return "R107";
        }

        var leadTypes = await _metadataService.GetLeadTypes();
        if (!leadTypes.ContainsKey(registration.LeadTypeCd))
        {
            _logger.LogError($"{logPrefix} - Lead Type is invalid");
            return "R108";
        }
        if (registration.LeadTypeCd == "WEBSITE" || registration.LeadTypeCd == "NEWSPAPERART" || registration.LeadTypeCd == "OTHER")
        {
            if (string.IsNullOrEmpty(registration.LeadTypeOther))
            {
                _logger.LogError($"{logPrefix} - Lead Type Other is required");
                return "R109";
            }
        }

        if (!registration.ConsentToReceiveEmailNotifications)
        {
            _logger.LogError($"{logPrefix} - Email consent need to be accepted");
            return "R110";
        }

        if (!registration.AcceptTermsAndConditions)
        {
            _logger.LogError($"{logPrefix} - Terms and Conditions need to be accepted");
            return "R111";
        }

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, registration.RecaptchaToken, "REGISTER");
        var recaptchaCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaCode;
        }

        if (string.IsNullOrEmpty((registration.PhoneNumberExtn ?? "").Trim())) registration.PhoneNumberExtn = null;
        if (string.IsNullOrEmpty((registration.LeadTypeOther ?? "").Trim())) registration.LeadTypeOther = null;

        var account = new UserAccount()
        {
            Username = registration.Username,
            PasswordHash = _keyService.GetPasswordHash(registration.Password),
            ActivationKey = _keyService.GetActivationKey(),
            ActivationKeyExpiry = DateTime.Now.AddMinutes(10),
            EmailAddress = registration.EmailAddress,
            AuthRepEmailAddressInd = registration.AuthRepEmailAddressInd,
            PhoneNumber = registration.PhoneNumber,
            PhoneNumberExtn = registration.PhoneNumberExtn,
            PhoneNumberTypeCd = registration.PhoneNumberTypeCd,
            LeadTypeCd = registration.LeadTypeCd,
            LeadTypeOther = registration.LeadTypeOther,
            CreatedBy = registration.Username
        };

        var added = await _userRepository.Register(requestId, correlationId, account);
        if (!added)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R003";
        }

        var emailSent = await _emailService.SendRegistrationEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress, account.ActivationKey);

        return "";
    }

    public ResendActivationViewModel GetForResendActivationLink(string requestId, string correlationId)
    {
        var model = new ResendActivationViewModel()
        {
            Username = "",
        };
        AssignRecaptcha(model, "RESENDACTIVATION");
        return model;
    }

    public async Task<string> ResendActivationLink(string requestId, string correlationId, string siteUrl, ResendActivationViewModel resendActivation)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to resend activation link";

        if (resendActivation == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        resendActivation.Username = (resendActivation.Username ?? "").Trim().ToLower();

        var account = await _userRepository.GetOne(resendActivation.Username);

        if (string.IsNullOrEmpty(resendActivation.Username)
                || account == null || account.UsernameVerifiedInd)
        {
            _logger.LogError($"{logPrefix} - Invalid Account {resendActivation.Username} or account is already activated");
            return "";
        }

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, resendActivation.RecaptchaToken, "RESENDACTIVATION");
        var recaptchaErrorCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaErrorCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaErrorCode;
        }

        account.ActivationKey = _keyService.GetActivationKey();
        account.ActivationKeyExpiry = DateTime.Now.AddMinutes(10);
        account.UsernameVerifiedInd = false;

        var updated = await _userRepository.ReinitiateActivation(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R006";
        }

        var emailSent = await _emailService.ResendActivationEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress, account.ActivationKey);

        return "";
    }

    public async Task<string> Activate(string requestId, string correlationId, string siteUrl, string activationKey)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to activate account";

        activationKey = (activationKey ?? "").Trim();
        if (activationKey.Length == 0) activationKey = null;

        var account = await _userRepository.GetOneByKey(requestId, correlationId, activationKey: activationKey);

        if (account == null || account.UsernameVerifiedInd)
        {
            _logger.LogError($"{logPrefix} - Account not found by key, or already verified");
            return "R001";
        }

        if (!account.ActivationKeyExpiry.HasValue || account.ActivationKeyExpiry.Value < DateTime.Now)
        {
            _logger.LogError($"{logPrefix} - Activation link expired");
            return "R121";
        }

        var updated = await _userRepository.Activate(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R007";
        }

        var emailSent = await _emailService.SendAccountActivatedEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress);

        return "";
    }

    public LogInViewModel GetForLogin(string requestId, string correlationId, string redirectUrl = null)
    {
        var model = new LogInViewModel()
        {
            Username = "",
            Password = "",
            ReturnUrl = (redirectUrl ?? "").Trim(),
        };
        AssignRecaptcha(model, "LOGIN");
        return model;
    }

    public async Task<string> Login(string requestId, string correlationId, LogInViewModel signIn)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to sign in";

        if (signIn == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        signIn.Username = (signIn.Username ?? "").Trim().ToLower();
        signIn.Password = (signIn.Password ?? "").Trim();

        var account = await _userRepository.GetOne(signIn.Username);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return "R001";
        }
        if (!account.UsernameVerifiedInd)
        {
            _logger.LogError($"{logPrefix} - Account not activated");
            return "R131";
        }

        var passwordHash = _keyService.GetPasswordHash(signIn.Password);
        if (account.PasswordHash != passwordHash)
        {
            _logger.LogError($"{logPrefix} - Invalid credentials");
            return "R132";
        }

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, signIn.RecaptchaToken, "LOGIN");
        var recaptchaErrorCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaErrorCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaErrorCode;
        }

        account = await _userRepository.Authenticate(requestId, correlationId, account);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R008";
        }

        return "";
    }

    public async Task<ChangePasswordViewModel> GetForChangePassword(string requestId, string correlationId, string username)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to change password";

        var account = await _userRepository.GetOne(username);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - User not found");
            return null;
        }

        var model = new ChangePasswordViewModel()
        {
            Username = account.Username,
            CurrentPassword = "",
            NewPassword = "",
            ConfirmationPassword = ""
        };
        AssignRecaptcha(model, "CHANGEPASSWORD");
        return model;
    }

    public async Task<string> ChangePassword(string requestId, string correlationId, string siteUrl, ChangePasswordViewModel changePassword)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to change password";

        if (changePassword == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        changePassword.Username = (changePassword.Username ?? "").Trim().ToLower();
        changePassword.CurrentPassword = (changePassword.CurrentPassword ?? "").Trim();
        changePassword.NewPassword = (changePassword.NewPassword ?? "").Trim();
        changePassword.ConfirmationPassword = (changePassword.ConfirmationPassword ?? "").Trim();

        var account = await _userRepository.GetOne(changePassword.Username);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return "R001";
        }
        if (!account.UsernameVerifiedInd)
        {
            _logger.LogError($"{logPrefix} - Account not activated");
            return "R131";
        }

        if (!_keyService.IsValidPassword(changePassword.CurrentPassword))
        {
            _logger.LogError($"{logPrefix} - Current Password is invalid");
            return "R141";
        }

        var currentPasswordHash = _keyService.GetPasswordHash(changePassword.CurrentPassword);
        if (currentPasswordHash != account.PasswordHash)
        {
            _logger.LogError($"{logPrefix} - Current Password mismatch");
            return "R142";
        }

        if (!_keyService.IsValidPassword(changePassword.NewPassword))
        {
            _logger.LogError($"{logPrefix} - New Password is invalid");
            return "R143";
        }

        if (changePassword.CurrentPassword == changePassword.NewPassword)
        {
            _logger.LogError($"{logPrefix} - Current and New Passwords must not match");
            return "R144";
        }

        if (!_keyService.IsValidPassword(changePassword.ConfirmationPassword))
        {
            _logger.LogError($"{logPrefix} - Confirmation Password is invalid");
            return "R145";
        }

        if (changePassword.NewPassword != changePassword.ConfirmationPassword)
        {
            _logger.LogError($"{logPrefix} - New Passwords must match");
            return "R146";
        }

        var passwordHash = _keyService.GetPasswordHash(changePassword.NewPassword);
        account.PasswordHash = passwordHash;

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, changePassword.RecaptchaToken, "CHANGEPASSWORD");
        var recaptchaErrorCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaErrorCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaErrorCode;
        }

        account.ModifiedBy = changePassword.Username;
        var updated = await _userRepository.ChangePassword(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R009";
        }

        var emailSent = await _emailService.SendPasswordChangedEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress);

        return "";
    }

    public ResendActivationViewModel GetForPasswordResetLink(string requestId, string correlationId)
    {
        var model = new ResendActivationViewModel()
        {
            Username = "",
        };
        AssignRecaptcha(model, "RESETPASSWORDREQUEST");
        return model;
    }

    public async Task<string> RequestPasswordReset(string requestId, string correlationId, string siteUrl, ResendActivationViewModel resetPasswordRequest)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to request password reset";

        if (resetPasswordRequest == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        resetPasswordRequest.Username = (resetPasswordRequest.Username ?? "").Trim().ToLower();

        var account = await _userRepository.GetOne(resetPasswordRequest.Username);

        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Invalid Account {resetPasswordRequest.Username} or account is already activated");
            return "";
        }

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, resetPasswordRequest.RecaptchaToken, "RESETPASSWORD");
        var recaptchaErrorCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaErrorCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaErrorCode;
        }

        account.ActivationKey = _keyService.GetActivationKey();
        account.ActivationKeyExpiry = DateTime.Now.AddMinutes(10);
        account.UsernameVerifiedInd = false;
        account.ModifiedBy = resetPasswordRequest.Username;

        var updated = await _userRepository.InitiatePasswordResetRequest(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R010";
        }

        var emailSent = await _emailService.SendPasswordResetRequestEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress, account.ActivationKey);

        return "";
    }

    public async Task<ChangePasswordViewModel> GetForPasswordResetRequest(string requestId, string correlationId, string passwordResetKey)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to reset password";

        passwordResetKey = (passwordResetKey ?? "").Trim();
        if (passwordResetKey.Length == 0) passwordResetKey = null;

        var account = await _userRepository.GetOneByKey(requestId, correlationId, passwordResetKey: passwordResetKey);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return new ChangePasswordViewModel() { Code = "R001" };
        }

        if (!account.PasswordResetKeyExpiry.HasValue || account.PasswordResetKeyExpiry.Value < DateTime.Now)
        {
            _logger.LogError($"Unable to reset password - Reset password link expired");
            return new ChangePasswordViewModel() { Code = "R151" };
        }

        var newKey = _keyService.GetActivationKey();
        var validated = await _userRepository.ExchangeKey(requestId, correlationId, newKey, passwordResetKey: passwordResetKey);
        if (!validated)
        {
            _logger.LogError($"{logPrefix} - Unable to exchange key");
            return new ChangePasswordViewModel() { Code = "R152" };
        }

        var model = new ChangePasswordViewModel()
        {
            Username = account.Username,
            CurrentPassword = "",
            NewPassword = "",
            ConfirmationPassword = "",
            ResetKey = newKey
        };
        AssignRecaptcha(model, "RESETPASSWORD");
        return model;
    }

    public async Task<string> ResetPassword(string requestId, string correlationId, string siteUrl, ChangePasswordViewModel resetPassword)
    {
        var logPrefix = $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to complete password reset";

        if (resetPassword == null)
        {
            _logger.LogError($"{logPrefix} - Invalid input");
            return "R101";
        }

        resetPassword.Username = (resetPassword.Username ?? "").Trim().ToLower();
        resetPassword.NewPassword = (resetPassword.NewPassword ?? "").Trim();
        resetPassword.ConfirmationPassword = (resetPassword.ConfirmationPassword ?? "").Trim();
        resetPassword.ResetKey = (resetPassword.ResetKey ?? "").Trim();

        var account = await _userRepository.GetOneByKey(requestId, correlationId, passwordResetKey: resetPassword.ResetKey);
        if (account == null)
        {
            _logger.LogError($"{logPrefix} - Account not found");
            return "R001";
        }

        if (!account.PasswordResetKeyExpiry.HasValue || account.PasswordResetKeyExpiry.Value < DateTime.Now)
        {
            _logger.LogError($"{logPrefix} - Reset password link expired");
            return "R151";
        }

        if (!_keyService.IsValidPassword(resetPassword.NewPassword))
        {
            _logger.LogError($"{logPrefix} - Invalid New Password");
            return "R143";
        }

        var newPasswordHash = _keyService.GetPasswordHash(resetPassword.NewPassword);
        if (newPasswordHash == account.PasswordHash)
        {
            _logger.LogError($"{logPrefix} - May not reuse an old password");
            return "R153";
        }

        if (!_keyService.IsValidPassword(resetPassword.ConfirmationPassword))
        {
            _logger.LogError($"{logPrefix} - Invalid Confirmation Password");
            return "R145";
        }

        if (resetPassword.NewPassword != resetPassword.ConfirmationPassword)
        {
            _logger.LogError($"{logPrefix} - Passwords must match");
            return "R146";
        }

        // RECAPTCHA
        var recaptchaVerification = await _recaptchaService.Validate(requestId, correlationId, resetPassword.RecaptchaToken, "RESETPASSWORD");
        var recaptchaErrorCode = recaptchaVerification?.ErrorCodes?.FirstOrDefault() ?? string.Empty;
        if (!string.IsNullOrEmpty(recaptchaErrorCode))
        {
            _logger.LogError($"{logPrefix} - reCAPTCHA validation failed");
            return recaptchaErrorCode;
        }

        account.PasswordHash = newPasswordHash;
        account.ModifiedBy = resetPassword.Username;

        var updated = await _userRepository.ChangePassword(requestId, correlationId, account);
        if (!updated)
        {
            _logger.LogError($"{logPrefix} - System or database exception");
            return "R011";
        }

        var emailSent = await _emailService.SendPasswordChangedEmail(requestId, correlationId, siteUrl, account.Username, account.EmailAddress);

        return "";
    }

    public async Task<LoginHelpViewModel> GetForLoginHelp(string requestId, string correlationId)
    {
        var model = new LoginHelpViewModel()
        {
            HelpTypes = await _metadataService.GetLoginHelpTypes(),
            HelpTypeCd = "PWD",
            Username = "",
            EmailAddress = ""
        };
        AssignRecaptcha(model, "LOGINHELP");
        return model;
    }
}