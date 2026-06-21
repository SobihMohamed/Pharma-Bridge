using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Shared.Common.Params.PrescriptionRequest;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Request
{
    public class AdminRequestsCountSpec : BaseSpecifications<PrescriptionRequestEntity, int>
    {
        public AdminRequestsCountSpec(PrescriptionRequestQueryParams p)
            : base(r =>
                (!p.Status.HasValue || r.Status == p.Status.Value) &&
                (!p.FromDate.HasValue || r.CreatedAt >= p.FromDate.Value) &&
                (!p.ToDate.HasValue || r.CreatedAt <= p.ToDate.Value) &&
                (string.IsNullOrEmpty(p.PatientId) || r.PatientProfileId == p.PatientId) &&
                (string.IsNullOrEmpty(p.Search) || (r.MedicineName != null && r.MedicineName.ToLower().Contains(p.Search.ToLower())))
            )
        {
        }
    }
}
