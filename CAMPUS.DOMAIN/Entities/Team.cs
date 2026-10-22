using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN.Entities
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<User> Members { get; set; } = [];
    }

}
