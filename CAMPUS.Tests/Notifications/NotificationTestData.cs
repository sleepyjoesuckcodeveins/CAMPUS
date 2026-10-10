using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using CAMPUS.Notifications;
using Microsoft.Extensions.Options;

namespace CAMPUS.Tests.Notifications;

/// <summary>Test data from the seed data in C3: request 2, Alex, Daan and Sam.</summary>
public static class NotificationTestData
{
    public static IOptions<EmailOptions> Options() => Microsoft.Extensions.Options.Options.Create(new EmailOptions
    {
        Enabled = true,
        Host = "localhost",
        Port = 25,
        UseSsl = false,
        FromAddress = "no-reply@campus.test",
        FromName = "CAMPUS Facilities",
        BaseUrl = "https://localhost:7100"
    });

    public static Team Technical() => new() { Id = 2, Name = "Technical" };

    public static User Alex() => new() { Id = 3, Name = "Alex Morgan", Email = "manager@campus.test", Role = Roles.FacilitiesManager };

    public static User Daan() => new() { Id = 2, Name = "Daan Jansen", Email = "employee2@campus.test", Role = Roles.Employee };

    public static User Sam() => new() { Id = 4, Name = "Sam Bakker", Email = "support@campus.test", Role = Roles.SupportTeam, TeamId = 2 };

    public static ServiceRequest Request2() => new()
    {
        Id = 2,
        Title = "Elevator B is making a loud noise",
        Description = "Elevator B makes a loud grinding noise between the second and third floor.",
        CategoryId = 5,
        Category = new Category { Id = 5, Name = "Elevator" },
        CampusId = 1,
        Campus = new Campus { Id = 1, Name = "Rotterdam" },
        Location = "De Lotus, elevator B",
        Priority = Priority.High,
        Status = RequestStatus.InProgress,
        ReporterId = 2,
        Reporter = Daan(),
        AssignedTeamId = 2,
        AssignedTeam = Technical(),
        AssignedAt = new DateTime(2026, 10, 1, 11, 0, 0, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 10, 1, 9, 30, 0, DateTimeKind.Utc),
        UpdatedAt = new DateTime(2026, 10, 2, 10, 0, 0, DateTimeKind.Utc)
    };
}
