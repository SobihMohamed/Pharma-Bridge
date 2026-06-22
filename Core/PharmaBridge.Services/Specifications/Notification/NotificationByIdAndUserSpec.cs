using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Notification
{
    public class NotificationByIdAndUserSpec : BaseSpecifications<Domain.Models.UserAccess.Notification, int>
    {
        public NotificationByIdAndUserSpec(int notificationId, string userId)
            : base(n => n.Id == notificationId && n.ApplicationUserId == userId)
        {
        }
    }
}
