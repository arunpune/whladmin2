using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using WHLAdmin.Common.Models;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Services;

public interface IUiHelperService
{
    Dictionary<string, string> ToDictionary(IEnumerable<CodeDescription> codeDescriptions);
    Dictionary<string, string> ToDropdownList(IEnumerable<CodeDescription> codeDescriptions, string defaultKey = "", string defaultValue = "Select One");
    string ToAddressTextSingleLine(ListingViewModel model);
    string ToDateTimeDisplayText(DateTime? dateTime, string dateFormat = null, string timeFormat = null);
    string ToDateEditorFormat(DateTime? date, string format = "yyyy-MM-dd");
    string ToTimeEditorFormat(DateTime? time, string format = "HH:mm:ss");
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
        if ((codeDescriptions?.Count() ?? 0) > 0)
        {
            foreach (var codeDescription in codeDescriptions)
            {
                optionsList.Add(codeDescription.Code, codeDescription.Description);
            }
        }
        return optionsList;
    }

    public Dictionary<string, string> ToDropdownList(IEnumerable<CodeDescription> codeDescriptions, string defaultKey = "", string defaultValue = "Select One")
    {
        var optionsList = new Dictionary<string, string>
        {
            { defaultKey, defaultValue }
        };
        if ((codeDescriptions?.Count() ?? 0) > 0)
        {
            foreach (var codeDescription in codeDescriptions)
            {
                optionsList.Add(codeDescription.Code, codeDescription.Description);
            }
        }
        return optionsList;
    }

    public string ToAddressTextSingleLine(ListingViewModel model)
    {
        if (model == null) return "";

        var addressText = (model.StreetLine1 ?? "").Trim();
        if (!string.IsNullOrEmpty((model.StreetLine2 ?? "").Trim()))
        {
            addressText += ", " + (model.StreetLine2 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty((model.StreetLine3 ?? "").Trim()))
        {
            addressText += ", " + (model.StreetLine3 ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.City))
        {
            addressText += ", " + (model.City ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.StateCd))
        {
            addressText += ", " + (model.StateCd ?? "").Trim();
        }
        if (!string.IsNullOrEmpty(model.ZipCode))
        {
            addressText += " " + (model.ZipCode ?? "").Trim();
        }
        return addressText;
    }

    public string ToDateTimeDisplayText(DateTime? dateTime, string dateFormat = null, string timeFormat = null)
    {
        if (dateTime.GetValueOrDefault(DateTime.MinValue) == DateTime.MinValue) return "";

        var displayText = "";

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

        return displayText;
    }

    public string ToDateEditorFormat(DateTime? date, string format = "yyyy-MM-dd")
    {
        return date.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? date.Value.Date.ToString(format) : "";
    }

    public string ToTimeEditorFormat(DateTime? time, string format = "HH:mm:ss")
    {
        return time.GetValueOrDefault(DateTime.MinValue) > DateTime.MinValue ? time.Value.ToString(format) : "";
    }
}