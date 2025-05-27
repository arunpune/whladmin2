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

public interface IMarketingAgentsService
{
    Task<MarketingAgentsViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<MarketingAgentViewModel>> GetAll(string requestId, string correlationId);
    Task<MarketingAgentViewModel> GetOne(string requestId, string correlationId, int agentId);
    EditableMarketingAgentViewModel GetOneForAdd(string requestId, string correlationId);
    Task<EditableMarketingAgentViewModel> GetOneForEdit(string requestId, string correlationId, int agentId);
    Task<string> Add(string requestId, string correlationId, string username, EditableMarketingAgentViewModel model);
    Task<string> Update(string requestId, string correlationId, string username, EditableMarketingAgentViewModel model);
    Task<string> Delete(string requestId, string correlationId, string username, int agentId);
    void Sanitize(MarketingAgent agent);
    string Validate(string requestId, string correlationIdMarketingAgent, MarketingAgent agent, IEnumerable<MarketingAgent> agents);
}

public class MarketingAgentsService : IMarketingAgentsService
{
    private readonly ILogger<MarketingAgentsService> _logger;
    private readonly IMarketingAgentRepository _agentRepository;
    private readonly IUsersService _usersService;

    public MarketingAgentsService(ILogger<MarketingAgentsService> logger, IMarketingAgentRepository agentRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<MarketingAgentsViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var agents = await _agentRepository.GetAll();
        var model = new MarketingAgentsViewModel
        {
            Agents = agents.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
        return model;
    }

    public async Task<IEnumerable<MarketingAgentViewModel>> GetAll(string requestId, string correlationId)
    {
        var agents = await _agentRepository.GetAll();
        return agents.Select(s => s.ToViewModel());
    }

    public async Task<MarketingAgentViewModel> GetOne(string requestId, string correlationId, int agentId)
    {
        var agent = await _agentRepository.GetOne(new MarketingAgent() { AgentId = agentId });
        return agent.ToViewModel();
    }

    public EditableMarketingAgentViewModel GetOneForAdd(string requestId, string correlationId)
    {
        return new EditableMarketingAgentViewModel()
        {
            AgentId = 0,
            AgentName = "",
            ContactName = "",
            PhoneNumber = "",
            EmailAddress = "",
            Active = true
        };
    }

    public async Task<EditableMarketingAgentViewModel> GetOneForEdit(string requestId, string correlationId, int agentId)
    {
        var agent = await _agentRepository.GetOne(new MarketingAgent() { AgentId = agentId });
        return agent.ToEditableViewModel();
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableMarketingAgentViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add marketing agent - Invalid Input");
            return "MA000";
        }

        var agent = new MarketingAgent()
        {
            AgentId = 0,
            Name = model.AgentName,
            ContactName = model.ContactName,
            PhoneNumber = model.PhoneNumber,
            EmailAddress = model.EmailAddress,
            UsageCount = 0,
            Active = true,
            CreatedBy = username
        };
        Sanitize(agent);

        var agents = await _agentRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, agent, agents);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for marketing agent - {agent.Name}");
            return validationCode;
        }

        var added = await _agentRepository.Add(correlationId, agent);
        if (!added)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to add marketing agent - {agent.Name} - Unknown error");
            return "MA003";
        }

        model.AgentId = agent.AgentId;
        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableMarketingAgentViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to update marketing agent - Invalid Input");
            return "MA000";
        }

        var agent = new MarketingAgent()
        {
            AgentId = model.AgentId,
            Name = model.AgentName,
            ContactName = model.ContactName,
            PhoneNumber = model.PhoneNumber,
            EmailAddress = model.EmailAddress,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(agent);

        var agents = await _agentRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, agent, agents);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for marketing agent - {agent.Name}");
            return validationCode;
        }

        var updated = await _agentRepository.Update(correlationId, agent);
        if (!updated)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to update marketing agent - {agent.Name} - Unknown error");
            return "MA004";
        }

        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, int agentId)
    {
        if (agentId <= 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to delete marketing agent - Invalid Input");
            return "MA000";
        }

        var existingMarketingAgent = await _agentRepository.GetOne(new MarketingAgent() { AgentId = agentId });
        if (existingMarketingAgent == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find marketing agent - {agentId}");
            return "MA001";
        }

        existingMarketingAgent.ModifiedBy = username;
        var deleted = await _agentRepository.Delete(correlationId, existingMarketingAgent);
        if (!deleted)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to delete marketing agent - {existingMarketingAgent.Name} - Unknown error");
            return "MA005";
        }

        return "";
    }

    public void Sanitize(MarketingAgent agent)
    {
        if (agent == null) return;

        agent.Name = (agent.Name ?? "").Trim();

        agent.ContactName = (agent.ContactName ?? "").Trim();
        if (string.IsNullOrEmpty(agent.ContactName)) agent.ContactName = null;

        agent.PhoneNumber = (agent.PhoneNumber ?? "").Trim();
        if (string.IsNullOrEmpty(agent.PhoneNumber)) agent.PhoneNumber = null;

        agent.EmailAddress = (agent.EmailAddress ?? "").Trim();
        if (string.IsNullOrEmpty(agent.EmailAddress)) agent.EmailAddress = null;
    }

    public string Validate(string requestId, string correlationId, MarketingAgent agent, IEnumerable<MarketingAgent> agents)
    {
        if (agent == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate marketing agent - Invalid Input");
            return "MA000";
        }

        Sanitize(agent);

        if (agent.Name.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate marketing agent - Marketing Agent Name is required");
            return "MA101";
        }

        if ((agents?.Count() ?? 0) > 0)
        {
            if (agent.AgentId > 0)
            {
                // Existence check
                var existingMarketingAgent = agents.FirstOrDefault(f => f.AgentId == agent.AgentId);
                if (existingMarketingAgent == null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find marketing agent - {agent.AgentId}");
                    return "MA001";
                }

                // Duplicate check
                var duplicateMarketingAgent = agents.FirstOrDefault(f => f.AgentId != agent.AgentId && f.Name.Equals(agent.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateMarketingAgent != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate marketing agent - Duplicate {agent.Name}");
                    return "MA002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateMarketingAgent = agents.FirstOrDefault(f => f.Name.Equals(agent.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateMarketingAgent != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate marketing agent - Duplicate {agent.Name}");
                    return "MA002";
                }
            }
        }

        return "";
    }
}