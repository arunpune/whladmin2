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

public class AmortizationsController : Controller
{
    private readonly ILogger<AmortizationsController> _logger;
    private readonly IAmortizationsService _amortizationsService;
    private readonly IMessageService _messageService;

    public AmortizationsController(ILogger<AmortizationsController> logger, IAmortizationsService amortizationsService, IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _amortizationsService = amortizationsService ?? throw new ArgumentNullException(nameof(amortizationsService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Index([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _amortizationsService.GetData(requestId, correlationId, username);
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
    [AdminRoleAuthorize("SYSADMIN")]
    public IActionResult Add([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = _amortizationsService.GetOneForAdd(requestId, correlationId);
            return PartialView("_AmortizationEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN")]
    public async Task<IActionResult> Add([FromHeader] string username, [FromForm] EditableAmortizationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _amortizationsService.Add(requestId, correlationId, username, model);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
            }
            return Ok(model);
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
    [AdminRoleAuthorize("SYSADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromQuery] decimal rate)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _amortizationsService.GetOneForEdit(requestId, correlationId, rate);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_AmortizationEditor", model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return PartialView("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromForm] EditableAmortizationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _amortizationsService.Update(requestId, correlationId, username, model);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
            }
            return Ok(model);
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

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN")]
    public async Task<IActionResult> Delete([FromHeader] string username, [FromQuery] int amortizationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _amortizationsService.Delete(requestId, correlationId, username, amortizationId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
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
}
