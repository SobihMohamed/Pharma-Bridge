using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications
{
    public class PatientComplaintsCountSpec : BaseSpecifications<Complaint, int>
    {
        public PatientComplaintsCountSpec(string patientId)
            : base(c => c.SubmittedById == patientId)
        {
        }
    }
}
