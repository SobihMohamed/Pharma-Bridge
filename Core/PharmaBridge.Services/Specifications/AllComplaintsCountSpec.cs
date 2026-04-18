using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class AllComplaintsCountSpec : BaseSpecifications<Complaint, int>
    {
        public AllComplaintsCountSpec(ComplaintQueryParams queryParams)
            : base(c => !queryParams.Status.HasValue || c.Status == queryParams.Status.Value)
        {
        }
    }
}
