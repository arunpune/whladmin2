using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Common.Services;
using WHLAdmin.Extensions;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

public class ApplicationsController : Controller
{
    private readonly ILogger<ApplicationsController> _logger;
    private readonly IHousingApplicationsService _applicationsService;
    private readonly IMessageService _messageService;
    private readonly IMetadataService _metadataService;

    public ApplicationsController(ILogger<ApplicationsController> logger, IHousingApplicationsService applicationsService, IMessageService messageService, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _applicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Index([FromHeader] string username, [FromQuery] long listingId
                                            , [FromQuery] string submissionTypeCd = "ALL"
                                            , [FromQuery] string statusCd = "ALL"
                                            , [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 1000
                                            , [FromQuery] string download = "N")
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        submissionTypeCd = string.IsNullOrEmpty((submissionTypeCd ?? "").Trim()) ? "ALL" : submissionTypeCd;
        statusCd = string.IsNullOrEmpty((statusCd ?? "").Trim()) ? "ALL" : statusCd;
        pageNo = pageNo < 1 ? 1 : pageNo;
        pageSize = pageSize < 100 ? 100 : pageSize;
        download = (download ?? "").Trim().ToUpper();

        try
        {
            if (download == "Y")
            {
                var contents = await _applicationsService.GetDownloadData(requestId, correlationId, username, listingId, submissionTypeCd, statusCd);
                return File(Encoding.UTF8.GetBytes(contents), "text/csv", $"Applications for Listing {listingId}.csv");
            }
            var model = await _applicationsService.GetData(requestId, correlationId, username, listingId, submissionTypeCd, statusCd, pageNo, pageSize);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Details([FromHeader] string username, [FromQuery] long applicationId, [FromQuery] string submissionTypeCd = "ALL", [FromQuery] string statusCd = "ALL")
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        applicationId = (applicationId < 0) ? 0 : applicationId;

        try
        {
            var model = await _applicationsService.GetOne(requestId, correlationId, username, applicationId);
            ViewData["SubmissionTypeCd"] = submissionTypeCd ?? "ALL";
            ViewData["StatusCd"] = statusCd ?? "ALL";
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<ActionResult> Download([FromHeader] string username, [FromQuery] long listingId, [FromQuery] string submissionTypeCd = "ALL", [FromQuery] string statusCd = "ALL")
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        submissionTypeCd = string.IsNullOrEmpty((submissionTypeCd ?? "").Trim()) ? "ALL" : submissionTypeCd;
        statusCd = string.IsNullOrEmpty((statusCd ?? "").Trim()) ? "ALL" : statusCd;

        try
        {
            var contents = await _applicationsService.GetDownloadData(requestId, correlationId, username, listingId, submissionTypeCd, statusCd);
            return File(Encoding.UTF8.GetBytes(contents), "text/csv", $"Applications for Listing {listingId}.csv");
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Duplicates([FromHeader] string username, [FromQuery] long listingId = 0)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;

        try
        {
            var model = await _applicationsService.GetPotentialDuplicates(requestId, correlationId, username, listingId);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DuplicateApplicationsByDoBLast4Ssn([FromHeader] string username, [FromQuery] long listingId
                                                                            , [FromQuery] string dateOfBirth, [FromQuery] string last4Ssn)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        dateOfBirth = string.IsNullOrEmpty((dateOfBirth ?? "").Trim()) ? null : dateOfBirth.Trim();
        last4Ssn = string.IsNullOrEmpty((last4Ssn ?? "").Trim()) ? null : last4Ssn.Trim();

        try
        {
            var model = await _applicationsService.GetPotentialDuplicatesByDoBLast4Ssn(requestId, correlationId, username, listingId, dateOfBirth, last4Ssn);
            return View("DuplicateApplications", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DuplicateApplicationsByName([FromHeader] string username, [FromQuery] long listingId
                                                                            , [FromQuery] string name)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        name = string.IsNullOrEmpty((name ?? "").Trim()) ? null : name.Trim();

        try
        {
            var model = await _applicationsService.GetPotentialDuplicatesByName(requestId, correlationId, username, listingId, name);
            return View("DuplicateApplications", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DuplicateApplicationsByEmailAddress([FromHeader] string username, [FromQuery] long listingId
                                                                            , [FromQuery] string emailAddress)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        emailAddress = string.IsNullOrEmpty((emailAddress ?? "").Trim()) ? null : emailAddress.Trim();

        try
        {
            var model = await _applicationsService.GetPotentialDuplicatesByEmailAddress(requestId, correlationId, username, listingId, emailAddress);
            return View("DuplicateApplications", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DuplicateApplicationsByPhoneNumber([FromHeader] string username, [FromQuery] long listingId
                                                                            , [FromQuery] string phoneNumber)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        phoneNumber = string.IsNullOrEmpty((phoneNumber ?? "").Trim()) ? null : phoneNumber.Trim();

        try
        {
            var model = await _applicationsService.GetPotentialDuplicatesByPhoneNumber(requestId, correlationId, username, listingId, phoneNumber);
            return View("DuplicateApplications", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> DuplicateApplicationsByStreetAddress([FromHeader] string username, [FromQuery] long listingId
                                                                            , [FromQuery] string streetAddress)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        listingId = (listingId < 0) ? 0 : listingId;
        streetAddress = string.IsNullOrEmpty((streetAddress ?? "").Trim()) ? null : streetAddress.Trim();

        try
        {
            var model = await _applicationsService.GetPotentialDuplicatesByStreetAddress(requestId, correlationId, username, listingId, streetAddress);
            return View("DuplicateApplications", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> UpdateDuplicateStatus([FromHeader] string username, [FromForm] EditableDuplicateApplicationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var siteUrl = Request?.SiteUrl();
            var returnCode = await _applicationsService.UpdateDuplicateStatus(requestId, correlationId, username, siteUrl, model);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER,LOTADMIN,LOTVIEWER")]
    public async Task<IActionResult> Comments([FromHeader] string username, [FromQuery] long applicationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _applicationsService.GetComments(requestId, correlationId, username, applicationId);
            return PartialView("_CommentsPartial", model);
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

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public IActionResult AddComment([FromHeader] string username, [FromQuery] long applicationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = _applicationsService.GetOneForAddComment(requestId, correlationId, username, applicationId);
            return PartialView("_CommentEditor", model);
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
    public async Task<IActionResult> AddComment([FromHeader] string username, [FromForm] EditableApplicationCommentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _applicationsService.AddComment(requestId, correlationId, username, model);
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
}