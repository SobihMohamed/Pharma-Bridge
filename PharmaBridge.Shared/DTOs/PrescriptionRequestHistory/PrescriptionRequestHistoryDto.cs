using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PrescriptionRequestHistory
{
    public class PrescriptionRequestHistoryDto
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }

        // We show the name of the person/system that made the change, not their raw DB Id
        public string ChangedByName { get; set; }

        // The FK is included for admins who need to programmatically link back to the request
        public int PrescriptionRequestId { get; set; }
    }
}
