using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace CAMPUS.Notifications;

public class SmtpEmailSender(IOptions<EmailOptions> options) : IEmailSender
{
    private readonly EmailOptions _options = options.Value;

    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mail = new MimeMessage();
        mail.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        mail.To.Add(new MailboxAddress(message.ToName, message.ToAddress));
        mail.Subject = message.Subject;
        mail.Body = new BodyBuilder
        {
            TextBody = message.TextBody,
            HtmlBody = message.HtmlBody
        }.ToMessageBody();

        using var client = new SmtpClient { Timeout = 10000 };
        var security = _options.UseSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        await client.ConnectAsync(_options.Host, _options.Port, security, cancellationToken);
        await client.SendAsync(mail, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}