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

public interface IAmortizationsService
{
    Task<AmortizationsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<AmortizationViewModel>> GetAll(string requestId, string correlationId);
    Task<AmortizationViewModel> GetOne(string requestId, string correlationId, decimal rate);
    EditableAmortizationViewModel GetOneForAdd(string requestId, string correlationId);
    Task<EditableAmortizationViewModel> GetOneForEdit(string requestId, string correlationId, decimal rate);
    Task<string> Add(string requestId, string correlationId, string username, EditableAmortizationViewModel model);
    Task<string> Update(string requestId, string correlationId, string username, EditableAmortizationViewModel model);
    Task<string> Delete(string requestId, string correlationId, string username, decimal rate);
    void Sanitize(string requestId, string correlationId, Amortization amortizationConfig);
    string Validate(string requestId, string correlationId, Amortization amortization, IEnumerable<Amortization> amortizations, bool forUpdate = false);
}

public class AmortizationsService : IAmortizationsService
{
    private readonly ILogger<AmortizationsService> _logger;
    private readonly IAmortizationRepository _amortizationConfigRepository;
    private readonly IUsersService _usersService;

    public AmortizationsService(ILogger<AmortizationsService> logger, IAmortizationRepository amortizationConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _amortizationConfigRepository = amortizationConfigRepository ?? throw new ArgumentNullException(nameof(amortizationConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<AmortizationsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var amortizations = await _amortizationConfigRepository.GetAll();
        return new AmortizationsViewModel()
        {
            Amortizations = amortizations.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<AmortizationViewModel>> GetAll(string requestId, string correlationId)
    {
        var amortizations = await _amortizationConfigRepository.GetAll();
        return amortizations.Select(s => s.ToViewModel());
    }

    public async Task<AmortizationViewModel> GetOne(string requestId, string correlationId, decimal rate)
    {
        var amortization = await _amortizationConfigRepository.GetOne(new Amortization() { Rate = rate });
        return amortization.ToViewModel();
    }

    public EditableAmortizationViewModel GetOneForAdd(string requestId, string correlationId)
    {
        var hhPctAmts = new List<AmiHhPctAmt>();
        for (var i = 1; i <= 10; i++)
        {
            hhPctAmts.Add(new AmiHhPctAmt() { Size = i });
        }

        return new EditableAmortizationViewModel()
        {
            Rate = 0m,
            RateInterestOnly = 0m,
            Rate10Year = 0m,
            Rate15Year = 0m,
            Rate20Year = 0m,
            Rate25Year = 0m,
            Rate30Year = 0m,
            Rate40Year = 0m,
            Active = true,
            Mode = "ADD"
        };
    }

    public async Task<EditableAmortizationViewModel> GetOneForEdit(string requestId, string correlationId, decimal rate)
    {
        var amortization = await _amortizationConfigRepository.GetOne(new Amortization() { Rate = rate });
        return amortization.ToEditableViewModel();
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableAmortizationViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Amortization - Invalid Input");
            return "AT000";
        }

        var amortization = new Amortization()
        {
            Rate = model.Rate,
            RateInterestOnly = model.RateInterestOnly,
            Rate10Year = model.Rate10Year,
            Rate15Year = model.Rate15Year,
            Rate20Year = model.Rate20Year,
            Rate25Year = model.Rate25Year,
            Rate30Year = model.Rate30Year,
            Rate40Year = model.Rate40Year,
            Active = true,
            CreatedBy = username
        };
        Sanitize(requestId, correlationId, amortization);

        var amortizations = await _amortizationConfigRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, amortization, amortizations);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Amortization - {amortization.Rate}");
            return validationCode;
        }

        var added = await _amortizationConfigRepository.Add(correlationId, amortization);
        if (!added)
        {
            _logger.LogError($"Failed to add Amortization - {amortization.Rate} - Unknown error");
            return "AT003";
        }

        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableAmortizationViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update Amortization - Invalid Input");
            return "AT000";
        }

        var amortization = new Amortization()
        {
            Rate = model.Rate,
            RateInterestOnly = model.RateInterestOnly,
            Rate10Year = model.Rate10Year,
            Rate15Year = model.Rate15Year,
            Rate20Year = model.Rate20Year,
            Rate25Year = model.Rate25Year,
            Rate30Year = model.Rate30Year,
            Rate40Year = model.Rate40Year,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(requestId, correlationId, amortization);

        var amortizations = await _amortizationConfigRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, amortization, amortizations, true);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for Amortization - {amortization.Rate}");
            return validationCode;
        }

        var updated = await _amortizationConfigRepository.Update(correlationId, amortization);
        if (!updated)
        {
            _logger.LogError($"Failed to update Amortization - {amortization.Rate} - Unknown error");
            return "AT004";
        }

        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, decimal rate)
    {
        if (rate <= 0)
        {
            _logger.LogError($"Unable to delete Amortization - Invalid Input");
            return "AT000";
        }

        var existingAmi = await _amortizationConfigRepository.GetOne(new Amortization() { Rate = rate });
        if (existingAmi == null)
        {
            _logger.LogError($"Unable to find Amortization - {rate}");
            return "AT001";
        }

        existingAmi.ModifiedBy = username;
        var deleted = await _amortizationConfigRepository.Delete(correlationId, existingAmi);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete Amortization - {existingAmi.Rate} - Unknown error");
            return "AT005";
        }

        return "";
    }

    public void Sanitize(string requestId, string correlationId, Amortization amortization)
    {
        if (amortization == null) return;

        amortization.Rate = amortization.Rate < 0 ? 0 : amortization.Rate;
        amortization.RateInterestOnly = amortization.RateInterestOnly < 0 ? 0 : amortization.RateInterestOnly;
        amortization.Rate10Year = amortization.Rate10Year < 0 ? 0 : amortization.Rate10Year;
        amortization.Rate15Year = amortization.Rate15Year < 0 ? 0 : amortization.Rate15Year;
        amortization.Rate20Year = amortization.Rate20Year < 0 ? 0 : amortization.Rate20Year;
        amortization.Rate25Year = amortization.Rate25Year < 0 ? 0 : amortization.Rate25Year;
        amortization.Rate30Year = amortization.Rate30Year < 0 ? 0 : amortization.Rate30Year;
        amortization.Rate40Year = amortization.Rate40Year < 0 ? 0 : amortization.Rate40Year;
    }

    public string Validate(string requestId, string correlationId, Amortization amortization, IEnumerable<Amortization> amortizations, bool forUpdate = false)
    {
        if (amortization == null)
        {
            _logger.LogError($"Unable to validate Amortization - Invalid Input");
            return "AT000";
        }

        Sanitize(requestId, correlationId, amortization);

        if (amortization.Rate <= 0 || amortization.Rate > 100)
        {
            _logger.LogError($"Unable to validate Amortization - Rate must be between 0 and 100");
            return "AT101";
        }

        if (amortization.RateInterestOnly <= 0 || amortization.RateInterestOnly > 100)
        {
            _logger.LogError($"Unable to validate Amortization - Interest Only Rate must be between 0 and 100");
            return "AT102";
        }

        if (amortization.Rate10Year <= 0 || amortization.Rate10Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 10 Year Rate must be between 0 and 100");
            return "AT103";
        }

        if (amortization.Rate15Year <= 0 || amortization.Rate15Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 15 Year Rate must be between 0 and 100");
            return "AT104";
        }

        if (amortization.Rate20Year <= 0 || amortization.Rate20Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 20 Year Rate must be between 0 and 100");
            return "AT105";
        }

        if (amortization.Rate25Year <= 0 || amortization.Rate25Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 25 Year Rate must be between 0 and 100");
            return "AT106";
        }

        if (amortization.Rate30Year <= 0 || amortization.Rate30Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 30 Year Rate must be between 0 and 100");
            return "AT107";
        }

        if (amortization.Rate40Year <= 0 || amortization.Rate40Year > 100)
        {
            _logger.LogError($"Unable to validate Amortization - 40 Year Rate must be between 0 and 100");
            return "AT108";
        }

        if (!forUpdate && (amortizations?.Count() ?? 0) > 0)
        {
            // Duplicate check
            var duplicateAmi = amortizations.FirstOrDefault(f => f.Rate.Equals(amortization.Rate));
            if (duplicateAmi != null)
            {
                _logger.LogError($"Unable to validate Amortization - Duplicate {amortization.Rate}");
                return "AT002";
            }
        }

        return "";
    }
}