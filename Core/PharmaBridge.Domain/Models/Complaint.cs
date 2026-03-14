using PharmaBridge.Domain.Models.User;
using System;

namespace PharmaBridge.Domain.Models
{
    public class Complaint : BaseEntity<int>
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // 2- ApplicationUser (1) To (Many) Complaints (Send)
        public string SubmitterId { get; set; }
        public  ApplicationUser Submitter { get; set; }

        // 3- ApplicationUser (1) To (Many) Complaints (Resolved_By)
        public string? ResolverId { get; set; }
        public  ApplicationUser Resolver { get; set; }
    }
}
