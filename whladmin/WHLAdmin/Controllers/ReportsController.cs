using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Common.Services;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

[AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
public class ReportsController : Controller
{
    private readonly ILogger<ReportsController> _logger;
    private readonly IReportsService _reportsService;
    private readonly IMessageService _messageService;

    public ReportsController(ILogger<ReportsController> logger, IReportsService reportsService, IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _reportsService = reportsService ?? throw new ArgumentNullException(nameof(reportsService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }

    [HttpGet]
    public IActionResult DemographicsReport([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = _reportsService.InitializeApplicationDemographicsReport();
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

    [HttpPost]
    public async Task<IActionResult> DemographicsReport([FromHeader] string username, [FromForm] DemographicsReportViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (model == null)
            {
                model = _reportsService.InitializeApplicationDemographicsReport();
                return View(model);
            }

            var returnCode = await _reportsService.GetApplicationDemographicsReport(model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
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

    [HttpGet]
    public IActionResult RegistrationsReport([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = _reportsService.InitializeRegistrationsSummaryReport();
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

    [HttpPost]
    public async Task<IActionResult> RegistrationsReport([FromHeader] string username, [FromForm] RegistrationsSummaryReportViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (model == null)
            {
                model = _reportsService.InitializeRegistrationsSummaryReport();
                return View(model);
            }

            var returnCode = await _reportsService.GetRegistrationsSummaryReport(model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
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

    [HttpGet]
    public async Task<IActionResult> MarketEvaluationsReport([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _reportsService.InitializeMarketEvaluationsReport();
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

    [HttpPost]
    public async Task<IActionResult> MarketEvaluationsReport([FromHeader] string username, [FromForm] MarketEvaluationsReportViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (model == null)
            {
                model = await _reportsService.InitializeMarketEvaluationsReport();
                return View(model);
            }

            var returnCode = await _reportsService.GetMarketEvaluationsReport(model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
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
}
