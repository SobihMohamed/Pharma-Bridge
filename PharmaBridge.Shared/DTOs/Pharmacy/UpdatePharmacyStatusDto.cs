using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class UpdatePharmacyStatusDto
    {
        [Required(ErrorMessage = "Pharmacy ID is required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "New status is required (e.g., Approved, Rejected)")]
        public string Status { get; set; }

        [MaxLength(500, ErrorMessage = "Rejected reasons cannot exceed 500 characters")]
        public string? RejectedReasons { get; set; }
    }
}
