using System;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Shared.Common.Params.Pharmacy
{
    public class PharmacyQueryParams : BaseQueryParam
    {
        public PharmacyStatus? Status { get; set; }
    }
}