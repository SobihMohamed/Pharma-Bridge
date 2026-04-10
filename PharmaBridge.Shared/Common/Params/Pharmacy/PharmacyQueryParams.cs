using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Pharmacy
{
    public class PharmacyQueryParams : BaseQueryParam
    {
        // e.g., Pending, Active, Suspended, Blocked
        public PharmacyStatus? Status { get; set; }

        // To search by Pharmacy Name or Phone Number
        public string? SearchTerm { get; set; }
    }
}