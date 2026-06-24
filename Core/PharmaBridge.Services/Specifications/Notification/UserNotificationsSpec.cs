using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Shared.Common.Params.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Notification
{
    public class UserNotificationsSpec : BaseSpecifications<Domain.Models.UserAccess.Notification, int>
    {
        public UserNotificationsSpec(string userId, NotificationQueryParams queryParams)
            : base(n => n.ApplicationUserId == userId)
        {
            AddOrderBy(n => n.CreatedAt,isDescending: true);

            // تطبيق الـ Pagination
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
