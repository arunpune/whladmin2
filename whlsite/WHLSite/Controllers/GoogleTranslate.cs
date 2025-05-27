using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WHLSite.Common.Settings;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

public class GoogleTranslateViewComponent : ViewComponent
{
    private readonly ILogger<GoogleTranslateViewComponent> _logger;
    private readonly bool _enabled;

    public GoogleTranslateViewComponent(ILogger<GoogleTranslateViewComponent> logger, IOptions<GoogleTranslateSettings> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _enabled = options?.Value?.Enabled ?? false;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var enabled = await Task.FromResult(_enabled);
            return View(enabled);
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