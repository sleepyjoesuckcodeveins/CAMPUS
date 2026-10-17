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

        public static string ToLabel(this UserRole role) => role switch
        {
            UserRole.Employee => "Employee",
            UserRole.FacilitiesManager => "Facilities manager",
            UserRole.SupportTeam => "Support team",
            UserRole.Administrator => "Administrator",
            _ => role.ToString()
        };
    }
}
