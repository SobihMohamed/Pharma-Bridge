using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.User;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PrescriptionRequestEntity : BaseEntity<int>
    {
        public string? ImageUrl { get; set; }
        public string? PatientNotes { get; set; }
        public string? MedicineName { get; set; }
        public PrescriptionStatus Status { get; set; } = PrescriptionStatus.Pending; // system sets to Pending by default when created
        public DateTime ExpiresAt { get; set; } // system sets to CreatedAt + 24 hours by default when created

        // 17 - Order (1) To (1) PrescriptionRequest (Has)
        // nullable because the prescription request can be created and not converted to order yet
        public virtual Order? Order { get; set; } // navigation property for the related Order, if any

        // 18 - PrescriptionRequest (1) To (Many) PrescriptionRequestHistory (Has)
        public virtual ICollection<PrescriptionRequestHistory> PrescriptionRequestHistorys{ get; set; } = new HashSet<PrescriptionRequestHistory>();

        // 20 - PrescriptionRequest (1) To (Many) Bid (Has) 
        public virtual ICollection<Bid> Bids { get; set; } = new HashSet<Bid>(); // navigation property for the related Bids

        public string PatientProfileId { get; set; } // 
        public virtual PatientProfile PatientProfile { get; set; }

        public int DeliveryAddressId { get; set; }
        public virtual PatientAddress DeliveryAddress { get; set; }
    }
}
