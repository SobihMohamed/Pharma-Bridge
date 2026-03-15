using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Domain.Models.UserAccess
{
    public class Complaint :BaseEntity<int>
    {
        public ComplaintStatus Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string? AdminNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser Send { get; set; }

        public string? ResolverId { get; set; }
        public virtual ApplicationUser? Resolved_By { get; set; }
    }
}
