using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Order
{
    public class OrderQueryParams : BaseQueryParam
    {
        // e.g., Preparing, OutForDelivery, Completed, Cancelled
        public OrderStatus? Status { get; set; }

        // For date filtering (e.g., "This Month" revenue)
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public string? Search { get; set; }

        // For Admin: filtering by a specific pharmacy or patient
        public int? PharmacyId { get; set; }
        public Guid? PatientId { get; set; }
    }
}
