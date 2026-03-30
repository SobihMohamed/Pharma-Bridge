using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequestsFlow
{
    public class PrescriptionRequestDto
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; }

        // Status returned as a string 
        public string Status { get; set; }

        public string? PatientNotes { get; set; }
        public string? ImageUrl { get; set; }

        // Patients need to see when their request expires so they know if it's still valid
        public DateTime ExpiresAt { get; set; }

        // CreatedAt comes from BaseEntity — useful to sort the list by newest first
        public DateTime CreatedAt { get; set; }
       
    }
}
