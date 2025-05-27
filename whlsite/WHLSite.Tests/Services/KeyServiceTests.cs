using Microsoft.Extensions.Logging;
using Moq;
using WHLSite.Common.Services;

namespace WHLSite.Tests.Services;

public class KeyServiceTests
{
    private readonly Mock<ILogger<KeyService>> _logger;

    public KeyServiceTests()
    {
        _logger = new Mock<ILogger<KeyService>>();
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("abc", false)]
    [InlineData("123", false)]
    [InlineData("ABC", false)]
    [InlineData("Ab3", false)]
    [InlineData("*&^%", false)]
    [InlineData("AB C", false)]
    [InlineData("1abcdefgh", false)]
    [InlineData("a1b2c3d4", true)]
    [InlineData("a1b2c3d4a1b2c3d4a1b2c3d4a1b2c3d4", true)]
    [InlineData("a1b2c3d4a1b2c3d4a1b2c3d4a1b2c3d4a", false)]
    public void IsValidUsername_Tests(string input, bool expected)
    {
        var service = new KeyService(_logger.Object);
        var actual = service.IsValidUsername(input);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("abc", false)]
    [InlineData("123", false)]
    [InlineData("ABC", false)]
    [InlineData("Ab3", false)]
    [InlineData("*&^%", false)]
    [InlineData("AB C", false)]
    [InlineData("Abc!23Abc!23Abc!23", true)]
    public void IsValidPassword_Tests(string input, bool expected)
    {
        var service = new KeyService(_logger.Object);
        var actual = service.IsValidPassword(input);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("ABC")]
    public void GetPasswordHashTests(string input)
    {
        var service = new KeyService(_logger.Object);
        var actual = service.GetPasswordHash(input);
        Assert.NotEmpty(actual);
    }

    [Theory]
    [InlineData(0, 8)]
    [InlineData(4, 8)]
    [InlineData(8, 8)]
    [InlineData(12, 12)]
    [InlineData(16, 16)]
    [InlineData(17, 16)]
    public void GetActivationKeyTests(int length, int expectedLength)
    {
        var service = new KeyService(_logger.Object);
        var actual = service.GetActivationKey(length);
        Assert.NotEmpty(actual);
        Assert.Equal(expectedLength, actual.Length);
    }

    [Theory]
    [InlineData(null, false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    [InlineData("abc", true)]
    [InlineData("123", false)]
    [InlineData("ABC", true)]
    [InlineData("Ab3", false)]
    [InlineData("*&^%", false)]
    [InlineData("AB C", true)]
    [InlineData("1abcdefgh", false)]
    [InlineData("a1b2c3d4", false)]
    [InlineData("a1b2c3d4a1b2c3d4a1b2c3d4a1b2c3d4", false)]
    [InlineData("a1b2c3d4a1b2c3d4a1b2c3d4a1b2c3d4a", false)]
    [InlineData("John Doe", true)]
    [InlineData("Jacques-Smith Doe", true)]
    [InlineData("O'Dean Smith", true)]
    [InlineData("Sean O'Connor", true)]
    [InlineData("Jamie Smith-Roe", true)]
    [InlineData("Sean-Jean O'Connor", true)]
    [InlineData("A R", true)]
    public void IsValidName_Tests(string input, bool expected)
    {
        var service = new KeyService(_logger.Object);
        var actual = service.IsValidName(input);
        Assert.Equal(expected, actual);
    }
}