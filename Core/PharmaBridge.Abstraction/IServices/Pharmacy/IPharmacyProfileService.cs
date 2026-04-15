using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PharmaBridge.Shared.DTOs.Pharmacy;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
    // This interface defines the contract for Pharmacy Profile Management and Onboarding.
    // Handles registration, updating working hours/location, and Admin approvals.
    public interface IPharmacyProfileService
    {
        // =========================================================================
        // --- Pharmacy (Provider) Operations ---
        // =========================================================================

        // Pharmacy completes their profile (Location, Working Hours, License Documents).
        Task<PharmacyDetailsDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, Guid userId);

        // Pharmacy views their own profile details.
        Task<PharmacyDetailsDto> GetMyProfileAsync(int pharmacyId, Guid userId);

        // Pharmacy updates their info (e.g., changing working hours or location on map).
        Task<PharmacyDetailsDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, Guid userId);

        // =========================================================================
        // --- Patient (Client) Operations ---
        // =========================================================================

        // Patient views basic, non-sensitive info about a pharmacy (e.g., Name, Location, IsOpen).
        Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId);


        // =========================================================================
        // --- Admin Operations ---
        // =========================================================================

        // Admin views all registered pharmacies (Pending, Active, Blocked) for review.
        //Task<Pagination<PharmacyDto>> GetAllPharmaciesAsync(PharmacyQueryParams queryParams);

        // Admin approves, suspends, or blocks a pharmacy.
        //Task<bool> UpdatePharmacyStatusAsync(Guid pharmacyId, UpdatePharmacyStatusDto statusDto);
    }
}

/*
// --- Query Params Classes (To be placed in Shared/Common/Params/Pharmacy folder) ---

public class PharmacyQueryParams : BaseQueryParams
{
    // e.g., Pending, Active, Suspended, Blocked
    public PharmacyStatus? Status { get; set; } 
    
    // To search by Pharmacy Name or Phone Number
    public string? SearchTerm { get; set; } 
}
*/