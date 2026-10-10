using CAMPUS.BLL.Notifications;
using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Formatting;
using CAMPUS.DOMAIN.Repositories;
using Microsoft.Extensions.Logging;

namespace CAMPUS.Notifications;

/// <summary>Sends the CAMPUS e-mails and logs every one of them. It never throws, so an e-mail can never break an action.</summary>
public class EmailNotificationService(
    IEmailSender emailSender,
    EmailTemplateBuilder templateBuilder,
    IUserRepository userRepository,
    INotificationLogRepository notificationLogRepository,
    TimeProvider timeProvider,
    ILogger<EmailNotificationService> logger) : INotificationService
{
    private const int MaxErrorLength = 1000;

    public async Task RequestSubmittedAsync(ServiceRequest request)
    {
        try
        {
            var managers = await userRepository.GetByRoleAsync(Roles.FacilitiesManager);
            if (managers.Count == 0)
            {
                logger.LogWarning("No facilities managers found, so no e-mail was sent for {RequestCode}.", request.RefCode());
                return;
            }

            foreach (var manager in managers)
            {
                var message = templateBuilder.BuildRequestSubmitted(request, manager);
                await SendAndLogAsync(message, request.Id, NotificationType.RequestSubmitted);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Sending the submitted e-mails for {RequestCode} failed.", request.RefCode());
        }
    }

    public async Task RequestAssignedAsync(ServiceRequest request)
    {
        try
        {
            if (request.AssignedTeamId is null)
            {
                logger.LogWarning("{RequestCode} has no team, so no assigned e-mail was sent.", request.RefCode());
                return;
            }

            if (request.Reporter is null || string.IsNullOrWhiteSpace(request.Reporter.Email))
            {
                logger.LogWarning("{RequestCode} has no reporter e-mail, so the reporter gets no e-mail.", request.RefCode());
            }
            else
            {
                var reporterMessage = templateBuilder.BuildRequestAssigned(request);
                await SendAndLogAsync(reporterMessage, request.Id, NotificationType.RequestAssigned);
            }

            var members = await userRepository.GetByTeamAsync(request.AssignedTeamId.Value);
            if (members.Count == 0)
            {
                logger.LogInformation("The team of {RequestCode} has no members, so no team e-mail was sent.", request.RefCode());
                return;
            }

            foreach (var member in members)
            {
                var memberMessage = templateBuilder.BuildTeamAssigned(request, member);
                await SendAndLogAsync(memberMessage, request.Id, NotificationType.TeamAssigned);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Sending the assigned e-mails for {RequestCode} failed.", request.RefCode());
        }
    }

    // Sends one e-mail and saves one NotificationLog row, whatever happens.
    private async Task SendAndLogAsync(EmailMessage message, int requestId, NotificationType type)
    {
        var succeeded = true;
        string? error = null;

        try
        {
            await emailSender.SendAsync(message);
            logger.LogInformation("{Type} e-mail for {RequestCode} was sent.", type, requestId.ToRefCode());
        }
        catch (Exception exception)
        {
            succeeded = false;
            error = exception.Message.Length > MaxErrorLength ? exception.Message[..MaxErrorLength] : exception.Message;
            logger.LogWarning(exception, "{Type} e-mail for {RequestCode} could not be sent.", type, requestId.ToRefCode());
        }

        try
        {
            await notificationLogRepository.AddAsync(new NotificationLog
            {
                ServiceRequestId = requestId,
                Type = type,
                RecipientEmail = message.ToAddress,
                Subject = message.Subject,
                SentAt = timeProvider.GetUtcNow().UtcDateTime,
                Succeeded = succeeded,
                Error = error
            });
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The log of the {Type} e-mail for {RequestCode} could not be saved.", type, requestId.ToRefCode());
        }
    }
}
