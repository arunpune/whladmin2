using System.Diagnostics.CodeAnalysis;

namespace WHLAdmin.Common.Models;

[ExcludeFromCodeCoverage]
public class UserRole
{
    public string RoleCd { get; set; }
    public string RoleDescription { get; set; }
}