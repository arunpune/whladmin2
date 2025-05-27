using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using WHLAdmin.Common.Extensions;
using WHLAdmin.Common.Models;
using WHLAdmin.Common.Repositories;
using WHLAdmin.Common.Settings;

namespace WHLAdmin.Common.Services;

public interface IEmailService
{
    bool IsValidEmailAddress(string requestId, string correlationId, string input);
    Task<string> SendEmail(string requestId, string correlationId, MailMessage mailMessage);

    Task<bool> SendOtpEmail(string requestId, string correlationId, string emailAddress, string otp);
    Task<bool> SendListingPendingReviewEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing);
    Task<bool> SendListingRequiresRevisionsEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, string reason);
    Task<bool> SendListingPublishedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing);
    Task<bool> SendListingUnpublishedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, string reason);

    Task<bool> SendApplicationSubmittedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application);

    Task<bool> SendPotentialDuplicateOnlineApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application);
    Task<bool> SendPotentialDuplicatePaperApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application);
    Task<bool> SendDuplicateOnlineApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application);
    Task<bool> SendDuplicatePaperApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application);
}

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly INotificationConfigRepository _notificationConfigRepository;
    private readonly IUserRepository _userRepository;
    private readonly SmtpSettings _smtpSettings;

    public EmailService(ILogger<EmailService> logger, INotificationConfigRepository notificationConfigRepository, IUserRepository userRepository, IOptions<SmtpSettings> smtpSettingsOptions)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _notificationConfigRepository = notificationConfigRepository ?? throw new ArgumentNullException(nameof(notificationConfigRepository));
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
            _logger.LogError(formatException, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Invalid Email Address");
            return false;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Invalid Email Address");
            return false;
        }
    }

    public async Task<string> SendEmail(string requestId, string correlationId, MailMessage mailMessage)
    {
        if (_smtpSettings == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Invalid SMTP settings");
            return "E001";
        }

        if (string.IsNullOrEmpty((_smtpSettings?.SmtpHost ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Invalid SMTP host");
            return "E002";
        }

        if ((_smtpSettings?.SmtpPort ?? 0) <= 0)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Invalid SMTP port");
            return "E003";
        }

        if (_smtpSettings.UseAuthentication)
        {
            if (string.IsNullOrEmpty((_smtpSettings?.SmtpUsername ?? "").Trim()))
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Invalid SMTP username");
                return "E004";
            }

            if (string.IsNullOrEmpty((_smtpSettings?.SmtpPassword ?? "").Trim()))
            {
                _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Invalid SMTP password");
                return "E005";
            }
        }

        if (string.IsNullOrEmpty(_smtpSettings?.SmtpFromAddress ?? ""))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - From Address is required");
            return "E006";
        }

        if (string.IsNullOrEmpty((mailMessage?.ToAddresses ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - To Address(es) is(are) required");
            return "E007";
        }

        if (string.IsNullOrEmpty((mailMessage?.Subject ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Subject is required");
            return "E008";
        }

        if (string.IsNullOrEmpty((mailMessage?.Body ?? "").Trim()))
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Body is required");
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
                _logger.LogInformation($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Sent email - {mailMessage?.Subject} to {mailMessage?.ToAddresses}");
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
            _logger.LogError(exception, $"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - System exception");
            return "E010";
        }
        finally
        {
            if (!string.IsNullOrEmpty(mailMessage.NotificationBody))
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

    public async Task<bool> SendOtpEmail(string requestId, string correlationId, string emailAddress, string otp)
    {
        var message = new MailMessage()
        {
            ToAddresses = emailAddress,
            Subject = $"Westchester County HomeSeeker Administration - Sign In Request",
            UseHtmlBody = true,
            Body = $"<p>Your one-time password is {otp}.</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendListingPendingReviewEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Listing Ready for Review" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        var link = $"{siteUrl}Listings/Details?listingId={listing.ListingId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#LISTID]", listing.ListingId.ToString())
                                            .Replace("[#NAME]", listing.Name.Trim())
                                            .Replace("[#ADDRESS]", address)
                                            .Replace("[#USERNAME]", username);
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Listing #{listing.ListingId} Ready for Review",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendListingRequiresRevisionsEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, string reason)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Listing Requires Revisions" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        reason = (reason ?? "").Trim();
        if (reason.EndsWith(".")) reason = reason[..^1];
        if (reason.Length == 0) reason = "Not specified";

        var link = $"{siteUrl}Listings/Details?listingId={listing.ListingId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#LISTID]", listing.ListingId.ToString())
                                            .Replace("[#NAME]", listing.Name.Trim())
                                            .Replace("[#ADDRESS]", address)
                                            .Replace("[#USERNAME]", username)
                                            .Replace("[#REASON]", reason);
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Listing #{listing.ListingId} Requires Revisions",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendListingPublishedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Listing Published" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        var link = $"{siteUrl}Listings/Details?listingId={listing.ListingId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#LISTID]", listing.ListingId.ToString())
                                            .Replace("[#NAME]", listing.Name.Trim())
                                            .Replace("[#ADDRESS]", address)
                                            .Replace("[#USERNAME]", username);
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Listing #{listing.ListingId} Published",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendListingUnpublishedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, string reason)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Listing Unpublished" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        reason = (reason ?? "").Trim();
        if (reason.EndsWith(".")) reason = reason[..^1];
        if (reason.Length == 0) reason = "Not specified";

        var link = $"{siteUrl}Listings/Details?listingId={listing.ListingId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#LISTID]", listing.ListingId.ToString())
                                            .Replace("[#NAME]", listing.Name.Trim())
                                            .Replace("[#ADDRESS]", address)
                                            .Replace("[#USERNAME]", username)
                                            .Replace("[#REASON]", reason);
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Listing #{listing.ListingId} Unpublished for revisions",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendApplicationSubmittedEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Paper-based Housing Application Submitted" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        var link = $"{siteUrl}Applications/Details?applicationId={application.ApplicationId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#APPLID]", application.ApplicationId.ToString()).Replace("[#LISTID]", listing.ListingId.ToString()).Replace("[#ADDRESS]", address).Replace("[#USERNAME]", username);
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Paper-form Housing Application #{application.ApplicationId} Submitted",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendPotentialDuplicateOnlineApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "APPLICANT", Title = "Potential Duplicate Housing Application Notification" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        if (!siteUrl.EndsWith("/")) siteUrl += "/";
        var link = $"{siteUrl}Applications/Details?applicationId={application.ApplicationId}";
        var address = listing.ToAddressTextSingleLine();
        var applicantName = application.ToApplicantName();
        var body = notificationConfig.Text.Replace("[#APPLID]", application.ApplicationId.ToString())
                                        .Replace("[#LISTID]", listing.ListingId.ToString())
                                        .Replace("[#NAME]", listing.Name)
                                        .Replace("[#ADDRESS]", address)
                                        .Replace("[#SUBDATE]", application.SubmittedDate.GetValueOrDefault(DateTime.MinValue).ToString("F"))
                                        .Replace("[#REASON]", application.DuplicateReason)
                                        .Replace("[#DUEDATE]", application.DuplicateCheckResponseDueDate.GetValueOrDefault(DateTime.MinValue).ToString("MMM d, yyyy"))
                                        .Replace("[#BR]", "<br />")
                                        .Replace("[#P]", "</p><p>");
        var message = new MailMessage()
        {
            ToAddresses = application.EmailAddress,
            BccAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Potential Duplicate Housing Application Notification for Application #{application.ApplicationId}",
            UseHtmlBody = true,
            Body = $"<p>Dear {applicantName}</p>"
                    + $"<p>Username {application.Username}</p>"
                    + $"<p>{body}</p>"
                    + $"<p>Reason: {application.DuplicateReason}</p>"
                    + "<ul>"
                        + $"<li>If you wish to Withdraw this application, please click <a href=\"{link}\">here</a>.</li>"
                        + $"<li>If you wish to dispute that this application is a Duplicate, and provide evidence/explanation to the contrary, please click <a href=\"{link}\">here</a>.</li>"
                        + $"<li>If you take no action, this application will be automatically withdrawn from consideration on {application.DuplicateCheckResponseDueDate.GetValueOrDefault(DateTime.MinValue).ToString("MMM d, yyyy")}. It will appear on your Dashboard marked as &quot;Duplicate - Eliminated&quot;.</li>"
                    + "</ul>",
            Username = application.Username,
            NotificationBody = $"Housing application {application.ApplicationId} marked potential duplicate for #{application.ListingId} - {address}. Reason: {application.DuplicateReason}.",
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendPotentialDuplicatePaperApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Potential Duplicate Paper-based Housing Application Notification" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        if (!siteUrl.EndsWith("/")) siteUrl += "/";
        var link = $"{siteUrl}Applications/Details?applicationId={application.ApplicationId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#APPLID]", application.ApplicationId.ToString())
                                        .Replace("[#LISTID]", listing.ListingId.ToString())
                                        .Replace("[#NAME]", listing.Name)
                                        .Replace("[#ADDRESS]", address)
                                        .Replace("[#SUBDATE]", application.SubmittedDate.GetValueOrDefault(DateTime.MinValue).ToString("F"))
                                        .Replace("[#REASON]", application.DuplicateReason)
                                        .Replace("[#APPLNAME]", application.ToApplicantName())
                                        .Replace("[#APPLEMAIL]", application.EmailAddress ?? "Not specified")
                                        .Replace("[#APPLPHONE]", application.PhoneNumber ?? "Not specified")
                                        .Replace("[#DUEDATE]", application.DuplicateCheckResponseDueDate.GetValueOrDefault(DateTime.MinValue).ToString("MMM d, yyyy"))
                                        .Replace("[#BR]", "<br />")
                                        .Replace("[#P]", "</p><p>");
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Potential Duplicate Paper-based Housing Application Notification for Application #{application.ApplicationId}",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendDuplicateOnlineApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "APPLICANT", Title = "Duplicate Housing Application Notification" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        if (!siteUrl.EndsWith("/")) siteUrl += "/";
        var link = $"{siteUrl}Applications/Details?applicationId={application.ApplicationId}";
        var address = listing.ToAddressTextSingleLine();
        var applicantName = application.ToApplicantName();
        var body = notificationConfig.Text.Replace("[#APPLID]", application.ApplicationId.ToString())
                                        .Replace("[#LISTID]", listing.ListingId.ToString())
                                        .Replace("[#NAME]", listing.Name)
                                        .Replace("[#ADDRESS]", address)
                                        .Replace("[#SUBDATE]", application.SubmittedDate.GetValueOrDefault(DateTime.MinValue).ToString("F"))
                                        .Replace("[#REASON]", application.DuplicateReason)
                                        .Replace("[#BR]", "<br />")
                                        .Replace("[#P]", "</p><p>");
        var message = new MailMessage()
        {
            ToAddresses = application.EmailAddress,
            BccAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Duplicate Housing Application Notification for Application #{application.ApplicationId}",
            UseHtmlBody = true,
            Body = $"<p>Dear {applicantName}</p>"
                    + $"<p>Username {application.Username}</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>",
            Username = application.Username,
            NotificationBody = $"Housing application {application.ApplicationId} marked duplicate for #{application.ListingId} - {address}. Reason: {application.DuplicateReason}.",
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }

    public async Task<bool> SendDuplicatePaperApplicationEmail(string requestId, string correlationId, string siteUrl, string username, Listing listing, HousingApplication application)
    {
        var notificationConfig = await _notificationConfigRepository.GetOneByTitle(new NotificationConfig() { CategoryCd = "INTERNAL", Title = "Duplicate Paper-based Housing Application Notification" });
        if (notificationConfig == null)
        {
            _logger.LogError($"RequestId: {requestId}, CorrelationId: {correlationId}, Message: Unable to send email - Missing notification configuration");
            return false;
        }

        if (!siteUrl.EndsWith("/")) siteUrl += "/";
        var link = $"{siteUrl}Applications/Details?applicationId={application.ApplicationId}";
        var address = listing.ToAddressTextSingleLine();
        var body = notificationConfig.Text.Replace("[#APPLID]", application.ApplicationId.ToString())
                                        .Replace("[#LISTID]", listing.ListingId.ToString())
                                        .Replace("[#NAME]", listing.Name)
                                        .Replace("[#ADDRESS]", address)
                                        .Replace("[#SUBDATE]", application.SubmittedDate.GetValueOrDefault(DateTime.MinValue).ToString("F"))
                                        .Replace("[#REASON]", application.DuplicateReason)
                                        .Replace("[#APPLNAME]", application.ToApplicantName())
                                        .Replace("[#APPLEMAIL]", application.EmailAddress ?? "Not specified")
                                        .Replace("[#APPLPHONE]", application.PhoneNumber ?? "Not specified")
                                        .Replace("[#BR]", "<br />")
                                        .Replace("[#P]", "</p><p>");
        var message = new MailMessage()
        {
            ToAddresses = notificationConfig.NotificationList,
            Subject = $"Westchester County HomeSeeker - Duplicate Paper-based Housing Application Notification for Application #{application.ApplicationId}",
            UseHtmlBody = true,
            Body = $"<p>To whom it may concern</p>"
                    + $"<p>{body}</p>"
                    + $"<p>{link}</p>"
        };

        var result = await SendEmail(requestId, correlationId, message);

        return string.IsNullOrEmpty(result);
    }
}