using System;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class AdminPharmacyDto
    {
        public int Id { get; set; }
        public string PharmacyName { get; set; } = null!;
        public string? PharmaOwnerName { get; set; }
        public string Status { get; set; } = null!;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ContactPhone { get; set; }
    }
}
