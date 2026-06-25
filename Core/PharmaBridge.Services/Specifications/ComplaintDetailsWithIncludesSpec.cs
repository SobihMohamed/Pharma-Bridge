using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class ComplaintDetailsWithIncludesSpec : BaseSpecifications<Complaint, int>
    {
        public ComplaintDetailsWithIncludesSpec(int complaintId) : base(c => c.Id == complaintId)
        {
            AddInclude(c => c.SubmittedBy);
            AddInclude(c => c.ResolvedBy);
        }
    }
}
