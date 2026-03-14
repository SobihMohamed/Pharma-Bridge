using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientProfile : BaseEntity<string>
    {
        // 5- ApplicationUser (1) To (1) Patient_Profile
        public string ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }

        // 9- Patient_Profile (1) To (Many) PatientAddress (Has)
        public  ICollection<PatientAddress> PatientAddresses { get; set; } = new HashSet<PatientAddress>();

        // 10- Patient_Profile (1) To (Many) Prescription_Requests (Request)
        public  ICollection<PrescriptionRequest> PrescriptionRequests { get; set; } = new HashSet<PrescriptionRequest>();
    }
}
