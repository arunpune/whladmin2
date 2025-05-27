using System;
using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class User : ModelBase
{
    public string UserId { get; set; }
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public string OrganizationCd { get; set; }
    public string OrganizationDescription { get; set; }
    public string RoleCd { get; set; }
    public string RoleDescription { get; set; }
    public string DeactivatedBy { get; set; }
    public DateTime? DeactivatedDate { get; set; }
    public DateTime? LastLoggedIn { get; set; }
}