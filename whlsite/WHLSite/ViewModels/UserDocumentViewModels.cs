using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Http;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class UserDocumentViewModel : UserDocument
{
}

[ExcludeFromCodeCoverage]
public class UserDocumentsViewModel
{
    public IEnumerable<UserDocumentViewModel> Documents { get; set; }
    public int Count { get { return Documents?.Count() ?? 0; } }
}

[ExcludeFromCodeCoverage]
public class EditableUserDocumentViewModel : ErrorViewModel
{
    public string Username { get; set; }
    public long DocId { get; set; }
    public byte[] DocContents { get; set; }

    [Display(Name = "Document Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Document type is required")]
    public string DocTypeCd { get; set; }

    [Display(Name = "Document Name")]
    public string DocName { get; set; }

    [Display(Name = "File Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "File is required")]
    public string FileName { get; set; }

    [Display(Name = "File Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "File is required")]
    public IFormFile File { get; set; }

    public Dictionary<string, string> DocumentTypes { get; set; }
}