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

public interface IAmiConfigsService
{
    Task<AmiConfigsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<AmiConfigViewModel>> GetAll();
    Task<AmiConfigViewModel> GetOne(int effectiveDate);
    EditableAmiConfigViewModel GetOneForAdd();
    Task<EditableAmiConfigViewModel> GetOneForEdit(int effectiveDate);
    Task<string> Add(string correlationId, string username, EditableAmiConfigViewModel model);
    Task<string> Update(string correlationId, string username, EditableAmiConfigViewModel model);
    Task<string> Delete(string correlationId, string username, int id);
    void Sanitize(AmiConfig amiConfig);
    string Validate(AmiConfig ami, IEnumerable<AmiConfig> amis, bool forUpdate = false);
}

public class AmiConfigsService : IAmiConfigsService
{
    private readonly ILogger<AmiConfigsService> _logger;
    private readonly IAmiConfigRepository _amiConfigRepository;
    private readonly IUsersService _usersService;

    public AmiConfigsService(ILogger<AmiConfigsService> logger, IAmiConfigRepository amiConfigRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _amiConfigRepository = amiConfigRepository ?? throw new ArgumentNullException(nameof(amiConfigRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<AmiConfigsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var amis = await _amiConfigRepository.GetAll();
        return new AmiConfigsViewModel()
        {
            Amis = amis.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
    }

    public async Task<IEnumerable<AmiConfigViewModel>> GetAll()
    {
        var amis = await _amiConfigRepository.GetAll();
        return amis.Select(s => s.ToViewModel());
    }

    public async Task<AmiConfigViewModel> GetOne(int effectiveDate)
    {
        var ami = await _amiConfigRepository.GetOne(new AmiConfig() { EffectiveDate = effectiveDate });
        return ami.ToViewModel();
    }

    public EditableAmiConfigViewModel GetOneForAdd()
    {
        var hhPctAmts = new List<AmiHhPctAmt>();
        for (var i = 1; i <= 10; i++)
        {
            hhPctAmts.Add(new AmiHhPctAmt() { Size = i });
        }

        return new EditableAmiConfigViewModel()
        {
            EffectiveDate = DateTime.Now.Date.ToString("yyyy-MM-dd"),
            EffectiveYear = DateTime.Now.Year,
            IncomeAmt = 0,
            HhPctAmts = hhPctAmts,
            Active = true,
            Mode = "ADD"
        };
    }

    public async Task<EditableAmiConfigViewModel> GetOneForEdit(int effectiveDate)
    {
        var ami = await _amiConfigRepository.GetOne(new AmiConfig() { EffectiveDate = effectiveDate });
        return ami.ToEditableViewModel();
    }

    public async Task<string> Add(string correlationId, string username, EditableAmiConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add AMI - Invalid Input");
            return "AM000";
        }

        int.TryParse(model.EffectiveDate?.Replace("-", ""), out var effectiveDate);

        model.HhPctAmts ??= [];
        if (model.HhPctAmts.Count < 10)
        {
            for (var ctr = model.HhPctAmts.Count - 1; ctr < 10; ctr++)
            {
                model.HhPctAmts.Add(new ());
            }
        }

        var ami = new AmiConfig()
        {
            EffectiveDate = effectiveDate,
            EffectiveYear = model.EffectiveYear,
            IncomeAmt = model.IncomeAmt,
            Hh1 = model.HhPctAmts[0].Pct,
            Hh2 = model.HhPctAmts[1].Pct,
            Hh3 = model.HhPctAmts[2].Pct,
            Hh4 = model.HhPctAmts[3].Pct,
            Hh5 = model.HhPctAmts[4].Pct,
            Hh6 = model.HhPctAmts[5].Pct,
            Hh7 = model.HhPctAmts[6].Pct,
            Hh8 = model.HhPctAmts[7].Pct,
            Hh9 = model.HhPctAmts[8].Pct,
            Hh10 = model.HhPctAmts[9].Pct,
            Active = true,
            CreatedBy = username
        };
        Sanitize(ami);

        var amis = await _amiConfigRepository.GetAll();

        var validationCode = Validate(ami, amis);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for AMI - {ami.EffectiveDate}");
            return validationCode;
        }

        var added = await _amiConfigRepository.Add(correlationId, ami);
        if (!added)
        {
            _logger.LogError($"Failed to add AMI - {ami.EffectiveDate} - Unknown error");
            return "AM003";
        }

        return "";
    }

    public async Task<string> Update(string correlationId, string username, EditableAmiConfigViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to update AMI - Invalid Input");
            return "AM000";
        }

        int.TryParse(model.EffectiveDate?.Replace("-", ""), out var effectiveDate);

        model.HhPctAmts ??= [];
        if (model.HhPctAmts.Count < 10)
        {
            for (var ctr = model.HhPctAmts.Count - 1; ctr < 10; ctr++)
            {
                model.HhPctAmts.Add(new ());
            }
        }

        var ami = new AmiConfig()
        {
            EffectiveDate = effectiveDate,
            EffectiveYear = model.EffectiveYear,
            IncomeAmt = model.IncomeAmt,
            Hh1 = model.HhPctAmts[0].Pct,
            Hh2 = model.HhPctAmts[1].Pct,
            Hh3 = model.HhPctAmts[2].Pct,
            Hh4 = model.HhPctAmts[3].Pct,
            Hh5 = model.HhPctAmts[4].Pct,
            Hh6 = model.HhPctAmts[5].Pct,
            Hh7 = model.HhPctAmts[6].Pct,
            Hh8 = model.HhPctAmts[7].Pct,
            Hh9 = model.HhPctAmts[8].Pct,
            Hh10 = model.HhPctAmts[9].Pct,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(ami);

        var amis = await _amiConfigRepository.GetAll();

        var validationCode = Validate(ami, amis, true);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for AMI - {ami.EffectiveDate}");
            return validationCode;
        }

        var updated = await _amiConfigRepository.Update(correlationId, ami);
        if (!updated)
        {
            _logger.LogError($"Failed to update AMI - {ami.EffectiveDate} - Unknown error");
            return "AM004";
        }

        return "";
    }

    public async Task<string> Delete(string correlationId, string username, int id)
    {
        if (id <= 0)
        {
            _logger.LogError($"Unable to delete AMI - Invalid Input");
            return "AM000";
        }

        var existingAmi = await _amiConfigRepository.GetOne(new AmiConfig() { EffectiveDate = id });
        if (existingAmi == null)
        {
            _logger.LogError($"Unable to find AMI - {id}");
            return "AM001";
        }

        existingAmi.ModifiedBy = username;
        var deleted = await _amiConfigRepository.Delete(correlationId, existingAmi);
        if (!deleted)
        {
            _logger.LogError($"Failed to delete AMI - {existingAmi.EffectiveDate} - Unknown error");
            return "AM005";
        }

        return "";
    }

    public void Sanitize(AmiConfig ami)
    {
        if (ami == null) return;

        ami.EffectiveDate = ami.EffectiveDate < 0 ? 0 : ami.EffectiveDate;
        ami.EffectiveYear = ami.EffectiveYear < 0 ? 0 : ami.EffectiveYear;
        ami.IncomeAmt = ami.IncomeAmt < 0 ? 0 : ami.IncomeAmt;
        ami.Hh1 = ami.Hh1 < 0 ? 0 : ami.Hh1;
        ami.Hh2 = ami.Hh2 < 0 ? 0 : ami.Hh2;
        ami.Hh3 = ami.Hh3 < 0 ? 0 : ami.Hh3;
        ami.Hh4 = ami.Hh4 < 0 ? 0 : ami.Hh4;
        ami.Hh5 = ami.Hh5 < 0 ? 0 : ami.Hh5;
        ami.Hh6 = ami.Hh6 < 0 ? 0 : ami.Hh6;
        ami.Hh7 = ami.Hh7 < 0 ? 0 : ami.Hh7;
        ami.Hh8 = ami.Hh8 < 0 ? 0 : ami.Hh8;
        ami.Hh9 = ami.Hh9 < 0 ? 0 : ami.Hh9;
        ami.Hh10 = ami.Hh10 < 0 ? 0 : ami.Hh10;
    }

    public string Validate(AmiConfig ami, IEnumerable<AmiConfig> amis, bool forUpdate = false)
    {
        if (ami == null)
        {
            _logger.LogError($"Unable to validate AMI - Invalid Input");
            return "AM000";
        }

        Sanitize(ami);

        if (ami.EffectiveDate < 20000101)
        {
            _logger.LogError($"Unable to validate AMI - Effective Date is required");
            return "AM101";
        }

        if (ami.EffectiveYear < 2000)
        {
            _logger.LogError($"Unable to validate AMI - Effective Year is required");
            return "AM102";
        }

        if (ami.IncomeAmt <= 0)
        {
            _logger.LogError($"Unable to validate AMI - 4 Person Household AMI is required");
            return "AM103";
        }

        if (ami.Hh1 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH1 is required");
            return "AM104";
        }

        if (ami.Hh2 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH2 is required");
            return "AM105";
        }

        if (ami.Hh3 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH3 is required");
            return "AM106";
        }

        if (ami.Hh4 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH4 is required");
            return "AM107";
        }

        if (ami.Hh5 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH5 is required");
            return "AM108";
        }

        if (ami.Hh6 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH6 is required");
            return "AM109";
        }

        if (ami.Hh7 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH7 is required");
            return "AM110";
        }

        if (ami.Hh8 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH8 is required");
            return "AM111";
        }

        if (ami.Hh9 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH9 is required");
            return "AM112";
        }

        if (ami.Hh10 <= 0)
        {
            _logger.LogError($"Unable to validate AMI - HH10 is required");
            return "AM113";
        }

        if (!forUpdate && (amis?.Count() ?? 0) > 0)
        {
            // Duplicate check
            var duplicateAmi = amis.FirstOrDefault(f => f.EffectiveDate.Equals(ami.EffectiveDate));
            if (duplicateAmi != null)
            {
                _logger.LogError($"Unable to validate AMI - Duplicate {ami.EffectiveDate}");
                return "AM002";
            }
        }

        return "";
    }
}