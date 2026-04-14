using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.Pharma_Requests
{
    public class PrescriptionRequestHistory : BaseEntity<int>
    {
        public string Notes { get; set; }
        public PrescriptionStatus NewStatus { get; set; }
        public PrescriptionStatus? OldStatus { get; set; }
        public DateTime ChangedAt { get; set; }

        // 18 - PrescriptionRequest (1) To (Many) PrescriptionRequestHistory (Has)
        public int PrescriptionRequestId { get; set; }
        public virtual PrescriptionRequestEntity PrescriptionRequest { get; set; }
        public string ChangedById { get; set; }
        public virtual ApplicationUser ChangedBy { get; set; }
    }
}
