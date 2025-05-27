using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Extensions;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IAmortizationsService
{
    Task<IEnumerable<AmortizationViewModel>> GetAll(string requestId, string correlationId);
    Task<AmortizationViewModel> GetOne(string requestId, string correlationId, decimal rate);
}

public class AmortizationsService : IAmortizationsService
{
    private readonly ILogger<AmortizationsService> _logger;
    private readonly IAmortizationRepository _amortizationConfigRepository;

    public AmortizationsService(ILogger<AmortizationsService> logger, IAmortizationRepository amortizationConfigRepository)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _amortizationConfigRepository = amortizationConfigRepository ?? throw new ArgumentNullException(nameof(amortizationConfigRepository));
    }

    public async Task<IEnumerable<AmortizationViewModel>> GetAll(string requestId, string correlationId)
    {
        var amortizations = await _amortizationConfigRepository.GetAll();
        return amortizations.Where(s => s.Active).Select(s => s.ToViewModel());
    }

    public async Task<AmortizationViewModel> GetOne(string requestId, string correlationId, decimal rate)
    {
        var amortization = await _amortizationConfigRepository.GetOne(new Amortization() { Rate = rate });
        return amortization.ToViewModel();
    }
}