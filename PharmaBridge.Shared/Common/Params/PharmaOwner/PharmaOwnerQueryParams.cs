using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Shared.Common.Params.PharmaOwner
{
    public class PharmaOwnerQueryParams : BaseQueryParam
    {
        // To search by PharmaOwner Name, Email, or Phone
        public PharmaOwnerStatus? Status { get; set; }
    }
}
