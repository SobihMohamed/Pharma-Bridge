using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToUpdateDto
    {
    
        [Required(ErrorMessage = "Pharmacy Name is required")]
        [MaxLength(100)]
        public string PharmacyName { get; set; }
    
        [Required(ErrorMessage = "General Area is required")]
        [MaxLength(50)]
        public string GeneralArea { get; set; }
    
        [Required]
        public decimal Latitude { get; set; }
    
        [Required]
        public decimal Longitude { get; set; }
    
        public TimeOnly? OpenTime { get; set; }
        public TimeOnly? CloseTime { get; set; }
        public bool Is24Hours { get; set; }
    
        [MaxLength(500)]
        public string? TextAddress { get; set; }
    
        [Phone]
        [MaxLength(11)]
        public string? ContactPhone { get; set; }
    
        // can update but it require review from admin
        public string? LicenseImageUrl { get; set; }
    
        // LicenseNumber -> can update but it require review from admin
        // PharmaOwnerId -> ownership transfer is a separate admin operation
        // Status        -> changed only through dedicated admin endpoints
    }
}
