using AutoMapper;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Pharmacy;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyProfileService(IUnitOfWork unitOfWork, IMapper mapper) : IPharmacyProfileService
    {
        public async Task<PharmacyDetailsDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, Guid userId)
        {
            var owner = await ValidatePharmacyRegistration(userId.ToString());
            
            var pharmacy = mapper.Map<Domain.Models.Pharma_Requests.Pharmacy>(createDto);
            pharmacy.PharmaOwnerId = owner.Id;
            pharmacy.Status = PharmacyStatus.Pending;

            await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().AddAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to register pharmacy profile.");

            return mapper.Map<PharmacyDetailsDto>(pharmacy);
        }

        public async Task<PharmacyDetailsDto> GetMyProfileAsync(int pharmacyId, Guid userId)
        {
            var pharmacy = await GetPharmacyWithVerification(pharmacyId, userId.ToString());
            return mapper.Map<PharmacyDetailsDto>(pharmacy);
        }

        public async Task<PharmacyDetailsDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, Guid userId)
        {
            var pharmacy = await GetPharmacyWithVerification(pharmacyId, userId.ToString());
            
            mapper.Map(updateDto, pharmacy);

            unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().UpdateAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to update pharmacy profile.");

            return mapper.Map<PharmacyDetailsDto>(pharmacy);
        }

        public async Task<PharmacyDto> GetPharmacyBasicInfoAsync(int pharmacyId)
        {
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdAsync(pharmacyId);
            if (pharmacy == null) 
                throw new NotFoundCutomeException("Pharmacy not found");

            return mapper.Map<PharmacyDto>(pharmacy);
        }

        private async Task<PharmaOwner> ValidatePharmacyRegistration(string userId)
        {
            var user = await unitOfWork.GetRepository<ApplicationUser, string>().GetByIdAsync(userId);
            if (user == null || user.Role != UserRole.PharmacyOwner)
                throw new UnAuthorizedCustomeException();
            
            var spec = new PharmaOwnerByAppUserIdSpec(userId);
            var owners = await unitOfWork.GetRepository<PharmaOwner, string>().GetAllWithSpecAsync(spec);
            var owner = owners.FirstOrDefault();

            if (owner == null)
            {
                owner = new PharmaOwner { ApplicationUserId = userId, Status = PharmaOwnerStatus.Pending };
                await unitOfWork.GetRepository<PharmaOwner, string>().AddAsync(owner);
                // Cannot check Pharmacies if just created, but it's new so zero length.
            }
            else if (owner.Pharmacies != null && owner.Pharmacies.Any())
            {
                throw new BadRequestCustomeException("User already has a registered pharmacy");
            }

            return owner;
        }

        private async Task<Domain.Models.Pharma_Requests.Pharmacy> GetPharmacyWithVerification(int pharmacyId, string userId)
        {
            var spec = new PharmacyWithProfileOwnerSpec(pharmacyId);
            var pharmacyInfo = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdWithSpecAsync(spec);

            if (pharmacyInfo == null)
                throw new NotFoundCutomeException("Pharmacy not found");

            if (pharmacyInfo.PharmaOwner.ApplicationUserId != userId)
                throw new UnAuthorizedCustomeException();

            return pharmacyInfo;
        }
    }
}
