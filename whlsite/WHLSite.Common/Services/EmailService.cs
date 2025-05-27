using System;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WHLSite.Common.Extensions;
using WHLSite.Common.Models;
using WHLSite.Common.Repositories;
using WHLSite.Common.Settings;

namespace WHLSite.Common.Services;

public interface IEmailService
{
    bool IsValidEmailAddress(string requestId, string correlationId, string input);
    Task<string> SendEmail(string requestId, string correlationId, MailMessage mailMessage);

    Task<bool> SendRegistrationEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key);
    Task<bool> ResendActivationEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key);
    Task<bool> SendAccountActivatedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress);
    Task<bool> SendPasswordResetRequestEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key);
    Task<bool> SendPasswordChangedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress);
    Task<bool> SendApplicationSubmittedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application);
    Task<bool> SendApplicationWithdrawnEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application);
    Task<bool> SendApplicationCommentEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application, string comment);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly SmtpSettings _smtpSettings;
    private readonly IUserRepository _userRepository;

    public EmailService(ILogger<EmailService> logger, IUserRepository userRepository, IOptions<SmtpSettings> smtpSettingsOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));

        if (smtpSettingsOptions != null)
        {
            _smtpSettings = smtpSettingsOptions.Value ?? throw new ArgumentNullException(nameof(smtpSettingsOptions));
            _smtpSettings.SmtpHost = (_smtpSettings.SmtpHost ?? "").Trim();
            _smtpSettings.SmtpUsername = (_smtpSettings.SmtpUsername ?? "").Trim();
            _smtpSettings.SmtpPassword = (_smtpSettings.SmtpPassword ?? "").Trim();
            _smtpSettings.SmtpFromAddress = (_smtpSettings.SmtpFromAddress ?? "").Trim();
            if (!_smtpSettings.UseSsl)
            {
                _logger.LogWarning($"WARNING: SMTP does not use SSL - this is not a good security practice!");
            }
            if (!_smtpSettings.UseAuthentication)
            {
                _logger.LogWarning($"WARNING: SMTP does not use authentication - this is not a good security practice!");
            }
        }
        else
        {
            throw new ArgumentNullException(nameof(smtpSettingsOptions));
        }
    }

    public bool IsValidEmailAddress(string requestId, string correlationId, string input)
    {
        try
        {
            var emailAddress = new System.Net.Mail.MailAddress(input).Address;
            return true;
        }
        catch (FormatException formatException)
        {
            _logger.LogError(formatException, $"Invalid Email Address");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Invalid Email Address");
            return false;
        }
    }

    public async Task<string> SendEmail(string requestId, string correlationId, MailMessage mailMessage)
    {
        if (_smtpSettings == null)
        {
            _logger.LogError($"Unable to send email - Invalid SMTP settings");
            return "E001";
        }

        if (string.IsNullOrEmpty((_smtpSettings?.SmtpHost ?? "").Trim()))
        {
            _logger.LogError($"Unable to send email - Invalid SMTP host");
            return "E002";
        }

        if ((_smtpSettings?.SmtpPort ?? 0) <= 0)
        {
            _logger.LogError($"Unable to send email - Invalid SMTP port");
            return "E003";
        }

        if (_smtpSettings.UseAuthentication)
        {
            if (string.IsNullOrEmpty((_smtpSettings?.SmtpUsername ?? "").Trim()))
            {
                _logger.LogError($"Unable to send email - Invalid SMTP username");
                return "E004";
            }

            if (string.IsNullOrEmpty((_smtpSettings?.SmtpPassword ?? "").Trim()))
            {
                _logger.LogError($"Unable to send email - Invalid SMTP password");
                return "E005";
            }
        }

        if (string.IsNullOrEmpty(_smtpSettings?.SmtpFromAddress ?? ""))
        {
            _logger.LogError($"Unable to send email - From Address is required");
            return "E006";
        }

        if (string.IsNullOrEmpty((mailMessage?.ToAddresses ?? "").Trim()))
        {
            _logger.LogError($"Unable to send email - To Address(es) is(are) required");
            return "E007";
        }

        if (string.IsNullOrEmpty((mailMessage?.Subject ?? "").Trim()))
        {
            _logger.LogError($"Unable to send email - Subject is required");
            return "E008";
        }

        if (string.IsNullOrEmpty((mailMessage?.Body ?? "").Trim()))
        {
            _logger.LogError($"Unable to send email - Body is required");
            return "E009";
        }

        var emailSentInd = false;

        try
        {
            var fromAddress = new MailboxAddress(_smtpSettings.SmtpFromName, _smtpSettings.SmtpFromAddress);

            var useHtmlBody = mailMessage?.UseHtmlBody ?? false;
            var bodyBuilder = new BodyBuilder
            {
                TextBody = useHtmlBody ? null : mailMessage?.Body,
                HtmlBody = useHtmlBody ? $"<!DOCTYPE html><html lang=\"en\"><head><meta charset=\"utf-8\" /><meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\" /><title>${mailMessage?.Subject}</title></head><body style=\"font-family: Arial;font-size: 1em;\">{mailMessage?.Body}</body></html>" : null
            };

            var message = new MimeMessage();
            message.From.Add(fromAddress);
            foreach (var a in mailMessage?.ToAddresses.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(MailboxAddress.Parse(a));
            }
            mailMessage.CcAddresses = (mailMessage.CcAddresses ?? "").Trim();
            if (!string.IsNullOrEmpty(mailMessage.CcAddresses))
            {
                foreach (var a in mailMessage?.CcAddresses.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Cc.Add(MailboxAddress.Parse(a));
                }
            }
            mailMessage.BccAddresses = (mailMessage.BccAddresses ?? "").Trim();
            if (!string.IsNullOrEmpty(mailMessage.BccAddresses))
            {
                foreach (var a in mailMessage?.BccAddresses.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
                {
                    message.Bcc.Add(MailboxAddress.Parse(a));
                }
            }
            message.Subject = mailMessage.Subject;
            message.Body = bodyBuilder.ToMessageBody();

            if (_smtpSettings.Enabled)
            {
                using (var client = new SmtpClient())
                {
                    var tlsOption = _smtpSettings.UseSsl ? MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable : MailKit.Security.SecureSocketOptions.None;
                    client.Connect(_smtpSettings.SmtpHost, _smtpSettings.SmtpPort, tlsOption);
                    if (_smtpSettings.UseAuthentication)
                    {
                        client.Authenticate(_smtpSettings.SmtpUsername, _smtpSettings.SmtpPassword);
                    }
                    await client.SendAsync(message);
                    client.Disconnect(true);

                    emailSentInd = true;
                };
                _logger.LogInformation($"Sent email - {mailMessage?.Subject} to {mailMessage?.ToAddresses}");
            }
            else
            {
                _logger.LogInformation($"Email sending is disabled - {mailMessage?.Subject} to {mailMessage?.ToAddresses}");
                _logger.LogDebug($"Email message - {mailMessage?.Body}");
            }
            return "";
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Unable to send email - System exception");
            return "E010";
        }
        finally
        {
            if (!string.IsNullOrEmpty(mailMessage.Username))
            {
                try
                {
                    await _userRepository.AddNotification(requestId, correlationId, new UserNotification()
                    {
                        Username = mailMessage.Username,
                        Subject = mailMessage.Subject,
                        Body = mailMessage.NotificationBody,
                        EmailSentInd = emailSentInd,
                        CreatedBy = "SYSTEM"
                    });
                }
                catch (Exception dbException)
                {
                    _logger.LogError(dbException, $"Unable to add notification to database - System exception");
                }
            }
        }
    }

    public async Task<bool> SendRegistrationEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key)
    {
        var link = $"{siteUrl}Account/Activate?k={key}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = "New HomeSeeker account created, pending activation.",
            ToAddresses = emailAddress,
            Subject = "Westchester County HomeSeeker - Welcome",
            UseHtmlBody = true,
            Body = $"<h4>Welcome, <strong>{username}</strong></h4>"
                    + $"<p><a href=\"{link}\">Click</a> to activate your new account. This link will expire in 10 minutes. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not register with this site, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> ResendActivationEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key)
    {
        var link = $"{siteUrl}Account/Activate?k={key}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = "Account activation link requested, pending activation.",
            ToAddresses = emailAddress,
            Subject = "Westchester County HomeSeeker - Account Activation Request",
            UseHtmlBody = true,
            Body = $"<p>Use this <a href=\"{link}\">link</a> to activate your account. This link will expire in 10 minutes. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not request an activation link, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendAccountActivatedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress)
    {
        var link = $"{siteUrl}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = "Account activated.",
            ToAddresses = emailAddress,
            Subject = "Westchester County HomeSeeker - Account Activated",
            UseHtmlBody = true,
            Body = $"<p>Dear <strong>{username}</strong></p>"
                    + $"<p>Your <strong>HomeSeeker</strong> account has now been activated. You may now <a href=\"{link}\">access</a> the site to view and apply for relevant opportunities. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not create an account with HomeSeeker, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendPasswordResetRequestEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, string key)
    {
        var link = $"{siteUrl}Account/ResetPassword?k={key}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = "Password reset requested.",
            ToAddresses = emailAddress,
            Subject = "Westchester County HomeSeeker - Password Reset Request",
            UseHtmlBody = true,
            Body = $"<p>Use this <a href=\"{link}\">link</a> to reset the password for your <strong>HomeSeeker</strong> account. This link will expire in 10 minutes. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not request a password reset, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendPasswordChangedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress)
    {
        var link = $"{siteUrl}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = "Password changed.",
            ToAddresses = emailAddress,
            Subject = "Westchester County HomeSeeker - Password Change Confirmation",
            UseHtmlBody = true,
            Body = $"<p>Dear <strong>{username}</strong></p>"
                    + $"<p>This is to confirm that the password for your <strong>HomeSeeker</strong> account has been changed. You may now <a href=\"{link}\">access</a> the site to view and apply for relevant opportunities. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not initiate this action, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendApplicationSubmittedEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application)
    {
        var link = $"{siteUrl}Applications/Submitted?applicationId={application.ApplicationId}";
        var displayName = $"{application.ToDisplayName()}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = $"Housing application {application.ApplicationId} submitted for #{application.ListingId} - {application.ListingAddress}.",
            ToAddresses = emailAddress,
            Subject = $"Westchester County HomeSeeker - Housing Application {application.ApplicationId} Submitted",
            UseHtmlBody = true,
            Body = $"<p>Dear <strong>{displayName}</strong></p>"
                    + $"<p>This is to confirm that you have submitted housing application #{application.ApplicationId} for #{application.ListingId} - {application.ListingAddress} from your HomeSeeker account at {application.SubmittedDate:F}. It will appear on your Dashboard marked as &quot;{application.StatusDescription}&quot;. You may review the submission <a href=\"{link}\">here</a>. If the link does not work, copy and paste the following URL into your browser window.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not initiate this action, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendApplicationWithdrawnEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application)
    {
        var link = $"{siteUrl}Applications/Withdrawn?applicationId={application.ApplicationId}";
        var displayName = $"{application.ToDisplayName()}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = $"Housing application {application.ApplicationId} withdrawn for #{application.ListingId} - {application.ListingAddress}.",
            ToAddresses = emailAddress,
            Subject = $"Westchester County HomeSeeker - Housing Application {application.ApplicationId} Withdrawn",
            UseHtmlBody = true,
            Body = $"<p>Dear <strong>{displayName}</strong></p>"
                    + $"<p>This is to confirm that your application for #{application.ListingId} - {application.ListingAddress} has been withdrawn. It will appear on your Dashboard as &quot;Withdrawn&quot;</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + "<p>If you did not initiate this action, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendApplicationCommentEmail(string requestId, string correlationId, string siteUrl, string username, string emailAddress, HousingApplication application, string comment)
    {
        var link = $"{siteUrl}Applications/Submitted?applicationId={application.ApplicationId}";
        var displayName = $"{application.ToDisplayName()}";
        var message = new MailMessage()
        {
            Username = username,
            NotificationBody = $"Housing application {application.ApplicationId} for #{application.ListingId} - {application.ListingAddress} - Comment: {comment}.",
            ToAddresses = emailAddress,
            BccAddresses = "tifa@westchestercountyny.gov, jee2@westchestercountyny.gov, jmlh@westchestercountyny.gov, asmith@wroinc.org",
            Subject = $"Westchester County HomeSeeker - Housing Application {application.ApplicationId} - Comment Added",
            UseHtmlBody = true,
            Body = $"<p>Dear <strong>{displayName}</strong></p>"
                    + $"<p>This is to confirm that you have added the following comment to your housing application #{application.ApplicationId} for #{application.ListingId} - {application.ListingAddress} from your HomeSeeker account at {DateTime.Now:F}.</p>"
                    + $"<p>{link}</p>"
                    + "<p>&nbsp;</p>"
                    + $"<p>Comment: {comment}</p>"
                    + "<p>If you did not initiate this action, please reach out to us immediately at <a href=\"mailto:homeseeker@westchestercountyny.gov\">homeseeker@westchestercountyny.gov</a>.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }
}