using System;
using System.Collections.Generic;
using WHLAdmin.Common.Models;
using WHLAdmin.Extensions;
using WHLAdmin.ViewModels;

namespace WHLAdmin.Tests.Extensions;

public class ModelExtensionsTests()
{
    [Fact]
    public void ToDictionaryTests()
    {
        // Null
        List<CodeDescription> codeDescriptions = null;
        var actual = codeDescriptions.ToDictionary();
        Assert.Empty(actual);

        // Empty
        codeDescriptions = [];
        actual = codeDescriptions.ToDictionary();
        Assert.Empty(actual);

        // With Values
        codeDescriptions.Add(new() { Code = "CODE", Description = "DESC" });
        actual = codeDescriptions.ToDictionary();
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void ToDropdownListTests()
    {
        // Null
        List<CodeDescription> codeDescriptions = null;
        var actual = codeDescriptions.ToDropdownList("", "Select One");
        Assert.NotEmpty(actual);

        // Empty
        codeDescriptions = [];
        actual = codeDescriptions.ToDropdownList("", "Select One");
        Assert.NotEmpty(actual);

        // With Values
        codeDescriptions.Add(new() { Code = "CODE", Description = "DESC" });
        actual = codeDescriptions.ToDropdownList("", "Select One");
        Assert.NotEmpty(actual);
    }

    [Fact]
    public void ToAddressTextSingleLineTests()
    {
        // Null
        ListingViewModel model = null;
        var actual = model.ToAddressTextSingleLine();
        Assert.Empty(actual);

        // Empty
        model = new ListingViewModel();
        actual = model.ToAddressTextSingleLine();
        Assert.Empty(actual);

        // Street Line 1
        model.StreetLine1 = "STREET1";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1", actual);

        // Street Line 2
        model.StreetLine2 = "STREET2";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1, STREET2", actual);

        // Street Line 3
        model.StreetLine3 = "STREET3";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1, STREET2, STREET3", actual);

        // City
        model.City = "CITY";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1, STREET2, STREET3, CITY", actual);

        // State
        model.StateCd = "STATE";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1, STREET2, STREET3, CITY, STATE", actual);

        // Zip Code
        model.ZipCode = "ZIPCD";
        actual = model.ToAddressTextSingleLine();
        Assert.Equal("STREET1, STREET2, STREET3, CITY, STATE ZIPCD", actual);
    }

    [Fact]
    public void ToDateTimeDisplayTextTests()
    {
        // Null
        DateTime? date = null;
        var actual = date.ToDateTimeDisplayText();
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = date.ToDateTimeDisplayText();
        Assert.Empty(actual);

        // Some other Date, no format
        date = DateTime.Now;
        actual = date.ToDateTimeDisplayText();
        Assert.Empty(actual);

        // Some other Date, only date format
        date = DateTime.Now;
        actual = date.ToDateTimeDisplayText("yyyy-MM-dd");
        Assert.Equal($"{DateTime.Now:yyyy-MM-dd}", actual);

        // Some other Date, only time format
        date = DateTime.Now;
        actual = date.ToDateTimeDisplayText(null, "h:mm tt");
        Assert.Equal($"{DateTime.Now:h:mm tt}", actual);

        // Some other Date, only time format
        date = DateTime.Now;
        actual = date.ToDateTimeDisplayText("yyyy-MM-dd", "h:mm tt");
        Assert.Equal($"{DateTime.Now:yyyy-MM-dd h:mm tt}", actual);
    }

    [Fact]
    public void ToDateEditorFormatTests()
    {
        // Null
        DateTime? date = null;
        var actual = date.ToDateEditorFormat();
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = date.ToDateEditorFormat();
        Assert.Empty(actual);

        // Some other Date
        date = DateTime.Now;
        actual = date.ToDateEditorFormat();
        Assert.Equal($"{DateTime.Now.Year}-{DateTime.Now.Month:00}-{DateTime.Now.Day:00}", actual);

        // Some other Date Format
        date = DateTime.Now;
        actual = date.ToDateEditorFormat("MM/dd/yyyy");
        Assert.Equal($"{DateTime.Now.Month:00}/{DateTime.Now.Day:00}/{DateTime.Now.Year}", actual);
    }

    [Fact]
    public void ToTimeEditorFormatTests()
    {
        // Null
        DateTime? date = null;
        var actual = date.ToTimeEditorFormat();
        Assert.Empty(actual);

        // Min Date
        date = DateTime.MinValue;
        actual = date.ToTimeEditorFormat();
        Assert.Empty(actual);

        // Some other Date
        date = DateTime.Now;
        actual = date.ToTimeEditorFormat();
        Assert.Equal($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}:{DateTime.Now.Second:00}", actual);

        // Some other Date Format
        date = DateTime.Now;
        actual = date.ToTimeEditorFormat("HH:mm");
        Assert.Equal($"{DateTime.Now.Hour:00}:{DateTime.Now.Minute:00}", actual);
    }

    [Fact]
    public void ToAmenityViewModelTests()
    {
        // Null
        Amenity amenity = null;
        var actual = amenity.ToViewModel();
        Assert.Null(actual);

        // Inactive
        amenity = new Amenity()
        {
            AmenityId = 1,
            Name = "NAME",
            Active = false
        };
        actual = amenity.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AmenityId);
        Assert.Equal("NAME", actual.Name);
        Assert.False(actual.Active);

        // Inactive
        amenity.Active = true;
        actual = amenity.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.AmenityId);
        Assert.Equal("NAME", actual.Name);
        Assert.True(actual.Active);

        // Editable
        var editable = amenity.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.AmenityId);
        Assert.Equal("NAME", editable.AmenityName);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToFaqConfigViewModelTests()
    {
        // Null
        FaqConfig faq = null;
        var actual = faq.ToViewModel();
        Assert.Null(actual);

        // Inactive
        faq = new FaqConfig()
        {
            FaqId = 1,
            Title = "TITLE",
            Text = "TEXT",
            Active = false
        };
        actual = faq.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FaqId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("TEXT", actual.Text);
        Assert.False(actual.Active);

        // Inactive
        faq.Active = true;
        actual = faq.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.FaqId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("TEXT", actual.Text);
        Assert.True(actual.Active);

        // Editable
        var editable = faq.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.FaqId);
        Assert.Equal("TITLE", editable.Title);
        Assert.Equal("TEXT", editable.Text);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToNotificationConfigViewModelTests()
    {
        // Null
        NotificationConfig notification = null;
        var actual = notification.ToViewModel();
        Assert.Null(actual);

        // Inactive
        notification = new NotificationConfig()
        {
            NotificationId = 1,
            Text = "TEXT",
            Active = false
        };
        actual = notification.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.NotificationId);
        Assert.Equal("TEXT", actual.Text);
        Assert.False(actual.Active);

        // Inactive
        notification.Active = true;
        actual = notification.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.NotificationId);
        Assert.Equal("TEXT", actual.Text);
        Assert.True(actual.Active);

        // Editable
        var editable = notification.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.NotificationId);
        Assert.Equal("TEXT", editable.Text);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToQuoteConfigViewModelTests()
    {
        // Null
        QuoteConfig quote = null;
        var actual = quote.ToViewModel();
        Assert.Null(actual);

        // Inactive
        quote = new QuoteConfig()
        {
            QuoteId = 1,
            Text = "TEXT",
            Active = false
        };
        actual = quote.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.QuoteId);
        Assert.Equal("TEXT", actual.Text);
        Assert.False(actual.Active);

        // Inactive
        quote.Active = true;
        actual = quote.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.QuoteId);
        Assert.Equal("TEXT", actual.Text);
        Assert.True(actual.Active);

        // Editable
        var editable = quote.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.QuoteId);
        Assert.Equal("TEXT", editable.Text);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToResourceConfigViewModelTests()
    {
        // Null
        ResourceConfig resource = null;
        var actual = resource.ToViewModel();
        Assert.Null(actual);

        // Inactive
        resource = new ResourceConfig()
        {
            ResourceId = 1,
            Title = "TITLE",
            Url = "URL",
            Active = false
        };
        actual = resource.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.ResourceId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
        Assert.False(actual.Active);

        // Inactive
        resource.Active = true;
        actual = resource.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.ResourceId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
        Assert.True(actual.Active);

        // Editable
        var editable = resource.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.ResourceId);
        Assert.Equal("TITLE", editable.Title);
        Assert.Equal("URL", editable.Url);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToUserViewModelTests()
    {
        // Null
        User user = null;
        var actual = user.ToViewModel();
        Assert.Null(actual);

        // Inactive
        user = new User()
        {
            EmailAddress = "EMAIL@UNIT.TST",
            DisplayName = "NAME",
            RoleCd = "ROLECD",
            OrganizationCd = "ORGCD",
            Active = false
        };
        actual = user.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
        Assert.Equal("NAME", actual.DisplayName);
        Assert.Equal("ROLECD", actual.RoleCd);
        Assert.Equal("ORGCD", actual.OrganizationCd);
        Assert.False(actual.Active);

        // Inactive
        user.Active = true;
        actual = user.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("EMAIL@UNIT.TST", actual.EmailAddress);
        Assert.Equal("NAME", actual.DisplayName);
        Assert.Equal("ROLECD", actual.RoleCd);
        Assert.Equal("ORGCD", actual.OrganizationCd);
        Assert.True(actual.Active);

        // Editable
        var editable = user.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal("EMAIL@UNIT.TST", editable.EmailAddress);
        Assert.Equal("NAME", editable.DisplayName);
        Assert.Equal("ROLECD", editable.RoleCd);
        Assert.Equal("ORGCD", editable.OrganizationCd);
    }

    [Fact]
    public void ToVideoConfigViewModelTests()
    {
        // Null
        VideoConfig video = null;
        var actual = video.ToViewModel();
        Assert.Null(actual);

        // Inactive
        video = new VideoConfig()
        {
            VideoId = 1,
            Title = "TITLE",
            Url = "URL",
            Active = false
        };
        actual = video.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.VideoId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
        Assert.False(actual.Active);

        // Inactive
        video.Active = true;
        actual = video.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.VideoId);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("URL", actual.Url);
        Assert.True(actual.Active);

        // Editable
        var editable = video.ToEditableViewModel();
        Assert.NotNull(editable);
        Assert.Equal(1, editable.VideoId);
        Assert.Equal("TITLE", editable.Title);
        Assert.Equal("URL", editable.Url);
        Assert.True(editable.Active);
    }

    [Fact]
    public void ToAuditViewModelTests()
    {
        // Null
        AuditEntry auditEntry = null;
        var actual = auditEntry.ToViewModel();
        Assert.Null(actual);

        // Valid
        auditEntry = new AuditEntry()
        {
            ActionCd = "ADD",
            ActionDescription = "Add",
            EntityDescription = "Amenity",
            EntityId = "1",
            EntityName = "NAME",
            EntityTypeCd = "AMENITY",
            Id = 1,
            Note = "New amenity added",
            Timestamp = DateTime.Now,
            Username = "USERNAME"
        };
        actual = auditEntry.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Id);
        Assert.Equal("NAME", actual.EntityName);
    }

    [Fact]
    public void ToNoteViewModelTests()
    {
        // Null
        Note note = null;
        var actual = note.ToViewModel();
        Assert.Null(actual);

        // Valid
        note = new Note()
        {
            ActionCd = "ADD",
            ActionDescription = "Add",
            EntityDescription = "Amenity",
            EntityId = "1",
            EntityName = "NAME",
            EntityTypeCd = "AMENITY",
            Id = 1,
            Note = "New note added",
            Timestamp = DateTime.Now,
            Username = "USERNAME"
        };
        actual = note.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Id);
        Assert.Equal("NAME", actual.EntityName);
    }
}