using PharmaBridge.Shared.DTOs.PharmaRequestsFlow;
using PharmaBridge.Shared.DTOs.PrescriptionRequestHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    public class AdminPrescriptionRequestDetailsDto : PrescriptionRequestDetailsDto
    {
       // 1. Patient Info (Flattened for Admin UX)
        public string PatientProfileId { get; set; }
        public string PatientName { get; set; }   // Added so admin knows who it is at a glance
        public string PatientPhone { get; set; }  // Added in case admin needs to call the patient for support

        // 2. The delivery address FK — admins may need the raw ID to query the address table
        public int DeliveryAddressId { get; set; }

        // 3. Full history of status changes — admins need the audit trail
        public List<PrescriptionRequestHistoryDto> History { get; set; } = new();
    }
}
