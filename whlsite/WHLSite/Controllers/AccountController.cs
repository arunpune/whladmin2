using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Services;
using WHLSite.Extensions;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountService _accountService;
    private readonly IMessageService _messageService;
    private readonly IProfileService _profileService;

    public AccountController(ILogger<AccountController> logger, IAccountService accountService, IMessageService messageService, IProfileService profileService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> CheckAvailability([FromHeader] string username, [FromQuery] string u, [FromQuery] string e)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                if (await _accountService.CheckAvailability(requestId, correlationId, u, e))
                {
                    return Ok();
                }
            }

            return StatusCode(500);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Register([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = await _accountService.GetForRegistration(requestId, correlationId);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register([FromHeader] string username, [FromForm] RegistrationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            if (model == null)
            {
                model = await _accountService.GetForRegistration(requestId, correlationId);
                return View(model);
            }

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.Register(requestId, correlationId, siteUrl, model);
            if (returnCode.Length > 0)
            {
                await _accountService.AssignMetadata(model);
                _accountService.AssignRecaptcha(model, "REGISTER");
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            model.Password = "";
            model.ConfirmationPassword = "";
            return View("Registered", new AccountViewModel() { Username = model.Username });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResendActivation([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = _accountService.GetForResendActivationLink(requestId, correlationId);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResendActivation([FromHeader] string username, [FromForm] ResendActivationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            if (model == null)
            {
                model = _accountService.GetForResendActivationLink(requestId, correlationId);
                return View(model);
            }

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.ResendActivationLink(requestId, correlationId, siteUrl, model);
            if (returnCode.Length > 0)
            {
                return View("ActivationFailed", new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
            }

            var account = new AccountViewModel() { Username = model.Username };
            return View("ActivationRequested", account);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> Activate([FromHeader] string username, [FromQuery] string k)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.Activate(requestId, correlationId, siteUrl, k);
            if (returnCode.Length > 0)
            {
                return View("ActivationFailed", new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
            }

            return View("Activated");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult LogIn([FromHeader] string username, [FromQuery] string r = null)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = _accountService.GetForLogin(requestId, correlationId, r);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> LogIn([FromHeader] string username, [FromForm] LogInViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            if (model == null)
            {
                model = _accountService.GetForLogin(requestId, correlationId);
                return View(model);
            }

            var returnCode = await _accountService.Login(requestId, correlationId, model);
            if (returnCode.Length > 0)
            {
                _accountService.AssignRecaptcha(model, "LOGIN");
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username),
                new("Username", model.Username)
            };
            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(25),
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            var profile = await _profileService.GetOne(requestId, correlationId, model.Username);
            if (_profileService.IsIncompleteProfile(requestId, correlationId, profile))
            {
                return RedirectToAction("Index", "Profile");
            }

            if ((model.ReturnUrl ?? "").Trim().Length > 0)
            {
                try
                {
                    return Redirect(model.ReturnUrl);
                }
                catch (Exception exception)
                {
                    _logger.LogWarning(exception, "Failed to redirect to requested return url after login!");
                }
            }

            return RedirectToAction("Index", "Listings");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> LogOut([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return LocalRedirect("/");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> ChangePassword([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _accountService.GetForChangePassword(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                return RedirectToAction("LogOut");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromHeader] string username, [FromForm] ChangePasswordViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            model.Username = usernameClaim.Value;
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.ChangePassword(requestId, correlationId, siteUrl, model);
            if (returnCode.Length > 0)
            {
                _accountService.AssignRecaptcha(model, "CHANGEPASSWORD");
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return View("PasswordChangeComplete");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult ResetPasswordRequest([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = _accountService.GetForPasswordResetLink(requestId, correlationId);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResetPasswordRequest([FromHeader] string username, [FromForm] ResendActivationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            if (model == null)
            {
                model = _accountService.GetForPasswordResetLink(requestId, correlationId);
                return View(model);
            }

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.RequestPasswordReset(requestId, correlationId, siteUrl, model);
            if (returnCode.Length > 0)
            {
                _accountService.AssignRecaptcha(model, "RESETPASSWORDREQUEST");
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return View("ResetPasswordRequested");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> ResetPassword([FromHeader] string username, [FromQuery] string k)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = await _accountService.GetForPasswordResetRequest(requestId, correlationId, k);
            if ((model.Code ?? "").Length > 0)
            {
                return View("ResetPasswordRequestFailed", new ErrorViewModel()
                {
                    Code = model.Code,
                    Message = _messageService.GetMessage(model.Code),
                    RequestId = requestId
                });
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> ResetPassword([FromHeader] string username, [FromForm] ChangePasswordViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _accountService.ResetPassword(requestId, correlationId, siteUrl, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return View("PasswordResetComplete");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult LoginHelp([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            var model = _accountService.GetForLoginHelp(requestId, correlationId);
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> LoginHelp([FromHeader] string username, [FromForm] LoginHelpViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Profile");
            }

            if (model == null)
            {
                model = await _accountService.GetForLoginHelp(requestId, correlationId);
                return View(model);
            }

            var siteUrl = Request?.SiteUrl();

            model.HelpTypeCd = (model.HelpTypeCd ?? "").Trim().ToUpper();
            if (model.HelpTypeCd.Equals("PWD", StringComparison.CurrentCultureIgnoreCase))
            {
                var returnCode = await _accountService.RequestPasswordReset(requestId, correlationId, siteUrl, new ResendActivationViewModel()
                {
                    RecaptchaAction = "RESETPASSWORDREQUEST",
                    RecaptchaEnabled = model.RecaptchaEnabled,
                    RecaptchaKey = model.RecaptchaKey,
                    RecaptchaToken = model.RecaptchaToken,
                    RecaptchaTokenUrl = model.RecaptchaTokenUrl,
                    RecaptchaVersion = model.RecaptchaVersion,
                });
                if (returnCode.Length > 0)
                {
                    _accountService.AssignRecaptcha(model, "RESETPASSWORDREQUEST");
                    model.Code = returnCode;
                    model.Message = _messageService.GetMessage(returnCode);
                    model.RequestId = requestId;
                    return View(model);
                }
            }
            else if (model.HelpTypeCd.Equals("USR", StringComparison.CurrentCultureIgnoreCase))
            {
                var returnCode = await _accountService.ResendActivationLink(requestId, correlationId, siteUrl, new ResendActivationViewModel()
                {
                    RecaptchaAction = "RETRIEVEUSERNAMEREQUEST",
                    RecaptchaEnabled = model.RecaptchaEnabled,
                    RecaptchaKey = model.RecaptchaKey,
                    RecaptchaToken = model.RecaptchaToken,
                    RecaptchaTokenUrl = model.RecaptchaTokenUrl,
                    RecaptchaVersion = model.RecaptchaVersion,
                });
                if (returnCode.Length > 0)
                {
                    _accountService.AssignRecaptcha(model, "RETRIEVEUSERNAMEREQUEST");
                    model.Code = returnCode;
                    model.Message = _messageService.GetMessage(returnCode);
                    model.RequestId = requestId;
                    return View(model);
                }
            }

            return View("LoginHelpRequested");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }
}