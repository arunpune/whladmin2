using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class QuestionConfigViewModel : QuestionConfig
{
}

[ExcludeFromCodeCoverage]
public class EditableQuestionConfigViewModel
{
    public int QuestionId { get; set; }

    [Display(Name = "Category")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Category is required")]
    public string CategoryCd { get; set; }

    [Display(Name = "Question")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Question is required")]
    [MaxLength(500)]
    public string Title { get; set; }

    [Display(Name = "Answer Type")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Answer Type is required")]
    public string AnswerTypeCd { get; set; }

    [Display(Name = "Minimum Length")]
    public int MinLength { get; set; }

    [Display(Name = "Maximum Length")]
    public int MaxLength { get; set; }

    [Display(Name = "Answer Options List")]
    [MaxLength(1000)]
    public string OptionsList { get; set; }

    [Display(Name = "Help Text")]
    [MaxLength(1000)]
    public string HelpText { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }

    public Dictionary<string, string> Categories { get; set; }
    public Dictionary<string, string> AnswerTypes { get; set; }
}

[ExcludeFromCodeCoverage]
public class QuestionConfigsViewModel
{
    public IEnumerable<QuestionConfigViewModel> Questions { get; set; }
}