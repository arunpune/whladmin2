using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Services;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

[Authorize]
public class HouseholdController : Controller
{
    private readonly ILogger<HouseholdController> _logger;
    private readonly IHouseholdService _householdService;
    private readonly IMessageService _messageService;
    private readonly IMetadataService _metadataService;

    public HouseholdController(ILogger<HouseholdController> logger, IHouseholdService householdService, IMessageService messageService, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _householdService = householdService ?? throw new ArgumentNullException(nameof(householdService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetOne(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditAddressInfo([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetForAddressInfoEdit(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditAddressInfo([FromHeader] string username, [FromForm] EditableAddressInfoViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.SaveAddressInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Household", null, "ADDR");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditVoucherInfo([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetForVoucherInfoEdit(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditVoucherInfo([FromHeader] string username, [FromForm] EditableVoucherInfoViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.SaveVoucherInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                model.VoucherTypes = await _metadataService.GetVoucherTypes();

                return View(model);
            }

            return RedirectToAction("Index", "Household", null, "VCH");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditLiveInAideInfo([FromHeader] string username)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetForLiveInAideInfoEdit(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditLiveInAideInfo([FromHeader] string username, [FromForm] EditableLiveInAideInfoViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.SaveLiveInAideInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return View(model);
            }

            return RedirectToAction("Index", "Household", null, "LIA");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditMember([FromHeader] string username, [FromQuery] long memberId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetForMemberInfoEdit(requestId, correlationId, usernameClaim.Value, memberId);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditMember([FromHeader] string username, [FromForm] EditableHouseholdMemberViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.SaveMemberInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                await _householdService.AssignMetadata(model);
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Household", null, "MBR");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteMember([FromHeader] string username, [FromQuery] long memberId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.DeleteMemberInfo(requestId, correlationId, usernameClaim.Value, memberId);
            if (returnCode.Length > 0)
            {
                var errorModel = new ErrorViewModel()
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                };
                return (returnCode == "H001" || returnCode == "H401") ? NotFound(errorModel) : StatusCode(500, errorModel);
            }

            return RedirectToAction("Index", "Household", new { a = "dmbr" });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> EditAccount([FromHeader] string username, [FromQuery] long accountId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var model = await _householdService.GetForAccountInfoEdit(requestId, correlationId, usernameClaim.Value, accountId);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View(model);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditAccount([FromHeader] string username, [FromForm] EditableHouseholdAccountViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.SaveAccountInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                await _householdService.AssignMetadata(model);
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return View(model);
            }

            return RedirectToAction("Index", "Household", null, "ACT");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteAccount([FromHeader] string username, [FromQuery] long accountId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return LocalRedirect("/");
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _householdService.DeleteAccountInfo(requestId, correlationId, usernameClaim.Value, accountId);
            if (returnCode.Length > 0)
            {
                var errorModel = new ErrorViewModel()
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                };
                return (returnCode == "H001" || returnCode == "H501") ? NotFound(errorModel) : StatusCode(500, errorModel);
            }

            return RedirectToAction("Index", "Household", new { a = "dacc" });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unhandled exception");
            return View("Error", new ErrorViewModel
            {
                RequestId = requestId,
                CorrelationId = correlationId,
                Code = "EX000",
                Message = exception.Message,
                Details = exception.StackTrace
            });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }
}