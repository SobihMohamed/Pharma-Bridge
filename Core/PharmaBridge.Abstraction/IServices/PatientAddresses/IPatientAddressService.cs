using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.PatientAddresses
{
    // This interface defines the contract for Patient Address Management.
    // Allows patients to manage multiple delivery addresses (e.g., Home, Work, Parents).
    public interface IPatientAddressService
    {
        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================

        // Get all saved addresses for a specific patient. 
        // Notice it returns IReadOnlyList instead of Pagination because a user usually has 2-5 addresses max.
        //Task<IReadOnlyList<PatientAddressDto>> GetPatientAddressesAsync(Guid patientId);

        // Add a new delivery address (includes Lat, Lng, TextAddress, and Label like "Home").
        //Task<PatientAddressDto> AddAddressAsync(Guid patientId, CreatePatientAddressDto createDto);

        // Update an existing address.
        //Task<PatientAddressDto> UpdateAddressAsync(int addressId, Guid patientId, UpdatePatientAddressDto updateDto);

        // Delete an address.
        //Task<bool> DeleteAddressAsync(int addressId, Guid patientId);

        // Set a specific address as the default delivery address.
        //Task<bool> SetDefaultAddressAsync(int addressId, Guid patientId);
    }
}