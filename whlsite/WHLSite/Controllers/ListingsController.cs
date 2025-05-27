using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

public class ListingsController : Controller
{
    private readonly ILogger<ListingsController> _logger;
    private readonly IListingService _listingsService;

    public ListingsController(ILogger<ListingsController> logger, IListingService listingsService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _listingsService = listingsService ?? throw new ArgumentNullException(nameof(listingsService));
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetData(requestId, correlationId);
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
    public async Task<IActionResult> Index([FromHeader] string username, [FromForm] ListingSearchViewModel searchModel)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.Search(requestId, correlationId, searchModel);
            return View("Index", model);
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
    public async Task<IActionResult> Details([FromHeader] string username, [FromQuery] int listingId, [FromQuery] string prn = null)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
                username ??= usernameClaim?.Value;
            }

            var model = await _listingsService.GetOne(requestId, correlationId, listingId, username);
            if (model == null)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = $"Listing #{listingId} not found!" });
            }

            if ((prn ?? "").Trim().Equals("1", StringComparison.CurrentCultureIgnoreCase))
            {
                return View("PrintableDetails", model);
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
    public async Task<IActionResult> PrintableForm([FromHeader] string username, [FromQuery] int listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetPrintableForm(requestId, correlationId, listingId);
            if (model == null)
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Listing #{listingId} not found!");
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = $"Listing #{listingId} not found!" });
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
    public async Task<IActionResult> ViewDocument([FromHeader] string username, [FromQuery] long documentId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _listingsService.GetDocument(requestId, correlationId, documentId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }
            if (model.Contents.Contains(";base64,"))
            {
                model.Contents = model.Contents.Substring(model.Contents.IndexOf(";base64,") + 8);
            }
            return File(Convert.FromBase64String(model.Contents), model.MimeType, model.FileName);
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
    public async Task<IActionResult> AffordabilityAnalysis([FromHeader] string username, [FromQuery] int listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetForAffordabilityAnalysis(requestId, correlationId, listingId);
            return PartialView("_AffordabilityAnalysisViewer", model);
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
    public async Task<IActionResult> AffordabilityDetails([FromHeader] string username, [FromQuery] int unitId, [FromQuery] decimal rate, [FromQuery] int term)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _listingsService.GetAffordabilityAnalysis(requestId, correlationId, unitId, rate, term);
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