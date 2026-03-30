using PharmaBridge.Shared.DTOs.PharmaRequestsFlow;
using PharmaBridge.Shared.DTOs.PrescriptionRequestHistory;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    public class AdminPrescriptionRequestDetailsDto : PrescriptionRequestDetailsDto
    {
        // admins need this to look up the patient profile
        public string PatientProfileId { get; set; }

        // The delivery address FK — admins may need the raw ID to query the address table
        public int DeliveryAddressId { get; set; }

        // Full history of status changes — admins need the audit trail
        // We expose a list of the history DTO (read-only, lightweight)
        public List<PrescriptionRequestHistoryDto> History { get; set; } = new();

    }
}
