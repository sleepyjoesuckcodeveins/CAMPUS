namespace CAMPUS.Notifications;

public interface IEmailSender
{
    /// <summary>Sends one e-mail.</summary>
    Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default);
}