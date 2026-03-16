using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PrescriptionRequestHistory : BaseEntity<int>
    {
        public string Notes { get; set; }
        public string NewStatus { get; set; }
        public string OldStatus { get; set; }
        public int ChangedByUserID { get; set; }
        public DateTime ChangedAt { get; set; }

        // 18 - PrescriptionRequest (1) To (Many) PrescriptionRequestHistory (Has)
        public int PrescriptionRequestId { get; set; }
        public virtual PrescriptionRequest PrescriptionRequest { get; set; }
    }
}
