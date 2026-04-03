using PharmaBridge.Shared.DTOs.PatientAddresses;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PatientProfiles
{
    public class PatientProfileDetailsDto : PatientProfileDto
    {
        // 1. Collections
        public ICollection<PatientAddressDto> Addresses { get; set; } = new HashSet<PatientAddressDto>();

        // 2. Dashboard Summaries
        // PrescriptionRequests
        public int TotalPrescriptionRequests { get; set; } 
        
        public int OrdersCount { get; set; } 
        
        public int ComplaintsSubmitted { get; set; } 
    }
}
