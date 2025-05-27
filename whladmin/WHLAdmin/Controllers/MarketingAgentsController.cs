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

public class MarketingAgentsController : Controller
{
    private readonly ILogger<MarketingAgentsController> _logger;
    private readonly IMarketingAgentsService _agentsService;
    private readonly IMessageService _messageService;

    public MarketingAgentsController(ILogger<MarketingAgentsController> logger, IMarketingAgentsService agentsService, IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _agentsService = agentsService ?? throw new ArgumentNullException(nameof(agentsService));
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
            var model = await _agentsService.GetData(requestId, correlationId, username);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult Add([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = _agentsService.GetOneForAdd(requestId, correlationId);
            return PartialView("_MarketingAgentEditor", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Add([FromHeader] string username, [FromForm] EditableMarketingAgentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _agentsService.Add(requestId, correlationId, username, model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromQuery] int agentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _agentsService.GetOneForEdit(requestId, correlationId, agentId);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_MarketingAgentEditor", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Edit([FromHeader] string username, [FromForm] EditableMarketingAgentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _agentsService.Update(requestId, correlationId, username, model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> Delete([FromHeader] string username, [FromQuery] int agentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _agentsService.Delete(requestId, correlationId, username, agentId);
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

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Details([FromHeader] string username, [FromQuery] int agentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _agentsService.GetOne(requestId, correlationId, agentId);
            return PartialView("_MarketingAgentViewer", model);
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
}