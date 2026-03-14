using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models
{
    public class PrescriptionRequest : BaseEntity<string>
    {
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public string? MedicineName { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Open;
        public DateTime ExpiresAt { get; set; }
        public decimal DeliveryLatitude { get; set; } // In Review
        public decimal DeliveryLongitude { get; set; } // In Review


    }
}
