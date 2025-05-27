using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IFaqService
{
    Task<FaqsViewModel> GetData(string requestId, string correlationId);
}

public class FaqService : IFaqService
{
    private readonly ILogger<FaqService> _logger;
    private readonly IFaqRepository _faqRepository;

    public FaqService(ILogger<FaqService> logger, IFaqRepository faqRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _faqRepository = faqRepository ?? throw new ArgumentNullException(nameof(faqRepository));
    }

    public async Task<FaqsViewModel> GetData(string requestId, string correlationId)
    {
        var model = new FaqsViewModel()
        {
            Categories = [],
            Faqs = []
        };

        var faqs = await _faqRepository.GetAll();
        if ((faqs?.Count() ?? 0) > 0)
        {
            var faqCategories = faqs.OrderBy(o => o.DisplayOrder).Select(s => s.CategoryName).Distinct();
            int categoryCtr = 1;
            foreach (var category in faqCategories)
            {
                model.Categories.Add(categoryCtr, category);
                categoryCtr++;
            }

            model.Faqs = faqs.Select(s => s.ToViewModel());
        }
        return model;
    }
}