using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PharmaBridge.Shared.DTOs.PharmaRequests
{
    // DTO for PUT /api/prescription-requests/{id} — editing an existing request

    public class UpdatePrescriptionRequestDto
    {
        // Mandatory — identifies which record to update

        [Required(ErrorMessage = "Request ID is required for update")]
        public int Id { get; set; }

        // Patient may want to re-upload a clearer image

        [Url(ErrorMessage = "ImageUrl must be a valid URL")]
        public string? ImageUrl { get; set; }

        // Patient may want to correct their notes

        [MaxLength(500, ErrorMessage = "Patient notes cannot exceed 500 characters")]
        public string? PatientNotes { get; set; }

        [MaxLength(200, ErrorMessage = "Medicine name cannot exceed 200 characters")]
        public string? MedicineName { get; set; }

        public int? DeliveryAddressId { get; set; }
    }
}
