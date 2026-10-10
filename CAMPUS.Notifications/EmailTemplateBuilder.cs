using System.Net;
using System.Text;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Formatting;
using Microsoft.Extensions.Options;

namespace CAMPUS.Notifications;

public class EmailTemplateBuilder(IOptions<EmailOptions> options)
{
    private const string Dash = "—";
    private const string Footer = "This is an automatic message from CAMPUS, so please don't reply.";

    private readonly EmailOptions _options = options.Value;

    /// <summary>Builds the e-mail to one facilities manager about a newly submitted request.</summary>
    public EmailMessage BuildRequestSubmitted(ServiceRequest request, User recipient)
    {
        var subject = $"New request {request.RefCode()}: {request.Title}";
        var sentence = $"{request.Reporter?.Name} submitted a new request at the {request.Campus?.Name} campus ({request.Location}). "
            + "It's waiting to be assigned to a support team.";

        return Build(request, recipient, subject, "A new request has been submitted", sentence);
    }

    /// <summary>Builds the e-mail to the reporter that their request was assigned to a team.</summary>
    public EmailMessage BuildRequestAssigned(ServiceRequest request)
    {
        var team = TeamName(request);
        var subject = $"{request.RefCode()} has been assigned to the {team} team";
        var sentence = $"Your request \"{request.Title}\" has been assigned to the {team} team. They'll start working on it soon.";

        return Build(request, request.Reporter ?? new User(), subject, "Your request has been assigned", sentence);
    }

    /// <summary>Builds the e-mail to one member of the team that the request was assigned to.</summary>
    public EmailMessage BuildTeamAssigned(ServiceRequest request, User recipient)
    {
        var team = TeamName(request);
        var subject = $"New request for the {team} team: {request.RefCode()}";
        var sentence = $"Request \"{request.Title}\" at the {request.Campus?.Name} campus ({request.Location}) "
            + $"has been assigned to the {team} team. Priority: {request.Priority.ToLabel()}.";

        return Build(request, recipient, subject, "A request has been assigned to your team", sentence);
    }

    // The shared layout of all three e-mails: an HTML version and a plain text version.
    private EmailMessage Build(ServiceRequest request, User recipient, string subject, string heading, string sentence)
    {
        var greeting = $"Hi {FirstName(recipient.Name)},";
        var link = $"{_options.BaseUrl.TrimEnd('/')}/Requests/Details/{request.Id}";
        var rows = new (string Label, string Value)[]
        {
            ("Request", request.RefCode()),
            ("Title", request.Title),
            ("Category", request.Category?.Name ?? Dash),
            ("Campus", request.Campus?.Name ?? Dash),
            ("Location", request.Location),
            ("Priority", request.Priority.ToLabel()),
            ("Status", request.Status.ToLabel()),
            ("Team", request.AssignedTeam?.Name ?? Dash)
        };

        var html = new StringBuilder();
        html.Append("<!DOCTYPE html><html><body style=\"margin:0;padding:24px 0;background-color:#F6F7F9;font-family:Arial,sans-serif;\">");
        html.Append("<table role=\"presentation\" width=\"600\" cellpadding=\"0\" cellspacing=\"0\" align=\"center\" ");
        html.Append("style=\"width:600px;margin:0 auto;background-color:#FFFFFF;border:1px solid #E6E8EC;border-radius:8px;\">");
        html.Append("<tr><td style=\"background-color:#5236AB;color:#FFFFFF;font-weight:bold;font-size:20px;padding:16px 24px;border-radius:8px 8px 0 0;\">CAMPUS</td></tr>");
        html.Append("<tr><td style=\"padding:24px;font-family:Arial,sans-serif;font-size:15px;line-height:1.5;color:#1B1F2A;\">");
        html.Append($"<p style=\"margin:0 0 16px;\">{Encode(greeting)}</p>");
        html.Append($"<h1 style=\"margin:0 0 12px;font-size:20px;color:#1B1F2A;\">{Encode(heading)}</h1>");
        html.Append($"<p style=\"margin:0 0 20px;\">{Encode(sentence)}</p>");
        html.Append("<table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" style=\"width:100%;margin:0 0 24px;border-collapse:collapse;\">");
        foreach (var (label, value) in rows)
        {
            html.Append("<tr>");
            html.Append($"<td style=\"padding:6px 12px 6px 0;width:120px;color:#6B7280;border-bottom:1px solid #E6E8EC;\">{Encode(label)}</td>");
            html.Append($"<td style=\"padding:6px 0;color:#1B1F2A;border-bottom:1px solid #E6E8EC;\">{Encode(value)}</td>");
            html.Append("</tr>");
        }
        html.Append("</table>");
        html.Append($"<a href=\"{Encode(link)}\" style=\"display:inline-block;background-color:#5236AB;color:#FFFFFF;text-decoration:none;font-weight:bold;padding:12px 20px;border-radius:6px;\">View request</a>");
        html.Append("</td></tr>");
        html.Append($"<tr><td style=\"padding:16px 24px;border-top:1px solid #E6E8EC;font-family:Arial,sans-serif;font-size:12px;color:#6B7280;\">{Encode(Footer)}</td></tr>");
        html.Append("</table></body></html>");

        var text = new StringBuilder();
        text.AppendLine(greeting);
        text.AppendLine();
        text.AppendLine(heading);
        text.AppendLine();
        text.AppendLine(sentence);
        text.AppendLine();
        foreach (var (label, value) in rows)
        {
            text.AppendLine($"{label}: {value}");
        }
        text.AppendLine();
        text.AppendLine($"View request: {link}");
        text.AppendLine();
        text.AppendLine(Footer);

        return new EmailMessage
        {
            ToAddress = recipient.Email,
            ToName = recipient.Name,
            Subject = subject,
            HtmlBody = html.ToString(),
            TextBody = text.ToString()
        };
    }

    private static string TeamName(ServiceRequest request) => request.AssignedTeam?.Name ?? Dash;

    private static string FirstName(string name)
    {
        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }

    private static string Encode(string value) => WebUtility.HtmlEncode(value);
}
