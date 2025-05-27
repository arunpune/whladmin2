using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using WHLSite.Common.Models;

namespace WHLSite.ViewModels;

[ExcludeFromCodeCoverage]
public class DocumentTypeViewModel : DocumentType
{
}

public class DocumentTypesViewModel
{
    public IEnumerable<DocumentTypeViewModel> DocumentTypes { get; set; }
}
