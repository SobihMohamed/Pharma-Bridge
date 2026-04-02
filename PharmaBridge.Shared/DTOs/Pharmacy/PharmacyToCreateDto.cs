using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToCreateDto
    {
        [Required]
        [MaxLength(100)]
        public string PharmacyName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LicenseNumber { get; set; }

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

        [Required]
        public string PharmaOwnerId { get; set; }

        // Id         -> auto-generated
        // Status     -> defaults to Pending
        // AverageRating      -> starts at 0.00
        // CompleteOrderCount -> starts at 0
    }
}
