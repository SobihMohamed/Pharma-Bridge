using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.Common.Params.Complaint;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class AllComplaintsCountSpec : BaseSpecifications<Complaint, int>
    {
        public AllComplaintsCountSpec(ComplaintQueryParams queryParams)
            : base(c => (!queryParams.Status.HasValue || c.Status == queryParams.Status.Value) &&
                (string.IsNullOrEmpty(queryParams.Search) ||
                 c.Title.ToLower().Contains(queryParams.Search.ToLower()) ||
                 c.Description.ToLower().Contains(queryParams.Search.ToLower())) &&
                (!queryParams.PharmacyId.HasValue || (c.Order != null && c.Order.PharmacyId == queryParams.PharmacyId.Value))


            )
        {
        }
    }
}
