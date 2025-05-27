using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace WHLSite.Common.Services;

public interface IKeyService
{
    bool IsValidUsername(string input);
    bool IsValidPassword(string input);
    string GetPasswordHash(string input);
    string GetActivationKey(int length = 12);
    bool IsValidName(string input);
    bool IsValidNameWithSpecialCharacters(string input);
}

public partial class KeyService : IKeyService
{
    private readonly ILogger<KeyService> _logger;
    private readonly Random _random = new Random();

    public KeyService(ILogger<KeyService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public bool IsValidUsername(string input)
    {
        input ??= string.Empty;
        return UsernameRegex().IsMatch(input);
    }

    public bool IsValidPassword(string input)
    {
        input ??= string.Empty;
        return PasswordRegex().IsMatch(input);
    }

    public string GetPasswordHash(string input)
    {
        using var sha = SHA512.Create();
        sha.Initialize();
        var passwordBytes = Encoding.UTF8.GetBytes(input);
        var encryptedBytes = sha.ComputeHash(passwordBytes);
        return Convert.ToBase64String(encryptedBytes);
    }

    public string GetActivationKey(int length = 12)
    {
        if (length < 8) length = 8;
        if (length > 16) length = 16;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }

    [ExcludeFromCodeCoverage]
    [GeneratedRegex(@"^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*\W)(?!.* ).{14,}$")]
    private static partial Regex PasswordRegex();

    [ExcludeFromCodeCoverage]
    //[GeneratedRegex(@"^[A-Za-z]{1}[A-Za-z0-9]{7,31}$")]
    [GeneratedRegex(@"^(?!.*(.).*\1{2})([A-Za-z]{1}[A-Za-z0-9]{7,31})$")]
    private static partial Regex UsernameRegex();

    public bool IsValidName(string input)
    {
        input ??= string.Empty;
        return NameRegex().IsMatch(input);
    }

    [ExcludeFromCodeCoverage]
    [GeneratedRegex(@"^([A-Za-z]+([\'\-]?[A-Za-z]+)?)([\ \-]?)(([A-Za-z]+([\'\-]?[A-Za-z]+)?)?)$")]
    private static partial Regex NameRegex();

    public bool IsValidNameWithSpecialCharacters(string input)
    {
        input ??= string.Empty;
        return NameWithSpecialCharactersRegex().IsMatch(input);
    }

    [ExcludeFromCodeCoverage]
    [GeneratedRegex(@"^([A-Za-z\u00C0-\u017F]+([\'\-]?[A-Za-z\u00C0-\u017F]+)?)([\ \-]?)(([A-Za-z\u00C0-\u017F]+([\'\-]?[A-Za-z\u00C0-\u017F]+)?)?)$")]
    private static partial Regex NameWithSpecialCharactersRegex();
}