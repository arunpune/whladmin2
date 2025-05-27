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

public class DocumentTypesController : Controller
{
    private readonly ILogger<DocumentTypesController> _logger;
    private readonly IDocumentTypesService _documentTypesService;
    private readonly IMessageService _messageService;

    public DocumentTypesController(ILogger<DocumentTypesController> logger, IDocumentTypesService documentTypesService, IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _documentTypesService = documentTypesService ?? throw new ArgumentNullException(nameof(documentTypesService));
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
            var model = await _documentTypesService.GetData(requestId, correlationId, username);
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
            var model = _documentTypesService.GetOneForAdd(requestId, correlationId);
            return PartialView("_DocumentTypeEditor", model);
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
    public async Task<IActionResult> Add([FromHeader] string username, [FromForm] EditableDocumentTypeViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _documentTypesService.Add(requestId, correlationId, username, model);
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
    public async Task<IActionResult> Edit([FromHeader] string username, [FromQuery] int documentTypeId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _documentTypesService.GetOneForEdit(requestId, correlationId, documentTypeId);
            if (model == null)
            {
                return NotFound();
            }
            return PartialView("_DocumentTypeEditor", model);
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
    public async Task<IActionResult> Edit([FromHeader] string username, [FromForm] EditableDocumentTypeViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _documentTypesService.Update(requestId, correlationId, username, model);
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
    public async Task<IActionResult> Delete([FromHeader] string username, [FromQuery] int documentTypeId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _documentTypesService.Delete(requestId, correlationId, username, documentTypeId);
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
