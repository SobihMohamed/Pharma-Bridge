using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PharmaBridge.Shared.DTOs.Pharmacy;

namespace PharmaBridge.Abstraction.IServices.Pharmacy
{
   
    public interface IPharmacyProfileService
    {
       

        Task<PharmacyDetailsDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, Guid userId);

        Task<PharmacyDetailsDto> GetMyProfileAsync(int pharmacyId, Guid userId);

        Task<PharmacyDetailsDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, Guid userId);

       
        Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId);
    }
}

/*
public class PharmacyQueryParams : BaseQueryParams
{
    public PharmacyStatus? Status { get; set; } 
    
    public string? SearchTerm { get; set; } 
}
*/