using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Complaint :BaseEntity<int>
    {
        public ComplaintStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
