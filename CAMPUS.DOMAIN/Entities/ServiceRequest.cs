using CAMPUS.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace CAMPUS.DOMAIN.Entities
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public int CampusId { get; set; }
        public Campus? Campus { get; set; }
        public string Location { get; set; } = string.Empty;
        public Priority Priority { get; set; } = Priority.Medium;
        public RequestStatus Status { get; set; } = RequestStatus.Open;
        public int ReporterId { get; set; }
        public User? Reporter { get; set; }
        public int? AssignedTeamId { get; set; }
        public Team? AssignedTeam { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public List<Comment> Comment { get; set; } = [];
    }
}
