using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.Patient;
using PharmaBridge.Shared.DTOs.PatientProfiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PharmaBridge.Abstraction.IServices.PatientProfiles
{
    // This interface defines the contract for Patient Profile Management.
    // Handles patient personal info, medical notes, and Admin controls over patient accounts.
    public interface IPatientProfileService
    {
        //  =========================================================================
        //  --- Patient(Client) Operations ---
        //  =========================================================================

        // Patient views their own profile details(Name, Phone, Medical Notes, etc.) by their Application User ID.
        Task<PatientProfileDetailsDto> GetMyProfileAsync(string applicationUserId);

        // Patient updates their personal info and medical notes.
        Task<PatientProfileDetailsDto> UpdateMyProfileAsync(string applicationUserId, PatientProfileToUpdateDto updateDto);

        //  =========================================================================
        //  --- Admin Operations ---
        //  =========================================================================

        // Admin views all registered patients with their status and details.
        Task<PaginationResponse<PatientProfileDto>> GetAllPatientsAsync(PatientQueryParams queryParams);

        Task<PatientProfileDetailsDto> GetPatientByApplicationUserIdAsync(string applicationUserId);

    }
}