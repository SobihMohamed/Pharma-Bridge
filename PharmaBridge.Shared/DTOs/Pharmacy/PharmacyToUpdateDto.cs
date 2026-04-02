using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    public class PharmacyToUpdateDto
    {
        [Required]
        public int Id { get; set; }    //identifies which record to update

        [Required]
        [MaxLength(100)]
        public string PharmacyName { get; set; }

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

        // LicenseNumber -> cannot be changed after registration
        // PharmaOwnerId -> ownership transfer is a separate admin operation
        // Status        -> changed only through dedicated admin endpoints
    }
}
