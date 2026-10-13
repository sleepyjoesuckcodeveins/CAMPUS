using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
