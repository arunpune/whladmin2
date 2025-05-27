using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Services;
using WHLSite.ViewModels;

namespace WHLSite.Tests.Services;

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
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 1, Code = "CODE1", Description = "DESC1"  },
            new() { CodeId = 2, Code = "CODE2", Description = "DESC2"  }
        };

        var service = new UiHelperService(_logger.Object);
        var actual = service.ToDictionary(data);
        Assert.NotEmpty(actual);
        Assert.Equal(2, actual.Count);

        var first = actual.First();
        Assert.Equal("CODE1", first.Key);
        Assert.Equal("DESC1", first.Value);
    }

    [Fact]
    public void ToToDropdownListTests()
    {
        // Setup
        var data = new List<CodeDescription>
        {
            new() { CodeId = 1, Code = "CODE1", Description = "DESC1"  },
            new() { CodeId = 2, Code = "CODE2", Description = "DESC2"  }
        };

        var service = new UiHelperService(_logger.Object);
        var actual = service.ToDropdownList(data, "", "SELECT ONE");
        Assert.NotEmpty(actual);
        Assert.Equal(3, actual.Count);

        var first = actual.First();
        Assert.Equal("", first.Key);
        Assert.Equal("SELECT ONE", first.Value);
    }

    [Theory]
    [InlineData(null, null, null, null, null, "")]
    [InlineData("", null, null, null, null, "")]
    [InlineData("TITLE", null, null, null, null, "TITLE")]
    [InlineData("TITLE ", null, null, null, null, "TITLE")]
    [InlineData(" TITLE", null, null, null, null, "TITLE")]
    [InlineData(" TITLE ", null, null, null, null, "TITLE")]
    [InlineData("TITLE ", "", null, null, null, "TITLE")]
    [InlineData("TITLE ", " ", null, null, null, "TITLE")]
    [InlineData("TITLE ", "FIRST", null, null, null, "TITLE FIRST")]
    [InlineData("TITLE ", "FIRST ", null, null, null, "TITLE FIRST")]
    [InlineData("TITLE ", " FIRST", null, null, null, "TITLE FIRST")]
    [InlineData("TITLE ", " FIRST ", null, null, null, "TITLE FIRST")]
    [InlineData("TITLE ", "FIRST", "", null, null, "TITLE FIRST")]
    [InlineData("TITLE ", "FIRST", " ", null, null, "TITLE FIRST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", null, null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", "MIDDLE ", null, null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", " MIDDLE", null, null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", " MIDDLE ", null, null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "", null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", " ", null, "TITLE FIRST MIDDLE")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", null, "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST ", null, "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", " LAST", null, "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", " LAST ", null, "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", "", "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", " ", "TITLE FIRST MIDDLE LAST")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", "SUFFIX", "TITLE FIRST MIDDLE LAST SUFFIX")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", "SUFFIX ", "TITLE FIRST MIDDLE LAST SUFFIX")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", " SUFFIX", "TITLE FIRST MIDDLE LAST SUFFIX")]
    [InlineData("TITLE ", "FIRST", "MIDDLE", "LAST", " SUFFIX ", "TITLE FIRST MIDDLE LAST SUFFIX")]
    public void ToDisplayNameTests(string title, string firstName, string middleName, string lastName, string suffix, string expected)
    {
        var service = new UiHelperService(_logger.Object);
        var actual = service.ToDisplayName(title, firstName, middleName, lastName, suffix);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, null, "")]
    [InlineData("", null, "")]
    [InlineData(" ", null, "")]
    [InlineData("TYPE", null, "TYPE -")]
    [InlineData("TYPE ", null, "TYPE -")]
    [InlineData(" TYPE", null, "TYPE -")]
    [InlineData(" TYPE ", null, "TYPE -")]
    [InlineData("TYPE", "", "TYPE -")]
    [InlineData("TYPE", " ", "TYPE -")]
    [InlineData("TYPE", "VALUE", "TYPE - VALUE")]
    [InlineData("TYPE", "VALUE ", "TYPE - VALUE")]
    [InlineData("TYPE", " VALUE", "TYPE - VALUE")]
    [InlineData("TYPE", " VALUE ", "TYPE - VALUE")]
    public void ToDisplayIdTypeValueTests(string description, string value, string expected)
    {
        var service = new UiHelperService(_logger.Object);
        var actual = service.ToDisplayIdTypeValue(description, value);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, null, null, null, null, null, "")]
    [InlineData("", null, null, null, null, null, "")]
    [InlineData(" ", null, null, null, null, null, "")]
    [InlineData("LINE1", null, null, null, null, null, "LINE1")]
    [InlineData("LINE1 ", null, null, null, null, null, "LINE1")]
    [InlineData(" LINE1", null, null, null, null, null, "LINE1")]
    [InlineData(" LINE1 ", null, null, null, null, null, "LINE1")]
    [InlineData("LINE1", "", null, null, null, null, "LINE1")]
    [InlineData("LINE1", " ", null, null, null, null, "LINE1")]
    [InlineData("LINE1", "LINE2", null, null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", "LINE2 ", null, null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", " LINE2", null, null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", " LINE2 ", null, null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", "LINE2 ", "", null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", "LINE2 ", " ", null, null, null, "LINE1, LINE2")]
    [InlineData("LINE1", "LINE2 ", "LINE3", null, null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", "LINE3 ", null, null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", " LINE3", null, null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", " LINE3 ", null, null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "", null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", "LINE3", " ", null, null, "LINE1, LINE2, LINE3")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", null, null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY ", null, null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", " CITY", null, null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", " CITY ", null, null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "", null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", " ", null, "LINE1, LINE2, LINE3, CITY")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", null, "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE ", null, "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", " STATE", null, "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", " STATE ", null, "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", "", "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", " ", "LINE1, LINE2, LINE3, CITY STATE")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", "ZIP", "LINE1, LINE2, LINE3, CITY STATE ZIP")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", "ZIP ", "LINE1, LINE2, LINE3, CITY STATE ZIP")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", " ZIP", "LINE1, LINE2, LINE3, CITY STATE ZIP")]
    [InlineData("LINE1", "LINE2 ", "LINE3", "CITY", "STATE", " ZIP ", "LINE1, LINE2, LINE3, CITY STATE ZIP")]
    public void ToAddressTextSingleLineTests(string line1, string line2, string line3, string city, string stateCd, string zipCode, string expected)
    {
        var service = new UiHelperService(_logger.Object);
        var model = new ListingViewModel()
        {
            City = city,
            StateCd = stateCd,
            StreetLine1 = line1,
            StreetLine2 = line2,
            StreetLine3 = line3,
            ZipCode = zipCode,
        };
        var actual = service.ToAddressTextSingleLine(model);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void ToDateTimeDisplayTextTests()
    {
        var service = new UiHelperService(_logger.Object);

        // Null Test
        DateTime? input = null;
        var actual = service.ToDateTimeDisplayText(input, It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("", actual);

        // Date, No Formats Test
        input = DateTime.Now;
        actual = service.ToDateTimeDisplayText(input, It.IsAny<string>(), It.IsAny<string>());
        Assert.Equal("", actual);

        // Date Format Test
        input = DateTime.Now;
        actual = service.ToDateTimeDisplayText(input, "MM/dd/yyyy", It.IsAny<string>());
        Assert.Equal($"{DateTime.Now:MM/dd/yyyy}", actual);

        // Time Format Test
        input = DateTime.Now;
        actual = service.ToDateTimeDisplayText(input, It.IsAny<string>(), "HH:mm");
        Assert.Equal($"{DateTime.Now:HH:mm}", actual);

        // Date/Time Format Test
        input = DateTime.Now;
        actual = service.ToDateTimeDisplayText(input, "MM/dd/yyyy", "HH:mm");
        Assert.Equal($"{DateTime.Now:MM/dd/yyyy} {DateTime.Now:HH:mm}", actual);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("", "")]
    [InlineData("123", "")]
    [InlineData("12345678901", "")]
    [InlineData("1234567890", "(123) 456-7890")]
    [InlineData(" 1234567890", "(123) 456-7890")]
    [InlineData("1234567890 ", "(123) 456-7890")]
    [InlineData(" 1234567890 ", "(123) 456-7890")]
    public void ToPhoneNumberTextTests(string input, string expected)
    {
        var service = new UiHelperService(_logger.Object);
        var actual = service.ToPhoneNumberText(input);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, null, null, "")]
    [InlineData("", null, null, "")]
    [InlineData(" ", null, null, "")]
    [InlineData("Code", null, null, "Code")]
    [InlineData("Code ", null, null, "Code")]
    [InlineData(" Code", null, null, "Code")]
    [InlineData(" Code ", null, null, "Code")]
    [InlineData("Code", "", null, "Code")]
    [InlineData("Code", " ", null, "Code")]
    [InlineData("Code", "Description", null, "Description")]
    [InlineData("Code", "Description ", null, "Description")]
    [InlineData("Code", " Description", null, "Description")]
    [InlineData("Code", " Description ", null, "Description")]
    [InlineData("Other", null, null, "Other")]
    [InlineData("Other", "", null, "Other")]
    [InlineData("Other", " ", null, "Other")]
    [InlineData("Other", "Description", null, "Description")]
    [InlineData("Other", "Description", "", "Description")]
    [InlineData("Other", "Description", " ", "Description")]
    [InlineData("Other", "Description", "Value", "Other: Value")]
    [InlineData("Other", "Description", "Value ", "Other: Value")]
    [InlineData("Other", "Description", " Value", "Other: Value")]
    [InlineData("Other", "Description", " Value ", "Other: Value")]
    public void ToOtherAndValueTextTests(string code, string description, string otherValue, string expected)
    {
        var service = new UiHelperService(_logger.Object);
        var actual = service.ToOtherAndValueText(code, description, otherValue);
        Assert.Equal(expected, actual);
    }
}