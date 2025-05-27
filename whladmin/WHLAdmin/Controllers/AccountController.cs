using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Okta.AspNetCore;
using WHLAdmin.Services;

namespace WHLAdmin.Controllers;

[ExcludeFromCodeCoverage]
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IUsersService _usersService;

    public AccountController(ILogger<AccountController> logger, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _usersService = usersService ?? throw new ArgumentException(nameof(usersService));
    }

    public async Task<IActionResult> SignInAsync()
    {
        if (!HttpContext.User.Identity.IsAuthenticated)
        {
            return Challenge(OktaDefaults.MvcAuthenticationScheme);
        }

        var emailAddress = HttpContext.User?.Claims?.FirstOrDefault(f => f.Type == "email")?.Value ?? "";
        if (emailAddress == null)
        {
            _logger.LogError($"User with Email Address: {emailAddress} is setup in Okta, but no email address setup!");
            return RedirectToAction("NotAuthorized", "Home", null);
        }

        var user = await _usersService.Authenticate(Guid.NewGuid().ToString(), null, emailAddress);
        if (user == null)
        {
            _logger.LogError($"User with Email Address: {emailAddress} is setup in Okta, but not setup in the application!");
            return RedirectToAction("NotAuthorized", "Home", null);
        }

        _logger.LogDebug($"User with Email Address: {emailAddress} logged in!");

        // Set session
        HttpContext.Session.SetString("username", user.UserId);
        HttpContext.Session.SetString("email", user.EmailAddress);
        HttpContext.Session.SetString("displayname", user.DisplayName);
        HttpContext.Session.SetString("role", user.RoleCd);
        HttpContext.Session.SetString("org", user.OrganizationCd);

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public new IActionResult SignOut()
    {
        var emailAddress = HttpContext.User?.Claims?.FirstOrDefault(f => f.Type == "email")?.Value ?? "";
        if (emailAddress != null)
        {
            _logger.LogDebug($"User with Email Address: {emailAddress} logged out!");
        }

        HttpContext.Session.Clear();

        return new SignOutResult(
            [
                OktaDefaults.MvcAuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme,
            ],
            new AuthenticationProperties { RedirectUri = "/Home/" });
    }
}