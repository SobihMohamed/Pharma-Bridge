using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.UserAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PatientProfile : BaseEntity<string>
    {
        // 15 - PatientProfile (1) To (Many) Orders (Placed)
        public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }
}
