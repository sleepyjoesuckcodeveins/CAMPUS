using CAMPUS.DOMAIN.Entities;
using CAMPUS.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN.Formatting
{
    public static class RequestFormatting
    {
        public static string ToRefCode(this int id) => $"REQ-{id:D4}";

        public static string RefCode(this ServiceRequest request) => request.Id.ToRefCode();

        public static string ToLabel(this RequestStatus status) => status switch
        {
            RequestStatus.Open => "Open",
            RequestStatus.InProgress => "In progress",
            RequestStatus.Resolved => "Resolved",
            _ => status.ToString()
        };

        public static string ToLabel(this Priority priority) => priority switch
        {
            Priority.Low => "Low",
            Priority.Medium => "Medium",
            Priority.High => "High",
            Priority.Urgent => "Urgent",
            _ => priority.ToString()
        };

        public static string RoleLabel(string role) => role switch
        {
            Roles.Employee => "Employee",
            Roles.FacilitiesManager => "Facilities manager",
            Roles.SupportTeam => "Support team",
            Roles.Administrator => "Administrator",
            _ => role
        };
    }
}
