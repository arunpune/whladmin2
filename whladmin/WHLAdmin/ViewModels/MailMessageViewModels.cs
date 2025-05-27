using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class MailMessageViewModel : MailMessage
{
    public string Result { get; set; }
}