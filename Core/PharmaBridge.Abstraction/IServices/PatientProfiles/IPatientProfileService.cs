using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace PharmaBridge.Abstraction.IServices.PatientProfiles
{
    // This interface defines the contract for Patient Profile Management.
    // Handles patient personal info, medical notes, and Admin controls over patient accounts.
    public interface IPatientProfileService
    {
        //  =========================================================================
        //  --- Patient(Client) Operations ---
        //  =========================================================================

        //  Patient views their own profile details(Name, Phone, Medical Notes, etc.).
        Task<PatientProfileDetailsDto> GetMyProfileAsync(string patientId);

        // Patient updates their personal info and medical notes.
        Task<PatientProfileDetailsDto> UpdateMyProfileAsync(string patientId, PatientProfileToUpdateDto updateDto);

        //  =========================================================================
        //  --- Admin Operations ---
        //  =========================================================================

        //  Admin views all registered patients with their status and violation history.
        Task<PaginationResponse<PatientProfileDto>> GetAllPatientsAsync(PatientQueryParams queryParams);
    }
}

/*
// --- Query Params Classes (To be placed in Shared/Common/Params/Patient folder) ---

public class PatientQueryParams : BaseQueryParams
{
    // To search by Patient Name, Email, or Phone
    public string? SearchTerm { get; set; } 
}
*/