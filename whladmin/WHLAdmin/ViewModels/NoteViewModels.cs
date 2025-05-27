using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class NoteViewModel : Note
{
}

[ExcludeFromCodeCoverage]
public class EditableNoteViewModel
{
    public int Id { get; set; }

    public string EntityTypeCd { get; set; }

    public string EntityId { get; set; }

    [Display(Name = "Note")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Note is required")]
    public string Note { get; set; }
}

[ExcludeFromCodeCoverage]
public class NoteViewerModel : Note
{
    public IEnumerable<NoteViewModel> Notes { get; set;}
}