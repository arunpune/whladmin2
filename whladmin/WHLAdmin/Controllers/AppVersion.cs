using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Repositories;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

public class AppVersionViewComponent : ViewComponent
{
    private readonly ILogger<AppVersionViewComponent> _logger;
    private readonly ISystemRepository _systemRepository;

    public AppVersionViewComponent(ILogger<AppVersionViewComponent> logger, ISystemRepository systemRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _systemRepository = systemRepository ?? throw new ArgumentNullException(nameof(systemRepository));
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var version = await _systemRepository.GetInfo();
            return View(new SystemInfoViewModel()
            {
                ReleaseVersion = version?.ReleaseVersion ?? "0.0.1D",
                Timestamp = version?.Timestamp ?? DateTime.Now
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View(new SystemInfoViewModel()
            {
                ReleaseVersion = "0.0.1E",
                Timestamp = DateTime.Now
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }
}