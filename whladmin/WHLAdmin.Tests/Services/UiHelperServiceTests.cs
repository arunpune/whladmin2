using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Services;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Services;

public class UiHelperServiceTests
{
    private readonly Mock<ILogger<UiHelperService>> _logger = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new UiHelperService(null));

        // Not Null
        var actual = new UiHelperService(_logger.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void ToDictionaryTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        List<CodeDescription> codeDescriptions = null;
        var actual = service.ToDictionary(codeDescriptions);
        Assert.Empty(actual);

        // Empty
        codeDescriptions = [];
        actual = service.ToDictionary(codeDescriptions);
        Assert.Empty(actual);

        // With Values
        codeDescriptions.Add(new() { Code = "CODE", Description = "DESC" });
        actual = service.ToDictionary(codeDescriptions);
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void ToDropdownListTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        List<CodeDescription> codeDescriptions = null;
        var actual = service.ToDropdownList(codeDescriptions, "", "Select One");
        Assert.NotEmpty(actual);

        // Empty
        codeDescriptions = [];
        actual = service.ToDropdownList(codeDescriptions, "", "Select One");
        Assert.NotEmpty(actual);

        // With Values
        codeDescriptions.Add(new() { Code = "CODE", Description = "DESC" });
        actual = service.ToDropdownList(codeDescriptions, "", "Select One");
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void ToAddressTextSingleLineTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        ListingViewModel model = null;
        var actual = service.ToAddressTextSingleLine(model);
        Assert.Empty(actual);

        // Empty
        model = new ListingViewModel();
        actual = service.ToAddressTextSingleLine(model);
        Assert.Empty(actual);

        // Street Line 1
        model.StreetLine1 = "STREET1";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1", actual);

        // Street Line 2
        model.StreetLine2 = "STREET2";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1, STREET2", actual);

        // Street Line 3
        model.StreetLine3 = "STREET3";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1, STREET2, STREET3", actual);

        // City
        model.City = "CITY";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1, STREET2, STREET3, CITY", actual);

        // State
        model.StateCd = "STATE";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1, STREET2, STREET3, CITY, STATE", actual);

        // Zip Code
        model.ZipCode = "ZIPCD";
        actual = service.ToAddressTextSingleLine(model);
        Assert.Equal("STREET1, STREET2, STREET3, CITY, STATE ZIPCD", actual);
    }

    [Fact]
    public void ToDateTimeDisplayTextTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        DateTime? date = null;
        var actual = service.ToDateTimeDisplayText(date);
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = service.ToDateTimeDisplayText(date);
        Assert.Empty(actual);

        // Some other Date, no format
        date = DateTime.Now;
        actual = service.ToDateTimeDisplayText(date);
        Assert.Empty(actual);

        // Some other Date, only date format
        date = DateTime.Now;
        actual = service.ToDateTimeDisplayText(date, "yyyy-MM-dd");
        Assert.Equal($"{DateTime.Now:yyyy-MM-dd}", actual);

        // Some other Date, only time format
        date = DateTime.Now;
        actual = service.ToDateTimeDisplayText(date, null, "h:mm tt");
        Assert.Equal($"{DateTime.Now:h:mm tt}", actual);

        // Some other Date, only time format
        date = DateTime.Now;
        actual = service.ToDateTimeDisplayText(date, "yyyy-MM-dd", "h:mm tt");
        Assert.Equal($"{DateTime.Now:yyyy-MM-dd h:mm tt}", actual);
    }

    [Fact]
    public void ToDateEditorFormatTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        DateTime? date = null;
        var actual = service.ToDateEditorFormat(date);
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = service.ToDateEditorFormat(date);
        Assert.Empty(actual);

        // Some other Date
        date = DateTime.Now;
        actual = service.ToDateEditorFormat(date);
        Assert.Equal($"{DateTime.Now.Year}-{DateTime.Now.Month:00}-{DateTime.Now.Day:00}", actual);

        // Some other Date Format
        date = DateTime.Now;
        actual = service.ToDateEditorFormat(date, "MM/dd/yyyy");
        Assert.Equal($"{DateTime.Now.Month:00}/{DateTime.Now.Day:00}/{DateTime.Now.Year}", actual);
    }

    [Fact]
    public void ToTimeEditorFormatTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null
        DateTime? date = null;
        var actual = service.ToTimeEditorFormat(date);
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = service.ToTimeEditorFormat(date);
        Assert.Empty(actual);

        // Some other Date
        date = DateTime.Now;
        actual = service.ToTimeEditorFormat(date);
        Assert.Equal($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00}", actual);

        // Some other Date Format
        date = DateTime.Now;
        actual = service.ToTimeEditorFormat(date, "HH:mm");
        Assert.Equal($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}", actual);
    }
}