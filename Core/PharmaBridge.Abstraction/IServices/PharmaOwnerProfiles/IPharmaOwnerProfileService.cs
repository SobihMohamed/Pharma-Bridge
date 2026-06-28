using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params.PharmaOwner;
using PharmaBridge.Shared.DTOs.PharmaOwners;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PharmaBridge.Abstraction.IServices.PharmaOwnerProfiles
{
    // This interface defines the contract for PharmaOwner Profile Management.
    // Handles pharma owner personal info, documents, and Admin controls over pharma owner accounts.
    public interface IPharmaOwnerProfileService
    {
        //  =========================================================================
        //  --- PharmaOwner(Client) Operations ---
        //  =========================================================================

        // PharmaOwner creates their profile with required documents (linked to their ApplicationUser via JWT).
        Task<PharmaOwnerDetailsDto> CreateMyProfileAsync(string applicationUserId, PharmaOwnerToCreateDto createDto);

        // PharmaOwner views their own profile details (Name, Phone, Documents, etc.) by their Application User ID.
        Task<PharmaOwnerDetailsDto> GetMyProfileAsync(string applicationUserId);

        // PharmaOwner updates their personal info and documents.
        Task<PharmaOwnerDetailsDto> UpdateMyProfileAsync(string applicationUserId, PharmaOwnerToUpdateDto updateDto);

        //  =========================================================================
        //  --- Admin Operations ---
        //  =========================================================================

        // Admin views all registered pharma owners with their status and details.
        Task<PaginationResponse<PharmaOwnerDto>> GetAllPharmaOwnersAsync(PharmaOwnerQueryParams queryParams);

        // Admin approves or rejects a pharma owner profile.
        Task<bool> UpdatePharmaOwnerStatusAsync(string pharmaOwnerId, PharmaBridge.Shared.EnumHelper.PharmaEnums.PharmaOwnerStatus status);
    }
}
