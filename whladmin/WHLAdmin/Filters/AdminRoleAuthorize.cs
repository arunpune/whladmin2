using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;

namespace WHLAdmin.Common.Filters;

[ExcludeFromCodeCoverage]
public class AdminRoleAuthorizeAttribute : TypeFilterAttribute
{
    public AdminRoleAuthorizeAttribute(string roles) : base(typeof(AdminRoleAuthorizeFilter))
    {
        Arguments = [roles];
    }
}

[ExcludeFromCodeCoverage]
public class AdminRoleAuthorizeFilter : IAuthorizationFilter
{
    private readonly string _roles;
    private readonly ILogger<AdminRoleAuthorizeFilter> _logger;
    private readonly string _authMode;

    public AdminRoleAuthorizeFilter(string roles, ILogger<AdminRoleAuthorizeFilter> logger, IConfiguration configuration)
    {
        _roles = roles;
        _logger = logger;
        _authMode = configuration.GetValue<string>("AuthMode");
    }

    public void OnAuthorization(AuthorizationFilterContext filterContext)
    {
        var user = filterContext?.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated == false)
        {
            filterContext.Result = _authMode == "OKTA" ? new RedirectToActionResult("SignIn", "Account", null) : new RedirectToActionResult("LogIn", "Home", null);
            return;
        }

        string userId = "";
        string emailAddress = "";
        if (_authMode == "OKTA")
        {
            userId = null;
            emailAddress = user?.Claims?.FirstOrDefault(f => f.Type == "email")?.Value ?? "";
            // if (filterContext.HttpContext.Session.TryGetValue(ClaimTypes.Name, out var userIdBytes))
            // {
            //     userId = Encoding.ASCII.GetString(userIdBytes);
            // }

            // if (filterContext.HttpContext.Session.TryGetValue(ClaimTypes.Email, out var emailAddressBytes))
            // {
            //     emailAddress = Encoding.ASCII.GetString(emailAddressBytes);
            // }
        }
        else
        {
            userId = (user?.Identity?.Name ?? "").Trim();
            if (userId.Contains("\\")) userId = userId.Substring(userId.IndexOf("\\") + 1);
            if (userId.Length == 0) userId = null;

            emailAddress = user?.Claims?.FirstOrDefault(f => f.Type == "email")?.Value ?? "";
            if (emailAddress.Length == 0) emailAddress = null;
        }

        if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(emailAddress))
        {
            filterContext.Result = new RedirectToActionResult("SignOut", "Account", null);
            return;
        }

        _logger.LogDebug($"Username: {userId}");
        _logger.LogDebug($"Email Address: {emailAddress}");

        var config = filterContext.HttpContext.RequestServices.GetService<IConfiguration>();
        var overrideEmailAddress = config.GetValue<string>("OverrideEmailAddress");
        overrideEmailAddress = (overrideEmailAddress ?? "").Trim();
        _logger.LogDebug($"Override Email Address: {overrideEmailAddress}");

        emailAddress = !string.IsNullOrEmpty(overrideEmailAddress) ? overrideEmailAddress : emailAddress;
        _logger.LogDebug($"Using Email Address: {emailAddress}");

        var adminUserRepository = filterContext.HttpContext.RequestServices.GetService<IAdminUserRepository>();
        var adminUser = adminUserRepository.GetOne(new User() { UserId = userId, EmailAddress = emailAddress }).Result;
        if (adminUser != null)
        {
            filterContext.HttpContext.Request.Headers["username"] = adminUser.UserId;
            if (string.IsNullOrEmpty(_roles)) return;
            if (_roles.Split(",", StringSplitOptions.RemoveEmptyEntries).Contains(adminUser.RoleCd)) return;
        }

        filterContext.Result = new RedirectToActionResult("NotAuthorized", "Home", null);
    }
}

// public class AdminRoleAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
// {
//     private readonly string _roles;

//     public AdminRoleAuthorizeAttribute(string roles)
//     {
//         _roles = roles;
//     }

//     public void OnAuthorization(AuthorizationFilterContext filterContext)
//     {
//         var user = filterContext.HttpContext.User;
//         var username = user.Identity.Name;
//         if (username.Contains("\\")) username = username.Substring(username.IndexOf("\\") + 1);

//         var emailAddress = $"{username}@westchestercountyny.gov";

//         var config = filterContext.HttpContext.RequestServices.GetService<IConfiguration>();
//         var overrideEmailAddress = config.GetValue<string>("OverrideEmailAddress");
//         emailAddress = overrideEmailAddress ?? emailAddress;

//         var adminUserRepository = filterContext.HttpContext.RequestServices.GetService<IAdminUserRepository>();
//         var adminUser = adminUserRepository.GetOne(new User() { EmailAddress = emailAddress }).Result;
//         if (adminUser != null)
//         {
//             if (string.IsNullOrEmpty(_roles)) return;
//             if (_roles.Split(",", System.StringSplitOptions.RemoveEmptyEntries).Contains(adminUser.RoleCd)) return;
//         }

//         filterContext.Result = new RedirectToActionResult("NotAuthorized", "Home", null);
//     }
// }