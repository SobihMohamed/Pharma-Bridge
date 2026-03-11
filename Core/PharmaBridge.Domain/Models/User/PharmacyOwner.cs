using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class PharmacyOwner : ApplicationUser
    {
        public string PharmacyOwnerId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
