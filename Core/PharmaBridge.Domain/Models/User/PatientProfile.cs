using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientProfile : BaseEntity<string>
    {

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        
        public virtual ICollection<PatientAddress> PatientAddresses { get; set; } = new HashSet<PatientAddress>();
        public virtual ICollection<PrescriptionRequest> PrescriptionRequests { get; set; } = new HashSet<PrescriptionRequest>();
        public virtual ICollection<PharmacyRating> PharmacyRating { get; set; } = new HashSet<PharmacyRating>();

    }
}
