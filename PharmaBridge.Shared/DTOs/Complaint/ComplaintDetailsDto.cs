using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Complaint
{
    public class ComplaintDetailsDto : ComplaintDto
    {
        // Removed validation attributes because this is an Output/Read DTO
        public string? AdminNotes { get; set; }

        // Flattened for Admin UI (e.g., "Resolved by: Tarek")
        public string? ResolvedByName { get; set; }
    }
}
