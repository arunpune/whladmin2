using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IQuoteConfigsService
{
    Task<QuoteConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<QuoteConfigViewModel>> GetAll();
    Task<QuoteConfigViewModel> GetOne(int id);
    EditableQuoteConfigViewModel GetOneForAdd();
    Task<EditableQuoteConfigViewModel> GetOneForEdit(int id);
    Task<string> Add(string correlationId, string username, EditableQuoteConfigViewModel quote);
    Task<string> Update(string correlationId, string username, EditableQuoteConfigViewModel quote);
    Task<string> Delete(string correlationId, string username, int id);
    void Sanitize(QuoteConfig quote);
    string Validate(QuoteConfig quote, IEnumerable<QuoteConfig> quotes);
}

public class QuoteConfigsService : IQuoteConfigsService
{
    private readonly ILogger<QuoteConfigsService> _logger;
    private readonly IQuoteConfigRepository _quoteConfigRepository;
    private readonly IUsersService _usersService;

    public QuoteConfigsService(ILogger<QuoteConfigsService> logger, IQuoteConfigRepository quoteConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _quoteConfigRepository = quoteConfigRepository ?? throw new ArgumentNullException(nameof(quoteConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<QuoteConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var quotes = await _quoteConfigRepository.GetAll();
        return new QuoteConfigsViewModel()
        {
            Quotes = quotes.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<QuoteConfigViewModel>> GetAll()
    {
        var quotes = await _quoteConfigRepository.GetAll();
        return quotes.Select(s => s.ToViewModel());
    }

    public async Task<QuoteConfigViewModel> GetOne(int id)
    {
        var quote = await _quoteConfigRepository.GetOne(new QuoteConfig() { QuoteId = id });
        return quote.ToViewModel();
    }

    public EditableQuoteConfigViewModel GetOneForAdd()
    {
        return new EditableQuoteConfigViewModel()
        {
            QuoteId = 0,
            Text = "",
            DisplayOnHomePageInd = false,
            Active = true
        };
    }

    public async Task<EditableQuoteConfigViewModel> GetOneForEdit(int id)
    {
        var quote = await _quoteConfigRepository.GetOne(new QuoteConfig() { QuoteId = id });
        return quote.ToEditableViewModel();
    }

    public async Task<string> Add(string correlationId, string username, EditableQuoteConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Quote - Invalid Input");
            return "QT000";
        }

        var quote = new QuoteConfig()
        {
            QuoteId = 0,
            Text = model.Text,
            DisplayOnHomePageInd = model.DisplayOnHomePageInd,
            Active = true,
            CreatedBy = username
        };
        Sanitize(quote);

        var quotes = await _quoteConfigRepository.GetAll();

        var validationCode = Validate(quote, quotes);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Quote - {quote.Text}");
            return validationCode;
        }

        var added = await _quoteConfigRepository.Add(correlationId, quote);
        if (!added)
        {
            _logger.LogError($"Failed to add Quote - {quote.Text} - Unknown error");
            return "QT003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableQuoteConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update Quote - Invalid Input");
            return "QT000";
        }

        var quote = new QuoteConfig()
        {
            QuoteId = model.QuoteId,
            Text = model.Text,
            DisplayOnHomePageInd = model.DisplayOnHomePageInd,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(quote);

        var quotes = await _quoteConfigRepository.GetAll();

        var validationCode = Validate(quote, quotes);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Quote - {quote.Text}");
            return validationCode;
        }

        var updated = await _quoteConfigRepository.Update(correlationId, quote);
        if (!updated)
        {
            _logger.LogError($"Failed to update Quote - {quote.Text} - Unknown error");
            return "QT004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete Quote - Invalid Input");
            return "QT000";
        }

        var existingQuote = await _quoteConfigRepository.GetOne(new QuoteConfig() { QuoteId = id });
        if (existingQuote == null)
        {
            _logger.LogError($"Unable to find quote - {id}");
            return "QT001";
        }

        existingQuote.ModifiedBy = username;
        var deleted = await _quoteConfigRepository.Delete(correlationId, existingQuote);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete Quote - {existingQuote.Text} - Unknown error");
            return "QT005";
        }

        return "";
    }

    public void Sanitize(QuoteConfig quote)
    {
        if (quote == null) return;

        quote.Text = (quote.Text ?? "").Trim();
    }

    public string Validate(QuoteConfig quote, IEnumerable<QuoteConfig> quotes)
    {
        if (quote == null)
        {
            _logger.LogError($"Unable to validate Quote - Invalid Input");
            return "QT000";
        }

        Sanitize(quote);

        if (quote.Text.Length == 0)
        {
            _logger.LogError($"Unable to validate quote - Text is required");
            return "QT101";
        }

        if ((quotes?.Count() ?? 0) > 0)
        {
            if (quote.QuoteId > 0)
            {
                // Existence check
                var existingResource = quotes.FirstOrDefault(f => f.QuoteId == quote.QuoteId);
                if (existingResource == null)
                {
                    _logger.LogError($"Unable to find Quote - {quote.QuoteId}");
                    return "QT001";
                }

                // Duplicate check
                var duplicateResource = quotes.FirstOrDefault(f => f.QuoteId != quote.QuoteId && f.Text.Equals(quote.Text, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateResource != null)
                {
                    _logger.LogError($"Unable to validate Quote - Duplicate {quote.Text}");
                    return "QT002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateResource = quotes.FirstOrDefault(f => f.Text.Equals(quote.Text, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateResource != null)
                {
                    _logger.LogError($"Unable to validate Quote - Duplicate {quote.Text}");
                    return "QT002";
                }
            }
        }

        return "";
    }
}