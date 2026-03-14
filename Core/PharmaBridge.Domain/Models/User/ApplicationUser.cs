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

        // 4- ApplicationUser (1) To (1) Pharma_Owner
        public  PharmaOwner PharmaOwner { get; set; }

        // 5- ApplicationUser (1) To (1) Patient_Profile
        public  PatientProfile PatientProfile { get; set; }

        // 1- ApplicationUser (1) to (Many) Notifications (Get)
        public  ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        // 2- ApplicationUser (1) To (Many) Complaints (Send)
        public  ICollection<Complaint> SentComplaints { get; set; } = new HashSet<Complaint>();

        // 3- ApplicationUser (1) To (Many) Complaints (Resolved_By)
        public  ICollection<Complaint> ResolvedComplaints { get; set; } = new HashSet<Complaint>();
    }
}
