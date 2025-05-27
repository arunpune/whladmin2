using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Extensions;

namespace WHLAdmin.Tests.Extensions;

public class CommonModelExtensionsTests()
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ToDataTableTests(bool withLogger)
    {
        var logger = new Mock<ILogger>().Object;

        // Null Test
        List<ListingUnitHousehold> list = null;
        var actual = list.ToDataTable(withLogger ? logger : It.IsAny<ILogger>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Rows);

        // Null Test
        list = [];
        actual = list.ToDataTable(withLogger ? logger : It.IsAny<ILogger>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.Empty(actual.Rows);

        // One or more items Test
        list.Add(new ListingUnitHousehold());
        actual = list.ToDataTable(withLogger ? logger : It.IsAny<ILogger>(), It.IsAny<string>());
        Assert.NotNull(actual);
        Assert.NotEmpty(actual.Rows);
    }

    [Theory]
    [InlineData(true, null, null, null, null, null, null, null, "")]
    [InlineData(false, null, null, null, null, null, null, null, "")]
    [InlineData(false, "", null, null, null, null, null, null, "")]
    [InlineData(false, " ", null, null, null, null, null, null, "")]
    [InlineData(false, " LINE 1", null, null, null, null, null, null, "LINE 1")]
    [InlineData(false, "LINE 1 ", null, null, null, null, null, null, "LINE 1")]
    [InlineData(false, " LINE 1 ", null, null, null, null, null, null, "LINE 1")]
    [InlineData(false, "LINE 1", null, null, null, null, null, null, "LINE 1")]
    [InlineData(false, "LINE 1", "", null, null, null, null, null, "LINE 1")]
    [InlineData(false, "LINE 1", " ", null, null, null, null, null, "LINE 1")]
    [InlineData(false, "LINE 1", " LINE 2", null, null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", "LINE 2 ", null, null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", " LINE 2 ", null, null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", "LINE 2", null, null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", "LINE 2", "", null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", "LINE 2", " ", null, null, null, null, "LINE 1, LINE 2")]
    [InlineData(false, "LINE 1", "LINE 2", " LINE 3", null, null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3 ", null, null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", " LINE 3 ", null, null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", null, null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "", null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", " ", null, null, null, "LINE 1, LINE 2, LINE 3")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", " CITY", null, null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY ", null, null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", " CITY ", null, null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", null, null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "", null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", " ", null, null, "LINE 1, LINE 2, LINE 3, CITY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", " ST", null, null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST ", null, null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", " ST ", null, null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", null, null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "", null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", " ", null, "LINE 1, LINE 2, LINE 3, CITY, ST")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", " ZIPCD", null, "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD ", null, "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", " ZIPCD ", null, "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", null, "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", "", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", " ", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", " COUNTY", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD COUNTY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", "COUNTY ", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD COUNTY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", " COUNTY ", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD COUNTY")]
    [InlineData(false, "LINE 1", "LINE 2", "LINE 3", "CITY", "ST", "ZIPCD", "COUNTY", "LINE 1, LINE 2, LINE 3, CITY, ST ZIPCD COUNTY")]
    public void ToAddressTextSingleLineTests(bool nullInput
                                                , string line1, string line2, string line3
                                                , string city, string stateCd, string zipCode
                                                , string county, string expected)
    {
        Listing listing = nullInput ? null : new Listing()
        {
            StreetLine1 = line1,
            StreetLine2 = line2,
            StreetLine3 = line3,
            City = city,
            StateCd = stateCd,
            ZipCode = zipCode,
            County = county
        };

        var actual = listing.ToAddressTextSingleLine();
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(true, null, null, null, null, "")]
    [InlineData(false, null, null, null, null, "")]
    [InlineData(false, "", null, null, null, "")]
    [InlineData(false, " ", null, null, null, "")]
    [InlineData(false, " FIRST", null, null, null, "FIRST")]
    [InlineData(false, "FIRST ", null, null, null, "FIRST")]
    [InlineData(false, " FIRST ", null, null, null, "FIRST")]
    [InlineData(false, "FIRST", null, null, null, "FIRST")]
    [InlineData(false, "FIRST", "", null, null, "FIRST")]
    [InlineData(false, "FIRST", " ", null, null, "FIRST")]
    [InlineData(false, "FIRST", " MIDDLE", null, null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", "MIDDLE ", null, null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", " MIDDLE ", null, null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", "MIDDLE", null, null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", "MIDDLE", "", null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", "MIDDLE", " ", null, "FIRST MIDDLE")]
    [InlineData(false, "FIRST", "MIDDLE", " LAST", null, "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST ", null, "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", " LAST ", null, "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", null, "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", "", "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", " ", "FIRST MIDDLE LAST")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", " SUFFIX", "FIRST MIDDLE LAST SUFFIX")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", "SUFFIX ", "FIRST MIDDLE LAST SUFFIX")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", " SUFFIX ", "FIRST MIDDLE LAST SUFFIX")]
    [InlineData(false, "FIRST", "MIDDLE", "LAST", "SUFFIX", "FIRST MIDDLE LAST SUFFIX")]
    public void ToApplicantNameTests(bool nullInput, string firstName, string middleName, string lastName, string suffix, string expected)
    {
        HousingApplication application = nullInput ? null : new HousingApplication()
        {
            FirstName = firstName,
            MiddleName = middleName,
            LastName = lastName,
            Suffix = suffix
        };

        var actual = application.ToApplicantName();
        Assert.Equal(expected, actual);
    }
}