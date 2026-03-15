using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientProfile : BaseEntity<string>
    {

        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<PatientAddress> PatientAddresses { get; set; } = new List<PatientAddress>();
        public virtual ICollection<PrescriptionRequest> PrescriptionRequests { get; set; } = new HashSet<PrescriptionRequest>();
    }
}
