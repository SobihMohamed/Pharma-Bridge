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
        public string Status { get; set; }
        public int? OrderId { get; set; }
        public string SubmittedById { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
