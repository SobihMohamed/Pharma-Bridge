using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class Patient : BaseEntity<string> 
    {

        public string PatientId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
