using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

[AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
public class AuditController : Controller
{
    private readonly ILogger<AuditController> _logger;
    private readonly IAuditService _auditService;

    public AuditController(ILogger<AuditController> logger, IAuditService auditService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
    }

    [HttpGet]
    [AdminRoleAuthorize("")]
    public async Task<IActionResult> Index([FromHeader] string username, [FromQuery] string entityType, [FromQuery] string entityId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _auditService.GetData(entityType, entityId);
            return PartialView(model);
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
