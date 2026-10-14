using CAMPUS.DOMAIN;
using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DAL.Seed
{
    public static class SeedData
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Campus>().HasData(
                new Campus { Id = 1, Name = "Rotterdam" },
                new Campus { Id = 2, Name = "Amsterdam" },
                new Campus { Id = 3, Name = "Amstelveen" },
                new Campus { Id = 4, Name = "Arnhem" },
                new Campus { Id = 5, Name = "Eindhoven" },
                new Campus { Id = 6, Name = "Groningen" },
                new Campus { Id = 7, Name = "Maastricht" });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Plumbing" },
                new Category { Id = 2, Name = "Electrical" },
                new Category { Id = 3, Name = "Climate" },
                new Category { Id = 4, Name = "Furniture" },
                new Category { Id = 5, Name = "Elevator" },
                new Category { Id = 6, Name = "Cleaning" },
                new Category { Id = 7, Name = "IT and AV" },
                new Category { Id = 8, Name = "Other" });

            modelBuilder.Entity<Team>().HasData(
                new Team { Id = 1, Name = "Electrical" },
                new Team { Id = 2, Name = "Technical" },
                new Team { Id = 3, Name = "Plumbing" },
                new Team { Id = 4, Name = "Climate" },
                new Team { Id = 5, Name = "IT" });

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Emma de Vries", Email = "employee@campus.test", Role = Roles.Employee },
                new User { Id = 2, Name = "Daan Jansen", Email = "employee2@campus.test", Role = Roles.Employee },
                new User { Id = 3, Name = "Alex Morgan", Email = "manager@campus.test", Role = Roles.FacilitiesManager },
                new User { Id = 4, Name = "Sam Bakker", Email = "support@campus.test", Role = Roles.SupportTeam, TeamId = 2 },
                new User { Id = 5, Name = "Lisa Peters", Email = "support2@campus.test", Role = Roles.SupportTeam, TeamId = 3 },
                new User { Id = 6, Name = "Nora Visser", Email = "admin@campus.test", Role = Roles.Administrator },
                new User { Id = 7, Name = "Tom Smit", Email = "support3@campus.test", Role = Roles.SupportTeam });

            modelBuilder.Entity<ServiceRequest>().HasData(
                new ServiceRequest
                {
                    Id = 1,
                    Title = "Water leak in north hallway",
                    Description = "Water is dripping from the ceiling near the north stairs and the floor is slippery.",
                    CategoryId = 1,
                    CampusId = 1,
                    Location = "De Lotus, north hallway",
                    Priority = Priority.Urgent,
                    Status = RequestStatus.Open,
                    ReporterId = 1,
                    CreatedAt = Utc(10, 1, 8, 10)
                },
                new ServiceRequest
                {
                    Id = 2,
                    Title = "Elevator B is making a loud noise",
                    Description = "Elevator B makes a loud grinding noise between the second and third floor.",
                    CategoryId = 5,
                    CampusId = 1,
                    Location = "De Lotus, elevator B",
                    Priority = Priority.High,
                    Status = RequestStatus.InProgress,
                    ReporterId = 2,
                    AssignedTeamId = 2,
                    CreatedAt = Utc(10, 1, 9, 30),
                    AssignedAt = Utc(10, 1, 11, 0),
                    UpdatedAt = Utc(10, 2, 10, 0)
                },
                new ServiceRequest
                {
                    Id = 3,
                    Title = "Heating not working in room 2.14",
                    Description = "The radiator in room 2.14 stays cold, even when the thermostat is turned up.",
                    CategoryId = 3,
                    CampusId = 6,
                    Location = "Floor 2, room 2.14",
                    Priority = Priority.High,
                    Status = RequestStatus.Open,
                    ReporterId = 1,
                    CreatedAt = Utc(10, 2, 7, 45)
                },
                new ServiceRequest
                {
                    Id = 4,
                    Title = "Broken chair in study area",
                    Description = "One of the chairs at the window has a broken leg and is not safe to sit on.",
                    CategoryId = 4,
                    CampusId = 5,
                    Location = "High Tech Campus 5, study area",
                    Priority = Priority.Medium,
                    Status = RequestStatus.Open,
                    ReporterId = 2,
                    AssignedTeamId = 2,
                    CreatedAt = Utc(10, 2, 13, 20),
                    AssignedAt = Utc(10, 3, 9, 0),
                    UpdatedAt = Utc(10, 3, 9, 0)
                },
                new ServiceRequest
                {
                    Id = 5,
                    Title = "Light flickering above reception",
                    Description = "The ceiling light above the reception desk keeps flickering all day.",
                    CategoryId = 2,
                    CampusId = 2,
                    Location = "Entrance, reception",
                    Priority = Priority.Low,
                    Status = RequestStatus.Resolved,
                    ReporterId = 1,
                    AssignedTeamId = 1,
                    CreatedAt = Utc(10, 3, 10, 0),
                    AssignedAt = Utc(10, 3, 11, 0),
                    UpdatedAt = Utc(10, 4, 15, 30),
                    ResolvedAt = Utc(10, 4, 15, 30)
                },
                new ServiceRequest
                {
                    Id = 6,
                    Title = "Projector in room 1.05 does not turn on",
                    Description = "The projector in room 1.05 shows no image and its power light stays off.",
                    CategoryId = 7,
                    CampusId = 4,
                    Location = "Building B42, room 1.05",
                    Priority = Priority.Medium,
                    Status = RequestStatus.Open,
                    ReporterId = 2,
                    AssignedTeamId = 5,
                    CreatedAt = Utc(10, 4, 8, 30),
                    AssignedAt = Utc(10, 4, 10, 0),
                    UpdatedAt = Utc(10, 4, 10, 0)
                },
                new ServiceRequest
                {
                    Id = 7,
                    Title = "Toilet keeps running on floor 3",
                    Description = "The toilet on floor 3 keeps running after flushing and wastes a lot of water.",
                    CategoryId = 1,
                    CampusId = 3,
                    Location = "Floor 3, toilets",
                    Priority = Priority.Medium,
                    Status = RequestStatus.Open,
                    ReporterId = 1,
                    AssignedTeamId = 3,
                    CreatedAt = Utc(10, 4, 14, 0),
                    AssignedAt = Utc(10, 5, 8, 15),
                    UpdatedAt = Utc(10, 5, 8, 15)
                },
                new ServiceRequest
                {
                    Id = 8,
                    Title = "Emergency exit door won't close",
                    Description = "The emergency exit door on the east side of the Main Hall does not close by itself.",
                    CategoryId = 8,
                    CampusId = 7,
                    Location = "Main Hall, east exit",
                    Priority = Priority.Urgent,
                    Status = RequestStatus.Open,
                    ReporterId = 2,
                    CreatedAt = Utc(10, 5, 9, 0)
                });

            modelBuilder.Entity<Comment>().HasData(
                new Comment { Id = 1, ServiceRequestId = 2, AuthorId = 4, Text = "A technician is scheduled for Thursday morning.", CreatedAt = Utc(10, 2, 10, 0) },
                new Comment { Id = 2, ServiceRequestId = 2, AuthorId = 2, Text = "Thank you. The noise is worst in the morning.", CreatedAt = Utc(10, 2, 11, 30) },
                new Comment { Id = 3, ServiceRequestId = 4, AuthorId = 2, Text = "The chair is the one next to the second window.", CreatedAt = Utc(10, 3, 9, 30) });
        }

        private static DateTime Utc(int month, int day, int hour, int minute) =>
            new(2026, month, day, hour, minute, 0, DateTimeKind.Utc);
    }
}
