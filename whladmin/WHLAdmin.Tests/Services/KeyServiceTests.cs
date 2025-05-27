using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Moq;
using WHLAdmin.Common.Services;

namespace WHLAdmin.Tests.Services;

public class KeyServiceTests()
{
    private readonly Mock<ILogger<KeyService>> _logger = new();

    [Fact]
    public void ConstructorTests()
    {
        // ArgumentNullExceptions
        Assert.Throws<ArgumentNullException>(() => new KeyService(null));

        // Not Null
        var actual = new KeyService(_logger.Object);
        Assert.NotNull(actual);
    }

    [Fact]
    public void IsValidPasswordTests()
    {
        var service = new KeyService(_logger.Object);

        // Null Input
        var actual = service.IsValidPassword(null);
        Assert.False(actual);

        // Empty Input
        actual = service.IsValidPassword("");
        Assert.False(actual);

        // Non-empty Input
        actual = service.IsValidPassword(" ");
        Assert.False(actual);

        // Alpha-only Input
        actual = service.IsValidPassword("ABCDEFGHIJKLMNO");
        Assert.False(actual);

        // Alphanumeric-only Input
        actual = service.IsValidPassword("ABC123abc123abc");
        Assert.False(actual);

        // Numeric-only Input
        actual = service.IsValidPassword("12345678901234");
        Assert.False(actual);

        // Symbol-only Input
        actual = service.IsValidPassword("!@#$%^&*(){}[]|");
        Assert.False(actual);

        // Less than 14 character input with at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 symbol
        actual = service.IsValidPassword("Abc!23abc123");
        Assert.False(actual);

        // Valid 14 character input with at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 symbol
        actual = service.IsValidPassword("Abc!23abc123ab");
        Assert.True(actual);

        // Valid more than 14 character input with at least 1 uppercase letter, 1 lowercase letter, 1 number and 1 symbol
        actual = service.IsValidPassword("Abc!23abc123ab!2");
        Assert.True(actual);
    }

    [Fact]
    public void GetPasswordHashTests()
    {
        var service = new KeyService(_logger.Object);

        var input = "Abc!23abc123abc";

        // Setup
        using var sha = SHA512.Create();
        sha.Initialize();
        var passwordBytes = Encoding.UTF8.GetBytes(input);
        var encryptedBytes = sha.ComputeHash(passwordBytes);
        var expected = Convert.ToBase64String(encryptedBytes);
    
        // Test
        var actual = service.GetPasswordHash(input);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void GetActivationKeyTests()
    {
        var service = new KeyService(_logger.Object);

        // Default length (12)
        var actual = service.GetActivationKey();
        Assert.Matches("^[A-Za-z0-9]{12}$", actual);

        // Minimum length (8)
        actual = service.GetActivationKey(4);
        Assert.Matches("^[A-Za-z0-9]{8}$", actual);

        // Maximum length (16)
        actual = service.GetActivationKey(20);
        Assert.Matches("^[A-Za-z0-9]{16}$", actual);
    }

    [Fact]
    public void GetOtpTests()
    {
        var service = new KeyService(_logger.Object);

        // Default length (8)
        var actual = service.GetOtp();
        Assert.Matches("^[0-9]{8}$", actual);

        // Minimum length (8)
        actual = service.GetOtp(4);
        Assert.Matches("^[0-9]{8}$", actual);

        // Maximum length (16)
        actual = service.GetOtp(20);
        Assert.Matches("^[0-9]{16}$", actual);
    }
}