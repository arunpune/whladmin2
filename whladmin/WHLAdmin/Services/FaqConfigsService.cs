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

public interface IFaqConfigsService
{
    Task<FaqConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<FaqConfigViewModel>> GetAll();
    Task<FaqConfigViewModel> GetOne(int id);
    EditableFaqConfigViewModel GetOneForAdd();
    Task<EditableFaqConfigViewModel> GetOneForEdit(int id);
    Task<string> Add(string correlationId, string username, EditableFaqConfigViewModel model);
    Task<string> Update(string correlationId, string username, EditableFaqConfigViewModel model);
    Task<string> Delete(string correlationId, string username, int id);
    void Sanitize(FaqConfig faqConfig);
    string Validate(FaqConfig faq, IEnumerable<FaqConfig> faqs);
}

public class FaqConfigsService : IFaqConfigsService
{
    private readonly ILogger<FaqConfigsService> _logger;
    private readonly IFaqConfigRepository _faqConfigRepository;
    private readonly IUsersService _usersService;

    public FaqConfigsService(ILogger<FaqConfigsService> logger, IFaqConfigRepository faqConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _faqConfigRepository = faqConfigRepository ?? throw new ArgumentNullException(nameof(faqConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<FaqConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var faqs = await _faqConfigRepository.GetAll();
        return new FaqConfigsViewModel()
        {
            Faqs = faqs.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<FaqConfigViewModel>> GetAll()
    {
        var faqs = await _faqConfigRepository.GetAll();
        return faqs.Select(s => s.ToViewModel());
    }

    public async Task<FaqConfigViewModel> GetOne(int id)
    {
        var faq = await _faqConfigRepository.GetOne(new FaqConfig() { FaqId = id });
        return faq.ToViewModel();
    }

    public EditableFaqConfigViewModel GetOneForAdd()
    {
        return new EditableFaqConfigViewModel()
        {
            FaqId = 0,
            CategoryName = "General",
            Title = "",
            Text = "",
            Url = "",
            Url1 = "",
            Url2 = "",
            Url3 = "",
            Url4 = "",
            Url5 = "",
            Url6 = "",
            Url7 = "",
            Url8 = "",
            Url9 = "",
            DisplayOrder = 0,
            Active = true
        };
    }

    public async Task<EditableFaqConfigViewModel> GetOneForEdit(int id)
    {
        var faq = await _faqConfigRepository.GetOne(new FaqConfig() { FaqId = id });
        return faq.ToEditableViewModel();
    }

    public async Task<string> Add(string correlationId, string username, EditableFaqConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add FAQ - Invalid Input");
            return "F000";
        }

        var faq = new FaqConfig()
        {
            FaqId = 0,
            CategoryName = model.CategoryName,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            Url1 = model.Url1,
            Url2 = model.Url2,
            Url3 = model.Url3,
            Url4 = model.Url4,
            Url5 = model.Url5,
            Url6 = model.Url6,
            Url7 = model.Url7,
            Url8 = model.Url8,
            Url9 = model.Url9,
            DisplayOrder = model.DisplayOrder,
            Active = true,
            CreatedBy = username
        };
        Sanitize(faq);

        var faqs = await _faqConfigRepository.GetAll();

        var validationCode = Validate(faq, faqs);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for FAQ - {faq.Title}");
            return validationCode;
        }

        var added = await _faqConfigRepository.Add(correlationId, faq);
        if (!added)
        {
            _logger.LogError($"Failed to add FAQ - {faq.Title} - Unknown error");
            return "F003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableFaqConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add FAQ - Invalid Input");
            return "F000";
        }

        var faq = new FaqConfig()
        {
            FaqId = model.FaqId,
            CategoryName = model.CategoryName,
            Title = model.Title,
            Text = model.Text,
            Url = model.Url,
            Url1 = model.Url1,
            Url2 = model.Url2,
            Url3 = model.Url3,
            Url4 = model.Url4,
            Url5 = model.Url5,
            Url6 = model.Url6,
            Url7 = model.Url7,
            Url8 = model.Url8,
            Url9 = model.Url9,
            DisplayOrder = model.DisplayOrder,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(faq);

        var faqs = await _faqConfigRepository.GetAll();

        var validationCode = Validate(faq, faqs);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for FAQ - {faq.Title}");
            return validationCode;
        }

        var updated = await _faqConfigRepository.Update(correlationId, faq);
        if (!updated)
        {
            _logger.LogError($"Failed to update FAQ - {faq.Title} - Unknown error");
            return "F004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete FAQ - Invalid Input");
            return "F000";
        }

        var existingFaq = await _faqConfigRepository.GetOne(new FaqConfig() { FaqId = id });
        if (existingFaq == null)
        {
            _logger.LogError($"Unable to find FAQ - {id}");
            return "F001";
        }

        existingFaq.ModifiedBy = username;
        var deleted = await _faqConfigRepository.Delete(correlationId, existingFaq);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete FAQ - {existingFaq.Title} - Unknown error");
            return "F005";
        }

        return "";
    }

    public void Sanitize(FaqConfig faq)
    {
        if (faq == null) return;

        faq.CategoryName = (faq.CategoryName ?? "").Trim();
        if (string.IsNullOrEmpty(faq.CategoryName)) faq.CategoryName = "General";

        faq.Title = (faq.Title ?? "").Trim();

        faq.Text = (faq.Text ?? "").Trim();

        faq.Url = (faq.Url ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url)) faq.Url = null;

        faq.Url1 = (faq.Url1 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url1)) faq.Url1 = null;

        faq.Url2 = (faq.Url2 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url2)) faq.Url2 = null;

        faq.Url3 = (faq.Url3 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url3)) faq.Url3 = null;

        faq.Url4 = (faq.Url4 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url4)) faq.Url4 = null;

        faq.Url5 = (faq.Url5 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url5)) faq.Url5 = null;

        faq.Url6 = (faq.Url6 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url6)) faq.Url6 = null;

        faq.Url7 = (faq.Url7 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url7)) faq.Url7 = null;

        faq.Url8 = (faq.Url8 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url8)) faq.Url8 = null;

        faq.Url9 = (faq.Url9 ?? "").Trim();
        if (string.IsNullOrEmpty(faq.Url9)) faq.Url9 = null;

        faq.DisplayOrder = faq.DisplayOrder < 0 ? 0 : faq.DisplayOrder;
    }

    public string Validate(FaqConfig faq, IEnumerable<FaqConfig> faqs)
    {
        if (faq == null)
        {
            _logger.LogError($"Unable to validate FAQ - Invalid Input");
            return "F000";
        }

        Sanitize(faq);

        if (faq.CategoryName.Length == 0)
        {
            _logger.LogError($"Unable to validate FAQ - Category Name is required");
            return "F101";
        }

        if (faq.Title.Length == 0)
        {
            _logger.LogError($"Unable to validate FAQ - Title is required");
            return "F102";
        }

        if (faq.Text.Length == 0)
        {
            _logger.LogError($"Unable to validate FAQ - Text is required");
            return "F103";
        }

        if ((faqs?.Count() ?? 0) > 0)
        {
            if (faq.FaqId > 0)
            {
                // Existence check
                var existingFaq = faqs.FirstOrDefault(f => f.FaqId == faq.FaqId);
                if (existingFaq == null)
                {
                    _logger.LogError($"Unable to find FAQ - {faq.FaqId}");
                    return "F001";
                }

                // Duplicate check
                var duplicateFaq = faqs.FirstOrDefault(f => f.FaqId != faq.FaqId && f.Title.Equals(faq.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateFaq != null)
                {
                    _logger.LogError($"Unable to validate FAQ - Duplicate {faq.Title}");
                    return "F002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateFaq = faqs.FirstOrDefault(f => f.Title.Equals(faq.Title, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateFaq != null)
                {
                    _logger.LogError($"Unable to validate FAQ - Duplicate {faq.Title}");
                    return "F002";
                }
            }
        }

        return "";
    }
}