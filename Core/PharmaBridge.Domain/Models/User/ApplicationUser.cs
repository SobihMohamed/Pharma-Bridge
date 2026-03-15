using Microsoft.AspNetCore.Identity;
using PharmaBridge.Domain.Contracts;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Collections.Generic;
using System.Text;
using PharmaBridge.Domain.Models.UserAccess;

namespace PharmaBridge.Domain.Models.User
{
    public class ApplicationUser : IdentityUser , IEntity<string>
    {
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        public virtual ICollection<Notification> Notifications { get; set; }  = new HashSet<Notification>();
        public virtual ICollection<Complaint> Complaints { get; set; }  = new HashSet<Complaint>();
        public virtual ICollection<Complaint> ResolvedComplaints { get; set; } = new HashSet<Complaint>();

        public virtual PatientProfile? PatientProfile { get; set; }
        public virtual PharmaOwner? PharmaOwnerProfile { get; set; }
    }
}
