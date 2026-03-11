using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class Patient : ApplicationUser 
    {

        public string PatientId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
