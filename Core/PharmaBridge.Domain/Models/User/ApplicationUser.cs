using Microsoft.AspNetCore.Identity;
using PharmaBridge.Domain.Contracts;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Domain.Models.User
{
    public class ApplicationUser : IdentityUser , IEntity<string>
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        public virtual Patient ProfilePatient { get; set; }
        public virtual PharmacyOwner ProfilePharmacyOwner { get; set; }
    }
}
