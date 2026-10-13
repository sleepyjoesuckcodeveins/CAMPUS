using System;
using System.Collections.Generic;
using System.Text;

namespace CAMPUS.DOMAIN.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public int ServiceRequestId { get; set; }
        public ServiceRequest? ServiceRequest { get; set; }
        public int AuthorId { get; set; }
        public User? Author { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
