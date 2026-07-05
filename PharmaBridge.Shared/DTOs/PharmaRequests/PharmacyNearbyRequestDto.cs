using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    public class PharmacyNearbyRequestDto
    {
        public int Id { get; set; }
        public string? MedicineName { get; set; }
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public PrescriptionStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        // Data Masking: هنرجع اسم المنطقة بس مش العنوان التفصيلي
        public string DeliveryArea { get; set; } = string.Empty;
    }
}
