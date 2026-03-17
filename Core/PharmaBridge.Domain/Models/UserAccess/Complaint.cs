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

        // add order reference to the complaint
        public int? OrderId { get; set; }
        public virtual Order? Order { get; set; }

        public string SubmittedById { get; set; }
        public virtual ApplicationUser SubmittedBy { get; set; }
        
        public string? ResolvedById { get; set; }
        public virtual ApplicationUser? ResolvedBy { get; set; }
    }
}
