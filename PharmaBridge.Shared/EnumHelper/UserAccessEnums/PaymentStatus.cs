using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.EnumHelper.NotificationEnums
{
    public enum PaymentStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3,
        Refunded = 4,
        Cancelled = 5
    }
}
