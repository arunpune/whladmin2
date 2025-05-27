using System;
using Microsoft.AspNetCore.Http;
using Moq;
using WHLSite.Common.Models;
using WHLSite.Extensions;

namespace WHLSite.Tests.Extensions;

public class ModelHelpersTests
{
    [Fact]
    public void ToFaqViewModelTests()
    {
        // Null
        FaqConfig item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Title = "TITLE" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TITLE", actual.Title);
    }

    [Fact]
    public void ToQuoteViewModelTests()
    {
        // Null
        QuoteConfig item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Text = "TEXT" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TEXT", actual.Text);
    }

    [Fact]
    public void ToResourceViewModelTests()
    {
        // Null
        ResourceConfig item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Title = "TITLE" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TITLE", actual.Title);
    }

    [Fact]
    public void ToVideoViewModelTests()
    {
        // Null
        VideoConfig item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Title = "TITLE" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TITLE", actual.Title);
    }

    [Fact]
    public void SiteUrlTests()
    {
        // Null
        var actual = ((HttpRequest)null).SiteUrl();
        Assert.Null(actual);

        // Without Port
        var request = new Mock<HttpRequest>();
        request.Setup(s => s.Scheme).Returns("http");
        request.Setup(s => s.Host).Returns(new HostString("abc.def"));
        actual = request.Object.SiteUrl();
        Assert.Equal("http://abc.def/", actual);

        // With Port
        request.Setup(s => s.Scheme).Returns("http");
        request.Setup(s => s.Host).Returns(new HostString("abc.def", 123));
        actual = request.Object.SiteUrl();
        Assert.Equal("http://abc.def:123/", actual);
    }

    [Fact]
    public void ToAccountViewModelTests()
    {
        // Null
        UserAccount item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Title = "TITLE" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TITLE", actual.Title);
    }

    [Fact]
    public void ToHousingApplicationViewModelTests()
    {
        // Null
        HousingApplication item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { ListingId = 1, ApplicationId = 1 };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1, actual.ListingId);
        Assert.Equal(1, actual.ApplicationId);
    }

    [Fact]
    public void ToUserNotificationViewModelTests()
    {
        // Null
        UserNotification item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Subject = "SUBJECT", Body = "BODY" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("SUBJECT", actual.Subject);
        Assert.Equal("BODY", actual.Body);
    }

    [Fact]
    public void ToListingViewModelTests()
    {
        // Null
        Listing item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Name = "NAME", StatusCd = "STATUSCD" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("NAME", actual.Name);
        Assert.Equal("STATUSCD", actual.StatusCd);
    }

    [Fact]
    public void ToPrintableFormViewModelTests()
    {
        // Null
        Listing item = null;
        var actual = item.ToPrintableViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Name = "NAME", StatusCd = "STATUSCD" };
        actual = item.ToPrintableViewModel();
        Assert.NotNull(actual);
        Assert.Equal("NAME", actual.Name);
        Assert.Equal("STATUSCD", actual.StatusCd);
    }

    [Fact]
    public void ToListingImageViewModelTests()
    {
        // Null
        ListingImage item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Title = "TITLE", Contents = "CONTENTS" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("TITLE", actual.Title);
        Assert.Equal("CONTENTS", actual.Contents);
    }

    [Fact]
    public void ToListingUnitViewModelTests()
    {
        // Null
        ListingUnit item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { UnitTypeCd = "UNITTYPECD", UnitsAvailableCnt = 1 };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("UNITTYPECD", actual.UnitTypeCd);
        Assert.Equal(1, actual.UnitsAvailableCnt);
    }

    [Fact]
    public void ToListingUnitHouseholdViewModelTests()
    {
        // Null
        ListingUnitHousehold item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { MaxHouseholdIncomeAmt = 1000M };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal(1000M, actual.MaxHouseholdIncomeAmt);
    }

    [Fact]
    public void ToAmenityViewModelTests()
    {
        // Null
        Amenity item = null;
        var actual = item.ToViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Name = "NAME" };
        actual = item.ToViewModel();
        Assert.NotNull(actual);
        Assert.Equal("NAME", actual.Name);
    }

    [Fact]
    public void ToProfileViewModelTests()
    {
        // Null
        UserAccount item = null;
        var actual = item.ToProfileViewModel();
        Assert.Null(actual);

        // Not Null
        item = new() { Username = "USERNAME" };
        actual = item.ToProfileViewModel();
        Assert.NotNull(actual);
        Assert.Equal("USERNAME", actual.Username);
    }

    [Fact]
    public void ToDateEditorStringFormatTests()
    {
        DateTime? input = null;
        var actual = input.ToDateEditorStringFormat();
        Assert.Equal("", actual);

        input = DateTime.MinValue;
        actual = input.ToDateEditorStringFormat();
        Assert.Equal("", actual);

        input = DateTime.Now;
        actual = input.ToDateEditorStringFormat();
        Assert.Equal($"{DateTime.Now:yyyy-MM-dd}", actual);

        input = DateTime.Now;
        actual = input.ToDateEditorStringFormat("M/d/yyyy");
        Assert.Equal($"{DateTime.Now:M/d/yyyy}", actual);
    }
}