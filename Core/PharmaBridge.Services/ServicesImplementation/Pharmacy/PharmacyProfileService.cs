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

            // assign the owner to the pharmacy to establish the relationship
            pharmacy.PharmaOwner = owner;

            await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().AddAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to register pharmacy profile.");

            // AutoMapper will automatically map the nested PharmaOwner properties to the PharmacyDetailsDto
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
            // not shown to the public if the pharmacy is not active yet
            if (pharmacy == null || pharmacy.Status != PharmacyStatus.Active)
                throw new NotFoundCutomeException("Pharmacy not found or not active yet.");

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
                owner = new PharmaOwner
                {
                    Id = Guid.NewGuid().ToString(),
                    ApplicationUserId = userId,
                    Status = PharmaOwnerStatus.Pending,
                    ApplicationUser = user // connect the ApplicationUser to the PharmaOwner
                };
                await unitOfWork.GetRepository<PharmaOwner, string>().AddAsync(owner);
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
