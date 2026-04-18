using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class AllComplaintsWithPaginationSpec : BaseSpecifications<Complaint, int>
    {
        public AllComplaintsWithPaginationSpec(ComplaintQueryParams queryParams)
            : base(c => (!queryParams.Status.HasValue || c.Status == queryParams.Status.Value))
        {
            AddInclude(c => c.SubmittedBy);
            AddOrderBy(c => c.Id, isDescending: true); 
            ApplyPaging(queryParams.PageSize, queryParams.PageIndex);
        }
    }
}
