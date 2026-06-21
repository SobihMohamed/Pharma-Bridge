using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Complaint
{
    public class UpdateComplaintStatusDto
    {
        [Required(ErrorMessage = "Status is required (e.g., InReview, Resolved, Dismissed).")]
        public ComplaintStatus Status { get; set; }

        [MaxLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? AdminNotes { get; set; }
    }
}
