using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class MarketingAgentViewModel : MarketingAgent
{
}

[ExcludeFromCodeCoverage]
public class EditableMarketingAgentViewModel
{
    public int AgentId { get; set; }

    [Display(Name = "Marketing Agent")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Marketing Agent Name is required")]
    [MaxLength(200)]
    public string AgentName { get; set; }

    [Display(Name = "Contact Name")]
    [MaxLength(200)]
    public string ContactName { get; set; }

    [Display(Name = "Phone Number")]
    [MaxLength(20)]
    public string PhoneNumber { get; set; }

    [Display(Name = "Email Address")]
    [MaxLength(200)]
    public string EmailAddress { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; }
}

[ExcludeFromCodeCoverage]
public class MarketingAgentsViewModel
{
    public IEnumerable<MarketingAgentViewModel> Agents { get; set; }
    public bool CanEdit { get; set; }
}