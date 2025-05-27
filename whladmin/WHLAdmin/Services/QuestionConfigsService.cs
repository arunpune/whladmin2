using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IQuestionConfigsService
{
    Task<IEnumerable<QuestionConfigViewModel>> GetAll();
    Task<QuestionConfigViewModel> GetOne(int id);
    Task<string> Add(string correlationId, string username, QuestionConfigViewModel question);
    Task<string> Update(string correlationId, string username, QuestionConfigViewModel question);
    Task<string> Delete(string correlationId, string username, int id);
}

[ExcludeFromCodeCoverage]
public class QuestionConfigsService : IQuestionConfigsService
{
    private readonly ILogger<QuestionConfigsService> _logger;
    private readonly IQuestionConfigRepository _questionConfigRepository;
    private readonly IMetadataService _metadataService;

    public QuestionConfigsService(ILogger<QuestionConfigsService> logger, IQuestionConfigRepository questionConfigRepository, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _questionConfigRepository = questionConfigRepository ?? throw new ArgumentNullException(nameof(questionConfigRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    public async Task<IEnumerable<QuestionConfigViewModel>> GetAll()
    {
        var questions = await _questionConfigRepository.GetAll();
        return questions.Select(s => new QuestionConfigViewModel()
        {
            Active = s.Active,
            AnswerTypeCd = s.AnswerTypeCd,
            AnswerTypeDescription = s.AnswerTypeDescription,
            CategoryCd = s.CategoryCd,
            CategoryDescription = s.CategoryDescription,
            CreatedBy = s.CreatedBy,
            CreatedDate = s.CreatedDate,
            HelpText = s.HelpText,
            MaxLength = s.MaxLength,
            MinLength = s.MinLength,
            ModifiedBy = s.ModifiedBy,
            ModifiedDate = s.ModifiedDate,
            Options = s.Options,
            OptionsList = s.OptionsList,
            QuestionId = s.QuestionId,
            Title = s.Title
        });
    }

    public async Task<QuestionConfigViewModel> GetOne(int id)
    {
        var question = await _questionConfigRepository.GetOne(new QuestionConfig() { QuestionId = id });
        return new QuestionConfigViewModel()
        {
            Active = question.Active,
            AnswerTypeCd = question.AnswerTypeCd,
            AnswerTypeDescription = question.AnswerTypeDescription,
            CategoryCd = question.CategoryCd,
            CategoryDescription = question.CategoryDescription,
            CreatedBy = question.CreatedBy,
            CreatedDate = question.CreatedDate,
            HelpText = question.HelpText,
            MaxLength = question.MaxLength,
            MinLength = question.MinLength,
            ModifiedBy = question.ModifiedBy,
            ModifiedDate = question.ModifiedDate,
            Options = question.Options,
            OptionsList = question.OptionsList,
            QuestionId = question.QuestionId,
            Title = question.Title
        };
    }

    public async Task<string> Add(string correlationId, string username, QuestionConfigViewModel question)
    {
        question = question ?? throw new ArgumentNullException(nameof(question));

        question.CategoryCd = question.CategoryCd?.Trim() ?? string.Empty;
        var questionCategories = await _metadataService.GetQuestionCategories();
        if (string.IsNullOrEmpty(question.CategoryCd) || !questionCategories.ContainsKey(question.CategoryCd))
        {
            _logger.LogError($"Unable to add question - Invalid Category");
            return "Q101";
        }

        question.Title = question.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(question.Title))
        {
            _logger.LogError($"Unable to add question - Invalid Question");
            return "Q102";
        }

        question.AnswerTypeCd = question.AnswerTypeCd?.Trim() ?? string.Empty;
        var _answerTypes = await _metadataService.GetAnswerTypes();
        if (string.IsNullOrEmpty(question.AnswerTypeCd) || !_answerTypes.ContainsKey(question.AnswerTypeCd))
        {
            _logger.LogError($"Unable to add question - Invalid Answer Type");
            return "Q103";
        }

        if (question.AnswerTypeCd == "SINGLESELECT" || question.AnswerTypeCd == "MULTISELECT")
        {
            var answerOptions = question.OptionsList?.Split("|", StringSplitOptions.RemoveEmptyEntries);
            if (answerOptions == null || (answerOptions?.Length ?? 0) == 0)
            {
                _logger.LogError($"Unable to add question - Invalid Answer Options");
                return "Q104";
            }
        }

        var questionConfigs = await _questionConfigRepository.GetAll();

        var duplicateQuestion = questionConfigs.FirstOrDefault(f => f.Title.Equals(question.Title, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateQuestion != null)
        {
            _logger.LogError($"Unable to add duplicate question - {question.Title}");
            return "Q002";
        }

        question.CreatedBy = username;
        var added = await _questionConfigRepository.Add(correlationId, question);
        if (!added)
        {
            return "Q003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, QuestionConfigViewModel question)
    {
        question = question ?? throw new ArgumentNullException(nameof(question));

        question.CategoryCd = question.CategoryCd?.Trim() ?? string.Empty;
        var questionCategories = await _metadataService.GetQuestionCategories();
        if (string.IsNullOrEmpty(question.CategoryCd) || !questionCategories.ContainsKey(question.CategoryCd))
        {
            _logger.LogError($"Unable to update question - Invalid Category");
            return "Q101";
        }

        question.Title = question.Title?.Trim() ?? string.Empty;
        if (string.IsNullOrEmpty(question.Title))
        {
            _logger.LogError($"Unable to update question - Invalid Title");
            return "Q102";
        }

        question.AnswerTypeCd = question.AnswerTypeCd?.Trim() ?? string.Empty;
        var _answerTypes = await _metadataService.GetAnswerTypes();
        if (string.IsNullOrEmpty(question.AnswerTypeCd) || !_answerTypes.ContainsKey(question.AnswerTypeCd))
        {
            _logger.LogError($"Unable to update question - Invalid Answer Type");
            return "Q103";
        }

        if (question.AnswerTypeCd == "SINGLESELECT" || question.AnswerTypeCd == "MULTISELECT")
        {
            var answerOptions = question.OptionsList?.Split("|", StringSplitOptions.RemoveEmptyEntries);
            if (answerOptions == null || (answerOptions?.Length ?? 0) == 0)
            {
                _logger.LogError($"Unable to update question - Invalid Answer Options");
                return "Q104";
            }
        }

        var questionConfigs = await _questionConfigRepository.GetAll();

        var existingQuestion = questionConfigs.FirstOrDefault(f => f.QuestionId == question.QuestionId);
        if (existingQuestion == null)
        {
            _logger.LogError($"Unable to find question - {question.QuestionId}");
            return "Q001";
        }

        var duplicateQuestion = questionConfigs.FirstOrDefault(f => f.QuestionId != question.QuestionId && f.Title.Equals(question.Title, StringComparison.CurrentCultureIgnoreCase));
        if (duplicateQuestion != null)
        {
            _logger.LogError($"Unable to update duplicate question - {question.Title}");
            return "Q002";
        }

        question.ModifiedBy = username;
        var updated = await _questionConfigRepository.Update(correlationId, question);
        if (!updated)
        {
            return "Q004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        var existingQuestion = await _questionConfigRepository.GetOne(new QuestionConfig() { QuestionId = id });
        if (existingQuestion == null)
        {
            _logger.LogError($"Unable to find question - {id}");
            return "Q001";
        }

        existingQuestion.ModifiedBy = username;
        var deleted = await _questionConfigRepository.Delete(correlationId, existingQuestion);
        if (!deleted)
        {
            return "Q005";
        }

        return "";
    }
}