using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public sealed class AdminRequestsSpec : AdminRequestsCountSpec
    {
        public AdminRequestsSpec(PrescriptionRequestQueryParams p) : base(p)
        {
            AddInclude(r => r.DeliveryAddress);
            AddInclude(r => r.PatientProfile);

            var UserNavigate = $"{nameof(PrescriptionRequestEntity.PatientProfile)}.{nameof(PatientProfile.ApplicationUser)}";
            AddInclude(UserNavigate);

            AddInclude(r => r.Bids);

            AddOrderBy(r => r.CreatedAt,true);
            ApplyPaging(p.PageSize, p.PageIndex);
        }
    }
}
