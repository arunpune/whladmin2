using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Services;
using WHLSite.Extensions;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Controllers;

[Authorize]
public class ApplicationsController : Controller
{
    private readonly ILogger<ApplicationsController> _logger;
    private readonly IMessageService _messageService;
    private readonly IHousingApplicationService _applicationsService;

    public ApplicationsController(ILogger<ApplicationsController> logger, IHousingApplicationService applicationsService, IMessageService messageService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _applicationsService = applicationsService ?? throw new ArgumentNullException(nameof(applicationsService));
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard([FromHeader] string username)
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
            var model = await _applicationsService.GetForDashboard(requestId, correlationId, usernameClaim.Value);
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
    public async Task<IActionResult> SubmitterInfo([FromHeader] string username, [FromQuery] int listingId = 0, [FromQuery] long applicationId = 0)
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
            var model = await _applicationsService.GetApplicantInfo(requestId, correlationId, usernameClaim.Value, listingId, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            if (!model.Editable)
            {
                return RedirectToAction("Submitted", new { applicationId = applicationId });
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
    public async Task<IActionResult> SubmitterInfo([FromHeader] string username, [FromForm] HAViewModel model)
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
            model = await _applicationsService.GetApplicantInfo(requestId, correlationId, usernameClaim.Value, model.ListingId, model.ApplicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            if (!model.Editable)
            {
                return RedirectToAction("Submitted", new { applicationId = model.ApplicationId });
            }

            return RedirectToAction("ApplicationInfo", new { applicationId = model.ApplicationId, listingId = model.ListingId });
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
    public async Task<IActionResult> ApplicationInfo([FromHeader] string username, [FromQuery] int listingId = 0, [FromQuery] long applicationId = 0)
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
            var model = await _applicationsService.GetForEdit(requestId, correlationId, usernameClaim.Value, listingId, applicationId);
            model.StepNumber = 1;
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            if (!model.Editable)
            {
                return RedirectToAction("Submitted", new { applicationId = applicationId });
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
    public async Task<IActionResult> ApplicationInfo([FromHeader] string username, [FromForm] EditableHousingApplicationViewModel model)
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

            var returnCode = await _applicationsService.ValidateApplicationInfo(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View(model);
            }

            TempData["UnitTypeCds"] = model.UnitTypeCds;
            TempData["MemberIds"] = model.MemberIds;
            TempData["CoApplicantMemberId"] = model.CoApplicantMemberId.ToString();
            TempData["AccessibilityCds"] = model.AccessibilityCds;
            TempData["LeadTypeCd"] = model.LeadTypeCd;
            TempData["LeadTypeOther"] = model.LeadTypeOther;

            return RedirectToAction("ReviewSubmitInfo", new { applicationId = model.ApplicationId, listingId = model.ListingId });
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
    public async Task<IActionResult> ReviewSubmitInfo([FromHeader] string username, [FromQuery] long applicationId = 0, [FromQuery] int listingId = 0)
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
            var model = await _applicationsService.GetForEdit(requestId, correlationId, usernameClaim.Value, listingId, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            if (!model.Editable)
            {
                return RedirectToAction("Submitted", new { applicationId = model.ApplicationId });
            }

            var unitTypeCds = (TempData["UnitTypeCds"] ?? "").ToString();
            var memberIds = (TempData["MemberIds"] ?? "").ToString();
            long.TryParse((TempData["CoApplicantMemberId"] ?? "0").ToString(), out var coApplicantMemberId);
            var accessibilityCds = (TempData["AccessibilityCds"] ?? "").ToString();
            var leadTypeCd = (TempData["LeadTypeCd"] ?? "").ToString();
            var leadTypeOther = (TempData["LeadTypeOther"] ?? "").ToString();

            var selectedMembers = new List<HouseholdMemberViewModel>();
            var selectedMemberIds = (memberIds ?? "").Trim().Split(",", StringSplitOptions.RemoveEmptyEntries);
            if (selectedMemberIds.Length != 0)
            {
                foreach (var selectedMemberId in selectedMemberIds)
                {
                    if (long.TryParse(selectedMemberId, out var smid) && smid > 0)
                    {
                        var selectedMember = model.Members.FirstOrDefault(m => m.MemberId == smid);
                        if (selectedMember != null)
                        {
                            selectedMembers.Add(selectedMember);
                        }
                    }
                }
            }

            var selectedAccounts = new List<HouseholdAccountViewModel>();
            foreach (var account in model.Accounts.Where(w => w.PrimaryHolderMemberId == 0))
            {
                selectedAccounts.Add(account);
            }
            if (selectedMembers.Count > 0)
            {
                foreach (var smid in selectedMembers.Select(s => s.MemberId))
                {
                    foreach (var account in model.Accounts.Where(w => w.PrimaryHolderMemberId == smid))
                    {
                        selectedAccounts.Add(account);
                    }
                }
            }

            var displayLeadType = "Not specified";
            if (!string.IsNullOrEmpty(leadTypeCd))
            {
                switch ((leadTypeCd ?? "").Trim().ToUpper())
                {
                    case "WEBSITE":
                    case "NEWSPAPERART":
                    case "OTHER":
                        displayLeadType = $"Other: {leadTypeOther}";
                        break;
                    default:
                        var description = model.LeadTypes.FirstOrDefault(f => f.Key.Equals(leadTypeCd, StringComparison.CurrentCultureIgnoreCase)).Value;
                        displayLeadType = description.Length > 0 ? description : leadTypeCd;
                        break;
                }
            }

            model.StepNumber = 2;
            model.UnitTypeCds = unitTypeCds;
            model.Members = selectedMembers;
            model.MemberIds = memberIds;
            model.CoApplicantInd = coApplicantMemberId > 0;
            model.CoApplicantMemberId = coApplicantMemberId;
            model.Accounts = selectedAccounts;
            model.AccessibilityCds = accessibilityCds;
            model.LeadTypeCd = leadTypeCd;
            model.LeadTypeOther = leadTypeOther;
            model.DisplayLeadType = displayLeadType;

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
    public async Task<IActionResult> ReviewSubmitInfo([FromHeader] string username, [FromForm] EditableHousingApplicationViewModel model)
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

            var siteUrl = Request?.SiteUrl();
            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _applicationsService.SubmitApplication(requestId, correlationId, usernameClaim.Value, siteUrl, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;
                return View("Error", model);
            }

            TempData["SubmittedApplicationId"] = model.ApplicationId.ToString();
            return RedirectToAction("Details", new { applicationId = model.ApplicationId });
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
    public async Task<IActionResult> Details([FromHeader] string username, [FromQuery] long applicationId)
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
            var model = await _applicationsService.GetSubmitted(requestId, correlationId, usernameClaim.Value, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            model.DisplayRailroad = long.TryParse((TempData["SubmittedApplicationId"] ?? "0").ToString(), out var submittedApplicationId) && submittedApplicationId > 0;
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
    public async Task<IActionResult> Submitted([FromHeader] string username, [FromQuery] long applicationId)
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
            var model = await _applicationsService.GetSubmitted(requestId, correlationId, usernameClaim.Value, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
            }

            if (model.IsWithdrawn)
            {
                return View("Withdrawn", model);
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
    public async Task<IActionResult> Withdrawn([FromHeader] string username, [FromQuery] long applicationId)
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
            var model = await _applicationsService.GetSubmitted(requestId, correlationId, usernameClaim.Value, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
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
    public async Task<IActionResult> GetDocument([FromHeader] string username, [FromQuery] long applicationId, [FromQuery] long docId)
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
            var model = await _applicationsService.GetDocument(requestId, correlationId, usernameClaim.Value, applicationId, docId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
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
    public async Task<IActionResult> AddDocument([FromHeader] string username, [FromQuery] long applicationId)
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
            var model = await _applicationsService.GetForAddDocument(requestId, correlationId, usernameClaim.Value, applicationId);
            if (model == null)
            {
                return View("Error", new ErrorViewModel { RequestId = requestId, CorrelationId = correlationId, Code = "EX000" });
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
    public async Task<IActionResult> AddDocument([FromHeader] string username, [FromForm] EditableApplicationDocumentViewModel model)
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

            var returnCode = await _applicationsService.AddDocument(requestId, correlationId, usernameClaim.Value, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return View("EditDocument", model);
            }

            return RedirectToAction("Submitted", "Applications", new { applicationId = model.ApplicationId }, "DOCS");
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
    public async Task<IActionResult> DeleteDocument([FromHeader] string username, [FromQuery] long applicationId, [FromQuery] long docId)
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

            var returnCode = await _applicationsService.DeleteDocument(requestId, correlationId, usernameClaim.Value, applicationId, docId);
            if (returnCode.Length > 0)
            {
                var erorModel = new ErrorViewModel()
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = requestId
                };
                return "|D101|D001|".Contains($"|{returnCode}|") ? BadRequest(erorModel) : StatusCode(500, erorModel);
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

    [HttpPost]
    public async Task<IActionResult> Withdraw([FromHeader] string username, [FromQuery] long applicationId)
    {
        var correlationId = username ?? Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext?.TraceIdentifier ?? correlationId;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            if (!User.Identity.IsAuthenticated)
            {
                return StatusCode(500);
            }

            var siteUrl = Request?.SiteUrl();
            var usernameClaim = User.Claims.FirstOrDefault(f => f.Type == "Username");
            var returnCode = await _applicationsService.WithdrawApplication(requestId, correlationId, usernameClaim.Value, siteUrl, applicationId);
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

    [HttpPost]
    public async Task<IActionResult> AddComment([FromHeader] string username, [FromForm] EditableApplicationCommentViewModel model)
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

            var siteUrl = Request?.SiteUrl();
            var returnCode = await _applicationsService.AddComment(requestId, correlationId, usernameClaim.Value, siteUrl, model);
            if (returnCode.Length > 0)
            {
                model.Code = returnCode;
                model.Message = _messageService.GetMessage(returnCode);
                model.RequestId = requestId;

                return StatusCode(500, model);
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