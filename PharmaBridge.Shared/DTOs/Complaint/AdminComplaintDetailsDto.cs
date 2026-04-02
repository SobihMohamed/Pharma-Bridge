using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Complaint
{
    public class AdminComplaintDetailsDto : ComplaintDto
    {
        [MaxLength(500)]
        public string? AdminNotes { get; set; }

        public string? ResolvedById { get; set; }
    }
}
