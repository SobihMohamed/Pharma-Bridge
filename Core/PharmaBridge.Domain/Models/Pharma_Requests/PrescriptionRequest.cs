using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PrescriptionRequest : BaseEntity<int>
    {
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public string? MedicineName { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Open;
        public DateTime ExpiresAt { get; set; }


    }
}
