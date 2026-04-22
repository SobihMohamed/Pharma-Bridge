using AutoMapper;
using PharmaBridge.Abstraction.IServices.Pharmacy;
using PharmaBridge.Abstraction.IServices.Attachement;
using PharmaBridge.Domain.Contracts.UnitOfWorkPattern;
using PharmaBridge.Domain.Exceptions;
using PharmaBridge.Domain.Models.Pharma_Requests;
using PharmaBridge.Domain.Models.User;
using PharmaBridge.Services.Specifications.Pharmacy;
using PharmaBridge.Shared.DTOs.Pharmacy;
using PharmaBridge.Shared.Dto_s.Attachment;
using PharmaBridge.Shared.EnumHelper.PharmaEnums;
using PharmaBridge.Shared.EnumHelper.UserEnums;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PharmaBridge.Services.ServicesImplementation.Pharmacy
{
    public class PharmacyProfileService(IUnitOfWork unitOfWork, IMapper mapper, IAttachementService attachementService) : IPharmacyProfileService
    {
        public async Task<PharmacyOwnerProfileDto> RegisterPharmacyProfileAsync(PharmacyToCreateDto createDto, Guid userId)
        {
            var owner = await ValidatePharmacyRegistrationAsync(userId.ToString());

            var pharmacy = mapper.Map<Domain.Models.Pharma_Requests.Pharmacy>(createDto);
            pharmacy.PharmaOwnerId = owner.Id;
            pharmacy.Status = PharmacyStatus.Pending;

            if (createDto.LicenseImage != null)
            {
                var uploadDto = new UploadFileDto { File = createDto.LicenseImage, FolderName = "Images/Profiles/Licenses" };
                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);
            }

            await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().AddAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to register pharmacy profile.");

            var savedPharmacy = await GetPharmacyWithVerificationAsync(pharmacy.Id, userId.ToString());
            return mapper.Map<PharmacyOwnerProfileDto>(savedPharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> GetMyProfileAsync(int pharmacyId, Guid userId)
        {
            var pharmacy = await GetPharmacyWithVerificationAsync(pharmacyId, userId.ToString());
            return mapper.Map<PharmacyOwnerProfileDto>(pharmacy);
        }

        public async Task<PharmacyOwnerProfileDto> UpdateMyProfileAsync(int pharmacyId, PharmacyToUpdateDto updateDto, Guid userId)
        {
            var pharmacy = await GetPharmacyWithVerificationAsync(pharmacyId, userId.ToString());
            
            mapper.Map(updateDto, pharmacy);

            if (updateDto.LicenseImage != null)
            {
                if (!string.IsNullOrEmpty(pharmacy.LicenseImageUrl))
                {
                    await attachementService.DeleteFileAsync(pharmacy.LicenseImageUrl);
                }

                var uploadDto = new UploadFileDto { File = updateDto.LicenseImage, FolderName = "Images/Profiles/Licenses" };
                pharmacy.LicenseImageUrl = await attachementService.UploadFileAsync(uploadDto);

                if (pharmacy.Status == PharmacyStatus.Active)
                {
                    pharmacy.Status = PharmacyStatus.Pending;
                }
            }

            unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().UpdateAsync(pharmacy);
            if (await unitOfWork.SaveChangesAsync() <= 0)
                throw new BadRequestCustomeException("Failed to update pharmacy profile.");

            return mapper.Map<PharmacyOwnerProfileDto>(pharmacy);
        }

        public async Task<PharmacyBasicDto> GetPharmacyBasicInfoAsync(int pharmacyId)
        {
            var pharmacy = await unitOfWork.GetRepository<Domain.Models.Pharma_Requests.Pharmacy, int>().GetByIdAsync(pharmacyId);
            if (pharmacy == null || pharmacy.Status != PharmacyStatus.Active)
                throw new NotFoundCutomeException("Pharmacy not found or not active yet.");

            return mapper.Map<PharmacyBasicDto>(pharmacy);
        }

        private async Task<PharmaOwner> ValidatePharmacyRegistrationAsync(string userId)
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

        private async Task<Domain.Models.Pharma_Requests.Pharmacy> GetPharmacyWithVerificationAsync(int pharmacyId, string userId)
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
