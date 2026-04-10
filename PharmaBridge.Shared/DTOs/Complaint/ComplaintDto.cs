using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Complaint
{
    public class ComplaintDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ComplaintStatus Status { get; set; }
        public int? OrderId { get; set; }
        // 1. Missing Timestamp added from BaseEntity
        public DateTime CreatedAt { get; set; } 
        public DateTime? ResolvedAt { get; set; }

        // 2. Flattened Name instead of raw Identity GUID
        public string SubmittedByName { get; set; }
    }
}
