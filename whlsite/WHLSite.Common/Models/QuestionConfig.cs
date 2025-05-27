using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WHLSite.Common.Models;

[ExcludeFromCodeCoverage]
public class QuestionConfig : ModelBase
{
    public int QuestionId { get; set; }
    public string CategoryCd { get; set; }
    public string CategoryDescription { get; set; }
    public string Title { get; set; }
    public string AnswerTypeCd { get; set; }
    public string AnswerTypeDescription { get; set; }
    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public string OptionsList { get; set; }
    public List<string> Options { get; set; }
    public string HelpText { get; set; }
}