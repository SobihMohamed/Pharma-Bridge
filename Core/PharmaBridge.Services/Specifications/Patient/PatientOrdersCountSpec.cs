using PharmaBridge.Domain.Contracts.SpecificationPattern.BaseSpec;
using PharmaBridge.Domain.Models.UserAccess;
using PharmaBridge.Shared.EnumHelper.UserAccessEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Services.Specifications.Patient
{
    public class PatientOrdersCountSpec : BaseSpecifications<Order, int>
    {
        public PatientOrdersCountSpec(string patientProfileId, OrderStatus? status = null)
            : base(o => o.PatientProfileId == patientProfileId && (!status.HasValue || o.OrderStatus == status.Value))
        {
        }
    }
}
