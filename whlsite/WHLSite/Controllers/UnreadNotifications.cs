using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

public class UnreadNotificationsViewComponent : ViewComponent
{
    private readonly ILogger<UnreadNotificationsViewComponent> _logger;
    private readonly IUserRepository _userRepository;

    public UnreadNotificationsViewComponent(ILogger<UnreadNotificationsViewComponent> logger, IUserRepository userRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<IViewComponentResult> InvokeAsync([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new UserNotificationsViewModel();
            if (User.Identity.IsAuthenticated)
            {
                var usernameClaim = HttpContext.User?.Claims?.FirstOrDefault(f => f.Type == "Username");
                var notifications = await _userRepository.GetNotifications(requestId, correlationId, usernameClaim.Value, "U");
                model.Notifications = notifications.Select(s => s.ToViewModel());
            }
            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View(new SystemInfoViewModel()
            {
                ReleaseVersion = "1.0.0E",
                Timestamp = DateTime.Now
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }
}