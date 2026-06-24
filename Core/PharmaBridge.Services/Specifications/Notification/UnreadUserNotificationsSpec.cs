using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Notification
{
    public class UnreadUserNotificationsSpec : BaseSpecifications<Domain.Models.UserAccess.Notification, int>
    {
        public UnreadUserNotificationsSpec(string userId)
            : base(n => n.ApplicationUserId == userId && !n.IsRead)
        {
        }
    }
}
