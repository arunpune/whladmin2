using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WHLAdmin.Common.Models;

namespace WHLAdmin.ViewModels;

[ExcludeFromCodeCoverage]
public class LoginViewModel()
{
    public string UserId { get; set; }
    public string EmailAddress { get; set; }
    public string Token { get; set; }
    public string ReturnUrl { get; set; }
}

[ExcludeFromCodeCoverage]
public class UserViewModel : User
{
}

[ExcludeFromCodeCoverage]
public class EditableUserViewModel
{
    [Display(Name = "User ID")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "User ID is required")]
    [MaxLength(4)]
    public string UserId { get; set; }

    [Display(Name = "Email Address")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Email address is required")]
    [MaxLength(200)]
    public string EmailAddress { get; set; }

    [Display(Name = "Display Name")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Display name is required")]
    [MaxLength(200)]
    public string DisplayName { get; set; }

    [Display(Name = "Organization")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Organization is required")]
    public string OrganizationCd { get; set; }

    [Display(Name = "Role")]
    [Required(AllowEmptyStrings = false, ErrorMessage = "Role is required")]
    public string RoleCd { get; set; }

    [Display(Name = "Role")]
    public string RoleDescription { get; set; }

    public Dictionary<string, string> Roles { get; set; }
    public Dictionary<string, string> Organizations { get; set; }
}

[ExcludeFromCodeCoverage]
public class UsersViewModel
{
    public IEnumerable<UserViewModel> Users { get; set; }
    public bool CanEdit { get; set; }
}
