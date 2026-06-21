using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    internal class PatientProfileWithDetailsSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientProfileWithDetailsSpec(string patientId)
        : base(p => p.ApplicationUserId == patientId)
        {
            AddInclude(p => p.ApplicationUser);
            AddInclude(p => p.PatientAddresses);
            AddInclude(p => p.Orders);
            AddInclude(p => p.PrescriptionRequests);
            AddInclude(p => p.PharmacyRatings);
        }
    }
}
