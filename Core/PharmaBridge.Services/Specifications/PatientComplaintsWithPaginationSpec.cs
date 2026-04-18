using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class PatientComplaintsWithPaginationSpec : BaseSpecifications<Complaint, int>
    {
        public PatientComplaintsWithPaginationSpec(string patientId, int pageSize, int pageIndex)
            : base(c => c.SubmittedById == patientId)
        {
            AddOrderBy(c => c.Id, isDescending: true);
            ApplyPaging(pageSize, pageIndex);
        }
    }
}
