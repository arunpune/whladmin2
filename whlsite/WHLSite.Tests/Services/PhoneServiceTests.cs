using System;
using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Services;

namespace WHLSite.Tests.Services;

public class PhoneServiceTests
{
    private readonly Mock<ILogger<PhoneService>> _logger = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new PhoneService(null));

        // Not Null
        var actual = new PhoneService(_logger.Object);
        Assert.NotNull(actual);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("123", false)]
    [InlineData("123 456 7890", false)]
    [InlineData("1234567890", true)]
    public void IsValidPhoneNumberTests(string input, bool expected)
    {
        var service = new PhoneService(_logger.Object);
        var actual = service.IsValidPhoneNumber(input);
        Assert.Equal(expected, actual);
    }
}