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

public interface INotesService
{
    Task<NoteViewerModel> GetData(string entityType, string entityId);
    Task<IEnumerable<NoteViewModel>> GetAll(string entityType, string entityId);
    EditableNoteViewModel GetOneForAdd(string entityType, string entityId);
    Task<string> Add(string correlationId, string username, EditableNoteViewModel model);
    void Sanitize(Note note);
    string Validate(Note note);
}

public class NotesService : INotesService
{
    private readonly ILogger<NotesService> _logger;
    private readonly INoteRepository _noteRepository;
    private readonly IMetadataService _metadataService;

    public NotesService(ILogger<NotesService> logger, INoteRepository noteRepository, IMetadataService metadataService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _noteRepository = noteRepository ?? throw new ArgumentNullException(nameof(noteRepository));
        _metadataService = metadataService ?? throw new ArgumentNullException(nameof(metadataService));
    }

    public async Task<NoteViewerModel> GetData(string entityTypeCd, string entityId)
    {
        var entries = await _noteRepository.GetAll(entityTypeCd, entityId);

        return new NoteViewerModel()
        {
            EntityTypeCd = entityTypeCd,
            EntityDescription = await _metadataService.GetEntityTypeDescription(entityTypeCd),
            EntityId = entityId,
            Notes = entries.OrderByDescending(o => o.Timestamp).Select(s => s.ToViewModel())
        };
    }

    public async Task<IEnumerable<NoteViewModel>> GetAll(string entityTypeCd, string entityId)
    {
        var entries = await _noteRepository.GetAll(entityTypeCd, entityId);
        return entries.Select(s => new NoteViewModel()
        {
            Id = s.Id,
            EntityDescription = s.EntityDescription,
            EntityId = s.EntityId,
            EntityName = s.EntityName,
            EntityTypeCd = s.EntityTypeCd,
            Note = s.Note,
            Timestamp = s.Timestamp,
            Username = s.Username,
        });
    }

    public EditableNoteViewModel GetOneForAdd(string entityType, string entityId)
    {
        return new EditableNoteViewModel()
        {
            Id = 0,
            EntityTypeCd = entityType,
            EntityId = entityId,
            Note = ""
        };
    }

    public async Task<string> Add(string correlationId, string username, EditableNoteViewModel model)
    {
        if (model == null)
        {
            _logger.LogError($"Unable to add Note - Invalid Input");
            return "NT000";
        }

        var note = new Note()
        {
            Id = 0,
            EntityTypeCd = model.EntityTypeCd,
            EntityId = model.EntityId,
            Note = model.Note,
            Username = username
        };
        Sanitize(note);

        var validationCode = Validate(note);
        if (!string.IsNullOrEmpty(validationCode))
        {
            _logger.LogError($"Validation failed for note - {note.Note}");
            return validationCode;
        }

        var added = await _noteRepository.Add(correlationId, note);
        if (!added)
        {
            _logger.LogError($"Failed to add note - {note.Note} - Unknown error");
            return "NT003";
        }

        return "";
    }

    public void Sanitize(Note note)
    {
        if (note == null) return;

        note.EntityTypeCd = (note.EntityTypeCd ?? "").Trim();
        note.EntityId = (note.EntityId ?? "").Trim();
        note.Note = (note.Note ?? "").Trim();
    }

    public string Validate(Note note)
    {
        if (note == null)
        {
            _logger.LogError($"Unable to validate Note - Invalid Input");
            return "NT000";
        }

        Sanitize(note);

        if (note.EntityTypeCd.Length == 0)
        {
            _logger.LogError($"Unable to validate Note - Entity Type is required");
            return "NT101";
        }

        if (!long.TryParse(note.EntityId, out var id) || id <= 0)
        {
            _logger.LogError($"Unable to validate Note - Entity Identifier is invalid");
            return "NT102";
        }

        if (note.Note.Length <= 0)
        {
            _logger.LogError($"Unable to validate Note - Note is required");
            return "NT103";
        }

        return "";
    }
}