using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.EnumHelper.PharmaEnums
{
    public enum PrescriptionStatus
    {
        InProgress = 1,
        Completed,
        Cancelled,
        Expired,
        OutForDelivery
    }
}
