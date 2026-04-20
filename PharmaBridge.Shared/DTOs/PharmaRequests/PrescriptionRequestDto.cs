using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
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

        // Business Logic Additions:
        // 1. To show activity/competition on the request
        public int BidsCount { get; set; } 

        // 2. Flattened from DeliveryAddress (e.g., City or Region ONLY)
        // Crucial for pharmacies to calculate delivery fee, without exposing the exact home address.
        public string DeliveryArea { get; set; }
    }
}
