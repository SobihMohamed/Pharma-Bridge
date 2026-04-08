using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.PrescriptionRequest
{
    public class PrescriptionRequestQueryParams : BaseQueryParam
    {
        // e.g., Pending, HasBids, Closed, Cancelled
        public PrescriptionStatus? Status { get; set; }

        // For date filtering
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        // Specifically for Pharmacy to filter by distance (Defaults to 5km if not provided)
        public double? RadiusInKm { get; set; } = 5.0;

        // For Admin to filter requests of a specific patient
        public Guid? PatientId { get; set; }
    }
}
