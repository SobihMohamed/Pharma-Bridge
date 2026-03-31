using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerDetailsDto
    {
        public string Id { get; set; }
        public string ApplicationUserId { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }
        public string Status { get; set; }
    }
}
