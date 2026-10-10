using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.DOMAIN.Repositories;
using CAMPUS.Notifications;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace CAMPUS.Tests.Notifications;

public class EmailNotificationServiceTests
{
    private static readonly DateTime Now = new(2026, 10, 10, 12, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IEmailSender> _sender = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<INotificationLogRepository> _logs = new();
    private readonly Mock<TimeProvider> _time = new();
    private readonly List<NotificationLog> _savedLogs = new();
    private readonly EmailNotificationService _service;

    public EmailNotificationServiceTests()
    {
        _time.Setup(t => t.GetUtcNow()).Returns(new DateTimeOffset(Now));
        _logs.Setup(l => l.AddAsync(It.IsAny<NotificationLog>()))
            .Callback<NotificationLog>(log => _savedLogs.Add(log))
            .Returns(Task.CompletedTask);
        _sender.Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _service = new EmailNotificationService(
            _sender.Object,
            new EmailTemplateBuilder(NotificationTestData.Options()),
            _users.Object,
            _logs.Object,
            _time.Object,
            NullLogger<EmailNotificationService>.Instance);
    }

    [Fact]
    public async Task RequestSubmittedAsync_OneManager_SendsAndLogsRequestSubmitted()
    {
        // Arrange
        _users.Setup(u => u.GetByRoleAsync(Roles.FacilitiesManager)).ReturnsAsync([NotificationTestData.Alex()]);

        // Act
        await _service.RequestSubmittedAsync(NotificationTestData.Request2());

        // Assert
        _sender.Verify(s => s.SendAsync(It.Is<EmailMessage>(m => m.ToAddress == "manager@campus.test"), It.IsAny<CancellationToken>()), Times.Once);
        var log = Assert.Single(_savedLogs);
        Assert.Equal(NotificationType.RequestSubmitted, log.Type);
        Assert.Equal(2, log.ServiceRequestId);
        Assert.Equal("manager@campus.test", log.RecipientEmail);
        Assert.Equal(Now, log.SentAt);
        Assert.True(log.Succeeded);
        Assert.Null(log.Error);
    }

    [Fact]
    public async Task RequestSubmittedAsync_NoManagers_SendsNothing()
    {
        // Arrange
        _users.Setup(u => u.GetByRoleAsync(Roles.FacilitiesManager)).ReturnsAsync([]);

        // Act
        await _service.RequestSubmittedAsync(NotificationTestData.Request2());

        // Assert
        _sender.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.Empty(_savedLogs);
    }

    [Fact]
    public async Task RequestSubmittedAsync_UserRepositoryThrows_DoesNotThrow()
    {
        // Arrange
        _users.Setup(u => u.GetByRoleAsync(It.IsAny<string>())).ThrowsAsync(new InvalidOperationException("Database down"));

        // Act
        var exception = await Record.ExceptionAsync(() => _service.RequestSubmittedAsync(NotificationTestData.Request2()));

        // Assert
        Assert.Null(exception);
        _sender.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RequestAssignedAsync_OneMember_SendsToReporterAndMember()
    {
        // Arrange
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([NotificationTestData.Sam()]);

        // Act
        await _service.RequestAssignedAsync(NotificationTestData.Request2());

        // Assert
        _sender.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Collection(_savedLogs,
            log =>
            {
                Assert.Equal(NotificationType.RequestAssigned, log.Type);
                Assert.Equal("employee2@campus.test", log.RecipientEmail);
            },
            log =>
            {
                Assert.Equal(NotificationType.TeamAssigned, log.Type);
                Assert.Equal("support@campus.test", log.RecipientEmail);
            });
    }

    [Fact]
    public async Task RequestAssignedAsync_TeamWithoutMembers_SendsOnlyToReporter()
    {
        // Arrange
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([]);

        // Act
        await _service.RequestAssignedAsync(NotificationTestData.Request2());

        // Assert
        _sender.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        var log = Assert.Single(_savedLogs);
        Assert.Equal("employee2@campus.test", log.RecipientEmail);
    }

    [Fact]
    public async Task RequestAssignedAsync_NoReporterEmail_StillSendsToTeam()
    {
        // Arrange
        var request = NotificationTestData.Request2();
        request.Reporter!.Email = string.Empty;
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([NotificationTestData.Sam()]);

        // Act
        await _service.RequestAssignedAsync(request);

        // Assert
        _sender.Verify(s => s.SendAsync(It.Is<EmailMessage>(m => m.ToAddress == "support@campus.test"), It.IsAny<CancellationToken>()), Times.Once);
        var log = Assert.Single(_savedLogs);
        Assert.Equal(NotificationType.TeamAssigned, log.Type);
    }

    [Fact]
    public async Task RequestAssignedAsync_SenderThrows_LogsFailureWithoutThrowing()
    {
        // Arrange
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([]);
        _sender.Setup(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("SMTP server not reachable"));

        // Act
        var exception = await Record.ExceptionAsync(() => _service.RequestAssignedAsync(NotificationTestData.Request2()));

        // Assert
        Assert.Null(exception);
        var log = Assert.Single(_savedLogs);
        Assert.False(log.Succeeded);
        Assert.Equal("SMTP server not reachable", log.Error);
    }

    [Fact]
    public async Task RequestAssignedAsync_FirstSendFails_StillSendsSecond()
    {
        // Arrange
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([NotificationTestData.Sam()]);
        _sender.Setup(s => s.SendAsync(It.Is<EmailMessage>(m => m.ToAddress == "employee2@campus.test"), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Mailbox not found"));

        // Act
        await _service.RequestAssignedAsync(NotificationTestData.Request2());

        // Assert
        _sender.Verify(s => s.SendAsync(It.Is<EmailMessage>(m => m.ToAddress == "support@campus.test"), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Collection(_savedLogs,
            log => Assert.False(log.Succeeded),
            log => Assert.True(log.Succeeded));
    }

    [Fact]
    public async Task RequestAssignedAsync_LogRepositoryThrows_DoesNotThrow()
    {
        // Arrange
        _users.Setup(u => u.GetByTeamAsync(2)).ReturnsAsync([NotificationTestData.Sam()]);
        _logs.Setup(l => l.AddAsync(It.IsAny<NotificationLog>())).ThrowsAsync(new InvalidOperationException("Database down"));

        // Act
        var exception = await Record.ExceptionAsync(() => _service.RequestAssignedAsync(NotificationTestData.Request2()));

        // Assert
        Assert.Null(exception);
        _sender.Verify(s => s.SendAsync(It.IsAny<EmailMessage>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
