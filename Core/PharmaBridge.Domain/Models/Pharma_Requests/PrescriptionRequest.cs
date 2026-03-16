using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PrescriptionRequest : BaseEntity<int>
    {
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public string? MedicineName { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Open;
        public DateTime ExpiresAt { get; set; }

        // 17 - Order (1) To (1) PrescriptionRequest (Has)
        public virtual Order Order { get; set; }

        // 18 - PrescriptionRequest (1) To (Many) PrescriptionRequestHistory (Has)
        public virtual ICollection<PrescriptionRequestHistory> PrescriptionRequestHistory { get; set; } = new HashSet<PrescriptionRequestHistory>();

        // 20 - PrescriptionRequest (1) To (Many) Bid (Has) 
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>();

        public string PatientProfileId { get; set; }
        public virtual PatientProfile PatientProfile { get; set; }
    }
}
