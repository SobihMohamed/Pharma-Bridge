using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.EnumHelper.NotificationEnums
{
    public enum OrderStatus
    {
        Pending = 1,
        Accepted = 2,
        InTransit = 3,
        Delivered = 4,
        Cancelled = 5,
        Returned = 6
    }
}
