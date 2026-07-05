using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests.AdminReq
{
    public class AdminPrescriptionRequestDto
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int BidsCount { get; set; }
        
        public string PatientName { get; set; } = string.Empty;
        public string PatientPhone { get; set; } = string.Empty;
        public string DeliveryArea { get; set; } = string.Empty; 
    }
}
