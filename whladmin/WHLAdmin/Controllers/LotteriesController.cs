using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Common.Services;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

public class LotteriesController : Controller
{
    private readonly ILogger<LotteriesController> _logger;
    private readonly ILotteriesService _lotteriesService;
    private readonly IMessageService _messageService;

    public LotteriesController(ILogger<LotteriesController> logger
                                , ILotteriesService lotteriesService
                                , IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _lotteriesService = lotteriesService ?? throw new ArgumentNullException(nameof(lotteriesService));
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
            var model = await _lotteriesService.GetEligibleListings(requestId, correlationId, username);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,LOTADMIN")]
    public async Task<IActionResult> RunLottery([FromHeader] string username, [FromQuery] long listingId, [FromQuery] bool rerun = false)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var returnCode = await _lotteriesService.RunLottery(requestId, correlationId, username, listingId, rerun);
            if (returnCode.Length > 0 && returnCode.StartsWith("LT"))
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                });
            }
            return Ok(new ListingViewModel() { ListingId = listingId, LotteryId = Convert.ToInt32(returnCode) });
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
    public async Task<IActionResult> Results([FromHeader] string username, [FromQuery] int lotteryId
                                                , [FromQuery] long listingId
                                                , [FromQuery] int pageNo = 1, [FromQuery] int pageSize = 1000
                                                , [FromQuery] string download = "N")
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        lotteryId = (lotteryId < 0) ? 0 : lotteryId;
        listingId = (listingId < 0) ? 0 : listingId;
        pageNo = pageNo < 1 ? 1 : pageNo;
        pageSize = pageSize < 100 ? 100 : pageSize;
        download = (download ?? "").Trim().ToUpper();

        try
        {
            if (download == "Y")
            {
                var contents = await _lotteriesService.GetDownloadData(requestId, correlationId, username, lotteryId);
                return File(Encoding.UTF8.GetBytes(contents), "text/csv", $"Lottery Results for Listing {listingId}.csv");
            }

            var model = await _lotteriesService.GetResults(requestId, correlationId, username, lotteryId, pageNo, pageSize);
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
    public async Task<ActionResult> Download([FromHeader] string username, [FromQuery] int lotteryId, [FromQuery] long listingId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        lotteryId = (lotteryId < 0) ? 0 : lotteryId;
        listingId = (listingId < 0) ? 0 : listingId;

        try
        {
            var contents = await _lotteriesService.GetDownloadData(requestId, correlationId, username, lotteryId);
            return File(Encoding.UTF8.GetBytes(contents), "text/csv", $"Lottery Results for Listing {listingId}.csv");
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