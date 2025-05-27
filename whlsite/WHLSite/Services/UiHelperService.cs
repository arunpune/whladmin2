using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using WHLSite.Common.Models;
using WHLSite.ViewModels;

namespace WHLSite.Services;

public interface IUiHelperService
{
    Dictionary<string, string> ToDictionary(IEnumerable<CodeDescription> codeDescriptions);
    Dictionary<string, string> ToDropdownList(IEnumerable<CodeDescription> codeDescriptions, string defaultKey = "", string defaultValue = "Select One");
    string ToDisplayName(string title, string firstName, string middleName, string lastName, string suffix);
    string ToDisplayIdTypeValue(string idTypeDescription = null, string idTypeValue = null);
    string ToAddressTextSingleLine(ListingViewModel model);
    string ToDateTimeDisplayText(DateTime? dateTime, string dateFormat = null, string timeFormat = null);
    string ToPhoneNumberText(string number);
    string ToOtherAndValueText(string code, string description, string otherValue = null, string defaultValue = "");
}

public class UiHelperService : IUiHelperService
{
    private readonly ILogger<UiHelperService> _logger;

    public UiHelperService(ILogger<UiHelperService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Dictionary<string, string> ToDictionary(IEnumerable<CodeDescription> codeDescriptions)
    {
        var optionsList = new Dictionary<string, string>();
        foreach (var codeDescription in codeDescriptions)
        {
            optionsList.Add(codeDescription.Code, codeDescription.Description);
        }
        return optionsList;
    }

    public Dictionary<string, string> ToDropdownList(IEnumerable<CodeDescription> codeDescriptions, string defaultKey = "", string defaultValue = "Select One")
    {
        var optionsList = new Dictionary<string, string>
        {
            { defaultKey, defaultValue }
        };
        foreach (var codeDescription in codeDescriptions)
        {
            optionsList.Add(codeDescription.Code, codeDescription.Description);
        }
        return optionsList;
    }

    public string ToDisplayName(string title, string firstName, string middleName, string lastName, string suffix)
    {
        var name = "";
        if (!string.IsNullOrEmpty(title?.Trim() ?? string.Empty)) name += title.Trim();
        if (!string.IsNullOrEmpty(firstName?.Trim() ?? string.Empty)) name += " " + firstName.Trim();
        if (!string.IsNullOrEmpty(middleName?.Trim() ?? string.Empty)) name += " " + middleName.Trim();
        if (!string.IsNullOrEmpty(lastName?.Trim() ?? string.Empty)) name += " " + lastName.Trim();
        if (!string.IsNullOrEmpty(suffix?.Trim() ?? string.Empty)) name += " " + suffix.Trim();
        return name.Trim();
    }

    public string ToDisplayIdTypeValue(string idTypeDescription = null, string idTypeValue = null)
    {
        idTypeDescription = (idTypeDescription ?? "").Trim();
        idTypeValue = (idTypeValue ?? "").Trim();

        return string.IsNullOrEmpty(idTypeDescription) ? "" : $"{idTypeDescription} - {idTypeValue}".Trim();
    }

    public string ToAddressTextSingleLine(ListingViewModel model)
    {
        var addressText = "";
        if (!string.IsNullOrEmpty((model.StreetLine1 ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? ", " : "") + (model.StreetLine1 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.StreetLine2 ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? ", " : "") + (model.StreetLine2 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.StreetLine3 ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? ", " : "") + (model.StreetLine3 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.City ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? ", " : "") + (model.City ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.StateCd ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? " " : "") + (model.StateCd ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.ZipCode ?? "").Trim()))
        {
            addressText += (addressText.Length > 0 ? " " : "") + (model.ZipCode ?? "").Trim();
        }
        return addressText.Trim();
    }

    public string ToDateTimeDisplayText(DateTime? dateTime, string dateFormat = null, string timeFormat = null)
    {
        var displayText = "";
        if (dateTime.HasValue && dateTime.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue)
        {
            dateFormat = (dateFormat ?? "").Trim();
            if (!string.IsNullOrEmpty(dateFormat))
            {
                displayText += dateTime.Value.ToString(dateFormat);
            }
            timeFormat = (timeFormat ?? "").Trim();
            if (!string.IsNullOrEmpty(timeFormat))
            {
                displayText += (displayText.Length > 0 ? " " : "") + dateTime.Value.ToString(timeFormat);
            }
        }
        return displayText;
    }

    public string ToPhoneNumberText(string number)
    {
        number = (number ?? "").Trim();
        if (number.Length == 10)
        {
            return $"({number[..3]}) {number.Substring(3, 3)}-{number[6..]}";
        }
        return "";
    }

    public string ToOtherAndValueText(string code, string description, string otherValue = null, string defaultValue = "")
    {
        code = (code ?? "").Trim();
        description = (description ?? "").Trim();
        otherValue = (otherValue ?? "").Trim();
        defaultValue = (defaultValue ?? "").Trim();
        if (!string.IsNullOrEmpty(code))
        {
            if (code.Equals("OTHER", StringComparison.CurrentCultureIgnoreCase) && !string.IsNullOrEmpty(otherValue))
            {
                return $"Other: {otherValue}";
            }
            return description.Length > 0 ? description : code;
        }
        return defaultValue;
    }
}