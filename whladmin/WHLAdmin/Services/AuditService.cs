using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IAuditService
{
    Task<AuditViewerModel> GetData(string entityType, string entityId);
    Task<IEnumerable<AuditViewModel>> GetAll(string entityType, string entityId);
}

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;
    private readonly IAuditRepository _auditRepository;
    private readonly IMetadataService _metadataService;

    public AuditService(ILogger<AuditService> logger, IAuditRepository auditRepository, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _auditRepository = auditRepository ?? throw new ArgumentNullException(nameof(auditRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    public async Task<AuditViewerModel> GetData(string entityTypeCd, string entityId)
    {
        var entries = await _auditRepository.GetAll(entityTypeCd, entityId);

        return new AuditViewerModel()
        {
            EntityTypeCd = entityTypeCd,
            EntityDescription = await _metadataService.GetEntityTypeDescription(entityTypeCd),
            EntityId = entityId,
            Entries = entries.OrderByDescending(o => o.Timestamp).Select(s => s.ToViewModel())
        };
    }

    public async Task<IEnumerable<AuditViewModel>> GetAll(string entityTypeCd, string entityId)
    {
        var entries = await _auditRepository.GetAll(entityTypeCd, entityId);
        return entries.Select(s => s.ToViewModel());
    }
}