using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IFaqService _faqsService;
    private readonly IQuoteService _quotesService;
    private readonly IResourceService _resourcesService;
    private readonly IVideoService _videosService;

    public HomeController(ILogger<HomeController> logger, IFaqService faqsService, IQuoteService quotesService, IResourceService resourcesService, IVideoService videosService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _faqsService = faqsService ?? throw new ArgumentNullException(nameof(faqsService));
        _quotesService = quotesService ?? throw new ArgumentNullException(nameof(quotesService));
        _resourcesService = resourcesService ?? throw new ArgumentNullException(nameof(resourcesService));
        _videosService = videosService ?? throw new ArgumentNullException(nameof(videosService));
    }

    public async Task<IActionResult> Index([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new HomeViewModel()
            {
                Quote = await _quotesService.GetQuoteForHomePage(requestId, correlationId),
                Video = await _videosService.GetVideoForHomePage(requestId, correlationId)
            };
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

    public async Task<IActionResult> About([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _videosService.GetData(requestId, correlationId);
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

    public async Task<IActionResult> Faqs([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _faqsService.GetData(requestId, correlationId);
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

    public async Task<IActionResult> Resources([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = await _resourcesService.GetData(requestId, correlationId);
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        return View(new ErrorViewModel { RequestId = requestId });
    }
}
