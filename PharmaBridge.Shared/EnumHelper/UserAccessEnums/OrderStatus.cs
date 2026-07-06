using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.EnumHelper.UserAccessEnums
{
    public enum OrderStatus
    {
        Pending = 1,
        Accepted = 2,
        InTransit = 3,
        Preparing = 4,
        Completed = 5,
        Cancelled = 6,
        Delivered=7,
        Returned = 8,
    }
}
