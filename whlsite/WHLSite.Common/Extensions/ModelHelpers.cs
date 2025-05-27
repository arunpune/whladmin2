using WHLSite.Common.Models;

namespace WHLSite.Common.Extensions;

public static class ModelHelpers
{
    public static string ToDisplayName(this HousingApplication application)
    {
        var name = "";
        if (!string.IsNullOrEmpty(application?.Title?.Trim() ?? string.Empty)) name += application.Title.Trim();
        if (!string.IsNullOrEmpty(application?.FirstName?.Trim() ?? string.Empty)) name += " " + application.FirstName.Trim();
        if (!string.IsNullOrEmpty(application?.MiddleName?.Trim() ?? string.Empty)) name += " " + application.MiddleName.Trim();
        if (!string.IsNullOrEmpty(application?.LastName?.Trim() ?? string.Empty)) name += " " + application.LastName.Trim();
        if (!string.IsNullOrEmpty(application?.Suffix?.Trim() ?? string.Empty)) name += " " + application.Suffix.Trim();
        return name.Trim();
    }
}
