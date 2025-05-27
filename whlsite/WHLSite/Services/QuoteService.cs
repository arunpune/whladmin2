using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IQuoteService
{
    Task<QuoteViewModel> GetQuoteForHomePage(string requestId, string correlationId);
}

public class QuoteService : IQuoteService
{
    private readonly ILogger<QuoteService> _logger;
    private readonly IQuoteRepository _quoteRepository;

    public QuoteService(ILogger<QuoteService> logger, IQuoteRepository quoteRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _quoteRepository = quoteRepository ?? throw new ArgumentNullException(nameof(quoteRepository));
    }

    public async Task<QuoteViewModel> GetQuoteForHomePage(string requestId, string correlationId)
    {
        var quotes = await _quoteRepository.GetAll();
        var quote = quotes.FirstOrDefault(f => f.DisplayOnHomePageInd);
        return quote.ToViewModel();
    }
}