using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequestsFlow
{
    public class PrescriptionRequestDetailsDto
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public string? MedicineName { get; set; }
        public string Status { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }

        // How many bids have been received shown as a counter badge on the detail page
        // ("3 pharmacies responded")
        public int BidCount { get; set; }

        // The delivery address text — patient wants to confirm where it will be delivered
        public string DeliveryAddress { get; set; }
    }
}
