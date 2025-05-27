using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Services;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly ILogger<ProfileController> _logger;
    private readonly IMessageService _messageService;
    private readonly IMetadataService _metadataService;
    private readonly IProfileService _profileService;

    public ProfileController(ILogger<ProfileController> logger, IMessageService messageService, IProfileService profileService, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
        _profileService = profileService ?? throw new ArgumentNullException(nameof(profileService));
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromHeader] string username, [FromQuery] string a = null)
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
            var model = await _profileService.GetOne(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            ViewBag.DisplayPasswordChangedAlert = (a?.Trim() ?? string.Empty) == "pwc";
            ViewBag.Hash = (a ?? "").Trim().ToUpper();

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
    public async Task<IActionResult> EditProfile([FromHeader] string username)
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
            var model = await _profileService.GetForProfileInfoEdit(requestId, correlationId, usernameClaim.Value);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditProfile([FromHeader] string username, [FromForm] EditableProfileViewModel model)
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
            var returnCode = await _profileService.SaveProfileInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                await _profileService.AssignMetadata(model);
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Profile");
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
            var model = await _profileService.GetForAddressInfoEdit(requestId, correlationId, usernameClaim.Value);
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
            var returnCode = await _profileService.SaveAddressInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Profile", null, "ADDRESS");
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
    public async Task<IActionResult> GetCountiesByStateCd([FromHeader] string username, [FromQuery] string stateCd)
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
            var model = await _profileService.GetCountiesByStateCd(requestId, correlationId, usernameClaim.Value, stateCd);
            if (model == null)
            {
                return StatusCode(500, "Application Error");
            }

            return Ok(model);
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
    public async Task<IActionResult> EditContactInfo([FromHeader] string username)
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
            var model = await _profileService.GetForContactInfoEdit(requestId, correlationId, usernameClaim.Value);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditContactInfo([FromHeader] string username, [FromForm] EditableProfileViewModel model)
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
            var returnCode = await _profileService.SaveContactInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                await _profileService.AssignMetadata(model);
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Profile", null, "CONTACT");
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
    public async Task<IActionResult> EditPreferences([FromHeader] string username)
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
            var model = await _profileService.GetForPreferencesInfoEdit(requestId, correlationId, usernameClaim.Value);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditPreferences([FromHeader] string username, [FromForm] EditablePreferencesViewModel model)
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
            var returnCode = await _profileService.SavePreferencesInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                await _profileService.AssignMetadata(model);
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            return RedirectToAction("Index", "Profile", null, "PREFS");
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
    public async Task<IActionResult> EditNetWorth([FromHeader] string username)
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
            var model = await _profileService.GetForNetWorthInfoEdit(requestId, correlationId, usernameClaim.Value);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> EditNetWorth([FromHeader] string username, [FromForm] EditableNetWorthViewModel model)
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
            var returnCode = await _profileService.SaveNetWorthInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return View(model);
            }

            return RedirectToAction("Index", "Profile", null, "NETWORTH");
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
    public async Task<IActionResult> Notifications([FromHeader] string username, [FromQuery] string filterTypeCd = null)
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
            var model = await _profileService.GetNotifications(requestId, correlationId, usernameClaim.Value, filterTypeCd);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpPost]
    public async Task<IActionResult> UpdateNotification([FromHeader] string username, [FromForm] EditableUserNotificationViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");

            var returnCode = await _profileService.UpdateNotification(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                var erorModel = new ErrorViewModel()
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                };
                return "|N101|N102|N103|".Contains($"|{returnCode}|") ? BadRequest(erorModel) : StatusCode(500, erorModel);
            }

            return Ok();
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
    public async Task<IActionResult> Documents([FromHeader] string username)
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
            var model = await _profileService.GetDocuments(requestId, correlationId, usernameClaim.Value);
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
            return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000", Message = exception.Message, Details = exception.StackTrace });
        }
        finally
        {
            _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Ended");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetDocument([FromHeader] string username, [FromQuery] long docId)
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
            var model = await _profileService.GetDocument(requestId, correlationId, usernameClaim.Value, docId);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return File(model.DocContents, model.MimeType, model.FileName);
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
    public async Task<IActionResult> AddDocument([FromHeader] string username)
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
            var model = await _profileService.GetForAddDocument(requestId, correlationId, usernameClaim.Value);
            if (model == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return LocalRedirect("/");
            }

            return View("EditDocument", model);
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
    public async Task<IActionResult> AddDocument([FromHeader] string username, [FromForm] EditableUserDocumentViewModel model)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");

            if (model.File != null)
            {
                model.FileName = model.File.FileName;
                using var memoryStream = new MemoryStream();
                await model.File.CopyToAsync(memoryStream);
                model.DocContents = memoryStream.ToArray();
            }

            // var model = new EditableUserDocumentViewModel()
            // {
            //     DocId = 0,
            //     DocTypeCd = form["DocTypeCd"].FirstOrDefault() ?? "",
            //     DocName = form["DocName"].FirstOrDefault() ?? "",
            // };
            // if (form.Files.Count > 0)
            // {
            //     using var memoryStream = new MemoryStream();
            //     await form.Files[0].CopyToAsync(memoryStream);
            //     model.DocContents = memoryStream.ToArray();
            //     model.FileName = form.Files[0].Name;
            // }

            var returnCode = await _profileService.AddDocument(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return View("EditDocument", model);
            }

            return RedirectToAction("Index", "Profile", null, "DOCS");
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
    public async Task<IActionResult> DeleteDocument([FromHeader] string username, [FromQuery] long docId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");

            var returnCode = await _profileService.DeleteDocument(requestId, correlationId, usernameClaim.Value, docId);
            if (returnCode.Length > 0)
            {
                var erorModel = new ErrorViewModel()
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                };
                return "|UD101|UD001|".Contains($"|{returnCode}|") ? BadRequest(erorModel) : StatusCode(500, erorModel);
            }

            return Ok();
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