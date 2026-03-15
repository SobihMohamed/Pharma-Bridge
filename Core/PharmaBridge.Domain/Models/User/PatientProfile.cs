using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.Pharma_Requests;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientProfile : BaseEntity<string>
    {

        public string ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; }

        public ICollection<PatientAddress> PatientAddresses { get; set; }
        public ICollection<PrescriptionRequest> PrescriptionRequests { get; set; }
    }
}
