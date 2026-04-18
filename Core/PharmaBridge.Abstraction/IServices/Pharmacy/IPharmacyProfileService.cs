using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PharmaBridge.Shared.DTOs.Pharmacy;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
   
    public interface IPharmacyProfileService
    {
       

        Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, Guid userId);

        Task<PharmacyOwnerProfileDto> GetMyProfileAsync(int pharmacyId, Guid userId);

        Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, Guid userId);

       
        Task<PharmacyBasicDto> GetPharmacyBasicInfoAsync(int pharmacyId);
    }
}

/*
public class PharmacyQueryParams : BaseQueryParams
{
    public PharmacyStatus? Status { get; set; } 
    
    public string? SearchTerm { get; set; } 
}
*/