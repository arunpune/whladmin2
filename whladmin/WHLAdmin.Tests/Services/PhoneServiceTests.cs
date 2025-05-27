using System;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Services;

namespace WHLAdmin.Tests.Services;

public class PhoneServiceTests()
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

    [Fact]
    public void IsValidPhoneNumberTests()
    {
        var service = new PhoneService(_logger.Object);

        // Null Input
        var actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), null);
        Assert.False(actual);

        // Empty Input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "");
        Assert.False(actual);

        // Non-empty Input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), " ");
        Assert.False(actual);

        // Alpha Input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "ABC");
        Assert.False(actual);

        // Alphanumeric Input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "ABC1234567890");
        Assert.False(actual);

        // Character Input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "^&*^*^(*)");
        Assert.False(actual);

        // Longer than 10 numbers input
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "12345678901");
        Assert.False(actual);

        // Valid 10-digit number
        actual = service.IsValidPhoneNumber(It.IsAny<string>(), It.IsAny<string>(), "1234567890");
        Assert.True(actual);
    }
}