using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Notification
{
    public class UserNotificationsCountSpec : BaseSpecifications<Domain.Models.UserAccess.Notification, int>
    {
        public UserNotificationsCountSpec(string userId)
            : base(n => n.ApplicationUserId == userId)
        {
        }
    }
}
