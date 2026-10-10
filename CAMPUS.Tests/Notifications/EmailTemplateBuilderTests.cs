using CAMPUS.Notifications;

namespace CAMPUS.Tests.Notifications;

public class EmailTemplateBuilderTests
{
    private readonly EmailTemplateBuilder _builder = new(NotificationTestData.Options());

    [Fact]
    public void BuildRequestSubmitted_Request_SubjectHasCodeAndTitle()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestSubmitted(request, NotificationTestData.Alex());

        // Assert
        Assert.Equal("New request REQ-0002: Elevator B is making a loud noise", message.Subject);
    }

    [Fact]
    public void BuildRequestSubmitted_Manager_SendsToManager()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestSubmitted(request, NotificationTestData.Alex());

        // Assert
        Assert.Equal("manager@campus.test", message.ToAddress);
        Assert.Contains("Hi Alex,", message.HtmlBody);
        Assert.Contains("Hi Alex,", message.TextBody);
    }

    [Fact]
    public void BuildRequestSubmitted_Request_BodyHasReporterAndCampus()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestSubmitted(request, NotificationTestData.Alex());

        // Assert
        Assert.Contains("Daan Jansen submitted a new request at the Rotterdam campus (De Lotus, elevator B).", message.TextBody);
    }

    [Fact]
    public void BuildRequestAssigned_Request_SubjectHasCodeAndTeam()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestAssigned(request);

        // Assert
        Assert.Equal("REQ-0002 has been assigned to the Technical team", message.Subject);
    }

    [Fact]
    public void BuildRequestAssigned_Request_SendsToReporter()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestAssigned(request);

        // Assert
        Assert.Equal("employee2@campus.test", message.ToAddress);
        Assert.Contains("Hi Daan,", message.TextBody);
    }

    [Fact]
    public void BuildTeamAssigned_Member_SubjectHasTeamAndCode()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildTeamAssigned(request, NotificationTestData.Sam());

        // Assert
        Assert.Equal("New request for the Technical team: REQ-0002", message.Subject);
    }

    [Fact]
    public void BuildTeamAssigned_Member_SendsToMember()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildTeamAssigned(request, NotificationTestData.Sam());

        // Assert
        Assert.Equal("support@campus.test", message.ToAddress);
        Assert.Contains("Hi Sam,", message.TextBody);
    }

    [Fact]
    public void BuildTeamAssigned_Request_BodyHasPriority()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildTeamAssigned(request, NotificationTestData.Sam());

        // Assert
        Assert.Contains("Priority: High", message.TextBody);
        Assert.Contains("Priority: High", message.HtmlBody);
    }

    [Fact]
    public void Build_TitleWithHtml_IsEncoded()
    {
        // Arrange
        var request = NotificationTestData.Request2();
        request.Title = "<script>alert('x')</script>";

        // Act
        var message = _builder.BuildRequestSubmitted(request, NotificationTestData.Alex());

        // Assert
        Assert.DoesNotContain("<script>", message.HtmlBody);
        Assert.Contains("&lt;script&gt;", message.HtmlBody);
    }

    [Fact]
    public void Build_Request_LinkPointsToDetails()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildRequestAssigned(request);

        // Assert
        Assert.Contains("https://localhost:7100/Requests/Details/2", message.HtmlBody);
        Assert.Contains("https://localhost:7100/Requests/Details/2", message.TextBody);
    }

    [Fact]
    public void Build_Request_HasTextVersion()
    {
        // Arrange
        var request = NotificationTestData.Request2();

        // Act
        var message = _builder.BuildTeamAssigned(request, NotificationTestData.Sam());

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(message.TextBody));
        Assert.DoesNotContain("<", message.TextBody);
        Assert.Contains("View request: https://localhost:7100/Requests/Details/2", message.TextBody);
    }
}
