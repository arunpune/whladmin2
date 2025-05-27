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

public interface IFundingSourcesService
{
    Task<FundingSourcesViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<FundingSourceViewModel>> GetAll(string requestId, string correlationId);
    Task<FundingSourceViewModel> GetOne(string requestId, string correlationId, int fundingSourceId);
    EditableFundingSourceViewModel GetOneForAdd(string requestId, string correlationId);
    Task<EditableFundingSourceViewModel> GetOneForEdit(string requestId, string correlationId, int fundingSourceId);
    Task<string> Add(string requestId, string correlationId, string username, EditableFundingSourceViewModel model);
    Task<string> Update(string requestId, string correlationId, string username, EditableFundingSourceViewModel model);
    Task<string> Delete(string requestId, string correlationId, string username, int fundingSourceId);
    void Sanitize(FundingSource fundingSource);
    string Validate(string requestId, string correlationIdFundingSource, FundingSource fundingSource, IEnumerable<FundingSource> fundingSources);
}

public class FundingSourcesService : IFundingSourcesService
{
    private readonly ILogger<FundingSourcesService> _logger;
    private readonly IFundingSourceRepository _fundingSourceRepository;
    private readonly IUsersService _usersService;

    public FundingSourcesService(ILogger<FundingSourcesService> logger, IFundingSourceRepository fundingSourceRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fundingSourceRepository = fundingSourceRepository ?? throw new ArgumentNullException(nameof(fundingSourceRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<FundingSourcesViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var fundingSources = await _fundingSourceRepository.GetAll();
        var model = new FundingSourcesViewModel
        {
            FundingSources = fundingSources.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
        return model;
    }

    public async Task<IEnumerable<FundingSourceViewModel>> GetAll(string requestId, string correlationId)
    {
        var fundingSources = await _fundingSourceRepository.GetAll();
        return fundingSources.Select(s => s.ToViewModel());
    }

    public async Task<FundingSourceViewModel> GetOne(string requestId, string correlationId, int fundingSourceId)
    {
        var fundingSource = await _fundingSourceRepository.GetOne(new FundingSource() { FundingSourceId = fundingSourceId });
        return fundingSource.ToViewModel();
    }

    public EditableFundingSourceViewModel GetOneForAdd(string requestId, string correlationId)
    {
        return new EditableFundingSourceViewModel()
        {
            FundingSourceId = 0,
            FundingSourceName = "",
            FundingSourceDescription = "",
            Active = true
        };
    }

    public async Task<EditableFundingSourceViewModel> GetOneForEdit(string requestId, string correlationId, int fundingSourceId)
    {
        var fundingSource = await _fundingSourceRepository.GetOne(new FundingSource() { FundingSourceId = fundingSourceId });
        return fundingSource.ToEditableViewModel();
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableFundingSourceViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add funding source Invalid Input");
            return "FS000";
        }

        var fundingSource = new FundingSource()
        {
            FundingSourceId = 0,
            Name = model.FundingSourceName,
            Description = model.FundingSourceDescription,
            UsageCount = 0,
            Active = true,
            CreatedBy = username
        };
        Sanitize(fundingSource);

        var fundingSources = await _fundingSourceRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, fundingSource, fundingSources);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for funding source {fundingSource.Name}");
            return validationCode;
        }

        var added = await _fundingSourceRepository.Add(correlationId, fundingSource);
        if (!added)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to add funding source {fundingSource.Name} - Unknown error");
            return "FS003";
        }

        model.FundingSourceId = fundingSource.FundingSourceId;
        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableFundingSourceViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to update funding source Invalid Input");
            return "FS000";
        }

        var fundingSource = new FundingSource()
        {
            FundingSourceId = model.FundingSourceId,
            Name = model.FundingSourceName,
            Description = model.FundingSourceDescription,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(fundingSource);

        var fundingSources = await _fundingSourceRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, fundingSource, fundingSources);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for funding source {fundingSource.Name}");
            return validationCode;
        }

        var updated = await _fundingSourceRepository.Update(correlationId, fundingSource);
        if (!updated)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to update funding source {fundingSource.Name} - Unknown error");
            return "FS004";
        }

        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, int fundingSourceId)
    {
        if (fundingSourceId <= 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to delete funding source Invalid Input");
            return "FS000";
        }

        var existingFundingSource = await _fundingSourceRepository.GetOne(new FundingSource() { FundingSourceId = fundingSourceId });
        if (existingFundingSource == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find funding source {fundingSourceId}");
            return "FS001";
        }

        existingFundingSource.ModifiedBy = username;
        var deleted = await _fundingSourceRepository.Delete(correlationId, existingFundingSource);
        if (!deleted)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to delete funding source {existingFundingSource.Name} - Unknown error");
            return "FS005";
        }

        return "";
    }

    public void Sanitize(FundingSource fundingSource)
    {
        if (fundingSource == null) return;

        fundingSource.Name = (fundingSource.Name ?? "").Trim();

        fundingSource.Description = (fundingSource.Description ?? "").Trim();
        if (string.IsNullOrEmpty(fundingSource.Description)) fundingSource.Description = null;
    }

    public string Validate(string requestId, string correlationId, FundingSource fundingSource, IEnumerable<FundingSource> fundingSources)
    {
        if (fundingSource == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate funding source Invalid Input");
            return "FS000";
        }

        Sanitize(fundingSource);

        if (fundingSource.Name.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate funding source FundingSource Name is required");
            return "FS101";
        }

        if ((fundingSources?.Count() ?? 0) > 0)
        {
            if (fundingSource.FundingSourceId > 0)
            {
                // Existence check
                var existingFundingSource = fundingSources.FirstOrDefault(f => f.FundingSourceId == fundingSource.FundingSourceId);
                if (existingFundingSource == null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find funding source {fundingSource.FundingSourceId}");
                    return "FS001";
                }

                // Duplicate check
                var duplicateFundingSource = fundingSources.FirstOrDefault(f => f.FundingSourceId != fundingSource.FundingSourceId && f.Name.Equals(fundingSource.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateFundingSource != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate funding source Duplicate {fundingSource.Name}");
                    return "FS002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateFundingSource = fundingSources.FirstOrDefault(f => f.Name.Equals(fundingSource.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateFundingSource != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate funding source Duplicate {fundingSource.Name}");
                    return "FS002";
                }
            }
        }

        return "";
    }
}