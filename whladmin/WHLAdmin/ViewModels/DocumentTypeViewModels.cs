using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class DocumentTypeViewModel : DocumentType
{
}

[ExcludeFromCodeCoverage]
public class EditableDocumentTypeViewModel
{
    public int DocumentTypeId { get; set; }

    [Display(Name = "Document Type Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Document Type Name is required")]
    [MaxLength(100)]
    public string DocumentTypeName { get; set; }

    [Display(Name = "Document Type Description")]
    [MaxLength(1000)]
    public string DocumentTypeDescription { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class DocumentTypesViewModel
{
    public IEnumerable<DocumentTypeViewModel> DocumentTypes { get; set; }
    public bool CanEdit { get; set; }
}
