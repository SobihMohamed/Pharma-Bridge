using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class AdminPharmacyDetailsDto : PharmacyDetailsDto
    {
        public string LicenseNumber { get; set; }
        public string Status { get; set; }
        public int CompleteOrderCount { get; set; }
        public string? RejectedReasons { get; set; }
        public string PharmaOwnerId { get; set; }
    }
}
