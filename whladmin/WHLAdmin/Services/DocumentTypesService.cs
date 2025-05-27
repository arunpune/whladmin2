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

public interface IDocumentTypesService
{
    Task<DocumentTypesViewModel> GetData(string requestId, string correlationId, string userId);
    Task<IEnumerable<DocumentTypeViewModel>> GetAll(string requestId, string correlationId);
    Task<DocumentTypeViewModel> GetOne(string requestId, string correlationId, int documentTypeId);
    EditableDocumentTypeViewModel GetOneForAdd(string requestId, string correlationId);
    Task<EditableDocumentTypeViewModel> GetOneForEdit(string requestId, string correlationId, int documentTypeId);
    Task<string> Add(string requestId, string correlationId, string username, EditableDocumentTypeViewModel model);
    Task<string> Update(string requestId, string correlationId, string username, EditableDocumentTypeViewModel model);
    Task<string> Delete(string requestId, string correlationId, string username, int documentTypeId);
    void Sanitize(DocumentType documentType);
    string Validate(string requestId, string correlationIdDocumentType, DocumentType documentType, IEnumerable<DocumentType> documentTypes);
}

public class DocumentTypesService : IDocumentTypesService
{
    private readonly ILogger<DocumentTypesService> _logger;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IUsersService _usersService;

    public DocumentTypesService(ILogger<DocumentTypesService> logger, IDocumentTypeRepository documentTypeRepository, IUsersService usersService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    public async Task<DocumentTypesViewModel> GetData(string requestId, string correlationId, string userId)
    {
        var userRole = await _usersService.GetUserRole(correlationId, userId);
        var documentTypes = await _documentTypeRepository.GetAll();
        var model = new DocumentTypesViewModel
        {
            DocumentTypes = documentTypes.Select(s => s.ToViewModel()),
            CanEdit = "|SYSADMIN|OPSADMIN|LOTADMIN|".Contains($"|{userRole}|")
        };
        return model;
    }

    public async Task<IEnumerable<DocumentTypeViewModel>> GetAll(string requestId, string correlationId)
    {
        var documentTypes = await _documentTypeRepository.GetAll();
        return documentTypes.Select(s => s.ToViewModel());
    }

    public async Task<DocumentTypeViewModel> GetOne(string requestId, string correlationId, int documentTypeId)
    {
        var documentType = await _documentTypeRepository.GetOne(new DocumentType() { DocumentTypeId = documentTypeId });
        return documentType.ToViewModel();
    }

    public EditableDocumentTypeViewModel GetOneForAdd(string requestId, string correlationId)
    {
        return new EditableDocumentTypeViewModel()
        {
            DocumentTypeId = 0,
            DocumentTypeName = "",
            DocumentTypeDescription = "",
            Active = true
        };
    }

    public async Task<EditableDocumentTypeViewModel> GetOneForEdit(string requestId, string correlationId, int documentTypeId)
    {
        var documentType = await _documentTypeRepository.GetOne(new DocumentType() { DocumentTypeId = documentTypeId });
        return documentType.ToEditableViewModel();
    }

    public async Task<string> Add(string requestId, string correlationId, string username, EditableDocumentTypeViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to add document type Invalid Input");
            return "DT000";
        }

        var documentType = new DocumentType()
        {
            DocumentTypeId = 0,
            Name = model.DocumentTypeName,
            Description = model.DocumentTypeDescription,
            UsageCount = 0,
            Active = true,
            CreatedBy = username
        };
        Sanitize(documentType);

        var documentTypes = await _documentTypeRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, documentType, documentTypes);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for document type {documentType.Name}");
            return validationCode;
        }

        var added = await _documentTypeRepository.Add(correlationId, documentType);
        if (!added)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to add document type {documentType.Name} - Unknown error");
            return "DT003";
        }

        model.DocumentTypeId = documentType.DocumentTypeId;
        return "";
    }

    public async Task<string> Update(string requestId, string correlationId, string username, EditableDocumentTypeViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to update document type Invalid Input");
            return "DT000";
        }

        var documentType = new DocumentType()
        {
            DocumentTypeId = model.DocumentTypeId,
            Name = model.DocumentTypeName,
            Description = model.DocumentTypeDescription,
            Active = model.Active,
            ModifiedBy = username
        };
        Sanitize(documentType);

        var documentTypes = await _documentTypeRepository.GetAll();

        var validationCode = Validate(requestId, correlationId, documentType, documentTypes);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Validation failed for document type {documentType.Name}");
            return validationCode;
        }

        var updated = await _documentTypeRepository.Update(correlationId, documentType);
        if (!updated)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to update document type {documentType.Name} - Unknown error");
            return "DT004";
        }

        return "";
    }

    public async Task<string> Delete(string requestId, string correlationId, string username, int documentTypeId)
    {
        if (documentTypeId <= 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to delete document type Invalid Input");
            return "DT000";
        }

        var existingDocumentType = await _documentTypeRepository.GetOne(new DocumentType() { DocumentTypeId = documentTypeId });
        if (existingDocumentType == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find document type {documentTypeId}");
            return "DT001";
        }

        existingDocumentType.ModifiedBy = username;
        var deleted = await _documentTypeRepository.Delete(correlationId, existingDocumentType);
        if (!deleted)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Failed to delete document type {existingDocumentType.Name} - Unknown error");
            return "DT005";
        }

        return "";
    }

    public void Sanitize(DocumentType documentType)
    {
        if (documentType == null) return;

        documentType.Name = (documentType.Name ?? "").Trim();

        documentType.Description = (documentType.Description ?? "").Trim();
        if (string.IsNullOrEmpty(documentType.Description)) documentType.Description = null;
    }

    public string Validate(string requestId, string correlationId, DocumentType documentType, IEnumerable<DocumentType> documentTypes)
    {
        if (documentType == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate document type Invalid Input");
            return "DT000";
        }

        Sanitize(documentType);

        if (documentType.Name.Length == 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate document type DocumentType Name is required");
            return "DT101";
        }

        if ((documentTypes?.Count() ?? 0) > 0)
        {
            if (documentType.DocumentTypeId > 0)
            {
                // Existence check
                var existingDocumentType = documentTypes.FirstOrDefault(f => f.DocumentTypeId == documentType.DocumentTypeId);
                if (existingDocumentType == null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to find document type {documentType.DocumentTypeId}");
                    return "DT001";
                }

                // Duplicate check
                var duplicateDocumentType = documentTypes.FirstOrDefault(f => f.DocumentTypeId != documentType.DocumentTypeId && f.Name.Equals(documentType.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateDocumentType != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate document type Duplicate {documentType.Name}");
                    return "DT002";
                }
            }
            else
            {
                // Duplicate check
                var duplicateDocumentType = documentTypes.FirstOrDefault(f => f.Name.Equals(documentType.Name, StringComparison.CurrentCultureIgnoreCase));
                if (duplicateDocumentType != null)
                {
                    _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to validate document type Duplicate {documentType.Name}");
                    return "DT002";
                }
            }
        }

        return "";
    }
}