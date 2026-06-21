using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PharmaBridge.Shared.DTOs.Pharmacy;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
   
    public interface IPharmacyProfileService
    {
       

        Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, string userId);

        Task<PharmacyOwnerProfileDto> GetMyProfileAsync(int pharmacyId, string userId);

        Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, string userId);

        Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId);
        Task<int> GetPharmacyIdByUserIdAsync(string userId);
    }
}

/*
public class PharmacyQueryParams : BaseQueryParams
{
    public PharmacyStatus? Status { get; set; } 
    
    public string? SearchTerm { get; set; } 
}
*/