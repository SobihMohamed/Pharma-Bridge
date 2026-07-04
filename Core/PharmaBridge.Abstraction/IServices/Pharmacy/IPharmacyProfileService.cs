using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.Common.Pagination;
using PharmaBridge.Shared.Common.Params;
using PharmaBridge.Shared.Common.Params.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
   
    public interface IPharmacyProfileService
    {
       

        Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, string userId);

        Task<PharmacyOwnerProfileDto> GetMyProfileAsync(string userId);

        Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(PharmacyToUpdateDto updateDto, string userId);

        Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId);
        Task<AdminPharmacyDetailsDto> GetPharmacyDetailsForAdminAsync(int pharmacyId);
        Task<int> GetPharmacyIdByUserIdAsync(string userId);
        Task<PaginationResponse<AdminPharmacyDto>> GetAllPharmaciesAsync(PharmacyQueryParams queryParams);
        Task<bool> UpdatePharmacyStatusAsync(int pharmacyId, PharmacyStatus status);
    }
}

/*
public class PharmacyQueryParams : BaseQueryParams
{
    public PharmacyStatus? Status { get; set; } 
    
    public string? SearchTerm { get; set; } 
}
*/