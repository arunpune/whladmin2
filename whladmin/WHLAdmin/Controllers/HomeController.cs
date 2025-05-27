using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Okta.AspNetCore;
using WHLAdmin.Common.Filters;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUsersService _usersService;
    private readonly string _authMode;
    private readonly string _overrideEmailAddress;

    public HomeController(IConfiguration configuration, ILogger<HomeController> logger, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));

        _authMode = configuration.GetValue<string>("AuthMode");
        _overrideEmailAddress = configuration.GetValue<string>("OverrideEmailAddress");
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public IActionResult Index([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            return View();
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

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public IActionResult Privacy([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            return View();
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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> GetOtp(LoginViewModel model)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var sentOtp = await _usersService.SendOtp(requestId, correlationId, model.UserId, model.EmailAddress);
            if (!sentOtp)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: {model.EmailAddress} not notified!");
                return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = "Not notified" });
            }

            return Ok();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return StatusCode(500, new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string r = null)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                if ((_authMode ?? "").Equals("OKTA"))
                {
                    return Challenge(OktaDefaults.MvcAuthenticationScheme);
                }

                var model = new LoginViewModel()
                {
                    UserId = "",
                    ReturnUrl = (r ?? "").Trim()
                };
                return View(model);
            }
            return RedirectToAction("Index");
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

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                var emailAddress = (model.EmailAddress ?? "").Trim();
                _logger.LogDebug($"Email Address: {emailAddress}");

                _logger.LogDebug($"Override Email Address: {_overrideEmailAddress}");

                emailAddress = !string.IsNullOrEmpty(_overrideEmailAddress) ? _overrideEmailAddress : emailAddress;
                _logger.LogDebug($"Using Email Address: {emailAddress}");

                var adminUser = await _usersService.AuthenticateOtp(correlationId, emailAddress, model.Token);

                if (adminUser == null)
                {
                    return RedirectToAction("NotAuthorized");
                }

                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, adminUser.UserId),
                    new(ClaimTypes.Email, adminUser.EmailAddress),
                    new("DisplayName", adminUser.DisplayName),
                    new("Role", adminUser.RoleCd),
                    new("Org", adminUser.OrganizationCd)
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
            }

            return RedirectToAction("Index");
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

    [HttpGet]
    public async Task<IActionResult> Logout([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if ((_authMode ?? "") == "OKTA")
            {
                return new SignOutResult(
                    [
                        OktaDefaults.MvcAuthenticationScheme,
                        CookieAuthenticationDefaults.AuthenticationScheme,
                    ],
                    new AuthenticationProperties { RedirectUri = "/Home/SignedOut" });
            }
            if (User.Identity.IsAuthenticated)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }

            return LocalRedirect("/");
            // return new SignOutResult(
            //    [
            //         OktaDefaults.MvcAuthenticationScheme,
            //         CookieAuthenticationDefaults.AuthenticationScheme,
            //    ],
            //    new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/Home/Login" });
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

    [HttpGet]
    [AllowAnonymous]
    public IActionResult NotAuthorized([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            return View();
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

    [HttpGet]
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId });
    }

    [HttpPost]
    public IActionResult OktaSignInCallback()
    {
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult OktaSignOutCallback()
    {
        return RedirectToAction("SignedOut", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignedOut()
    {
        var correlationId = Guid.NewGuid().ToString();
        return View();
    }

    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public IActionResult Profile()
    {
        return View(HttpContext.User.Claims);
    }
}
