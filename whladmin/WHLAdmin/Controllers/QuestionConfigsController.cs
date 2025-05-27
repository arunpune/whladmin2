using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Filters;
using WHLAdmin.Common.Services;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Controllers;

[ExcludeFromCodeCoverage]
public class QuestionConfigsController : Controller
{
    private readonly ILogger<QuestionConfigsController> _logger;
    private readonly IQuestionConfigsService _questionConfigsService;
    private readonly IMessageService _messageService;
    private readonly IMetadataService _metadataService;

    public QuestionConfigsController(ILogger<QuestionConfigsController> logger, IQuestionConfigsService questionConfigsService, IMessageService messageService, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _questionConfigsService = questionConfigsService ?? throw new ArgumentNullException(nameof(questionConfigsService));
        _messageService = messageService ?? throw new ArgumentNullException(nameof(messageService));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN,OPSVIEWER")]
    public async Task<IActionResult> Index()
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new QuestionConfigsViewModel()
            {
                Questions = await _questionConfigsService.GetAll()
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

    [HttpGet]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Add()
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var model = new EditableQuestionConfigViewModel()
            {
                QuestionId = 0,
                CategoryCd = "PROFILE",
                Categories = await _metadataService.GetQuestionCategories(true),
                Title = "",
                AnswerTypeCd = "STRING",
                AnswerTypes = await _metadataService.GetAnswerTypes(true),
                MinLength = 0,
                MaxLength = 0,
                OptionsList = "",
                HelpText = "",
                Active = true
            };
            return PartialView("_QuestionConfigEditor", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Add(EditableQuestionConfigViewModel model)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var username = User.Identity.Name;
            if (username.Contains("\\")) username = username.Substring(username.IndexOf("\\") + 1);

            var question = new QuestionConfigViewModel()
            {
                QuestionId = 0,
                CategoryCd = model.CategoryCd,
                Title = model.Title,
                AnswerTypeCd = model.AnswerTypeCd,
                MinLength = model.MinLength,
                MaxLength = model.MaxLength,
                OptionsList = model.OptionsList,
                HelpText = model.HelpText,
                Active = model.Active
            };
            var returnCode = await _questionConfigsService.Add(correlationId, username, question);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Edit(int questionId)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var question = await _questionConfigsService.GetOne(questionId);
            if (question == null)
            {
                return NotFound();
            }
            var model = new EditableQuestionConfigViewModel()
            {
                QuestionId = questionId,
                CategoryCd = question.CategoryCd,
                Categories = await _metadataService.GetQuestionCategories(true),
                Title = question.Title,
                AnswerTypeCd = question.AnswerTypeCd,
                AnswerTypes = await _metadataService.GetAnswerTypes(true),
                MinLength = question.MinLength,
                MaxLength = question.MaxLength,
                OptionsList = question.OptionsList,
                HelpText = question.HelpText,
                Active = question.Active
            };
            return PartialView("_QuestionConfigEditor", model);
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
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Edit(EditableQuestionConfigViewModel model)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var username = User.Identity.Name;
            if (username.Contains("\\")) username = username.Substring(username.IndexOf("\\") + 1);

            var question = new QuestionConfigViewModel()
            {
                QuestionId = model.QuestionId,
                CategoryCd = model.CategoryCd,
                Title = model.Title,
                AnswerTypeCd = model.AnswerTypeCd,
                MinLength = model.MinLength,
                MaxLength = model.MaxLength,
                OptionsList = model.OptionsList,
                HelpText = model.HelpText,
                Active = model.Active
            };
            var returnCode = await _questionConfigsService.Update(correlationId, username, question);
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

    [HttpPost]
    [AdminRoleAuthorize("SYSADMIN,OPSADMIN")]
    public async Task<IActionResult> Delete(int questionId)
    {
        var correlationId = Guid.NewGuid().ToString();
        var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        _logger.LogDebug($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Started");

        try
        {
            var username = User.Identity.Name;
            if (username.Contains("\\")) username = username.Substring(username.IndexOf("\\") + 1);

            var returnCode = await _questionConfigsService.Delete(correlationId, username, questionId);
            if (returnCode.Length > 0)
            {
                return BadRequest(new ErrorViewModel
                {
                    Code = returnCode,
                    Message = _messageService.GetMessage(returnCode),
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
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
