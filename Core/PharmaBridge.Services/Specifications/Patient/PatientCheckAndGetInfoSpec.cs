using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientCheckAndGetInfoSpec : BaseSpecifications<PatientProfile, string>
    {
        public PatientCheckAndGetInfoSpec(string applicationUserId)
             : base(p => p.ApplicationUserId == applicationUserId)
        {
            AddInclude(p => p.ApplicationUser);
        }
    }
}
