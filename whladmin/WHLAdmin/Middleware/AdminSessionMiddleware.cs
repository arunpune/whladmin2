using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WHLAdmin.Services;

namespace WHLAdmin.Admin.Middleware;

[ExcludeFromCodeCoverage]
public class AdminSessionMiddleware
{
    private RequestDelegate _next;

    private readonly ILogger<AdminSessionMiddleware> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUsersService _userService;

    public AdminSessionMiddleware(RequestDelegate next, ILogger<AdminSessionMiddleware> logger, IConfiguration configuration, IUsersService userService)
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (context == null || context.Session == null)
        {
            await _next(context);
            return;
        }

        if (context.Session.GetString("INIT") == "Y")
        {
            await _next(context);
            return;
        }

        Initialize(context);
        await _next(context);
    }

    private async void Initialize(HttpContext context)
    {
        if (context.Session.GetString("INIT") == "Y")
        {
            return;
        }

        // Initialize session
        var user = context.User;
        var emailAddress = user.Claims.FirstOrDefault(f => f.Type == "email")?.Value ?? "";

        _logger.LogDebug($"Email Address: {emailAddress}");

        var overrideEmailAddress = _configuration.GetValue<string>("OverrideEmailAddress");
        overrideEmailAddress = (overrideEmailAddress ?? "").Trim();
        _logger.LogDebug($"Override Email Address: {overrideEmailAddress}");

        emailAddress = !string.IsNullOrEmpty(overrideEmailAddress) ? overrideEmailAddress : emailAddress;
        _logger.LogDebug($"Using Email Address: {emailAddress}");

        var correlationId = emailAddress ?? Guid.NewGuid().ToString();
        var adminUser = await _userService.Authenticate(correlationId, emailAddress: emailAddress);
        if (adminUser != null)
        {
            context.Session.SetString(ClaimTypes.Name, adminUser.UserId);
            context.Session.SetString("DisplayName", adminUser.DisplayName);
            context.Session.SetString(ClaimTypes.Email, adminUser.EmailAddress);
            context.Session.SetString("Role", adminUser.RoleCd);
            context.Session.SetString("Org", adminUser.OrganizationCd);
            context.Session.SetString("INIT", "Y");
        }
    }
}