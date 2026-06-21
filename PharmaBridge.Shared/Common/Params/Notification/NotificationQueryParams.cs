using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.Common.Params.Notification
{
    public class NotificationQueryParams : BaseQueryParam
    {
        // null = get all 
        // true = get read only
        // false = get not read
        public bool? IsRead { get; set; }
    }
}
