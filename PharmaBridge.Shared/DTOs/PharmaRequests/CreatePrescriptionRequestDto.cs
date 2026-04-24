using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    /// DTO for POST /api/prescription-requests — creating a new prescription request
    public class CreatePrescriptionRequestDto
    {

        public IFormFile? ImageUrl { get; set; }

        [MaxLength(500, ErrorMessage = "Patient notes cannot exceed 500 characters")]
        public string? PatientNotes { get; set; }

        // The medicine name if the patient types it manually instead of uploading a photo

        [MaxLength(200, ErrorMessage = "Medicine name cannot exceed 200 characters")]
        public string? MedicineName { get; set; }

        // We take the FK integer, not the full address object the address already exists in DB

        [Required(ErrorMessage = "Delivery address is required.")]
        public int DeliveryAddressId { get; set; }

    }
}
