using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Complaint
{
    public class ComplaintQueryParams : BaseQueryParam
    {
        // e.g., Open, InReview, Resolved
        public ComplaintStatus? Status { get; set; }
        public Guid? PharmacyId { get; set; }
    }
}
