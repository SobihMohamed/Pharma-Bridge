using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaOwners
{
    public class PharmaOwnerDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public string? Email { get; set; } 
        public string? PhoneNumber { get; set; }
    }
}
