using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Shared.DTOs.Pharmacy
{
    // Inherits: Id, PharmacyName, TextAddress, AverageRating, CompleteOrderCount, Is24Hours, OpenTime, CloseTime
    public class PharmacyDetailsDto : PharmacyDto
    {
        // Added for the Map View in the details screen
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // REMOVED: ContactPhone 
        // Reason: To prevent "Platform Leakage". Patients should only get the phone number after an order is confirmed.
    }
}
