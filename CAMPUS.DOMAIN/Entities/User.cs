using System;
using System.Collections.Generic;
using System.Text;
using CAMPUS.DOMAIN.Enums;

namespace CAMPUS.DOMAIN.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Auth0Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public Roles Role { get; set; } = Roles.Employee;
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
